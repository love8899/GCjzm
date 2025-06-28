using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Services.TimeSheet;
using Wfm.Admin.Extensions;

namespace Wfm.Admin.Models.TimeSheet
{
    public class WorkTimeChangeAfterInvoiceModel_BL
    {
        public List<WorkTimeChangeAfterInvoiceModel> GetWorkTimeAfterInvoiceModel(DateTime startDate,
                                                                                    DateTime endDate,
                                                                                    IWorkTimeService _workTimeService)
        {
            var workTimes = _workTimeService.GetWorkTimeNotFullyInvoicedByStartEndDateAsQueryable(startDate, endDate).ToList();
            var changes = new List<WorkTimeChangeAfterInvoiceModel>();
            foreach (var cwt in workTimes)
            {
                var reason = cwt.InvoiceDate == null ? "New" : "Update";

                if (!String.IsNullOrWhiteSpace(reason))
                {
                    var change = new WorkTimeChangeAfterInvoiceModel();

                    if (reason == "New")
                    {
                        change.CandidateWorkTimeBaseModel = cwt.ToBaseModel();
                        change.CandidateWorkTimeLogModel = new CandidateWorkTimeLogModel();
                        change.CandidateWorkTimeLogModel.CandidateWorkTimeId = cwt.Id;
                        change.CandidateWorkTimeLogModel.OriginalHours = 0;
                        change.CandidateWorkTimeLogModel.NewHours = cwt.NetWorkTimeInHours;
                        change.CandidateWorkTimeLogModel.CreatedOnUtc = cwt.ApprovedOnUtc;
                    }
                    else if (reason == "Update")
                    {
                        var latestChange = cwt.CandidateWorkTimeChanges.OrderBy(y => y.CreatedOnUtc).LastOrDefault();
                        change.CandidateWorkTimeBaseModel = cwt.ToBaseModel();
                        change.CandidateWorkTimeBaseModel.InvoiceDate = cwt.InvoiceDate.Value.ToLocalTime();
                        change.CandidateWorkTimeLogModel = latestChange.ToModel();
                    }

                    change.CandidateWorkTimeLogModel.Reason = reason;
                    changes.Add(change);
                }

            }
            return changes;
        }
    }
}