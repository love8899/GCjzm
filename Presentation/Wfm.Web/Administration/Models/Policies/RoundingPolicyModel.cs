using FluentValidation.Attributes;
using Wfm.Admin.Validators.Policies;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Policies
{
    [Validator(typeof(RoundingPolicyValidator))]
    public partial class RoundingPolicyModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }

        [WfmResourceDisplayName("Admin.Policy.RoundingPolicy.Fields.IntervalInMinutes")]
        public int IntervalInMinutes { get; set; }

        [WfmResourceDisplayName("Admin.Policy.RoundingPolicy.Fields.GracePeriodInMinutes")]
        public int GracePeriodInMinutes { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

    }
}