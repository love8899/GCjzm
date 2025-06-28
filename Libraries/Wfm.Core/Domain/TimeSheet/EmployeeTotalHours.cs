namespace Wfm.Core.Domain.TimeSheet
{
    public class EmployeeTotalHours
    {
        public int CandidateId { get; set; }
        public int FranchiseId { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public decimal TotalHours { get; set; }
    }
}
