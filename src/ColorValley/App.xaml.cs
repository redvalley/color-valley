using System.Diagnostics;
using Plugin.AdMob;
using Plugin.AdMob.Services;

namespace ColorValley
{
    public partial class App : Application
    {
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

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
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

            var mainWindow = new Window(new CompanySplashPage());
            mainWindow.Resumed += MainWindowOnResumed;

            return mainWindow;
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
    }
}