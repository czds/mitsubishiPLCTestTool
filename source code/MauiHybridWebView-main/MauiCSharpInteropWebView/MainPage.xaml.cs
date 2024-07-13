using DeviceComm;
using DeviceComm.MELSEC;
using HybridWebView;
using MauiCSharpInteropWebView.DeviceServices;
using MauiCSharpInteropWebView.Fun;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using System.Text.RegularExpressions;


namespace MauiCSharpInteropWebView;

public partial class MainPage : ContentPage
{
    private HybridAppPageID _currentPage;
    private int _messageCount;

    public string CurrentPageName => $"Current hybrid page: {_currentPage}";

    public string ConnectBtn { get; private set; } = "连接";

    public bool btnClicked { get; private set; } = true; //绑定按钮是否可用

    public string MessageLog { get; private set; }
    public int MessageLogPosition { get; private set; }
    public bool PageAllowsRawMessage => _currentPage == HybridAppPageID.RawMessages; //绑定按钮是否可用
    public bool PageAllowsMethodInvoke => _currentPage == HybridAppPageID.MethodInvoke; //绑定按钮是否可用




    private string _PLCAddress = "";
    private int _PLCPort = 0;

    private Melsec3E PLC = null;

    private enum HybridAppPageID
    {
        MainPage = 0,
        RawMessages = 1,
        MethodInvoke = 2,
        ProxyUrls = 3,
    }
    public MainPage()
    {

        BindingContext = this;

        InitializeComponent();

#if ANDROID
        var deviceOrientationService = new DeviceOrientationService();
        deviceOrientationService.SetDeviceOrientation(DisplayOrientation.Portrait);
#endif


        connectPLC.BackgroundColor = Color.FromRgb(52, 146, 232); //设置连接按钮颜色

        //AidTool.SqlConnectionString = "Data Source=192.168.31.123;Initial Catalog=dbxxx;User ID=sa;Password=PWD123;TrustServerCertificate=true";

#if DEBUG
        myHybridWebView.EnableWebDevTools = true;
#endif


        myHybridWebView.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().EnableZoomControls(true);

        myHybridWebView.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().DisplayZoomControls(true);

        myHybridWebView.JSInvokeTarget = new MyJSInvokeTarget(this);

        myHybridWebView.ProxyRequestReceived += MyHybridWebView_OnProxyRequestReceived;

        myHybridWebView.HybridWebViewInitialized += MyHybridWebView_WebViewInitialized;
    }

    private void MyHybridWebView_WebViewInitialized(object sender, HybridWebViewInitializedEventArgs e)
    {
#if WINDOWS
            // Disable the user manually zooming
            e.WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
#endif

    }




    private async void 执行JS方法(object sender, EventArgs e)
    {
        _ = await myHybridWebView.EvaluateJavaScriptAsync($"SendToJs('Sent from .NET, the time is: {DateTimeOffset.Now}!')");
    }

    private async void 调用JS代码(object sender, EventArgs e)
    {
        var sum = await myHybridWebView.InvokeJsMethodAsync<int>("JsAddNumbers", 123, 456);
        WriteToLog($"JS Return value received with sum: {sum}");
    }

    private void OnHybridWebViewRawMessageReceived(object sender, HybridWebView.HybridWebViewRawMessageReceivedEventArgs e)
    {
        const string PagePrefix = "page:";
        if (e.Message.StartsWith(PagePrefix, StringComparison.Ordinal)) //根据当前页面编号,改变变量值,通知页面
        {
            _currentPage = (HybridAppPageID)int.Parse(e.Message.Substring(PagePrefix.Length));
            OnPropertyChanged(nameof(CurrentPageName));  //通知页面文本内容已变更
            OnPropertyChanged(nameof(PageAllowsRawMessage)); //通知页面按钮是否可用
            OnPropertyChanged(nameof(PageAllowsMethodInvoke)); //通知页面按钮是否可用
        }
        else
        {
            WriteToLog($"收到页面消息: {e.Message}");
        }
    }
    // HTML页面如何加载本地文件或其他网页等内容，JS与C#互通本质上是频道间收发 stream 流（字符串消息）

    private void WriteToLog(string message)
    {
        //显示日志多了就先清空
        if (_messageCount == 33)
        {
            _messageCount = 0;
            MessageLog = "";
        }

        MessageLog += Environment.NewLine + $"时间 {DateTime.Now.ToString("hh:mm:ss")} | {_messageCount++}:  " + message;
        MessageLogPosition = MessageLog.Length;
        OnPropertyChanged(nameof(MessageLog));
        OnPropertyChanged(nameof(MessageLogPosition));
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TestPage));
    }

    private void port_Completed(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(port.Text) ? false : Regex.IsMatch(port.Text, @"^[1-9]\d*$"))
        {
            _PLCPort = int.Parse(port.Text);
        }
        else
        {
            port.Text = "";
        }
    }

    private void IPAddr_Completed(object sender, EventArgs e)
    {
        if (Regex.IsMatch(IPAddr.Text, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"))
        {
            _PLCAddress = IPAddr.Text;
        }
        else
        {
            IPAddr.Text = "";
        }
    }

    private void connectBtn_Clicked(object sender, EventArgs e)
    {
        if (!btnClicked) return;

        IPAddr_Completed(null, null);
        port_Completed(null, null);

        if (IPAddr.Text == "" | _PLCAddress == "") return;
        if (port.Text == "") return;
        if (_PLCPort <= 0) return;

        if (ConnectBtn == "连接")
        {
            //连接PLC
            PLC = new Melsec3E(_PLCAddress, _PLCPort);
            PLC.ConnectTimeOut = 1000; //连接超时时间1s

            btnClicked = false;//禁用按钮,避免重复点击
            OnPropertyChanged(nameof(btnClicked));

            OperateResult res = PLC.ConnectServer();

            btnClicked = true;
            OnPropertyChanged(nameof(btnClicked));

            if (res.IsSuccess)
            {
                ConnectBtn = "断开";
                OnPropertyChanged(nameof(ConnectBtn));
                connectPLC.BackgroundColor = Color.FromRgb(255, 102, 102); //设置连接按钮颜色

                WriteToLog($"已连接IP地址  {_PLCAddress} 的PLC (端口 {_PLCPort})");
                return; //连接成功
            }
            //连接失败
            try { PLC.ConnectClose(); }catch { }
            WriteToLog($"IP地址  {_PLCAddress} 的PLC (端口 {_PLCPort})，连接测试失败");
            PLC = null;
            return;
        }

        //断开连接
        if (PLC == null) return;

        btnClicked = false;//禁用按钮,避免重复点击
        OnPropertyChanged(nameof(btnClicked));

        PLC.ConnectClose();

        btnClicked = true;
        OnPropertyChanged(nameof(btnClicked));
        ConnectBtn = "连接";
        OnPropertyChanged(nameof(ConnectBtn));
        connectPLC.BackgroundColor = Color.FromRgb(52, 146, 232); //设置连接按钮颜色

        WriteToLog($"与PLC（IP地址为 {_PLCAddress}）的连接已断开");

        PLC = null;


    }
}
