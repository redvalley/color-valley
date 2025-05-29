namespace ColorValley.Models;

public class GetOverallScoresResult
{
    public IEnumerable<HighScoreEntry> Entries { get; set; } = new List<HighScoreEntry>();

    public string ErrorMessage { get; set; } = string.Empty;

    public bool HasError => string.IsNullOrEmpty(ErrorMessage);

}