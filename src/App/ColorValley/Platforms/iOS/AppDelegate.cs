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
            MauiApp? app = null;
            try
            {
                app = MauiProgram.CreateMauiApp();

                if (MobileAds.SharedInstance != null)
                {
                    Google.MobileAds.MobileAds.SharedInstance.Start(completionHandler: null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Error occured during startup: " + e.Message);
            }

            if (app == null)
            {
                throw new InvalidOperationException("App is null, it seems that the app could not be initialized correctly!");
            }
            
            return app;
        }
    }
}
