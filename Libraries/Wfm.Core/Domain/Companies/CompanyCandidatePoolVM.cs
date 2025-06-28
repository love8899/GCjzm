using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyCandidatePoolVM 
    {
        public int CompanyCandidateId { get; set; }

        public int CandidateId { get; set; }
        public string EmployeeId { get; set; }
        public Guid CandidateGuid { get; set; }
        public string LastName { get; set; }
        //public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public int? RatingValue { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public int GenderId { get; set; }
        //public string Age { get; set; }
        //public string SearchKeys { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public string LastWorkingLocation { get; set; }
        public string LastWorkingShift { get; set; }
        public decimal? TotalWorkingHours { get; set; }
        public string MajorIntersection { get; set; }
        public string PreferredWorkLocation { get; set; }
        public string Position { get; set; }
        public int? ShiftId { get; set; }
        public int? TransportationId { get; set; }
        public int FranchiseId { get; set; }
        public string Note { get; set; }

        // from Candidate Availability
        public int AvailableShiftId { get; set; }
    }
}
