using System;
using System.ComponentModel.DataAnnotations;
using Kendo.Mvc.UI;
using Wfm.Web.Framework;
using Wfm.Web.Framework.CustomAttribute;


namespace Wfm.Admin.Models.Commmon
{
    public class SchedulerEvent : ISchedulerEvent
    {
        public SchedulerEvent()
        {
            UniqueId = Guid.NewGuid();
        }

        public Guid UniqueId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [WfmRequired(ErrorMessage = "Common.StartDate.Required")]
        public DateTime Start { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [WfmRequired(ErrorMessage = "Common.EndDate.Required")]
        public DateTime End { get; set; }

        public bool IsAllDay { get; set; }

        public bool ReadOnly { get; set; }

        // not used, from ISchedulerEvent
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        [WfmResourceDisplayName("Common.IsFullDay")]
        public string Timezone { get; set; }
    }
}
