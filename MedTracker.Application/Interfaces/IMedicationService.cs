using MedTracker.Core.Entities;
using MedTracker.Core.Models;

namespace MedTracker.AppServices.Interfaces
{
    public interface IMedicationService
    {
        Task<IEnumerable<Medication>> GetAllMedicationsAsync();
        Task<Medication?> GetMedicationByIdAsync(int id);
        Task<Medication> CreateMedicationAsync(CreateMedicationRequest request);
        Task<Medication> UpdateMedicationAsync(int id, CreateMedicationRequest request);
        Task DeleteMedicationAsync(int id);
        Task<bool> MedicationExistsAsync(int id);
        Task<MedicationStatistics> GetMedicationStatisticsAsync(int medicationId, DateTime startDate, DateTime endDate);
    }
}