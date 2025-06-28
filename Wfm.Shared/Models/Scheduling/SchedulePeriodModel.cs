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
    [Validator(typeof(SchedulePeriodValidator))]
    public class SchedulePeriodModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public int? CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public int? CompanyDepartmentId { get; set; }
        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime PeriodStartDate { get; set; }
        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime? PeriodEndDate { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public string LocationText { get; set; }
    }
}
