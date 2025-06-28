using System.Text;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.Messages;
using System.Collections.Generic;

namespace Wfm.Admin.Models.Messages
{
    [Validator(typeof(MessageTemplateValidator))]
    public class MessageTemplateModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.MessageTemplate.Fields.TagName")]
        [AllowHtml]
        public string TagName { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.MessageTemplate.Fields.CCEmailAddresses")]
        public string CCEmailAddresses { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.MessageTemplate.Fields.BccEmailAddresses")]
        public string BccEmailAddresses { get; set; }

        [WfmResourceDisplayName("Common.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [WfmResourceDisplayName("Common.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.MessageTemplate.Fields.PossibleVariables")]
        [AllowHtml]
        public string PossibleVariables { get; set; }


        [WfmResourceDisplayName("Common.Note")]
        [AllowHtml]
        public string Note { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.MessageTemplate.Fields.EmailAccountId")]
        public int EmailAccountId { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string EmailAccountName { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [WfmResourceDisplayName("Admin.MessageCategory")]
        public int MessageCategoryId { get; set; }
        public bool IsGeneral { get; set; }

        //public virtual List<MessageNotification> MessageNotifications { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TagName  = " + this.TagName );
            sb.AppendLine("Subject  = " + this.Subject  );
            sb.AppendLine("Body = " + this.Body );
            sb.AppendLine(" PossibleVariables = " + this.PossibleVariables);
            sb.AppendLine("Note   = " + this.Note);
            sb.AppendLine("EnteredBy   = " + this.EnteredBy);
            sb.AppendLine("FranchiseId  = " + this.FranchiseId);
            return sb.ToString();
        }

        [WfmResourceDisplayName("Admin.Accounts.Account.Fields.AccountRoleSystemName")]
        public List<int> AccountRolesIds { get; set; }

    }
}