using System;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Messages
{
    public class MessageModel : BaseWfmEntityModel
    {
        public string MailFrom { get; set; }

        public string FromName { get; set; }

        public string MailTo { get; set; }

        public string ToName { get; set; }

        public string CC { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string AttachmentFileName { get; set; }

        public string Note { get; set; }

        public string MessageCategory { get; set; }

        public string Recipient { get; set; }

        public bool ByEmail { get; set; }

        public bool ByMessage { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadOnUtc { get; set; }
    }
}
