using MedTracker.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedTracker.Core.Interfaces
{
    public interface IMedicationScheduleRepository
    {
        Task<IEnumerable<MedicationSchedule>> GetByMedicationIdAsync(int medicationId);
        Task<MedicationSchedule?> GetByIdAsync(int id);
        Task AddAsync(MedicationSchedule schedule);
        Task UpdateAsync(MedicationSchedule schedule);
        Task DeleteAsync(int id);
    }
}
