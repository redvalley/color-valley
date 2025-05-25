using ColorValley;
using Foundation;
using Google.MobileAds;

namespace ACAB.App
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            var app = MauiProgram.CreateMauiApp();

            if (MobileAds.SharedInstance != null)
            {
                Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: null);    
            }
            

            return app;
        }
    }
}
