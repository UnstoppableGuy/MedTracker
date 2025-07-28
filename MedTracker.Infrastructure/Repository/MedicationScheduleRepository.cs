using MedTracker.Core.Entities;
using MedTracker.Core.Interfaces;
using MedTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedTracker.Infrastructure.Repository
{
    public class MedicationScheduleRepository : IMedicationScheduleRepository
    {
        private readonly AppDbContext _context;

        public MedicationScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicationSchedule>> GetByMedicationIdAsync(int medicationId)
        {
            return await _context.MedicationSchedules
                .Where(s => s.MedicationId == medicationId)
                .Include(s => s.Phases)
                .ToListAsync();
        }

        public async Task<MedicationSchedule?> GetByIdAsync(int id)
        {
            return await _context.MedicationSchedules
                .Include(s => s.Phases)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(MedicationSchedule schedule)
        {
            await _context.MedicationSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MedicationSchedule schedule)
        {
            _context.MedicationSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var schedule = await _context.MedicationSchedules.FindAsync(id);
            if (schedule != null)
            {
                _context.MedicationSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }
    }
}
