using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Candidate
{
    public class CandidateBasicInfoModel : BaseWfmEntityModel
    {
        public string EmployeeId { get; set; }
        public Guid CandidateGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int FranchiseId { get; set; }
        public string Education { get; set; }
        public string FranchiseName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }
        //public IList<CandidateKeySkillModel> CandidateKeySkills { get; set; }
        //public IList<CandidateAttachmentModel> CandidateAttachments { get; set; }
    }
}