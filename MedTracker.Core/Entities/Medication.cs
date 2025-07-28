using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedTracker.Core.Entities
{
    public class Medication
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public List<MedicationSchedule> Schedules { get; set; } = [];
    }
}
