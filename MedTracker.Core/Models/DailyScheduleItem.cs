namespace MedTracker.Core.Models
{
    public class DailyScheduleItem
    {
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public int DoseQuantity { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string Condition { get; set; } = string.Empty;
        public bool IsTaken { get; set; }
        public DateTime? TakenAt { get; set; }
        public int ScheduleId { get; set; }
        public int PhaseId { get; set; }
    }
}