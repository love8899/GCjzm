using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Wfm.Client.Extensions;
using Wfm.Core;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.TimeSheet;
using Wfm.Shared.Models.Search;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Client.Models.TimeSheet
{
    public class EmployeeWorkTimeApprovalModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }
        [WfmResourceDisplayName("Common.JobOrderId")]
        public int JobOrderId { get; set; }
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public int CandidateWorkTimeStatusId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string EmployeeFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string EmployeeLastName { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string JobTitle { get; set; }
        [WfmResourceDisplayName("Common.Shift")]
        public string JobShift { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.Department")]
        public int CompanyDepartmentId { get; set; }
        [WfmResourceDisplayName("Common.Contact")]
        public int CompanyContactId { get; set; }

        [Required]
        [WfmResourceDisplayName("Common.JobStartDateTime")]
        public DateTime JobStartDateTime { get; set; }
        [WfmResourceDisplayName("Common.JobEndDateTime")]
        public DateTime JobEndDateTime { get; set; }

        [WfmResourceDisplayName("Common.Year")]
        public int Year { get; set; }
        [WfmResourceDisplayName("Common.WeekOfYear")]
        public int WeekOfYear { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ClockIn")]
        public DateTime? ClockIn { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ClockOut")]
        public DateTime? ClockOut { get; set; }
        [WfmResourceDisplayName("Common.ClockDeviceUid")]

        public string Source { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.ClockTimeInHours")]
        public decimal ClockTimeInHours { get; set; }

        [Required]
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.NetWorkTimeInHours")]
        public decimal NetWorkTimeInHours { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.AdjustmentInMinutes")]
        public decimal AdjustmentInMinutes { get; set; }

        [Required]
        [WfmResourceDisplayName("Common.Note")]
        [AllowHtml]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }
        [WfmResourceDisplayName("Common.UpdatedBy")]
        public int UpdatedBy { get; set; }

        public bool AllowSuperVisorModifyWorkTime { get; set; }

        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.OvertimeMessage")]
        public string OvertimeMessage { get; set; }
        [WfmResourceDisplayName("Admin.TimeSheet.CandidateWorkTime.Fields.OtherAlerts")]
        public string OtherAlerts { get; set; }

        public int Payroll_BatchId { get; set; }

        public bool IsCurrentWeek
        {
            get
            {
                return DateTime.Today.AddDays(7 - (int)(DateTime.Today.DayOfWeek)).Equals( // next Sunday 
                    JobStartDateTime.Date.AddDays(7 - (int)(JobStartDateTime.Date.DayOfWeek)));
            }
        }
    }


    public class CandidateWorkTimeBL
    {
        #region fields

        private readonly IWorkTimeService _workTimeService;
        private readonly IWorkContext _workContext;
        
        #endregion


        #region Ctor
        public CandidateWorkTimeBL(
                        IWorkTimeService workTimeService,
             IWorkContext workContextService
            )
        {
            _workTimeService = workTimeService;
            _workContext = workContextService;
        }

        #endregion


        public DataSourceResult EmployeeWorkTimeApproval([DataSourceRequest] DataSourceRequest request, SearchTimeSheetModel model)
        {
            var thisWeekStart = DateTime.Today.StartOfWeek(DayOfWeek.Sunday);   // this week
            var weekStartDate = model.sf_From.StartOfWeek(DayOfWeek.Sunday);    // selected week
            var weekEndDate = weekStartDate.AddDays(6);
            var submittedOnly = model.sf_CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval;
            var totalData = _workTimeService.GetEmployeeWorkTimeApprovalAsQueryable(weekStartDate, account: _workContext.CurrentAccount, showRejected: true, submittedOnly: submittedOnly);
            if (weekStartDate <= thisWeekStart && (thisWeekStart - weekStartDate).Days <= 28)    // within 4 weeks
                totalData = totalData.Concat(_workTimeService.GetExpectedTimeSheets(_workContext.CurrentAccount, weekStartDate));

            // limit to the selected data range (i.s.o. whole week)
            // TODO: modify the above 2 query to filter by date range
            if (weekStartDate < model.sf_From || model.sf_To < weekEndDate)
            {
                var theNextDayAfterTo = model.sf_To.AddDays(1);
                totalData = totalData.Where(x => x.JobStartDateTime >= model.sf_From && x.JobStartDateTime < theNextDayAfterTo);
            }

            var gridModel = totalData.ToDataSourceResult(request, x => x.ToModel());
            if (request.Groups.Any())
            {
                var topLevelGroupModel = (IEnumerable<AggregateFunctionsGroup>)(gridModel.Data);
                // var secondLevelGroupModel = topLevelGroupModel.SelectMany(g => (IEnumerable<AggregateFunctionsGroup>)(g.Items));
                var cwtModels = topLevelGroupModel.SelectMany(g => (IEnumerable<EmployeeWorkTimeApprovalModel>)(g.Items)).ToArray();
                CandidateWorkOverTime[] overtimeData = new CandidateWorkOverTime[] { };
                Alerts[] alertsData = new Alerts[] { };
                if (cwtModels.Any())
                {
                    int[] candidateIds = cwtModels.Select(x => x.CandidateId).Distinct().ToArray();
                    int[] jobOrderIds = cwtModels.Select(x => x.JobOrderId).Distinct().ToArray();
                    var timeWindowStart = cwtModels.Min(x => x.JobStartDateTime).StartOfWeek(DayOfWeek.Sunday);
                    var timeWindowEnd = cwtModels.Max(x => x.JobEndDateTime).StartOfWeek(DayOfWeek.Sunday).AddDays(7);
                    overtimeData = _workTimeService.GetCandidateOvertime(candidateIds, jobOrderIds, timeWindowStart, timeWindowEnd).ToArray();
                    alertsData = _workTimeService.GetCandidateUnacknowledgedAlerts(candidateIds).ToArray();
                }
                foreach (EmployeeWorkTimeApprovalModel cwtModel in cwtModels)
                {
                    var candidateOvertime = overtimeData.Where(x => x.CandidateId == cwtModel.CandidateId && x.StartDate <= cwtModel.JobStartDateTime && x.EndDate >= cwtModel.JobEndDateTime);
                    if (candidateOvertime.Any())
                    {
                        cwtModel.OvertimeMessage = string.Format("Calculated overtime hrs in this period is {0} hrs", candidateOvertime.Sum(x => x.OvertimeHours)); //, cwtModel.WeekOfYear);
                    }
                    else
                    {
                        var candidateAlerts = alertsData.Where(x => x.CandidateId == cwtModel.CandidateId && x.Year == cwtModel.Year && x.WeekOfYear == cwtModel.WeekOfYear);
                        if (candidateAlerts.Any())
                        {
                            cwtModel.OtherAlerts = string.Join(" | ", candidateAlerts.Select(x => x.Message).Distinct().ToArray());
                        }
                    }
                }
            }

            return gridModel;
        }
    }
}
