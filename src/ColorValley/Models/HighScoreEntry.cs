using System.Text.Json.Serialization;

namespace ColorValley.Models;

/// <summary>
/// This class holds a high score entry.
/// </summary>
public class HighScoreEntry
{
    /// <summary>
    /// The rank or trophy
    /// </summary>
    public string RankOrTrophy { get; set; }


    /// <summary>
    /// The high score value.
    /// </summary>
    [JsonInclude]
    public int Score { get; set; }

    /// <summary>
    /// The name of the person that has created the high score.
    /// </summary>
    [JsonInclude]
    public string Name { get; set; }
}

