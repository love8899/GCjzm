using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Shared.Validators.JobPost;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Shared.Models.Scheduling
{
    public class ShiftJobRoleModel
    {
        public int Id { get; set; }
        public int SchedulePeriodId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string Department { get; set; }
        public int CompanyJobRoleId { get; set; }
        public string JobRole { get; set; }
        public int CompanyShiftId { get; set; }
        public string Shift { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int[] Planned { get; set; }
    }

}
