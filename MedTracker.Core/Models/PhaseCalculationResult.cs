using MedTracker.Core.Entities;

namespace MedTracker.Core.Models
{
    public class PhaseCalculationResult
    {
        public SchedulePhase? CurrentPhase { get; set; }
        public DateTime? NextPhaseStartDate { get; set; }
        public bool IsInBreakPeriod { get; set; }
        public DateTime? BreakEndDate { get; set; }
        public bool IsScheduleCompleted { get; set; }
    }
}