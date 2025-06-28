using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Common
{

    public partial class SecurityQuestionModel : BaseWfmEntityModel
    {
        public string Question { get; set; } 
    }
}