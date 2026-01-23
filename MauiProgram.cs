using Microsoft.Extensions.Logging;
using SecureJournal.Components.Data.Services; // Using this to register JournalDB (SQLite service)

namespace SecureJournal
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Using this to enable MAUI Blazor Hybrid
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            // Using this for Blazor debugging tools while developing
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Using this to register SQLite service as Singleton (one shared DB helper)
            builder.Services.AddSingleton<JournalDB>();

            return builder.Build();
        }
    }
}
