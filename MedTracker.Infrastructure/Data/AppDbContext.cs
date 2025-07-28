using MedTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;


namespace MedTracker.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly string _dbPath;

        public AppDbContext(string dbPath)
        {
            _dbPath = dbPath;
            Database.EnsureCreated();
        }

        public DbSet<Medication> Medications { get; set; }
        public DbSet<MedicationSchedule> MedicationSchedules { get; set; }
        public DbSet<SchedulePhase> SchedulePhases { get; set; }
        public DbSet<MedicationLog> MedicationLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={_dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MedicationSchedule>()
                .HasMany(s => s.Phases)
                .WithOne(p => p.Schedule)
                .HasForeignKey(p => p.ScheduleId);

            modelBuilder.Entity<Medication>()
                .HasMany(m => m.Schedules)
                .WithOne(s => s.Medication)
                .HasForeignKey(s => s.MedicationId);

            modelBuilder.Entity<MedicationLog>()
                .HasOne(l => l.Medication)
                .WithMany()
                .HasForeignKey(l => l.MedicationId);
        }
    }
}
