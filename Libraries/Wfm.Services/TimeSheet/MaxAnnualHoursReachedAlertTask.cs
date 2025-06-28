using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Companies;
using Wfm.Services.ExportImport;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;
using Wfm.Services.TimeSheet;


namespace Wfm.Services.TimeSheet
{
    public partial class MaxAnnualHoursReachedAlertTask : IScheduledTask
    {
        #region Fields

        private readonly ICompanyService _companyService;
        private readonly ICompanySettingService _companySetttingService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IExportManager _exportManager;
        private readonly IWorkflowMessageService _workflowMessageService;
        //private readonly IAccountService _accountService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public MaxAnnualHoursReachedAlertTask(
            ICompanyService companyService,
            ICompanySettingService companySetttingService,
            IRecruiterCompanyService recruiterCompanyService,
            IWorkTimeService workTimeService,
            IExportManager exportManager,
            IWorkflowMessageService workflowMessageService,
            //IAccountService accountService,
            ILogger logger)
        {
            _companyService = companyService;
            _companySetttingService = companySetttingService;
            _recruiterCompanyService = recruiterCompanyService;
            _workTimeService = workTimeService;
            this._exportManager = exportManager;
            this._workflowMessageService = workflowMessageService;
            //this._accountService = accountService;
            this._logger = logger;
        }

        #endregion


        public virtual void Execute()
        {
            try
            {
                var companyIds = _companySetttingService.GetAllSettings().Where(x => x.Name.Contains("MaxAnnualHours."))
                                    .Select(x => x.CompanyId).Distinct().ToList();
                foreach (var id in companyIds)
                {
                    var companyName = _companyService.GetCompanyByIdForScheduleTask(id).CompanyName;
                    var maxAnnualHours = _companySetttingService.GetMaxAnnualHours(id);
                    if (maxAnnualHours.MaxHours > 0)
                    {
                        var year = DateTime.Today.AddYears(-1).AddDays(1).Year;
                        var startDate = new DateTime(year, maxAnnualHours.StartDate.Month, maxAnnualHours.StartDate.Day);
                        var endDate = new DateTime(DateTime.Today.Year, maxAnnualHours.EndDate.Month, maxAnnualHours.EndDate.Day);
                        var totalHours = _workTimeService.GetAllEmployeeTotalHours(startDate, endDate, new[] { (int)CandidateWorkTimeStatus.Approved }, id)
                                            .Where(x => x.TotalHours >= maxAnnualHours.Threshold);
                        var franchises = totalHours.Select(x => x.FranchiseId).Distinct().ToList();
                        foreach (var franchise in franchises)
                        {
                            var fracnhiseTotalHours = totalHours.Where(x => x.FranchiseId == franchise);
                            if (fracnhiseTotalHours.Any())
                            {
                                var recruiters = _recruiterCompanyService.GetAllRecruitersAsQueryable()
                                    .Where(x => x.CompanyId == id && x.Account.FranchiseId == franchise)
                                    .Select(x => x.Account.Email).ToList();
                                byte[] attachmentBytes = null;
                                var attachmentName = _GetEmployeeTotalHoursExcelFile(_exportManager, fracnhiseTotalHours, out attachmentBytes);
                                _workflowMessageService.SendMaxAnnualHoursReachedAlert(recruiters, attachmentName, attachmentBytes, companyName, startDate, endDate, maxAnnualHours.MaxHours);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Failed to send max annual hours reached alert. Error message : {0}", exc.Message), exc);
            }
        }


        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }


        private string _GetEmployeeTotalHoursExcelFile(IExportManager _exportManager, IEnumerable<EmployeeTotalHours> totalHours, out byte[] bytes)
        {
            var fileName = String.Empty;
            using (var stream = new MemoryStream())
            {
                fileName = _exportManager.ExportEmployeeTotalHoursToXlsx(stream, totalHours);
                bytes = stream.ToArray();
            }

            return fileName;
        }

    }

}
