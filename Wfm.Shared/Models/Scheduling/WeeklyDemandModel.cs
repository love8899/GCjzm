using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Shared.Validators;
using Wfm.Shared.Validators.JobPost;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Shared.Models.Scheduling
{
    [Validator(typeof(WeeklyDemandValidator))]
    public class WeeklyDemandModel : BaseWfmEntityModel
    {
        public int SchedulePeriodId { get; set; }
        public int CompanyDepartmentId { get; set; }
        public string Department { get; set; }
        public int CompanyJobRoleId { get; set; }
        public string JobRole { get; set; }

        public int ShiftId { get; set; }
        public string Shift { get; set; }
        public DateTime StartTime { get; set; }
        public decimal LengthInHours { get; set; }

        public int SupervisorId { get; set; }

        public int Sunday { get; set; }
        public int Monday { get; set; }
        public int Tuesday { get; set; }
        public int Wednesday { get; set; }
        public int Thursday { get; set; }
        public int Friday { get; set; }
        public int Saturday { get; set; }
    }
}
