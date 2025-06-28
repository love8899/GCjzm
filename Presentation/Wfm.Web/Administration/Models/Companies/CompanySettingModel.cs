using FluentValidation.Attributes;
using System.Web.Mvc;
using Wfm.Admin.Validators.Comany;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Companies
{
    [Validator(typeof(CompanySettingValidator))]
    public partial class CompanySettingModel : BaseWfmEntityModel
    {
        public CompanySettingModel() { }

        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Name")]
        public virtual string Name { get; set; }

        [WfmResourceDisplayName("Common.Value")]
        public virtual string Value { get; set; }

    }
}