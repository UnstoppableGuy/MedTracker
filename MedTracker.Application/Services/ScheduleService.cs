using MedTracker.Application.Interfaces;
using MedTracker.Core.Entities;
using MedTracker.Core.Interfaces;
using MedTracker.Core.Models;

namespace MedTracker.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IMedicationRepository _medicationRepository;
        private readonly IMedicationScheduleRepository _scheduleRepository;
        public ScheduleService(
            IMedicationRepository medicationRepository,
            IMedicationScheduleRepository scheduleRepository)
        {
            _medicationRepository = medicationRepository;
            _scheduleRepository = scheduleRepository;        }

        public async Task<MedicationSchedule> CreateScheduleAsync(CreateScheduleRequest request)
        {
            var validation = await ValidateScheduleAsync(request);
            if (!validation.IsValid)
                throw new ArgumentException($"Неверное расписание: {string.Join(", ", validation.Errors)}");

            var schedule = new MedicationSchedule
            {
                MedicationId = request.MedicationId,
                CreatedAt = DateTime.Now,
                IsActive = true,
                Phases = request.Phases.Select(p => new SchedulePhase
                {
                    DoseQuantity = p.DoseQuantity,
                    FrequencyPerDay = p.FrequencyPerDay,
                    DurationInDays = p.DurationInDays,
                    StartDate = p.StartDate,
                    Condition = p.Condition,
                    DoseTimes = p.DoseTimes,
                    BreakAfterDays = p.BreakAfterDays
                }).ToList()
            };

            await _scheduleRepository.AddAsync(schedule);
            return schedule;
        }

        public async Task<MedicationSchedule> UpdateScheduleAsync(int scheduleId, CreateScheduleRequest request)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new ArgumentException($"Расписание с ID {scheduleId} не найдено");

            var validation = await ValidateScheduleAsync(request);
            if (!validation.IsValid)
                throw new ArgumentException($"Неверное расписание: {string.Join(", ", validation.Errors)}");

            schedule.MedicationId = request.MedicationId;
            schedule.Phases = request.Phases.Select(p => new SchedulePhase
            {
                ScheduleId = scheduleId,
                DoseQuantity = p.DoseQuantity,
                FrequencyPerDay = p.FrequencyPerDay,
                DurationInDays = p.DurationInDays,
                StartDate = p.StartDate,
                Condition = p.Condition,
                DoseTimes = p.DoseTimes,
                BreakAfterDays = p.BreakAfterDays
            }).ToList();

            await _scheduleRepository.UpdateAsync(schedule);
            return schedule;
        }

        public async Task DeleteScheduleAsync(int scheduleId)
        {
            await _scheduleRepository.DeleteAsync(scheduleId);
        }

        public async Task<MedicationSchedule?> GetScheduleByIdAsync(int scheduleId)
        {
            return await _scheduleRepository.GetByIdAsync(scheduleId);
        }

        public async Task<IEnumerable<MedicationSchedule>> GetSchedulesByMedicationIdAsync(int medicationId)
        {
            return await _scheduleRepository.GetByMedicationIdAsync(medicationId);
        }

        public async Task<PhaseCalculationResult> GetCurrentPhaseAsync(int scheduleId, DateTime currentDate)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                return new PhaseCalculationResult { IsScheduleCompleted = true };

            var result = new PhaseCalculationResult();
            DateTime currentPhaseStart = DateTime.MinValue;

            foreach (var phase in schedule.Phases.OrderBy(p => p.StartDate))
            {
                var phaseStart = phase.StartDate.Date;
                var phaseEnd = phaseStart.AddDays(phase.DurationInDays);
                var breakEnd = phaseEnd.AddDays(phase.BreakAfterDays);

                if (currentDate >= phaseStart && currentDate < phaseEnd)
                {
                    result.CurrentPhase = phase;
                    result.IsInBreakPeriod = false;
                    result.NextPhaseStartDate = breakEnd;
                    return result;
                }

                if (phase.BreakAfterDays > 0 && currentDate >= phaseEnd && currentDate < breakEnd)
                {
                    result.IsInBreakPeriod = true;
                    result.BreakEndDate = breakEnd;
                    return result;
                }

                currentPhaseStart = breakEnd;
            }

            result.IsScheduleCompleted = true;
            return result;
        }

        public async Task<IEnumerable<DailyScheduleItem>> GetDailyScheduleAsync(DateTime date)
        {
            var medications = await _medicationRepository.GetAllAsync();
            var scheduleItems = new List<DailyScheduleItem>();

            foreach (var medication in medications)
            {
                foreach (var schedule in medication.Schedules.Where(s => s.IsActive))
                {
                    var phaseResult = await GetCurrentPhaseAsync(schedule.Id, date);

                    if (phaseResult.CurrentPhase != null && !phaseResult.IsInBreakPeriod)
                    {
                        var phase = phaseResult.CurrentPhase;

                        foreach (var doseTime in phase.DoseTimes)
                        {
                            var scheduledDateTime = date.Date.Add(doseTime.ToTimeSpan());

                            scheduleItems.Add(new DailyScheduleItem
                            {
                                MedicationId = medication.Id,
                                MedicationName = medication.Name,
                                Dosage = medication.Dosage,
                                DoseQuantity = phase.DoseQuantity,
                                ScheduledTime = scheduledDateTime,
                                Condition = phase.Condition,
                                ScheduleId = schedule.Id,
                                PhaseId = phase.Id
                            });
                        }
                    }
                }
            }

            return scheduleItems.OrderBy(s => s.ScheduledTime);
        }

        public async Task<IEnumerable<DailyScheduleItem>> GetUpcomingScheduleAsync(DateTime fromDate, int daysAhead = 7)
        {
            var allItems = new List<DailyScheduleItem>();

            for (int i = 0; i < daysAhead; i++)
            {
                var date = fromDate.AddDays(i);
                var dailyItems = await GetDailyScheduleAsync(date);
                allItems.AddRange(dailyItems);
            }

            return allItems.OrderBy(s => s.ScheduledTime);
        }

        public async Task<ScheduleValidationResult> ValidateScheduleAsync(CreateScheduleRequest request)
        {
            var result = new ScheduleValidationResult { IsValid = true };

            // Проверка существования лекарства
            var medicationExists = await _medicationRepository.ExistsAsync(request.MedicationId);
            if (!medicationExists)
            {
                result.IsValid = false;
                result.Errors.Add("Лекарство не найдено");
                return result;
            }

            // Проверка фаз
            if (!request.Phases.Any())
            {
                result.IsValid = false;
                result.Errors.Add("Должна быть хотя бы одна фаза");
                return result;
            }

            foreach (var phase in request.Phases)
            {
                if (phase.DoseQuantity <= 0)
                {
                    result.IsValid = false;
                    result.Errors.Add("Количество доз должно быть больше нуля");
                }

                if (phase.FrequencyPerDay <= 0)
                {
                    result.IsValid = false;
                    result.Errors.Add("Частота приема должна быть больше нуля");
                }

                if (phase.DurationInDays <= 0)
                {
                    result.IsValid = false;
                    result.Errors.Add("Продолжительность фазы должна быть больше нуля");
                }

                if (phase.DoseTimes.Count != phase.FrequencyPerDay)
                {
                    result.IsValid = false;
                    result.Errors.Add("Количество времен приема должно соответствовать частоте");
                }

                if (phase.StartDate < DateTime.Today)
                {
                    result.Warnings.Add("Дата начала фазы в прошлом");
                }
            }

            return result;
        }

        public async Task ActivateScheduleAsync(int scheduleId)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule != null)
            {
                schedule.IsActive = true;
                await _scheduleRepository.UpdateAsync(schedule);
            }
        }

        public async Task DeactivateScheduleAsync(int scheduleId)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule != null)
            {
                schedule.IsActive = false;
                await _scheduleRepository.UpdateAsync(schedule);
            }
        }
    }
}