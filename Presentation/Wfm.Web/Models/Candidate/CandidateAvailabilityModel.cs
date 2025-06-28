using System;
using System.ComponentModel.DataAnnotations;
using Kendo.Mvc.UI;
using Wfm.Web.Framework;


namespace Wfm.Web.Models.Candidate
{
    public class CandidateAvailabilityModel : ISchedulerEvent
    {
        // 3 shifts assumed
        private string[] _SHIFTS { get { return new string[] { "Day", "Afternoon", "Any" }; } }


        public Guid UniqueId { get; set; }

        public int Id { get; set; }

        public int CandidateId { get; set; }
        public int ShiftId { get; set; }
        public int TypeId { get; set; }
        public string Note { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }

        public bool IsAllDay { get; set; }


        // TODO: use shift table display order
        public int DisplayOrder { get { return Array.IndexOf(_SHIFTS, Title); } }
        public bool ReadOnly { get; set; }
        public bool Hidden { get { return ReadOnly && TypeId <= 0; } }
        public bool Changed { get; set; }


        // from ISchedulerEvent
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        [WfmResourceDisplayName("Common.IsFullDay")]
        public string Timezone { get; set; }


        public CandidateAvailabilityModel()
        {
            UniqueId = Guid.NewGuid();
        }
    }
}