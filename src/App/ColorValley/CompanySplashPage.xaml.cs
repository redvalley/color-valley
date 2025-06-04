using ColorValley.Services;
using Plugin.AdMob.Services;

namespace ColorValley;

public partial class CompanySplashPage : ContentPage
{
#if!PRO_VERSION
    private readonly IColorValleyInterstitualAdService _colorValleyInterstitualAdService;
#endif

    public CompanySplashPage()
    {
        InitializeComponent();
    }

#if PRO_VERSION
    public SplashPage()
    {
        InitializeComponent();
    }
#else
    public CompanySplashPage(IColorValleyInterstitualAdService colorValleyInterstitualAdService)
    {
        _colorValleyInterstitualAdService = colorValleyInterstitualAdService;
        InitializeComponent();
    }
#endif


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(2000);
        if (Application.Current?.Windows != null)
        {
#if !PRO_VERSION
            Application.Current.Windows[0].Page = new NavigationPage(new MainPage(_colorValleyInterstitualAdService));
#else
            Application.Current.Windows[0].Page = new NavigationPage(new MainPage());
#endif
        }

    }
}