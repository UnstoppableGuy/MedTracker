namespace MedTracker.Core.Models
{
    public class MedicationTakeRequest
    {
        public int MedicationId { get; set; }
        public DateTime TakenAt { get; set; } = DateTime.Now;
        public bool ConfirmedByUser { get; set; } = true;
    }
}