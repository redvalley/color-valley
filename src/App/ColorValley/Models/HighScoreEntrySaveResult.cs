namespace ColorValley.Models;

public class HighScoreEntrySaveResult
{
    public bool WasSavedOnline { get; set; }

    public string SavedOnlineError { get; set; } = string.Empty;

    public bool WasSavedLocal { get; set; }

    public string SavedLocalError { get; set; } = string.Empty;

    public HighScoreEntry? Entry { get; set; }

}