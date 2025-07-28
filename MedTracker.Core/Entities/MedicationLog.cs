using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedTracker.Core.Entities
{
    public class MedicationLog
    {
        public int Id { get; set; }
        public int MedicationId { get; set; }
        public Medication Medication { get; set; } = null!;
        public DateTime TakenAt { get; set; }
        public bool ConfirmedByUser { get; set; } = false;
    }
}
