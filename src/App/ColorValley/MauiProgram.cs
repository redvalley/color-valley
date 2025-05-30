
using Microsoft.Extensions.Logging;
using Plugin.AdMob.Configuration;
#if !PRO_VERSION
using Plugin.AdMob;
#endif

namespace ColorValley
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
#if DEBUG
            AdConfig.UseTestAdUnitIds = true;
            
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
