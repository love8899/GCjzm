using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace Wfm.Core.Domain.Messages
{
    /// <summary>
    /// Represents an resume from email
    /// </summary>
    public partial class Resume : BaseEntity
    {
        private ICollection<ResumeHistory> _resumeHistories;


        public string Account { get; set; }

        public int UniqueId { get; set; }

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
        public byte[] AttachmentBinary { get; set; }
        public string AttachmentText { get; set; }

        public string EmailFile { get; set; }
        public byte[] EmailBinary { get; set; }

        public bool IsRead { get; set; }


        public virtual ICollection<ResumeHistory> ResumeHistories
        {
            get { return _resumeHistories ?? (_resumeHistories = new List<ResumeHistory>()); }
            protected set { _resumeHistories = value; }
        }

        [NotMapped]
        public bool IsContacted { get { return ResumeHistories.Any(); } }
    }


    public class ResumeCriteria
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string SearchWords { get; set; }
        public bool ByAllWords { get; set; }
        public bool InSubject { get; set; }
        public bool InBody { get; set; }
        public bool InAttachment { get; set; }
    }
}
