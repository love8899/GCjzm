using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Shared.Validators.JobPost;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Scheduling
{
    public class ShiftScheduleModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.SchedulePeriod")]
        public int SchedulePeriodId { get; set; }
        [WfmResourceDisplayName("Common.Shift")]
        public int CompanyShiftId { get; set; }
        [WfmResourceDisplayName("Common.Monday")]
        public bool MondaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Tuesday")]
        public bool TuesdaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Wednesday")]
        public bool WednesdaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Thursday")]
        public bool ThursdaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Friday")]
        public bool FridaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Saturday")]
        public bool SaturdaySwitch { get; set; }
        [WfmResourceDisplayName("Common.Sunday")]
        public bool SundaySwitch { get; set; }
        [WfmResourceDisplayName("Common.StartTime")]
        public DateTime? StartTimeOfDay { get; set; }
        [WfmResourceDisplayName("Common.LengthInHours")]
        public decimal LengthInHours { get; set; }
        [WfmResourceDisplayName("Common.Shift")]
        public CompanyShiftDropdownModel CompanyShift { get; set; }
    }

    public class CompanyShiftDropdownModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
