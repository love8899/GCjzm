using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Scheduling
{
    public class EmployeeScheduleDailyModel : BaseWfmEntityModel, ISchedulerEvent
    {
        public EmployeeScheduleDailyModel()
        {
            this.Breaks = new List<EmployeeScheduleDailyBreakModel>();
        }
        public int EmployeeScheduleId { get; set; }
        public DateTime ScheduelDate { get; set; }
        public TimeSpan StartTimeOfDay { get; set; }
        public TimeSpan EndTimeOfDay { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        [WfmResourceDisplayName("Common.IsFullDay")]
        public bool IsAllDay { get; set; }
        public bool IsDeleted { get; set; }
        public int? ReplacementEmployeeId { get; set; }
        public int? ReplacementCompanyJobRoleId { get; set; }
        public bool IsAdhoc { get; set; }


        public DateTime Start
        {
            get
            {
                return ScheduelDate.Add(StartTimeOfDay);
            }
            set
            {
                StartTimeOfDay = value - ScheduelDate;
            }
        }

        public DateTime End
        {
            get
            {
                return ScheduelDate.Add(EndTimeOfDay);
            }

            set
            {
                EndTimeOfDay = value - ScheduelDate;
            }
        }

        public string StartTimezone { get; set; }

        public string EndTimezone { get; set; }

        public string RecurrenceRule { get; set; }

        public string RecurrenceException { get; set; }

        public ICollection<EmployeeScheduleDailyBreakModel> Breaks { get; set; }
    }
}
