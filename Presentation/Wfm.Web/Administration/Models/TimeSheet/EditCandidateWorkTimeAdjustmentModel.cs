using System;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.TimeSheet;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.TimeSheet
{
    [Validator(typeof(EditCandidateWorkTimeAdjustmentValidator))]
    public class EditCandidateWorkTimeAdjustmentModel : BaseWfmModel
    {
        [WfmResourceDisplayName("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.CandidateWorkTimeId")]
        public int CandidateWorkTimeId { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.AdjustmentInMinutes")]
        public decimal AdjustmentInMinutes { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.GrossWorkTimeInMinutes")]
        public decimal GrossWorkTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.GrossWorkTimeInHours")]
        public decimal GrossWorkTimeInHours { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.NetWorkTimeInMinutes")]
        public decimal NetWorkTimeInMinutes { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.NetWorkTimeInHours")]
        public decimal NetWorkTimeInHours { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.EditCandidateWorkTimeAdjustment.Fields.CandidateWorkTimeStatusId")]

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

    }

}