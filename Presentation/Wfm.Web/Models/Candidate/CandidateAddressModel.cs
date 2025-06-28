using Wfm.Shared.Models.Common;
using Wfm.Web.Framework;
using Wfm.Web.Models.Common;

namespace Wfm.Web.Models.Candidate
{
    public class CandidateAddressModel : AddressModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Web.Candidate.CandidateAddress.Fields.AddressTypes")]
        public int AddressTypeId { get; set; }

        public virtual AddressTypeModel AddressTypeModel { get; set; }
    }
}