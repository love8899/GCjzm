using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Models.Candidate;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.JobOrder;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Admin.Models.JobOrder
{
    [Validator(typeof(JobOrderWorkTimeValidator))]
    public class JobOrderWorkTimeModel : BaseWfmEntityModel
    {
        public JobOrderModel JobOrder { get; set; }
        public DateTime WeekEndSaturdayDate { get; set; }
        public IList<WeeklyManualWorkTimeModel> ManualWorkTimeEntries { get; set; }

        // public int? AttachedDocumentid { get; set; }
        public IList<ClientTimeSheetDocumentModel> ClientTimeSheetDocuments { get; set; }
    }

    public class WeeklyManualWorkTimeModel
    {
        public int CandidateId { get; set; }
        public string CandidateFirstName { get; set; }
        public string CandidateLastName { get; set; }
        public decimal?[] WorkHoursWeekdays { get; set; }
        public bool[] WorkHoursEditableWeekdays { get; set; }
    }
}
