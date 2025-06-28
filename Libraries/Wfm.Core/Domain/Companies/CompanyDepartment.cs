using System.Collections.Generic;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyDepartment : BaseEntity
    {
        public int CompanyId { get; set; }
        public int CompanyLocationId { get; set; }
        public string DepartmentName { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Company Company { get; set; }
        public virtual CompanyLocation CompanyLocation { get; set; }
        public virtual ICollection<CompanyShift> CompanyShifts { get; set; }

    }
}