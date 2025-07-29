namespace MedTracker.Core.Models
{
    public class CreateScheduleRequest
    {
        public int MedicationId { get; set; }
        public List<CreatePhaseRequest> Phases { get; set; } = new();
    }
}