using MedTracker.Core.Entities;
using MedTracker.Core.Models;

namespace MedTracker.AppServices.Interfaces
{
    public interface IMedicationLogService
    {
        Task<MedicationLog> RecordMedicationTakenAsync(MedicationTakeRequest request);
        Task<MedicationLog> UpdateMedicationLogAsync(int logId, MedicationTakeRequest request);
        Task DeleteMedicationLogAsync(int logId);
        
        Task<IEnumerable<MedicationLog>> GetMedicationHistoryAsync(int medicationId);
        Task<IEnumerable<MedicationLog>> GetHistoryByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MedicationLog>> GetTodayLogsAsync();
        
        Task<bool> IsMedicationTakenTodayAsync(int medicationId);
        Task<MedicationLog?> GetLogForScheduledTimeAsync(int medicationId, DateTime scheduledTime);
        
        Task MarkAsMissedAsync(int medicationId, DateTime scheduledTime);
        Task<IEnumerable<DailyScheduleItem>> UpdateScheduleWithLogsAsync(IEnumerable<DailyScheduleItem> schedule);
    }
}