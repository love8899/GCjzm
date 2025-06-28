using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Core.Domain.Employees
{
    public class Employee : BaseEntity
    {
        public Employee()
        {
            this.EmployeeAvailabilities = new List<EmployeeAvailability>();
            this.EmployeePayrollSettings = new List<EmployeePayrollSetting>();
        }
        public Guid CandidateGuid { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeType { get; set; }
        public int CompanyId { get; set; }
        public int? CompanyLocationId { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public int SalutationId { get; set; }
        public int? GenderId { get; set; }
        public int? EthnicTypeId { get; set; }
        public int? VetranTypeId { get; set; }
        public int? SourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string EmergencyPhone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string SocialInsuranceNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool? Entitled { get; set; }
        public bool? CanRelocate { get; set; }
        public string JobTitle { get; set; }
        public string Education { get; set; }
        public string Education2 { get; set; }
        public string Note { get; set; }
        public int FranchiseId { get; set; }
        //public string SearchKeys { get; set; }
        public int? EmployeeTypeId { get; set; }
        public int EnteredBy { get; set; }
        public DateTime? SINExpiryDate { get; set; }
        public DateTime? SINExtensionSubmissionDate { get; set; }
        public string WorkPermit { get; set; }
        public DateTime? WorkPermitExpiry { get; set; }
        //
        public virtual Company Company { get; set; }
        public virtual CompanyLocation CompanyLocation { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Salutation Salutation { get; set; }
        public virtual EthnicType EthnicType { get; set; }
        public virtual Account EnteredByAccount { get; set; }
        public virtual VetranType VetranType { get; set; }
        public virtual ICollection<EmployeeAvailability> EmployeeAvailabilities { get; set; }
        public virtual ICollection<EmployeePayrollSetting> EmployeePayrollSettings { get; set; }
        public virtual ICollection<EmployeeJobRole> EmployeeJobRoles { get; set; }
        public virtual ICollection<EmployeeSchedule> EmployeeSchedules { get; set; }
        public virtual ICollection<EmployeeTimeoffBooking> EmployeeTimeoffBookings { get; set; }
        //
        public override string ToString()
        {
            return string.Format("{0} {1}", FirstName, LastName);
        }
    }
}
