namespace MauiCSharpInteropWebView.DeviceServices
{
#if ANDROID
    public partial class DeviceOrientationService : IDeviceOrientationService
    {
        public partial void SetDeviceOrientation(DisplayOrientation displayOrientation);
    }
#endif
}
