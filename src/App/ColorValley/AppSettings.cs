using System.Security;

namespace ColorValley;

/// <summary>
/// Holds all app related settings
/// </summary>
public static class AppSettings
{
    public static string HighScoreServiceUrl { get; set; } = "https://sslsites.de/redvalley-software.com/apps/colorvalley/api.php/";

    public static string HashSalt = "7fbc49c403a8dd0da1ae7f24c27539094c7d0f3d14b5606ca9f756c06cd656e6";

#if IOS
    public static string AdMobAdUnitIdAppOpener = "ca-app-pub-6864374918270893/6262661560";

    public static string AdMobAdUnitIdInterstitial = "ca-app-pub-6864374918270893/1009817964";

    public static string AdMobAdUnitIdMainBanner = "ca-app-pub-6864374918270893/8745430657";

#else
    public static string AdMobAdUnitIdAppOpener = "ca-app-pub-6864374918270893/3232269588";

    public static string AdMobAdUnitIdInterstitial = "ca-app-pub-6864374918270893/4065633825";

    public static string AdMobAdUnitIdMainBanner = "ca-app-pub-6864374918270893/6672681529";
#endif
}