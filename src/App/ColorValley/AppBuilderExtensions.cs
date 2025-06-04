using ColorValley.Services;
using Plugin.AdMob.Services;

namespace ColorValley;

public static class AppBuilderExtensions
{
    public static void AddAppServices(this MauiAppBuilder appBuilder)
    {
        appBuilder.Services.AddSingleton<IColorValleyAppOpenAdService>(
            provider => new ColorValleyAppOpenAdService(provider.GetRequiredService<IAppOpenAdService>(), AppSettings.AdMobAdUnitIdAppOpener) );

        appBuilder.Services.AddSingleton<IColorValleyInterstitualAdService>(
            provider => new ColorValleyInterstitualAdService(provider.GetRequiredService<IInterstitialAdService>(), AppSettings.AdMobAdUnitIdInterstitial));
    }
}