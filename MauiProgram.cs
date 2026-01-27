using Microsoft.Extensions.Logging;
using SecureJournal.Data;
using SecureJournal.Data.Services;


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

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Database + services (same “simple services + events” pattern as reference)
            builder.Services.AddSingleton<AppSqliteDb>();
            builder.Services.AddSingleton<JournalEntryService>();
            builder.Services.AddSingleton<CalendarStateService>();
            builder.Services.AddSingleton<PinLockService>();
            builder.Services.AddSingleton<PdfExportService>();

            return builder.Build();
        }
    }
}
