using System;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.TimeSheet;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.TimeSheet
{
    [Validator(typeof(EditCandidateWorkTimeStatusValidator))]
    public class EditCandidateWorkTimeStatusModel : BaseWfmModel
    {
        [WfmResourceDisplayName("Admin.TimeSheet.EditCandidateWorkTimeStatus.Fields.CandidateWorkTimeId")]
        public int CandidateWorkTimeId { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.EditCandidateWorkTimeStatus.Fields.CandidateWorkTimeStatusId")]
        public int CandidateWorkTimeStatusId { get; set; }




        [WfmResourceDisplayName("Common.FirstName")]
        public string EmployeeFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string EmployeeLastName { get; set; }
        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }
        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }
        [WfmResourceDisplayName("Common.JobStartDateTime")]
        public DateTime JobStartDateTime { get; set; }
        [WfmResourceDisplayName("Common.JobEndDateTime")]
        public DateTime JobEndDateTime { get; set; }
        public string Note { get; set; }

    }

}