using MedTracker.AppServices.Interfaces;
using MedTracker.Core.Entities;
using MedTracker.Core.Interfaces;
using MedTracker.Core.Models;

namespace MedTracker.AppServices.Services
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

        public async Task<MedicationLog> RecordMedicationTakenAsync(MedicationTakeRequest request)
        {
            var medicationExists = await _medicationRepository.ExistsAsync(request.MedicationId);
            if (!medicationExists)
                throw new ArgumentException($"Лекарство с ID {request.MedicationId} не найдено");

            var log = new MedicationLog
            {
                MedicationId = request.MedicationId,
                TakenAt = request.TakenAt,
                ConfirmedByUser = request.ConfirmedByUser
            };

            await _logRepository.AddAsync(log);
            return log;
        }

        public async Task<MedicationLog> UpdateMedicationLogAsync(int logId, MedicationTakeRequest request)
        {
            var log = await _logRepository.GetByIdAsync(logId);
            if (log == null)
                throw new ArgumentException($"Запись о приеме с ID {logId} не найдена");

            var medicationExists = await _medicationRepository.ExistsAsync(request.MedicationId);
            if (!medicationExists)
                throw new ArgumentException($"Лекарство с ID {request.MedicationId} не найдено");

            log.MedicationId = request.MedicationId;
            log.TakenAt = request.TakenAt;
            log.ConfirmedByUser = request.ConfirmedByUser;

            await _logRepository.UpdateAsync(log);
            return log;
        }

        public async Task DeleteMedicationLogAsync(int logId)
        {
            await _logRepository.DeleteAsync(logId);
        }

        public async Task<IEnumerable<MedicationLog>> GetMedicationHistoryAsync(int medicationId)
        {
            var medicationExists = await _medicationRepository.ExistsAsync(medicationId);
            if (!medicationExists)
                throw new ArgumentException($"Лекарство с ID {medicationId} не найдено");

            return await _logRepository.GetByMedicationIdAsync(medicationId);
        }

        public async Task<IEnumerable<MedicationLog>> GetHistoryByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Дата начала не может быть больше даты окончания");

            return await _logRepository.GetByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<MedicationLog>> GetTodayLogsAsync()
        {
            return await _logRepository.GetTodayLogsAsync();
        }

        public async Task<bool> IsMedicationTakenTodayAsync(int medicationId)
        {
            return await _logRepository.IsMedicationTakenAsync(medicationId, DateTime.Today);
        }

        public async Task<MedicationLog?> GetLogForScheduledTimeAsync(int medicationId, DateTime scheduledTime)
        {
            return await _logRepository.GetLogForMedicationAtTimeAsync(medicationId, scheduledTime);
        }

        public async Task MarkAsMissedAsync(int medicationId, DateTime scheduledTime)
        {
            var existingLog = await GetLogForScheduledTimeAsync(medicationId, scheduledTime);
            if (existingLog == null)
            {
                var missedLog = new MedicationLog
                {
                    MedicationId = medicationId,
                    TakenAt = scheduledTime,
                    ConfirmedByUser = false // Не подтверждено пользователем = пропущено
                };

                await _logRepository.AddAsync(missedLog);
            }
        }

        public async Task<IEnumerable<DailyScheduleItem>> UpdateScheduleWithLogsAsync(IEnumerable<DailyScheduleItem> schedule)
        {
            var scheduleList = schedule.ToList();
            var dateRange = scheduleList.Select(s => s.ScheduledTime.Date).Distinct();

            var allLogs = new List<MedicationLog>();
            foreach (var date in dateRange)
            {
                var dayLogs = await _logRepository.GetByDateRangeAsync(date, date.AddDays(1).AddSeconds(-1));
                allLogs.AddRange(dayLogs);
            }

            foreach (var item in scheduleList)
            {
                var relevantLog = allLogs.FirstOrDefault(l =>
                    l.MedicationId == item.MedicationId &&
                    Math.Abs((l.TakenAt - item.ScheduledTime).TotalMinutes) <= 30);

                if (relevantLog != null)
                {
                    item.IsTaken = relevantLog.ConfirmedByUser;
                    item.TakenAt = relevantLog.TakenAt;
                }
            }

            return scheduleList;
        }
    }
}