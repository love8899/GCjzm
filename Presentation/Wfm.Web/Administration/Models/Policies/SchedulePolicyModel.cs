using FluentValidation.Attributes;
using Wfm.Admin.Validators.Policies;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Policies
{
    [Validator(typeof(SchedulePolicyValidator))]
    public partial class SchedulePolicyModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }

        [WfmResourceDisplayName("Common.MealPolicy")]
        public int MealPolicyId { get; set; }

        [WfmResourceDisplayName("Common.BreakPolicy")]
        public int BreakPolicyId { get; set; }

        [WfmResourceDisplayName("Common.RoundingPolicy")]
        public int RoundingPolicyId { get; set; }
        [WfmResourceDisplayName("Admin.Policy.SchedulePolicy.Fields.IsStrictSchedule")]
        public bool IsStrictSchedule { get; set; }
        [WfmResourceDisplayName("Admin.Policy.SchedulePolicy.Fields.OvertimeGracePeriodInMinutes")]
        public int OvertimeGracePeriodInMinutes { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }




        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Admin.Policy.SchedulePolicy.Fields.MealTimeInMinutes")]
        public decimal MealTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.Policy.SchedulePolicy.Fields.BreakTimeInMinutes")]
        public decimal BreakTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.Policy.SchedulePolicy.Fields.RoundingIntervalInMinutes")]
        public decimal RoundingIntervalInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.Policy.SchedulePolicy.Fields.RoundingGracePeriodInMinutes")]
        public decimal RoundingGracePeriodInMinutes { get; set; }

    }
}