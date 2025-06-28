using Wfm.Web.Framework;
using Wfm.Client.Models.Common;
using Wfm.Shared.Models.Common;


namespace Wfm.Client.Models.Candidate
{
    public class CandidateAddressModel : AddressModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.CandidateAddress.Fields.AddressTypeId")]
        public int AddressTypeId { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public virtual AddressTypeModel AddressTypeModel { get; set; }
    }
}