using System;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Companies
{
    public class SearchCompanyCandidateModel : BaseWfmEntityModel
    {
        public Guid Guid { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.Gender")]
        public int GenderId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SearchKeys")]
        public string SearchKeys { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HomePhone")]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MobilePhone")]
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmergencyPhone")]
        public string EmergencyPhone { get; set; }

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public int? ShiftId { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.TransportationId")]
        public int TransportationId { get; set; }

        public bool IsBanned { get; set; }

        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.City")]
        public int? CityId { get; set; }

        [WfmResourceDisplayName("Common.StateProvince")]
        public int? StateProvinceId { get; set; }

        public string EmployeeId { get; set; }


    }
}