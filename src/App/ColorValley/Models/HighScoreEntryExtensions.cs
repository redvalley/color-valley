namespace ColorValley.Models;

public static class HighScoreEntryExtensions
{
    public static IEnumerable<HighScoreEntry> GenerateRankedEntries(this IEnumerable<HighScoreEntry> entries)
    {
        var orderedEntries = entries.OrderByDescending(entry => entry.Score).ToList();
        var rankedEntries = new List<HighScoreEntry>();

        int position = 1;
        foreach (var orderedEntry in orderedEntries)
        {
            if (orderedEntry.Score < 1)
            {
                continue;
            }

            if (rankedEntries.Exists(entry => entry.Score == orderedEntry.Score))
            {
                continue;
            }

            var rankedEntry = new HighScoreEntry()
            {
                Score = orderedEntry.Score,
                Name = orderedEntry.Name,
                RankOrTrophy = GetRankOrTrophy(position)
            };

            

            rankedEntries.Add(rankedEntry);
            position++;
        }

        return rankedEntries;
    }

    public static string GetRankOrTrophy(int position)
    {
        if (position == 1)
        {
            return $"{position} 🏆";
        }

        if (position == 2)
        {
            return $"{position} 🥈";
        }

        if (position == 3)
        {
            return $"{position} 🥉";
        }

        if (position == 4)
        {
            return $"{position} 🏅";
        }

        if (position == 5)
        {
            return $"{position} 🏅";
        }

        return position.ToString();
    }
}