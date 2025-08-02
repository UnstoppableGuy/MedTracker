using MedTracker.AppServices.Interfaces;
using MedTracker.AppServices.Services;
using MedTracker.Core.Interfaces;
using MedTracker.Infrastructure.Data;
using MedTracker.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedTracker
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
            builder.Services.AddScoped<IMedicationScheduleRepository, MedicationScheduleRepository>();
            builder.Services.AddScoped<IMedicationLogRepository, MedicationLogRepository>();

            builder.Services.AddScoped<IMedicationService, MedicationService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<IMedicationLogService, MedicationLogService>();

            //Pages
            //builder.Services.AddTransient<MainPage>();


            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={GetDatabasePath()}"));
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static string GetDatabasePath()
        {
            var databasePath = "";
            var databaseName = "medtracker.db";

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                databasePath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
            }
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                SQLitePCL.Batteries_V2.Init();
                databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", databaseName); ;
            }

            return databasePath;

        }
    }
}
