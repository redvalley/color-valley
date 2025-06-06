﻿using System.Text.Json.Serialization;
using ColorValley.Models;
using ColorValley.Settings;

namespace ColorValley;

public class AppUserSettings : UserSettings
{
    public const int MaxHighScoreEntries = 20;

    public const string DefaultPlayerName = "Color Valley"; 

    /// <summary>
    /// The list of high score entries
    /// </summary>
    [JsonInclude]
    public IEnumerable<HighScoreEntry> LocalHighScoreEntries { get; set; } = new List<HighScoreEntry>();


    /// <summary>
    /// The list of high score entries
    /// </summary>
    [JsonInclude]
    public IEnumerable<HighScoreEntry> LastOnlineHighScoreEntries { get; set; } = new List<HighScoreEntry>();

    /// <summary>
    /// Determines if the game was started for the first time.
    /// </summary>
    [JsonInclude]
    public bool IsGameStartedFirstTime { get; set; } = true;

    /// <summary>
    /// The name of the current player.
    /// </summary>
    [JsonInclude]
    public string PlayerName { get; set; } = DefaultPlayerName;

    /// <summary>
    /// Gets the current high score.
    /// </summary>
    public HighScoreEntry? GetLocalTopScore()
    {
        if (!LocalHighScoreEntries.Any())
        {
            return null;
        }
        return LocalHighScoreEntries.OrderByDescending(highScoreEntry => highScoreEntry.Score).First();
    }

    /// <summary>
    /// Add the specified high score to the high score list.
    /// </summary>
    /// <param name="newHighScoreEntry">The new high score entry for which a new high score should be created.</param>
    public bool AddLocalHighScore(HighScoreEntry newHighScoreEntry)
    {
        if (newHighScoreEntry.Score == 0)
        {
            return false;
        }

        if (LocalHighScoreEntries.Any(entry => entry.Score == newHighScoreEntry.Score && entry.Level == newHighScoreEntry.Level))
        {
            return false;
        }

        var currentHighScoreEntryList = LocalHighScoreEntries.ToList();
        currentHighScoreEntryList.Add(newHighScoreEntry);

        if (currentHighScoreEntryList.Count > MaxHighScoreEntries)
        {
            LocalHighScoreEntries = currentHighScoreEntryList.OrderByDescending(highScoreEntry => highScoreEntry.Score).Take(20);
        }
        else
        {
            LocalHighScoreEntries = currentHighScoreEntryList.OrderByDescending(highScoreEntry => highScoreEntry.Score).ToList();
        }

        return true;
    }
}