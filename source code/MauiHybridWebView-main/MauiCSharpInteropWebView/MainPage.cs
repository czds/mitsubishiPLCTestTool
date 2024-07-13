using DeviceComm;
using MauiCSharpInteropWebView.Fun;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.IO.Compression;
using System.Text;


namespace MauiCSharpInteropWebView;

public partial class MainPage : ContentPage
{

    private async Task MyHybridWebView_OnProxyRequestReceived(HybridWebView.HybridWebViewProxyEventArgs args)
    {
        if (args.QueryParams.ContainsKey("operation"))
        {
            switch (args.QueryParams["operation"])
            {
                case "loadImageFromZip":
                    // Ensure the file name parameter is present.
                    if (args.QueryParams.TryGetValue("fileName", out string fileName) && fileName != null)
                    {
                        // Load local zip file. 
                        using var stream = await FileSystem.OpenAppPackageFileAsync("media/pictures.zip");

                        // Unzip file and check to see if it has the requested file name.
                        using var archive = new ZipArchive(stream);

                        var file = archive.Entries.Where(x => x.FullName == fileName).FirstOrDefault();

                        if (file != null)
                        {
                            // Copy the file stream to a memory stream.
                            var ms = new MemoryStream();
                            using (var fs = file.Open())
                            {
                                await fs.CopyToAsync(ms);
                            }

                            // Rewind stream.
                            ms.Position = 0;

                            args.ResponseStream = ms;
                            args.ResponseContentType = "image/jpeg";
                        }
                    }
                    break;

                case "loadImageFromWeb":
                    if (args.QueryParams.TryGetValue("tileId", out string tileIdString) && int.TryParse(tileIdString, out var tileId))
                    {
                        // Apply custom logic. In this case convert into a quadkey value for Bing Maps.
                        var quadKey = new StringBuilder();
                        for (var i = tileId; i > 0; i /= 4)
                        {
                            quadKey.Insert(0, (tileId % 4).ToString(CultureInfo.InvariantCulture));
                        }

                        //Create URL using the tileId parameter. 
                        var url = $"https://ecn.t0.tiles.virtualearth.net/tiles/a{quadKey}.jpeg?g=14245";

#if ANDROID
                                                var client = new HttpClient(new Xamarin.Android.Net.AndroidMessageHandler());
#else
                        var client = new HttpClient();
#endif

                        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

                        // Copy the response stream to a memory stream.
                        var ms2 = new MemoryStream();

                        // TODO: Remove the Wait()
                        response.Content.CopyToAsync(ms2).Wait();
                        ms2.Position = 0;
                        args.ResponseStream = ms2;
                        args.ResponseContentType = "image/jpeg";
                    }
                    break;


                default:
                    break;
            }
        }
    }


    private sealed class MyJSInvokeTarget  // JS调用的C#方法类
    {
        private readonly MainPage _mainPage;

        public MyJSInvokeTarget(MainPage mainPage)
        {
            _mainPage = mainPage;
        }


        public string 带参数调用(string message, int value)
        {
            _mainPage.WriteToLog($"I'm a .NET method called from JavaScript with message='{message}' and value={value}");
            return "str";
        }

        public void 空参数调用(string thisIsNull, int thisIsUndefined)
        {
            _mainPage.WriteToLog($"I'm a .NET method called from JavaScript with null='{thisIsNull}' and undefined={thisIsUndefined}");
        }

        public void 无参数调用()
        {
            _mainPage.WriteToLog($"I'm a .NET method called from JavaScript with no params");
        }

        public string 有返回带参数调用(double value)
        {
            _mainPage.WriteToLog($"input parameter value={value}");

            return "val";
        }


        public object GetObjectResponse()// 返回一个对象数组
        {
            _mainPage.WriteToLog($"an object without any input parameters");


            AidTool.SqlDataReaderResult res = AidTool.vra_pool_open("SELECT TOP 100 [frmjobno],[barcode],[startPoint],[endPoint],[pltTime],  case [dbo].[Pltmsg].[istatus] when 0 then '任务等待下发' when 1 then '已下发给AGV' when 2 then 'AGV已完成任务' end [status],   [scanTime],[sendAgvTaskTime],[agvStartTime],[agvEndTime],   [dbo].[Pltmsg].[robotjobid],[dbo].[JobM].[sstatus],[dupdate]     FROM [dbo].[Pltmsg] LEFT JOIN [dbo].[JobM] ON [dbo].[Pltmsg].[robotjobid]=[dbo].[JobM].[robotjobid]  WHERE [dbo].[Pltmsg].[istatus]!=2 ORDER BY [pltTime] DESC");

            if (!res.successed)
            {

                return "";
            };

            DataRowCollection rows = res.dataTable.Rows;

            //DataRow tb = res.dataTable.Rows[0];
            //if (tb["cn"].ToString() != "0")

            string json = $"{{\"rows\": {JsonConvert.SerializeObject(res.dataTable)},\"total\":{rows.Count}}}";

            return json;

            //return new List<object>()
            //{
            //    new { Name = "John", Age = 42 },
            //    new { Name = "Jane", Age = 39 },
            //    new { Name = "Sam", Age = 13 },
            //};
        }

        public object getCurrentJobInfo()    // 数据库操作
        {
            AidTool.SqlDataReaderResult res = AidTool.vra_pool_open("SELECT TOP 100 [frmjobno],[barcode],[startPoint],[endPoint],[pltTime],  case [dbo].[Pltmsg].[istatus] when 0 then '任务等待下发' when 1 then '已下发给AGV' when 2 then 'AGV已完成任务' end [status],   [scanTime],[sendAgvTaskTime],[agvStartTime],[agvEndTime],   [dbo].[Pltmsg].[robotjobid],[dbo].[JobM].[sstatus],[dupdate]     FROM [dbo].[Pltmsg] LEFT JOIN [dbo].[JobM] ON [dbo].[Pltmsg].[robotjobid]=[dbo].[JobM].[robotjobid]  WHERE [dbo].[Pltmsg].[istatus]!=2 ORDER BY [pltTime] DESC");

            if (!res.successed)
            {

                return "";
            };

            DataRowCollection rows = res.dataTable.Rows;

            //DataRow tb = res.dataTable.Rows[0];
            //if (tb["cn"].ToString() != "0")

            string json = $"{{\"rows\": {JsonConvert.SerializeObject(res.dataTable)},\"total\":{rows.Count}}}";

            return json;

        }

        public string RoundTripCallFromScript(string message, int value)
        {
            _mainPage.WriteToLog($"called from JavaScript with message='{message}' and value={value}");

            return $"C# says: I got your message='{message}' and value={value}";
        }


        //..............以上为测试用，下面是与PLC通讯部分

        public string readPLCVal(string addr, string type, string plcAddr, string remark)
        {

            if (_mainPage.PLC == null) return "未连接设备";

            string result = "不支持的数据类型";

            if (type == "bool")
            {
                OperateResult<bool> res = _mainPage.PLC.ReadBool(addr);
                if (!res.IsSuccess)
                {
                    _mainPage.WriteToLog($"读取的PLC地址：'{plcAddr}' 备注：{remark}, 通讯读取失败");
                    return "读取操作通讯失败";
                }

                //读取成功，界面显示日志
                result = res.Content.ToString();
                LogReadMsg(plcAddr, remark, res.Content.ToString());
                return result;
            }

            if (type == "ushort")
            {
                OperateResult<ushort> res = _mainPage.PLC.ReadUInt16(addr);
                if (!res.IsSuccess)
                {
                    _mainPage.WriteToLog($"读取的PLC地址：'{plcAddr}' 备注：{remark}, 通讯读取失败");
                    return "读取操作通讯失败";
                }

                //读取成功，界面显示日志
                result = res.Content.ToString();
                LogReadMsg(plcAddr, remark, res.Content.ToString());
                return result;
            }

            if (type == "short")
            {
                OperateResult<short> res = _mainPage.PLC.ReadInt16(addr);
                if (!res.IsSuccess)
                {
                    _mainPage.WriteToLog($"读取的PLC地址：'{plcAddr}' 备注：{remark}, 通讯读取失败");
                    return "读取操作通讯失败";
                }

                //读取成功，界面显示日志
                result = res.Content.ToString();
                LogReadMsg(plcAddr, remark, res.Content.ToString());
                return result;
            }

            if (type == "uint")
            {
                OperateResult<uint> res = _mainPage.PLC.ReadUInt32(addr);
                if (!res.IsSuccess)
                {
                    _mainPage.WriteToLog($"读取的PLC地址：'{plcAddr}' 备注：{remark}, 通讯读取失败");
                    return "读取操作通讯失败";
                }

                //读取成功，界面显示日志
                result = res.Content.ToString();
                LogReadMsg(plcAddr, remark, res.Content.ToString());
                return result;
            }
            if (type == "int")
            {
                OperateResult<int> res = _mainPage.PLC.ReadInt32(addr);
                if (!res.IsSuccess)
                {
                    _mainPage.WriteToLog($"读取的PLC地址：'{plcAddr}' 备注：{remark}, 通讯读取失败");
                    return "读取操作通讯失败";
                }

                //读取成功，界面显示日志
                result = res.Content.ToString();
                LogReadMsg(plcAddr, remark, res.Content.ToString());
                return result;
            }


            if (type == "float")
            {
                OperateResult<float> res = _mainPage.PLC.ReadFloat(addr);
                if (!res.IsSuccess)
                {
                    _mainPage.WriteToLog($"读取的PLC地址：'{plcAddr}' 备注：{remark}, 通讯读取失败");
                    return "读取操作通讯失败";
                }

                //读取成功，界面显示日志
                result = res.Content.ToString();
                LogReadMsg(plcAddr, remark, res.Content.ToString());
                return result;
            }



            return result;
        }

        private void LogReadMsg(string plcAddr, string remark, string content)
        {

            if (remark != "") remark = $" ~ {remark}";

            _mainPage.WriteToLog($"{plcAddr.PadRight(7)} {remark.PadRight(9)} 读到的值为: {content}");

        }





    }

}
