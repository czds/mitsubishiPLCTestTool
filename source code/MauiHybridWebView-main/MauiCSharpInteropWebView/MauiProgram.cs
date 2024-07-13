using CommunityToolkit.Maui;
using MauiCSharpInteropWebView.ViewModel;
using Microsoft.Extensions.Logging;
using MauiCSharpInteropWebView.DeviceServices;

namespace MauiCSharpInteropWebView;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddHybridWebView();

        //注册页面依赖,禁用设备横屏

#if ANDROID
        builder.Services.AddSingleton<IDeviceOrientationService, DeviceOrientationService>();
#endif


        //注册页面依赖
        builder.Services.AddSingleton<TestPage>();
        builder.Services.AddSingleton<TestViewModel>();
        //注册页面依赖,用到时加载
        builder.Services.AddTransient<DetailPage>();
        builder.Services.AddTransient<DetailViewModel>();

        return builder.Build();
    }
}
