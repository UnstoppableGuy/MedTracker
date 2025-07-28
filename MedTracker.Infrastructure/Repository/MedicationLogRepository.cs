using MedTracker.Core.Entities;
using MedTracker.Core.Interfaces;
using MedTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MedTracker.Infrastructure.Repository
{
    public class MedicationLogRepository : IMedicationLogRepository
    {
        private readonly AppDbContext _context;

        public MedicationLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicationLog>> GetByMedicationIdAsync(int medicationId)
        {
            return await _context.MedicationLogs
                .Where(l => l.MedicationId == medicationId)
                .Include(l => l.Medication)
                .OrderByDescending(l => l.TakenAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.MedicationLogs
                .Where(l => l.TakenAt >= startDate && l.TakenAt <= endDate)
                .Include(l => l.Medication)
                .OrderByDescending(l => l.TakenAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicationLog>> GetTodayLogsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            return await _context.MedicationLogs
                .Where(l => l.TakenAt >= today && l.TakenAt < tomorrow)
                .Include(l => l.Medication)
                .OrderBy(l => l.TakenAt)
                .ToListAsync();
        }

        public async Task<MedicationLog?> GetByIdAsync(int id)
        {
            return await _context.MedicationLogs
                .Include(l => l.Medication)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<MedicationLog?> GetLogForMedicationAtTimeAsync(int medicationId, DateTime targetTime)
        {
            var startTime = targetTime.AddMinutes(-30);
            var endTime = targetTime.AddMinutes(30);

            return await _context.MedicationLogs
                .FirstOrDefaultAsync(l => l.MedicationId == medicationId 
                                        && l.TakenAt >= startTime 
                                        && l.TakenAt <= endTime);
        }

        public async Task AddAsync(MedicationLog log)
        {
            await _context.MedicationLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MedicationLog log)
        {
            _context.MedicationLogs.Update(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var log = await _context.MedicationLogs.FindAsync(id);
            if (log != null)
            {
                _context.MedicationLogs.Remove(log);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsMedicationTakenAsync(int medicationId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.MedicationLogs
                .AnyAsync(l => l.MedicationId == medicationId 
                             && l.TakenAt >= startOfDay 
                             && l.TakenAt < endOfDay
                             && l.ConfirmedByUser);
        }
    }
}