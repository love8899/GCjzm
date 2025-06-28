using System.Collections.Generic;
using Wfm.Shared.Models.Localization;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Common
{
    public partial class LanguageSelectorModel : BaseWfmModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }
    }
}