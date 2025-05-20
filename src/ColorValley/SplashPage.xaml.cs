using ACAB.App;
using Android.Gms.Ads.Interstitial;
using Plugin.AdMob;
using Plugin.AdMob.Services;
using Debug = System.Diagnostics.Debug;

namespace ColorValley;

public partial class SplashPage : ContentPage
{

    public SplashPage()
    {
        InitializeComponent();
        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

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

    private static void ShowMainPage()
    {
        if (Application.Current?.Windows != null)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new MainPage());
        }
    }
}