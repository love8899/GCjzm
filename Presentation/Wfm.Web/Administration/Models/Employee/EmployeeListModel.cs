using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Employee;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Employee
{
    public class EmployeeListModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }

        [WfmResourceDisplayName("Common.Vendor")]
        public int FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }

        [WfmResourceDisplayName("Common.FirstName")]
        public string FirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string LastName { get; set; }

        [WfmResourceDisplayName("Common.MiddleName")]
        public string MiddleName { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.EmployeeType")]
        public int? EmployeeTypeId { get; set; }

        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.SocialInsuranceNumber")]
        [Display(Prompt = "123 456 789")]
        public string SocialInsuranceNumber { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.IsBanned")]
        public bool IsBanned { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        public string BankAccount { get; set; }
    }
}