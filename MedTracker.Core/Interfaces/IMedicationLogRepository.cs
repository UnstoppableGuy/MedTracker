using MedTracker.Core.Entities;

namespace MedTracker.Core.Interfaces
{
    public interface IMedicationLogRepository
    {
        Task<IEnumerable<MedicationLog>> GetByMedicationIdAsync(int medicationId);
        Task<IEnumerable<MedicationLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MedicationLog>> GetTodayLogsAsync();
        Task<MedicationLog?> GetByIdAsync(int id);
        Task<MedicationLog?> GetLogForMedicationAtTimeAsync(int medicationId, DateTime targetTime);
        Task AddAsync(MedicationLog log);
        Task UpdateAsync(MedicationLog log);
        Task DeleteAsync(int id);
        Task<bool> IsMedicationTakenAsync(int medicationId, DateTime date);
    }
}