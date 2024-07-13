


//using Foundation;
//using UIKit;


//namespace MauiCSharpInteropWebView.DeviceServices;

//public partial class DeviceOrientationService
//{

//    private static readonly IReadOnlyDictionary<DisplayOrientation, UIInterfaceOrientation> _iosDisplayOrientationMap =
//        new Dictionary<DisplayOrientation, UIInterfaceOrientation>
//        {
//            [DisplayOrientation.Landscape] = UIInterfaceOrientation.LandscapeLeft,
//            [DisplayOrientation.Portrait] = UIInterfaceOrientation.Portrait,
//        };

//    public  void SetDeviceOrientation(DisplayOrientation displayOrientation)
//    {
//        if (UIDevice.CurrentDevice.CheckSystemVersion(16, 0))
//        {

//            var scene = (UIApplication.SharedApplication.ConnectedScenes.ToArray()[0] as UIWindowScene);
//            if (scene != null)
//            {
//                var uiAppplication = UIApplication.SharedApplication;
//                var test = UIApplication.SharedApplication.KeyWindow?.RootViewController;
//                if (test != null)
//                {
//                    test.SetNeedsUpdateOfSupportedInterfaceOrientations();
//                    scene.RequestGeometryUpdate(
//                        new UIWindowSceneGeometryPreferencesIOS(UIInterfaceOrientationMask.Portrait), error => { });
//                }
//            }
//        }
//        else
//        {
//            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));
//        }


//    }
//}