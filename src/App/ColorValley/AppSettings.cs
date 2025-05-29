using System.Security;

namespace ColorValley;

/// <summary>
/// Holds all app related settings
/// </summary>
public static class AppSettings
{
    public static string HighScoreServiceUrl { get; set; } = "https://sslsites.de/redvalley-software.com/apps/colorvalley/api.php/";

    public static string HashSalt = "7fbc49c403a8dd0da1ae7f24c27539094c7d0f3d14b5606ca9f756c06cd656e6";
}