using System;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Messages
{
    public class ResumeModel : BaseWfmEntityModel
    {
        public string From { get; set; }
        public string FromName { get; set; }

        public string To { get; set; }
        public string ToName { get; set; }

        public string Subject { get; set; }

        public DateTime Date { get; set; }

        public string Body { get; set; }
        public string HtmlBody { get; set; }

        public string StoredPath { get; set; }

        public string AttachmentFileName { get; set; }

        public string EmailFile { get; set; }

        public bool IsRead { get; set; }

        public bool IsContacted { get; set; }
    }
}
