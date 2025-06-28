using FluentValidation.Attributes;
using Wfm.Admin.Validators.Common;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Common
{
    [Validator(typeof(BankValidator))]
    public class BankModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.Bank.Fields.BankName")]
        public string BankName { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.Bank.Fields.BankCode")]
        public string BankCode { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
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