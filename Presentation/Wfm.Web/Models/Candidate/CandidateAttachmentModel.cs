using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Candidate
{
    public class CandidateAttachmentModel : BaseWfmEntityModel
    {
        public Guid CandidateAttachmentGuid { get; set; }
        public int CandidateId { get; set; }
        public int AttachmentTypeId { get; set; }
        public string AttachmentName { get; set; }
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        //public string StoredPath { get; set; }
        public string ContentType { get; set; }
        public string ContentText { get; set; }
        public int FileSizeInKB { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

       [WfmResourceDisplayName("Admin.Configuration.DocumentType")]
        public int DocumentTypeId { get; set; }

       [WfmResourceDisplayName("Admin.Franchises.Franchise.Fields.GeneralLiabilityCertificateExpiryDate")]
       public DateTime? ExpiryDate { get; set; }

       public bool IsPublic { get; set; }

       public int? CompanyId { get; set; }
    }
}