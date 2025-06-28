using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Scheduling;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Scheduling
{
    public class DailyShiftScheduleModel : BaseWfmEntityModel, ISchedulerEvent
    {
        public DailyShiftScheduleModel()
        {
            OpeningDictionary = new Dictionary<string, DailyCount>();
        }

        public int SchedulePeriodId { get; set; }
        public int CompanyShiftId { get; set; }
        public string Description { get; set; }
        public DateTime End { get; set; }
        public string EndTimezone { get; set; }
        [WfmResourceDisplayName("Common.IsFullDay")]
        public bool IsAllDay { get; set; }
        public string RecurrenceException { get; set; }
        public string RecurrenceRule { get; set; }
        public DateTime Start { get; set; }
        public string StartTimezone { get; set; }
        public string Title { get; set; }
        public Dictionary<string, DailyCount> OpeningDictionary { get; set; }

        public ShiftScheduleDaily ToEntity()
        {
            return new ShiftScheduleDaily()
            {
                Id = this.Id,
                CompanyShiftId = this.CompanyShiftId,
                SchedulePeriodId = this.SchedulePeriodId,
                ScheduelDate = this.Start.Date,
                StartTimeOfDayTicks = (this.Start - this.Start.Date).Ticks,
                LengthInHours = Convert.ToDecimal((this.End - this.Start).TotalHours),
                CreatedOnUtc = this.CreatedOnUtc,
                UpdatedOnUtc = this.UpdatedOnUtc
            };
        }
    }

    public class DailyCount
    {
        public int MandantoryRequiredCount { get; set; }
        public int ContingencyRequiredCount { get; set; }
    }
}
