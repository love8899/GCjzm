using System;


namespace Wfm.Core.Domain.Candidates
{
    public class EmployeeSeniority
    {
        public int CandidateId { get; set; }
        public Guid CandidateGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        public DateTime? FirstHireDate { get; set; }
        public DateTime? LastHireDate { get; set; }
        public DateTime? FirstDayWorked { get; set; }
        public DateTime? LastDayWorked { get; set; }
        public int? LastClientWorked { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? ROE_Date { get; set; }
        public string ROE_Reason { get; set; }
        public DateTime? DNR_Date { get; set; }
        public string DNR_Reason { get; set; }
        public DateTime? FirstPlacement { get; set; }
        public DateTime? LastPlacement { get; set; }
    }
}