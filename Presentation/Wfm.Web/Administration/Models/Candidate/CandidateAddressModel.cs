using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Admin.Models.Common;
using Wfm.Admin.Validators.Candidate;
using System;

namespace Wfm.Admin.Models.Candidate
{
    [Validator(typeof(CandidateAddressValidator))]
    public class CandidateAddressModel : AddressModel
    {
        public Guid CandidateGuid { get; set; }
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.Vendor")]
        public int FranchiseId { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.CandidateAddress.Fields.AddressTypeId")]

        public int AddressTypeId { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }



        // Helper fields
        [WfmResourceDisplayName("Admin.Common.Address")]
        public string FullAddress { get; set; }


        public virtual AddressTypeModel AddressTypeModel { get; set; }

        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
       
    }
}