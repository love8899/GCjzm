using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Messages;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Messages
{
    [Validator(typeof(QueuedEmailValidator))]
    public partial class QueuedEmailModel: BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Id")]
        public override int Id { get; set; }

        public int EmailAccountId { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.Priority")]
        public int Priority { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.From")]
        public string From { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.FromName")]
        public string FromName { get; set; }

        public int FromAccountId { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.To")]
        public string To { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.ToName")]
        public string ToName { get; set; }

        public int ToAccountId { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.ReplyTo")]
        public string ReplyTo { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.ReplyToName")]
        public string ReplyToName { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.CC")]
        public string CC { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.Bcc")]
        public string Bcc { get; set; }

        [WfmResourceDisplayName("Common.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [WfmResourceDisplayName("Common.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.AttachmentFilePath")]
        public string AttachmentFilePath { get; set; }
        public string AttachmentFileName { get; set; }
        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.SentTries")]
        public int SentTries { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.SentOn")]
        [DisplayFormat(DataFormatString="{0}", NullDisplayText="Not sent yet")]
        public DateTime? SentOn { get; set; }

        [WfmResourceDisplayName("Admin.System.QueuedEmails.Fields.EmailAccountName")]
        public string EmailAccountName { get; set; }

        public int MessageCategoryId { get; set; }
        public string FullPath { get { return AttachmentFilePath + AttachmentFileName; } }
        public byte[] AttachmentFile { get; set; }
        public int? AttachmentTypeId { get; set; }
        public string AttachmentFileName2 { get; set; }
        public int? AttachmentTypeId2 { get; set; }
        public byte[] AttachmentFile2 { get; set; }
        public string AttachmentFileName3 { get; set; }
        public int? AttachmentTypeId3 { get; set; }
        public byte[] AttachmentFile3 { get; set; }

        public bool IncludeLogo { get; set; }
        public string LogoFilePath { get; set; }
        public int? ConfirmationEmailLinkId { get; set; }
    }
}