using System;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Messages
{
    public class MessageHistoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.Priority")]
        public int Priority { get; set; }

        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.MailFrom")]
        public string MailFrom { get; set; }

        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.FromName")]
        public string FromName { get; set; }

        public int FromAccountId { get; set; }

        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.MailTo")]
        public string MailTo { get; set; }

        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.ToName")]
        public string ToName { get; set; }

        public int ToAccountId { get; set; }

        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.CC")]
        public string CC { get; set; }

        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.Bcc")]
        public string Bcc { get; set; }

        [WfmResourceDisplayName("Common.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [WfmResourceDisplayName("Common.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.Category")]
        public int MessageCategoryId { get; set; }
        public string MessageCategory { get; set; }

        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.SentTries")]
        public int SentTries { get; set; }

        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.SentOnUtc")]
        public DateTime? SentOnUtc { get; set; }

        [WfmResourceDisplayName("Admin.Messages.MessageHistory.Fields.EmailAccountId")]
        public int EmailAccountId { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string EmailAccountEmail { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }
    }
}
