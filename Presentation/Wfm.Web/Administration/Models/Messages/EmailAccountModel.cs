using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.Messages;

namespace Wfm.Admin.Models.Messages
{
    [Validator(typeof(EmailAccountValidator))]
    public class EmailAccountModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.EmailAccount.Fields.DisplayName")]
        public string DisplayName { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.EmailAccount.Fields.Host")]
        public string Host { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.EmailAccount.Fields.Port")]
        public string Port { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.EmailAccount.Fields.Username")]
        public string Username { get; set; }

        [WfmResourceDisplayName("Common.Password")]
        public string Password { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.EmailAccount.Fields.EnableSsl")]
        public bool EnableSsl { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.EmailAccount.Fields.UseDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}