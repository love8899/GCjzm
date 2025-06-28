using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Employees
{
    public class EmployeeGridModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public virtual string EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }
        public string EmployeeType { get; set; }
        public int EmployeeTypeId { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.PreferredWorkLocation")]
        public string PreferredWorkLocation { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HomePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        public string HomePhone { get; set; }
        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        public string MobilePhone { get; set; }
        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }
    }
}
