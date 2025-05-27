namespace ColorValley;

public partial class CompanySplashPage : ContentPage
{

    public CompanySplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(2000);
        if (Application.Current?.Windows != null)
        {
            Application.Current.Windows[0].Page = new SplashPage();
        }

    }
}