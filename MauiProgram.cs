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

            // Database connection service
            builder.Services.AddSingleton<AppSqliteDb>();

            // Entry CRUD + search + stats service
            builder.Services.AddSingleton<JournalEntryService>();

            // Selected date state for calendar pages
            builder.Services.AddSingleton<CalendarStateService>();

            // PIN lock state and validation service
            builder.Services.AddSingleton<PinLockService>();

            // PDF export service
            builder.Services.AddSingleton<PdfExportService>();

            // Popup notification service (errors + validation messages)
            builder.Services.AddSingleton<NotificationService>();

            return builder.Build();
        }
    }
}
