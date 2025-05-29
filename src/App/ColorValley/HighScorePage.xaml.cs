using ColorValley.Models;
using ColorValley.Services;
using ColorValley.Settings;
using ColorValley.ViewModels;

namespace ColorValley;

public partial class HighScorePage : ContentPage
{
    public HighScoreViewModel ViewModel { get; set; }


	public HighScorePage()
	{
		InitializeComponent();

        this.BindingContext = ViewModel = new HighScoreViewModel();
    }

    protected override async void OnAppearing()
    {
        var highScoreService = new HighScoreService();

        base.OnAppearing();
        ViewModel.LocalEntries = UserSettings.LoadDecrypted<AppUserSettings>()?.LocalHighScoreEntries?.GenerateRankedEntries() ??
            new List<HighScoreEntry>();

        var onlineResult = await highScoreService.GetOverallScores();
        ViewModel.OnlineEntries = onlineResult.Entries?.GenerateRankedEntries() ?? new List<HighScoreEntry>();
    }
}