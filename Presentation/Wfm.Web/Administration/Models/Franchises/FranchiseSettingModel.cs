using FluentValidation.Attributes;
using System.Web.Mvc;
using Wfm.Admin.Validators.Franchise;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Franchises
{
    [Validator(typeof(FranchiseSettingValidator))]
    public partial class FranchiseSettingModel : BaseWfmEntityModel
    {
        public FranchiseSettingModel() { }

        [WfmResourceDisplayName("Common.Franchise")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.Name")]
        public virtual string Name { get; set; }

        [AllowHtml]
        [WfmResourceDisplayName("Common.Value")]
        public virtual string Value { get; set; }

    }
}