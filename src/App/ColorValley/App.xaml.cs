using System.Diagnostics;
using ColorValley.Services;
#if !PRO_VERSION
using Plugin.AdMob;
using Plugin.AdMob.Services;
#endif

namespace ColorValley
{
    public partial class App : Application
    {


#if !PRO_VERSION
        private readonly IColorValleyAppOpenAdService _colorValleyAppOpenAdService;
        private readonly IColorValleyInterstitualAdService _colorValleyInterstitualAdService;
        private bool _appWasDeactivated = false;
#endif

#if !PRO_VERSION
        public App(IColorValleyAppOpenAdService colorValleyAppOpenAdService, IColorValleyInterstitualAdService colorValleyInterstitualAdService)
        {
            _colorValleyAppOpenAdService = colorValleyAppOpenAdService;
            _colorValleyInterstitualAdService = colorValleyInterstitualAdService;

            InitializeComponent();
        }
#else
        public App()
        {
            InitializeComponent();
        }
#endif


        protected override Window CreateWindow(IActivationState? activationState)
        {
#if !PRO_VERSION
            InitializeAds();
#endif

            var splashPage = activationState?.Context.Services?.GetRequiredService<SplashPage>();
            if (splashPage == null) return base.CreateWindow(activationState);
            var mainWindow = new Window(splashPage);
                
#if !PRO_VERSION
            mainWindow.Activated += MainWindowOnActivated;
            mainWindow.Deactivated += MainWindowOnDeactivated;
#endif
            return mainWindow;

        }

        private void MainWindowOnDeactivated(object? sender, EventArgs e)
        {
            if (Windows.First().Page is NavigationPage navigationPage)
            {
                if (!(navigationPage.CurrentPage is MainPage mainPage && mainPage.IsInterstitualAdShowing))
                {
                    _appWasDeactivated = true;
                }
            }
        }


#if !PRO_VERSION
        private void InitializeAds()
        {
            _colorValleyAppOpenAdService.LoadAd();
            _colorValleyInterstitualAdService.LoadAd();
            
        }

        private async void MainWindowOnActivated(object? sender, EventArgs e)
        {
            if (Windows.First().Page is NavigationPage && _appWasDeactivated)
            {
                _appWasDeactivated = false;
                await _colorValleyAppOpenAdService.ShowAd(() => { });
            }
            
        }
#endif
    }
}