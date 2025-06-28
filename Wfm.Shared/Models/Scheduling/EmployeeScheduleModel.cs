using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Shared.Models.Employees;
using Wfm.Shared.Validators;
using Wfm.Shared.Validators.JobPost;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Scheduling
{
    public class EmployeeScheduleModel : EmployeeGridModel
    {
        public int SchedulePeriodId { get; set; }
        [WfmResourceDisplayName("Common.ScheduledShiftName")]
        public int? CompanyShiftId { get; set; }
        [WfmResourceDisplayName("Common.ScheduledJobRoleName")]
        public int? JobRoleId { get; set; }
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.ScheduledShiftName")]
        public string ScheduledShiftName { get; set; }
        [WfmResourceDisplayName("Common.ScheduledJobRoleName")]
        public string ScheduledJobRoleName { get; set; }

        public DateTime StartDate { get; set; }
        public bool ForDailyAdhoc { get; set; }
    }
}
