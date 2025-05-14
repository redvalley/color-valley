using ColorValley.Models;
using ColorValley.Settings;

namespace ColorValley;

public partial class HighScorePage : ContentPage
{
	public HighScorePage()
	{
		InitializeComponent();
        this.BindingContext =
            UserSettings.LoadDecrypted<AppUserSettings>()?.HighScoreEntries?.GenerateRankedEntries() ??
            new List<HighScoreEntry>();
    }
}