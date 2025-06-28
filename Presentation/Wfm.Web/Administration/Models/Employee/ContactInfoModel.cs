using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Employee;
using Wfm.Web.Framework;


namespace Wfm.Admin.Models.Employee
{
    [Validator(typeof(ContactInfoValidator<ContactInfoModel>))]
    public class ContactInfoModel
    {
        public Guid CandidateGuid { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.HomePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        [DataType(DataType.PhoneNumber)]
        public string HomePhone { get; set; }

        [WfmResourceDisplayName("Admin.Candidate.Candidate.Fields.MobilePhone")]
        [RegularExpression(@"^[\+0-9\s\-]+$", ErrorMessage = "Invalid phone number")]
        public string MobilePhone { get; set; }
    }
}