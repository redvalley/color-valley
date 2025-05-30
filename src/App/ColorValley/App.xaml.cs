using System.Diagnostics;
#if !PRO_VERSION
using Plugin.AdMob;
using Plugin.AdMob.Services;
#endif

namespace ColorValley
{
    public partial class App : Application
    {

#if !PRO_VERSION
        /// <summary>
        /// The main interstitial Ad
        /// </summary>
        public static IInterstitialAd? MainInterstitialAd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if interstitial ad is showing already.
        /// </summary>
        public static bool IsInterstitialAdShowing { get; set; }

        /// <summary>
        /// The app open Ad
        /// </summary>
        public IAppOpenAd? AppOpenAd { get; set; }
#endif

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
#if !PRO_VERSION
            InitializeAds();
#endif
            var mainWindow = new Window(new SplashPage());
#if !PRO_VERSION
            mainWindow.Resumed += MainWindowOnResumed;
#endif
            return mainWindow;
        }
#if !PRO_VERSION
        private void InitializeAds()
        {
            var interstitialAdService = IPlatformApplication.Current?.Services.GetService<IInterstitialAdService>();
            if (interstitialAdService != null)
            {
                
#if IOS
                string interstitialAddUnitId = "ca-app-pub-6864374918270893/1009817964";
#else
                //string interstitialAddUnitId = "ca-app-pub-6864374918270893/4065633825";
                //secondary

                string interstitialAddUnitId = "ca-app-pub-6864374918270893/2188976181";
#endif
                MainInterstitialAd = interstitialAdService.CreateAd(interstitialAddUnitId);
                MainInterstitialAd.Load();
            }
            
            var appOpenAdService = IPlatformApplication.Current?.Services.GetService<IAppOpenAdService>();
            if (appOpenAdService != null)
            {
#if IOS
                string appOpenAddUnitId = "ca-app-pub-6864374918270893/6262661560";
#else
                //string appOpenAddUnitId = "ca-app-pub-6864374918270893/3232269588";
                //secondary

                string appOpenAddUnitId = "ca-app-pub-6864374918270893/1576917889";
#endif
                AppOpenAd = appOpenAdService.CreateAd(appOpenAddUnitId);
                AppOpenAd.Load();
            }
        }

        private void MainWindowOnResumed(object? sender, EventArgs e)
        {
            if (AppOpenAd is { IsLoaded: true })
            {
                if (!IsInterstitialAdShowing)
                {
                    AppOpenAd.Show();
                }
                
            }
        }
#endif
    }
}