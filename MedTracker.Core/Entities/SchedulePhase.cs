using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedTracker.Core.Entities
{
    public class SchedulePhase
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public MedicationSchedule Schedule { get; set; } = null!;

        public int DoseQuantity { get; set; }
        public int FrequencyPerDay { get; set; }
        public int DurationInDays { get; set; } 
        public DateTime StartDate { get; set; } 
        public string Condition { get; set; } = "без условий"; 
        public List<TimeOnly> DoseTimes { get; set; } = new();
        public int BreakAfterDays { get; set; } = 0;
    }
}
