using MedTracker.Application.Interfaces;
using MedTracker.Core.Entities;
using MedTracker.Core.Interfaces;
using MedTracker.Core.Models;

namespace MedTracker.Application.Services
{
    public class MedicationLogService : IMedicationLogService
    {
        private readonly IMedicationLogRepository _logRepository;
        private readonly IMedicationRepository _medicationRepository;

        public MedicationLogService(
            IMedicationLogRepository logRepository,
            IMedicationRepository medicationRepository)
        {
            _logRepository = logRepository;
            _medicationRepository = medicationRepository;
        }

        public Task DeleteMedicationLogAsync(int logId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MedicationLog>> GetHistoryByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<MedicationLog?> GetLogForScheduledTimeAsync(int medicationId, DateTime scheduledTime)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MedicationLog>> GetMedicationHistoryAsync(int medicationId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MedicationLog>> GetTodayLogsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsMedicationTakenTodayAsync(int medicationId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsMissedAsync(int medicationId, DateTime scheduledTime)
        {
            throw new NotImplementedException();
        }

        public Task<MedicationLog> RecordMedicationTakenAsync(MedicationTakeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MedicationLog> UpdateMedicationLogAsync(int logId, MedicationTakeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DailyScheduleItem>> UpdateScheduleWithLogsAsync(IEnumerable<DailyScheduleItem> schedule)
        {
            throw new NotImplementedException();
        }
    }
}