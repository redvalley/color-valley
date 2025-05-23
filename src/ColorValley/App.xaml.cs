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
            var mainWindow = new Window(new CompanySplashPage());
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
                MainInterstitialAd = interstitialAdService.CreateAd("ca-app-pub-6864374918270893/4065633825");
                MainInterstitialAd.Load();
            }
            
            var appOpenAdService = IPlatformApplication.Current?.Services.GetService<IAppOpenAdService>();
            if (appOpenAdService != null)
            {
                AppOpenAd = appOpenAdService.CreateAd("ca-app-pub-6864374918270893/3232269588");
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