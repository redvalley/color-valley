using ACAB.App;
#if !PRO_VERSION
using Plugin.AdMob;
using Plugin.AdMob.Services;
#endif
using Debug = System.Diagnostics.Debug;

namespace ColorValley;

public partial class SplashPage : ContentPage
{
    private bool _isMainPageShowing = false;
    private bool _isConsentInfoFormShown = false;

#if !PRO_VERSION
    private readonly IAdConsentService? _adConsentService = null;
#endif

    public SplashPage()
    {
        InitializeComponent();
#if PRO_VERSION
        this.LabelAppNameColorValleyPro.IsVisible = true;
        this.LabelAppNameColorValley.IsVisible = false;

        this.LabelSplashWelcomePro.IsVisible = true;
        this.LabelSplashWelcome.IsVisible = false;

        this.ImageColorValleyPro.IsVisible = true;
        this.ImageColorValley.IsVisible = false;
#else
        this.LabelAppNameColorValleyPro.IsVisible = false;
        this.LabelAppNameColorValley.IsVisible = true;

        this.LabelSplashWelcomePro.IsVisible = false;
        this.LabelSplashWelcome.IsVisible = true;

        this.ImageColorValleyPro.IsVisible = false;
        this.ImageColorValley.IsVisible = true;
        _adConsentService = IPlatformApplication.Current?.Services.GetService<IAdConsentService>();
#endif
    }

    protected override async void OnAppearing()
    {
        this._isMainPageShowing = false;
        this._isConsentInfoFormShown = false;
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
        if (App.MainInterstitialAd != null)
        {
            App.MainInterstitialAd.OnAdFailedToShow += MainInterstitialAdOnOnAdFailedToShow;
            App.MainInterstitialAd.OnAdFailedToLoad += MainInterstitialAdOnOnAdFailedToLoad;
            App.MainInterstitialAd.OnAdDismissed += MainInterstitialAdOnOnAdDismissed;
        }

        await Task.Run(async () =>
        {
            await Task.Delay(3000);
            if (App.MainInterstitialAd != null)
            {
                if (App.MainInterstitialAd.IsLoaded)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        App.IsInterstitialAdShowing = true;
                        App.MainInterstitialAd.Show();
                    });
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(ShowMainPage);
                }
            }
        });
    }

    private void MainInterstitialAdOnOnAdDismissed(object? sender, EventArgs e)
    {
        App.IsInterstitialAdShowing = false;
        ShowMainPage();
    }

    private void MainInterstitialAdOnOnAdFailedToShow(object? sender, IAdError e)
    {
        ShowMainPage();
    }

    private void MainInterstitialAdOnOnAdFailedToLoad(object? sender, IAdError e)
    {
        ShowMainPage();
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
            Application.Current.Windows[0].Page = new CompanySplashPage();
        }
    }
}