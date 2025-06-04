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
            mainWindow.Resumed += MainWindowOnResumed;
#endif
            return mainWindow;

        }

        
#if !PRO_VERSION
        private void InitializeAds()
        {
            _colorValleyAppOpenAdService.LoadAd();
            _colorValleyInterstitualAdService.LoadAd();
            
        }

        private void MainWindowOnResumed(object? sender, EventArgs e)
        {
            _colorValleyAppOpenAdService.ShowAd(()=>{});
        }
#endif
    }
}