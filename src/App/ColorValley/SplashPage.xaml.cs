using ACAB.App;
#if !PRO_VERSION
using ColorValley.Services;
using Plugin.AdMob;
using Plugin.AdMob.Services;
#endif
using Debug = System.Diagnostics.Debug;

namespace ColorValley;

public partial class SplashPage : ContentPage
{
    private bool _isMainPageShowing = false;

#if !PRO_VERSION
    private readonly IAdConsentService? _adConsentService = null;
    private readonly IColorValleyAppOpenAdService _colorValleyAppOpenAdService;
    private readonly IColorValleyInterstitualAdService _colorValleyInterstitualAdService;
#endif


#if PRO_VERSION
    public SplashPage()
    {
        InitializeComponent();
        this.LabelAppNameColorValleyPro.IsVisible = true;
        this.LabelAppNameColorValley.IsVisible = false;

        this.LabelSplashWelcomePro.IsVisible = true;
        this.LabelSplashWelcome.IsVisible = false;

        this.ImageColorValleyPro.IsVisible = true;
        this.ImageColorValley.IsVisible = false;

    }
#else
    public SplashPage(IAdConsentService adConsentService, 
        IColorValleyAppOpenAdService colorValleyAppOpenAdService,
        IColorValleyInterstitualAdService colorValleyInterstitualAdService)
    {
        InitializeComponent();
        _adConsentService = adConsentService;
        _colorValleyAppOpenAdService = colorValleyAppOpenAdService;
        _colorValleyInterstitualAdService = colorValleyInterstitualAdService;

        this.LabelAppNameColorValleyPro.IsVisible = false;
        this.LabelAppNameColorValley.IsVisible = true;

        this.LabelSplashWelcomePro.IsVisible = false;
        this.LabelSplashWelcome.IsVisible = true;

        this.ImageColorValleyPro.IsVisible = false;
        this.ImageColorValley.IsVisible = true;
    }
#endif


    protected override async void OnAppearing()
    {
        this._isMainPageShowing = false;
        base.OnAppearing();

#if !PRO_VERSION
        if (_adConsentService != null && !_adConsentService.CanRequestAds())
        {
            _adConsentService.LoadAndShowConsentFormIfRequired();
        }

        await ShowMainPageAfterAd();
#else
        await ShowMainPageForProVersion();
#endif
    }



#if !PRO_VERSION
    private async Task ShowMainPageAfterAd()
    {
        await Task.Run(async () =>
        {
            await Task.Delay(5000);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this._colorValleyAppOpenAdService.ShowAd(ShowMainPage);
            });
        });
    }
#endif

#if PRO_VERSION
    private async Task ShowMainPageForProVersion()
    {
        await Task.Run(async () =>
        {
            await Task.Delay(2000);
            await MainThread.InvokeOnMainThreadAsync(ShowMainPage);
        });
    }
#endif
    private void ShowMainPage()
    {
        if (_isMainPageShowing)
        {
            return;
        }

        _isMainPageShowing = true;

        if (Application.Current?.Windows != null)
        {
#if PRO_VERSION
            Application.Current.Windows[0].Page = new CompanySplashPage();
#else
            Application.Current.Windows[0].Page = new CompanySplashPage(_colorValleyInterstitualAdService);
#endif
        }
    }
}