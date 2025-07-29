namespace MedTracker.Core.Models
{
    public class CreatePhaseRequest
    {
        public int DoseQuantity { get; set; }
        public int FrequencyPerDay { get; set; }
        public int DurationInDays { get; set; }
        public DateTime StartDate { get; set; }
        public string Condition { get; set; } = "без условий";
        public List<TimeOnly> DoseTimes { get; set; } = new();
        public int BreakAfterDays { get; set; } = 0;
    }
}