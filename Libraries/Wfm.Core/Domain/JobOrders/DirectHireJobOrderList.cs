using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Wfm.Core.Domain.JobOrders
{   
    public class DirectHireJobOrderList  
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid JobOrderGuid { get; set; } 
        public int CompanyId { get; set; }
        public int Id { get; set; }     
        public Guid CompanyGuid { get; set; } 
        public string JobTitle { get; set; }
        public string CompanyJobNumber { get; set; }
        public DateTime? HiringDurationExpiredDate { get; set; }
        public DateTime? StartDate { get; set; } 
        
        public decimal? SalaryMax { get; set; }
        public decimal? SalaryMin { get; set; }
        public string Salary { get; set; } 
        public string Status { get; set; }
        public string RecruiterName { get; set; }
        public string OwnerName { get; set; }
        public string CompanyName { get; set; }
        public int CompanyLocationId { get; set; }      
        public int FranchiseId { get; set; } 
        public bool IsPublished { get; set; }
        public string JobOrderCategory { get; set; } 
       public string FeeType { get; set; }
       public DateTime? UpdatedOnUtc { get; set; }
       public DateTime? CreatedOnUtc { get; set; }

       public int? RecruiterId { get; set; } 
    }
}