using System;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Media;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateAttachment : BaseEntity
    {
        public Guid CandidateAttachmentGuid { get; set; }
        public int CandidateId { get; set; }
        public int AttachmentTypeId { get; set; }
        public string AttachmentName { get; set; }
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        public string StoredPath { get; set; }
        public string ContentType { get; set; }
        public string ContentText { get; set; }       
        public int FileSizeInKB { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int DocumentTypeId { get; set; }
        public virtual Candidate Candidate { get; set; }
        public virtual AttachmentType AttachmentType { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public byte[] AttachmentFile { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? CompanyId { get; set; }

        public bool IsCompressed { get; set; }

        public virtual Company Company { get; set; }
    }
}