
using Microsoft.Extensions.Logging;

#if !PRO_VERSION
using Plugin.AdMob;
using Plugin.AdMob.Configuration;
#endif

namespace ColorValley
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
#if DEBUG
#if !PRO_VERSION
            AdConfig.UseTestAdUnitIds = true;
#endif      
#endif
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<ColorValley.App>()
#if !PRO_VERSION
                .UseAdMob(automaticallyAskForConsent: false)
#endif
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif


            return builder.Build();
        }
    }
}
