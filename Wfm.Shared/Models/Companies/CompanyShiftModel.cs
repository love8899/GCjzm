using FluentValidation.Attributes;
using System;
using Wfm.Shared.Validators;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Companies
{
    [Validator(typeof(CompanyShiftValidator))]
    public class CompanyShiftModel : BaseWfmEntityModel
    {
        public CompanyShiftModel()
        {
            EffectiveDate = DateTime.Today;
        }

        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }
        [WfmResourceDisplayName("Common.Shift")]
        public int ShiftId { get; set; }
        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public int CompanyDepartmentId { get; set; }
        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime EffectiveDate { get; set; }
        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime? ExpiryDate { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        [WfmResourceDisplayName("Common.SchedulePolicy")]
        public int SchedulePolicyId { get; set; }
    }
}
