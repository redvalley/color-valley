using ColorValley.Properties;

namespace ColorValley.Models;

public class LevelSettings
{
    public const int LastLevel = 5;

    private readonly Random _random = new();

    public const int DefaultTotalTimeSeconds = 2;

    public const int DefaultGameTimerIntervallSeconds = 2;

    public const int DefaultBoxCount = 5;

    public const int DefaultRowCount = 3;

    public const int DefaultColumnCount = 3;

    public const int InitialLevel = 1;

    public const int DefaultBoxLength = 70;

    public static readonly IEnumerable<Color> DefaultColors = new List<Color> { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Purple };

    public int RowCount { get; set; } = DefaultRowCount;

    public int ColumnCount { get; set; } = DefaultColumnCount;

    public int Level { get; set; } = InitialLevel;

    public int TotalTimeSeconds { get; set; } = DefaultTotalTimeSeconds;

    public int GameTimerIntervallSeconds = DefaultGameTimerIntervallSeconds;

    public string Name { get; set; } = string.Empty;

    public int BoxCount { get; set; } = DefaultBoxCount;

    public int BoxLength { get; set; } = DefaultBoxLength;

    public int MiddleColumnIndex => ColumnCount / 2;

    public int MiddleRowIndex => RowCount / 2;

    public double BoxMargin { get; set; } = 5;

    public IEnumerable<Color> LevelColors = DefaultColors;


    public Color GetRandomColor()
    {
        return GetRandomColor(LevelColors);
    }

    public Color GetRandomColor(IEnumerable<Color> colorPalette)
    {
        var colors = colorPalette.ToList();
        
        return colors[_random.Next(colors.Count)];
    }

    public LinearGradientBrush GetRandomLinearGradientBrush()
    {
        var firstColor = GetRandomColor();
        var colorPalette = LevelColors.ToList();
        colorPalette.Remove(firstColor);
        var secondColor = GetRandomColor(colorPalette);

        return new LinearGradientBrush(
            [
                new GradientStop { Color = firstColor, Offset = 0.0f },
                new GradientStop { Color = secondColor, Offset = 1.0f }
            ],
            new Point(0, 0),
            new Point(1, 1));
    }

    public static LevelSettings CreateLevel1Settings()
    {

        return new LevelSettings()
        {
            Level = 1,
            Name = Resources.Level1Name,
        };
    }

    public static LevelSettings CreateLevel2Settings()
    {
        return new LevelSettings()
        {
            Level = 2,
            Name = Resources.Level2Name,
            GameTimerIntervallSeconds = 1
        };
    }

    public static LevelSettings CreateLevel3Settings()
    {
        return new LevelSettings()
        {
            Level = 3,
            Name = Resources.Level3Name,
            RowCount = 5,
            ColumnCount = 5,
            BoxCount = DefaultBoxCount * 2
        };
    }

    public static LevelSettings CreateLevel4Settings()
    {
        return new LevelSettings()
        {
            Level = 4,
            Name = Resources.Level4Name,
            RowCount = 5,
            ColumnCount = 5,
            BoxCount = DefaultBoxCount * 2,
            LevelColors = new List<Color> { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Purple, Colors.Orange, Colors.Pink, Colors.Aqua, Colors.Maroon }
        };
    }

    public static LevelSettings CreateLevel5Settings()
    {
        return new LevelSettings()
        {
            Level = 5,
            Name = Resources.Level5Name,
            RowCount = 5,
            ColumnCount = 5,
            BoxCount = DefaultBoxCount * 2,
            LevelColors = new List<Color> { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Purple, Colors.Orange, Colors.Pink, Colors.Aqua, Colors.Maroon },
            GameTimerIntervallSeconds = 1
        };
    }


}