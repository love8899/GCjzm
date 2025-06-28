using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Company;

namespace Wfm.Admin.Models.Companies
{
    [Validator(typeof(CompanyConfirmationEmailValidator))]
    public class CompanyConfirmationEmailModel
    {
        
        public int CompanyId { get; set; }

        public int CandidateId { get; set; }

        public int LocationId { get; set; }

        public int DepartmentId { get; set; }

        public Guid JobOrderGuid { get; set; }
        [UIHint("DateTimeNullable")]
        public DateTime Start { get; set; }
        [UIHint("DateTimeNullable")]
        public DateTime? End { get; set; }
        public string CandidateName { get; set; }
        public Guid CandidateGuid { get; set; }
        public int AvailableOpening { get; set; }
    }
}