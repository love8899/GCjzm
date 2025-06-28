using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Companies
{
   public  class CompanyContact
    {
        public Guid AccountGuid { get; set; }
        public  int Id { get; set; }
      
        public string Email { get; set; }     
        public string FirstName { get; set; }       
        public string LastName { get; set; }
        public string FullName { get { return String.Concat(FirstName , " " , LastName).Trim(); } }   
        public string WorkPhone { get; set; }        
        public int CompanyId { get; set; }
        public int CompanyLocationId { get; set; }     
        public int CompanyDepartmentId { get; set; }   
        public int ManagerId { get; set; }      
        public string Title { get; set; }     
        public bool IsActive { get; set; }       
        public bool IsDeleted { get; set; }     
        public int EnteredBy { get; set; }    
        public string CompanyName { get; set; }     
        public string CompanyLocationName { get; set; }    
        public string CompanyDepartmentName { get; set; }
        public string AccountRoleSystemName { get; set; }
        public string ShiftName { get; set; } 
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
    }
}
