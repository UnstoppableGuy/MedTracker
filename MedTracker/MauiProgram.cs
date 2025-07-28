using MedTracker.Application.Interfaces;
using MedTracker.Application.Services;
using MedTracker.Core.Interfaces;
using MedTracker.Infrastructure.Data;
using MedTracker.Infrastructure.Repository;
using MedTracker.Infrastructure.Services;
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

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "medtracker.db");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
