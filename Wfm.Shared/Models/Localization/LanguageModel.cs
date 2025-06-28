using System.Collections.Generic;
using FluentValidation.Attributes;
using Wfm.Shared.Validators.Localization;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Localization
{
    [Validator(typeof(LanguageValidator))]
    public class LanguageModel : BaseWfmEntityModel
    {
        public LanguageModel()
        {
            FlagFileNames = new List<string>();
        }

        [WfmResourceDisplayName("Common.Name")]

        public string Name { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.Languages.Fields.LanguageCulture")]
        public string LanguageCulture { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.Languages.Fields.UniqueSeoCode")]
        public string UniqueSeoCode { get; set; }

        //flags
        [WfmResourceDisplayName("Admin.Configuration.Languages.Fields.FlagImageFileName")]
        public string FlagImageFileName { get; set; }
        public IList<string> FlagFileNames { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.Languages.Fields.Rtl")]
        public bool Rtl { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

    }
}
