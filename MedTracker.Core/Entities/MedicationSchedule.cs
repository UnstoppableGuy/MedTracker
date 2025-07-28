using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedTracker.Core.Entities
{
    public class MedicationSchedule
    {
        public int Id { get; set; }
        public int MedicationId { get; set; }
        public Medication Medication { get; set; } = null!;
        public List<SchedulePhase> Phases { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
