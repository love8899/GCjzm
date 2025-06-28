using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    public partial class CandidatePictureModel : BaseWfmEntityModel
    {

        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Pictures.Fields.FileName")]
        public string FileName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Pictures.Fields.FilePath")]
        public string FilePath { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Pictures.Fields.MimeType")]
        public string MimeType { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Pictures.Fields.IsNew")]
        public bool IsNew { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }


        [WfmResourceDisplayName("Common.Photo")]
        public string PictureUrl { get; set; }
    }

}