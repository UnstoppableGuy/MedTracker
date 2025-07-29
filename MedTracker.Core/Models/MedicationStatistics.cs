namespace MedTracker.Core.Models
{
    public class MedicationStatistics
    {
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public int TotalScheduledDoses { get; set; }
        public int TakenDoses { get; set; }
        public int MissedDoses { get; set; }
        public double CompliancePercentage { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}