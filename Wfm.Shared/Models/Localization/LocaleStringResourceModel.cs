using FluentValidation.Attributes;
using Wfm.Shared.Validators.Localization;
using Wfm.Shared.Models.Localization;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Localization
{
    [Validator(typeof(LocaleStringResourceValidtor))]
    public class LocaleStringResourceModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.LanguageId")]
        public virtual int LanguageId { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.ResourceName")]
        public virtual string ResourceName { get; set; }

        [WfmResourceDisplayName("Common.Value")]
        public virtual string ResourceValue { get; set; }
    }
}
