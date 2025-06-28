using System;
using Wfm.Services.Companies;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    public class CandidatePoolModel : BaseWfmEntityModel, ICompanyCandidatePriorityInfo
    {
        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }
        public string EmployeeId { get; set; }

        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string Name { get { return String.Concat(LastName, " ", FirstName); } }

        public int AvailableShiftId { get; set; }

        public string Position { get; set; }
        public int? RatingValue { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public int GenderId { get; set; }
        public string Age { get; set; }
        public string SearchKeys { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public string LastWorkingLocation { get; set; }
        public string LastWorkingShift { get; set; }
        public decimal? TotalWorkingHours { get; set; }
        public string MajorIntersection { get; set; }
        public string PreferredWorkLocation { get; set; }
        public int? ShiftId { get; set; }
        public int? TransportationId { get; set; }
        public int FranchiseId { get; set; }
        public string Note { get; set; }
        public virtual CandidateAddressModel CandidateAddressModel { get; set; }
        public virtual CandidateJobOrderModel CandidateJobOrderModel { get; set; }
    }
}