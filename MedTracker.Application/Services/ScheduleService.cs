using MedTracker.Application.Interfaces;
using MedTracker.Core.Entities;
using MedTracker.Core.Interfaces;
using MedTracker.Core.Models;

namespace MedTracker.Infrastructure.Services
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
            _scheduleRepository = scheduleRepository;
        }

        public Task ActivateScheduleAsync(int scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<MedicationSchedule> CreateScheduleAsync(CreateScheduleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task DeactivateScheduleAsync(int scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteScheduleAsync(int scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<PhaseCalculationResult> GetCurrentPhaseAsync(int scheduleId, DateTime currentDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DailyScheduleItem>> GetDailyScheduleAsync(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<MedicationSchedule?> GetScheduleByIdAsync(int scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MedicationSchedule>> GetSchedulesByMedicationIdAsync(int medicationId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DailyScheduleItem>> GetUpcomingScheduleAsync(DateTime fromDate, int daysAhead = 7)
        {
            throw new NotImplementedException();
        }

        public Task<MedicationSchedule> UpdateScheduleAsync(int scheduleId, CreateScheduleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ScheduleValidationResult> ValidateScheduleAsync(CreateScheduleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}