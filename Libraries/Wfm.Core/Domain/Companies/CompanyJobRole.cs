using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Scheduling;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyJobRole : BaseEntity
    {
        public CompanyJobRole()
        {
            this.RequiredSkills = new List<CompanyJobRoleSkill>();
        }
        public int PositionId { get; set; }
        public int CompanyId { get; set; }
        public int? CompanyLocationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? StandardCostHourlyRate { get; set; }

        public virtual Position Position { get; set; }
        public virtual Company Company { get; set; }
        public virtual CompanyLocation CompanyLocation { get; set; }
        public virtual ICollection<CompanyJobRoleSkill> RequiredSkills { get; set; }
        public virtual ICollection<EmployeeJobRole> EmployeeJobRoles { get; set; }
        public virtual ICollection<EmployeeSchedule> EmployeeSchedules { get; set; }
    }
}
