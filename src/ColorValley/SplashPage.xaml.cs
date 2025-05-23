using ACAB.App;
#if !PRO_VERSION
using Android.Gms.Ads.Interstitial;
using Plugin.AdMob;
using Plugin.AdMob.Services;
#endif
using Debug = System.Diagnostics.Debug;

namespace ColorValley;

public partial class SplashPage : ContentPage
{

    public SplashPage()
    {
        InitializeComponent();
#if PRO_VERSION
        this.LabelAppNameColorValleyPro.IsVisible = true;
        this.LabelAppNameColorValley.IsVisible = false;

        this.LabelSplashWelcomePro.IsVisible = true;
        this.LabelSplashWelcome.IsVisible = false;
#else
        this.LabelAppNameColorValleyPro.IsVisible = false;
        this.LabelAppNameColorValley.IsVisible = true;

        this.LabelSplashWelcomePro.IsVisible = false;
        this.LabelSplashWelcome.IsVisible = true;
#endif
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

#if PRO_VERSION
        await ShowMainPageForProVersion();
#else
    await ShowMainPageAfterAd();
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
                        ShowMainPage();
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
            ShowMainPage();
        });
    }
#endif
    private static void ShowMainPage()
    {
        if (Application.Current?.Windows != null)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new MainPage());
        }
    }
}