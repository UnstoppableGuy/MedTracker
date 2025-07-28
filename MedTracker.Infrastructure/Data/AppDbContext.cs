using MedTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedTracker.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Medication> Medications { get; set; }
        public DbSet<MedicationSchedule> MedicationSchedules { get; set; }
        public DbSet<SchedulePhase> SchedulePhases { get; set; }
        public DbSet<MedicationLog> MedicationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Medication configuration
            modelBuilder.Entity<Medication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Form).HasMaxLength(50);
                entity.Property(e => e.Dosage).HasMaxLength(50);

                entity.HasMany(m => m.Schedules)
                      .WithOne(s => s.Medication)
                      .HasForeignKey(s => s.MedicationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // MedicationSchedule configuration
            modelBuilder.Entity<MedicationSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();

                entity.HasMany(s => s.Phases)
                      .WithOne(p => p.Schedule)
                      .HasForeignKey(p => p.ScheduleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // SchedulePhase configuration
            modelBuilder.Entity<SchedulePhase>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DoseQuantity).IsRequired();
                entity.Property(e => e.FrequencyPerDay).IsRequired();
                entity.Property(e => e.DurationInDays).IsRequired();
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.Condition).HasMaxLength(200);
                entity.Property(e => e.DoseTimesJson).HasColumnName("DoseTimesJson");
            });

            // MedicationLog configuration
            modelBuilder.Entity<MedicationLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TakenAt).IsRequired();
                entity.Property(e => e.ConfirmedByUser).IsRequired();

                entity.HasOne(l => l.Medication)
                      .WithMany()
                      .HasForeignKey(l => l.MedicationId)
                      .OnDelete(DeleteBehavior.Cascade);


                entity.HasIndex(e => new { e.MedicationId, e.TakenAt })
                      .HasDatabaseName("IX_MedicationLog_MedicationId_TakenAt");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}