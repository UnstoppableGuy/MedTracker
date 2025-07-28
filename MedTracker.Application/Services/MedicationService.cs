using MedTracker.Application.Interfaces;
using MedTracker.Core.Entities;
using MedTracker.Core.Interfaces;
using MedTracker.Core.Models;

namespace MedTracker.Infrastructure.Services
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

        public Task<Medication> CreateMedicationAsync(CreateMedicationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMedicationAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Medication>> GetAllMedicationsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Medication?> GetMedicationByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<MedicationStatistics> GetMedicationStatisticsAsync(int medicationId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MedicationExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Medication> UpdateMedicationAsync(int id, CreateMedicationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}