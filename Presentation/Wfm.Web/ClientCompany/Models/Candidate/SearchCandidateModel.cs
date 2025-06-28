using System;
using System.Collections.Generic;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.Accounts;
using Wfm.Web.Framework;

namespace Wfm.Client.Models.Candidate
{
    public class SearchCandidateModel
    {
        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }

        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.MiddleName")]
        public string MiddleName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.Phone")]
        public string Phone { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }

        [WfmResourceDisplayName("Common.AddressLine1")]
        public string AddressLine1 { get; set; }

        [WfmResourceDisplayName("Common.AddressLine2")]
        public string AddressLine2 { get; set; }

        [WfmResourceDisplayName("Common.AddressLine3")]
        public string AddressLine3 { get; set; }

        [WfmResourceDisplayName("Common.Country")]
        public string Country { get; set; }

        [WfmResourceDisplayName("Common.StateProvince")]
        public string StateProvince { get; set; }

        [WfmResourceDisplayName("Common.City")]
        public string City { get; set; }

        [WfmResourceDisplayName("Common.PostalCode")]
        public string PostalCode { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.PreferredWorkLocation")]
        public string PreferredWorkLocation { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.MajorIntersection1")]
        public string MajorIntersection1 { get; set; }

        [WfmResourceDisplayName("Common.MajorIntersection2")]
        public string MajorIntersection2 { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.KeySkill")]
        public string KeySkill { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.KeyWord")]
        public string KeyWord { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.BeforeDateCreated")]
        public DateTime BeforeDateCreated { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.AfterDateCreated")]
        public DateTime AfterDateCreated { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.Resume")]
        public string Resume { get; set; }

        [WfmResourceDisplayName("Common.Gender")]
        public int Gender { get; set; }

        [WfmResourceDisplayName("Common.Salutation")]
        public int Salutation { get; set; }

        [WfmResourceDisplayName("Common.Shift")]
        public int Shift { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.Transportation")]
        public int Transportation { get; set; }

        [WfmResourceDisplayName("Common.Owner")]
        public int Owner { get; set; }

        [WfmResourceDisplayName("Common.Franchise")]
        public int Franchise { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.IsRelocate")]
        public bool IsRelocate { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.SearchCandidate.Fields.IsBanned")]
        public bool IsBanned { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }


        public IList<Gender> _GenderList { get; set; }
        public IList<Salutation> _SalutationList { get; set; }
        public IList<Shift> _ShiftList { get; set; }
        public IList<Transportation> _TransportationList { get; set; }
        public IList<Account> _AccountList { get; set; }
        public IList<Franchise> _FranchiseList { get; set; }
    }
}