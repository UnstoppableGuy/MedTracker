using MedTracker.Core.Entities;
using MedTracker.Core.Interfaces;
using MedTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace MedTracker.Infrastructure.Repository
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly AppDbContext _context;

        public MedicationRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Medication>> GetAllAsync()
        {
            return await _context.Medications
                .Include(m => m.Schedules)
                .ThenInclude(s => s.Phases)
                .ToListAsync();
        }

        public async Task<Medication?> GetByIdAsync(int id)
        {
            return await _context.Medications
                .Include(m => m.Schedules)
                .ThenInclude(s => s.Phases)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Medication medication)
        {
            await _context.Medications.AddAsync(medication);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Medication medication)
        {
            _context.Medications.Update(medication);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication != null)
            {
                _context.Medications.Remove(medication);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Medications.AnyAsync(m => m.Id == id);
        }
    }
}
