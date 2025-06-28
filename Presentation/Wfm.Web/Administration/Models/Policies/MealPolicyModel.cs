using FluentValidation.Attributes;
using Wfm.Admin.Validators.Policies;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Policies
{
    [Validator(typeof(MealPolicyValidator))]
    public partial class MealPolicyModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }

        [WfmResourceDisplayName("Admin.Policy.MealPolicy.Fields.MealTimeInMinutes")]
        public int MealTimeInMinutes { get; set; }

        [WfmResourceDisplayName("Admin.Policy.MealPolicy.Fields.MinWorkHours")]
        public decimal MinWorkHours { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}