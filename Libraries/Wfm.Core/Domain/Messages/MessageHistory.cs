using System;

namespace Wfm.Core.Domain.Messages
{
    /// <summary>
    /// Represents an email item
    /// </summary>
    public partial class MessageHistory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        public int EmailAccountId { get; set; }

        /// <summary>
        /// Gets the email account email
        /// </summary>
        public string EmailAccountEmail { get; set; }


        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the From property
        /// </summary>
        public string MailFrom { get; set; }

        /// <summary>
        /// Gets or sets the FromName property
        /// </summary>
        public string FromName { get; set; }

        public int FromAccountId { get; set; }

        /// <summary>
        /// Gets or sets the To property
        /// </summary>
        public string MailTo { get; set; }

        /// <summary>
        /// Gets or sets the ToName property
        /// </summary>
        public string ToName { get; set; }

        public int ToAccountId { get; set; }

        /// <summary>
        /// Gets or sets the ReplyTo property
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the ReplyToName property
        /// </summary>
        public string ReplyToName { get; set; }

        /// <summary>
        /// Gets or sets the CC
        /// </summary>
        public string CC { get; set; }

        /// <summary>
        /// Gets or sets the Bcc
        /// </summary>
        public string Bcc { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the attachment file path (full file path)
        /// </summary>
        public string AttachmentFilePath { get; set; }

        /// <summary>
        /// Gets or sets the attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.
        /// </summary>
        public string AttachmentFileName { get; set; }

        public int? AttachmentTypeId { get; set; }

        public byte[] AttachmentFile { get; set; }

        public string AttachmentFileName2 { get; set; }

        public int? AttachmentTypeId2 { get; set; }

        public byte[] AttachmentFile2 { get; set; }
        public string AttachmentFileName3 { get; set; }

        public int? AttachmentTypeId3 { get; set; }

        public byte[] AttachmentFile3 { get; set; }
        public bool IncludeLogo { get; set; }
        public string LogoFilePath { get; set; }
        public int MessageCategoryId { get; set; }


        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOnUtc { get; set; }

        public int EnteredBy { get; set; }

        public int FranchiseId { get; set; }

        public int? ConfirmationEmailLinkId { get; set; }
        public virtual MessageCategory MessageCategory { get; set; }
        public virtual ConfirmationEmailLink ConfirmationEmailLink { get; set; }
    }
}
