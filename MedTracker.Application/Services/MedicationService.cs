using MedTracker.AppServices.Interfaces;
using MedTracker.Core.Entities;
using MedTracker.Core.Interfaces;
using MedTracker.Core.Models;

namespace MedTracker.AppServices.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationRepository _medicationRepository;
        private readonly IMedicationLogRepository _logRepository;

        public MedicationService(
            IMedicationRepository medicationRepository,
            IMedicationLogRepository logRepository)
        {
            _medicationRepository = medicationRepository;
            _logRepository = logRepository;
        }

        public async Task<IEnumerable<Medication>> GetAllMedicationsAsync()
        {
            return await _medicationRepository.GetAllAsync();
        }

        public async Task<Medication?> GetMedicationByIdAsync(int id)
        {
            return await _medicationRepository.GetByIdAsync(id);
        }

        public async Task<Medication> CreateMedicationAsync(CreateMedicationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Название лекарства не может быть пустым");

            var medication = new Medication
            {
                Name = request.Name.Trim(),
                Form = request.Form.Trim(),
                Dosage = request.Dosage.Trim()
            };

            await _medicationRepository.AddAsync(medication);
            return medication;
        }

        public async Task<Medication> UpdateMedicationAsync(int id, CreateMedicationRequest request)
        {
            var medication = await _medicationRepository.GetByIdAsync(id);
            if (medication == null)
                throw new ArgumentException($"Лекарство с ID {id} не найдено");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Название лекарства не может быть пустым");

            medication.Name = request.Name.Trim();
            medication.Form = request.Form.Trim();
            medication.Dosage = request.Dosage.Trim();

            await _medicationRepository.UpdateAsync(medication);
            return medication;
        }

        public async Task DeleteMedicationAsync(int id)
        {
            var exists = await _medicationRepository.ExistsAsync(id);
            if (!exists)
                throw new ArgumentException($"Лекарство с ID {id} не найдено");

            await _medicationRepository.DeleteAsync(id);
        }

        public async Task<bool> MedicationExistsAsync(int id)
        {
            return await _medicationRepository.ExistsAsync(id);
        }

        public async Task<MedicationStatistics> GetMedicationStatisticsAsync(int medicationId, DateTime startDate, DateTime endDate)
        {
            var medication = await _medicationRepository.GetByIdAsync(medicationId);
            if (medication == null)
                throw new ArgumentException($"Лекарство с ID {medicationId} не найдено");

            var logs = await _logRepository.GetByDateRangeAsync(startDate, endDate);
            var medicationLogs = logs.Where(l => l.MedicationId == medicationId).ToList();

            var takenDoses = medicationLogs.Count(l => l.ConfirmedByUser);
            var totalDoses = CalculateScheduledDoses(medication, startDate, endDate);

            return new MedicationStatistics
            {
                MedicationId = medicationId,
                MedicationName = medication.Name,
                TotalScheduledDoses = totalDoses,
                TakenDoses = takenDoses,
                MissedDoses = Math.Max(0, totalDoses - takenDoses),
                CompliancePercentage = totalDoses > 0 ? (double)takenDoses / totalDoses * 100 : 0,
                PeriodStart = startDate,
                PeriodEnd = endDate
            };
        }

        private int CalculateScheduledDoses(Medication medication, DateTime startDate, DateTime endDate)
        {
            // Упрощенный расчет - в реальности нужно учитывать все фазы и расписания
            int totalDoses = 0;

            foreach (var schedule in medication.Schedules.Where(s => s.IsActive))
            {
                foreach (var phase in schedule.Phases)
                {
                    var phaseStart = phase.StartDate.Date;
                    var phaseEnd = phaseStart.AddDays(phase.DurationInDays);

                    // Пересечение с запрашиваемым периодом
                    var effectiveStart = phaseStart > startDate ? phaseStart : startDate;
                    var effectiveEnd = phaseEnd < endDate ? phaseEnd : endDate;

                    if (effectiveStart <= effectiveEnd)
                    {
                        var daysInPeriod = (effectiveEnd - effectiveStart).Days + 1;
                        totalDoses += daysInPeriod * phase.FrequencyPerDay;
                    }
                }
            }

            return totalDoses;
        }
    }
}