using System;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.TimeSheet;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.TimeSheet
{
    [Validator(typeof(CalculateCandidateWorkTimeValidator))]
    public class CalculateCandidateWorkTimeModel : BaseWfmModel
    {

        [WfmResourceDisplayName("Admin.TimeSheet.CalculateCandidateWorkTime.Fields.FromDate")]
        public DateTime? FromDate { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CalculateCandidateWorkTime.Fields.ToDate")]
        
        public DateTime? ToDate { get; set; }

        public int JobOrderId { get; set; }
        public Guid JobOrderGuid { get; set; }
    }

}