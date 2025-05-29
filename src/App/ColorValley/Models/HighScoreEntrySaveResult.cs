namespace ColorValley.Models;

public class HighScoreEntrySaveResult
{
    public bool WasSavedOnline { get; set; }

    public string SavedOnlineError { get; set; }

    public bool WasSavedLocal { get; set; }

    public string SavedLocalError { get; set; }

    public HighScoreEntry? Entry { get; set; }

}