using MedTracker.Core.Entities;
using MedTracker.Core.Models;

namespace MedTracker.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<MedicationSchedule> CreateScheduleAsync(CreateScheduleRequest request);
        Task<MedicationSchedule> UpdateScheduleAsync(int scheduleId, CreateScheduleRequest request);
        Task DeleteScheduleAsync(int scheduleId);
        Task<MedicationSchedule?> GetScheduleByIdAsync(int scheduleId);
        Task<IEnumerable<MedicationSchedule>> GetSchedulesByMedicationIdAsync(int medicationId);
        
        Task<PhaseCalculationResult> GetCurrentPhaseAsync(int scheduleId, DateTime currentDate);
        Task<IEnumerable<DailyScheduleItem>> GetDailyScheduleAsync(DateTime date);
        Task<IEnumerable<DailyScheduleItem>> GetUpcomingScheduleAsync(DateTime fromDate, int daysAhead = 7);
        
        Task<ScheduleValidationResult> ValidateScheduleAsync(CreateScheduleRequest request);
        Task ActivateScheduleAsync(int scheduleId);
        Task DeactivateScheduleAsync(int scheduleId);
    }
}