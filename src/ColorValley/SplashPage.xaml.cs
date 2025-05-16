using ACAB.App;
using Plugin.AdMob;
using Plugin.AdMob.Services;
using Debug = System.Diagnostics.Debug;

namespace ColorValley;

public partial class SplashPage : ContentPage
{
    private IInterstitialAdService? _interstitialAdService;
    private IInterstitialAd _interstitialAd;

    public SplashPage()
    {
        InitializeComponent();
        _interstitialAdService = IPlatformApplication.Current.Services.GetService<IInterstitialAdService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _interstitialAd = _interstitialAdService.CreateAd("ca-app-pub-6864374918270893/4065633825");
        _interstitialAd.OnAdLoaded += InterstitialAd_OnAdLoaded;
        _interstitialAd.OnAdFailedToLoad += InterstitialAdOnOnAdFailedToLoad;
        await Task.Delay(3000);
        _interstitialAd.Load();

    }

    private void InterstitialAdOnOnAdFailedToLoad(object? sender, IAdError e)
    {
        if (Application.Current?.Windows != null)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new MainPage());
        }
    }

    private void InterstitialAd_OnAdLoaded(object? sender, EventArgs e)
    {
        _interstitialAd.Show();
        if (Application.Current?.Windows != null)
        {
            Application.Current.Windows[0].Page = new NavigationPage(new MainPage());
        }
    }

}