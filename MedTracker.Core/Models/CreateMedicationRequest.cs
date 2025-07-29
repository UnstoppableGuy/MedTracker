namespace MedTracker.Core.Models
{
    public class CreateMedicationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
    }
}