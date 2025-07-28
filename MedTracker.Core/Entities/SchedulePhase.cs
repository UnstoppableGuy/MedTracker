using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        public int BreakAfterDays { get; set; } = 0;

        [Column("DoseTimesJson")]
        public string DoseTimesJson { get; set; } = "[]";

        [NotMapped]
        public List<TimeOnly> DoseTimes
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(DoseTimesJson))
                        return new List<TimeOnly>();

                    var timeStrings = JsonSerializer.Deserialize<List<string>>(DoseTimesJson) ?? new List<string>();
                    return timeStrings.Select(TimeOnly.Parse).ToList();
                }
                catch
                {
                    return new List<TimeOnly>();
                }
            }
            set
            {
                try
                {
                    var timeStrings = value?.Select(t => t.ToString("HH:mm")).ToList() ?? new List<string>();
                    DoseTimesJson = JsonSerializer.Serialize(timeStrings);
                }
                catch
                {
                    DoseTimesJson = "[]";
                }
            }
        }

        [NotMapped]
        public DateTime EndDate => StartDate.AddDays(DurationInDays);

        [NotMapped]
        public DateTime NextPhaseStartDate => EndDate.AddDays(BreakAfterDays);

        [NotMapped]
        public bool IsActive => DateTime.Today >= StartDate.Date && DateTime.Today < EndDate.Date;

        [NotMapped]
        public bool IsInBreakPeriod => DateTime.Today >= EndDate.Date && DateTime.Today < NextPhaseStartDate.Date && BreakAfterDays > 0;
    }
}