using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Models.Media;
using System;

namespace Wfm.Admin.Models.Candidate
{
    [Validator(typeof(CandidateAttachmentValidator))]
    public class CandidateAttachmentModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }

        public Guid CandidateAttachmentGuid { get; set; }

        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.AttachmentTypeId")]
        public int AttachmentTypeId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.OriginalFileName")]
        public string OriginalFileName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.StoredFileName")]
        public string StoredFileName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.ContentType")]
        public string ContentType { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.ContentText")]
        [AllowHtml]
        public string ContentText { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.FileSizeInKB")]
        public int FileSizeInKB { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.DocumentType")]
        public int DocumentTypeId { get; set; }

        [WfmResourceDisplayName("Admin.Franchises.Franchise.Fields.GeneralLiabilityCertificateExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        public int? CompanyId { get; set; }

         [WfmResourceDisplayName("Common.Company")]
        public string CompanyName { get; set; }
         public bool UseForDirectPlacement { get; set; }
        //public virtual AttachmentTypeModel AttachmentTypeModel { get; set; }
        //public virtual DocumentTypeModel DocumentTypeModel { get; set; } 
        //public virtual CandidateModel CandidateModel { get; set; }

    }

}