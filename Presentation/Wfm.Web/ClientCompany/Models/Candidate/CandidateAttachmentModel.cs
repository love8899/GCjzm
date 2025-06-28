using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Client.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Client.Models.Media;

namespace Wfm.Client.Models.Candidate
{
    [Validator(typeof(CandidateAttachmentValidator))]
    public class CandidateAttachmentModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.AttachmentTypeId")]
        public int AttachmentTypeId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.OriginalFileName")]
        public string OriginalFileName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.StoredFileName")]
        public string StoredFileName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.StoredPath")]
        public string StoredPath { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.ContentType")]
        public string ContentType { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.ContentText")]
        [AllowHtml]
        public string ContentText { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAttachment.Fields.FileSizeInKB")]
        public int FileSizeInKB { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        [AllowHtml]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.DocumentType")]
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }

        public virtual AttachmentTypeModel AttachmentTypeModel { get; set; }
        public virtual CandidateModel CandidateModel { get; set; }
        public int? CompanyId { get; set; }
    }

}