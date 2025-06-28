using FluentValidation.Attributes;
using Wfm.Client.Validators.Common;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.QuickSearch
{
    [Validator(typeof(QuickSearchValidator))]
    public class QuickSearchModel : BaseWfmEntityModel
    {
        public string KeyWord { get; set; }
        public string Domain { get; set; }

    }
}