using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Wfm.Core.Domain.JobOrders
{
    public class DirectHireCandidatePoolList  
    {
        public Guid CandidateGuid { get; set; }
       public int CandidateId { get; set; }
       public string FirstName { get; set; } 
       public string LastName { get; set; } 
       public string KeySkill { get; set; }
       public decimal? YearsOfExperience { get; set; }
       public DateTime? LastUsedDate { get; set; }
       public bool IsDeleted { get; set; }
       public string Note { get; set; }
       public string EmployeeId { get; set; }
    }
}