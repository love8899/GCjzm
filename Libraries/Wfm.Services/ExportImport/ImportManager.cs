using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using Wfm.Services.Candidates;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Services.TimeSheet;


namespace Wfm.Services.ExportImport
{
    public partial class ImportManager : IImportManager
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private ILanguageService _languageService;
        private readonly CandidateSettings _candidateSettings;
        private readonly ISalutationService _salutationService;
        private readonly IGenderService _genderService;
        
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;
        private readonly IAddressTypeService _addressTypeService;
        private readonly ICityService _cityService;
        private readonly IStateProvinceService _provinceService;
        private readonly ICountryService _countryService;
        private readonly ISourceService _sourceService;
        private readonly IAccountService _accountService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICompanyContactService _companyContactService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly ICandidateService _candidatesService;
        private readonly ICandidateAddressService _candidateAddressservice;
        private readonly ICandidateKeySkillService _canidateSkillService;
        private readonly ISkillService _skillService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidateJobOrderStatusHistoryService _candidateJobOrderStatusHistoryService;
        private readonly IShiftService _shiftService;
        private readonly ITransportationService _transportationService;
        private readonly IIntersectionService _intersectionService;
        private readonly CandidateWorkTimeSettings _candidateWorkTimeSettings;
        private readonly IWorkTimeService _workTimeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICandidateBankAccountService _candidateBankAccountService;

        #endregion

        #region Ctor

        public ImportManager(
            IWebHelper webHelper,
            IWorkContext workContext,
            ILanguageService languageService,
            CandidateSettings candidateSettigns,
            ISalutationService salutationService,
            IGenderService genderService,
            
            IActivityLogService activityLogService,
            ILocalizationService localizationService,
            IAddressTypeService addressTypeAddress,
            ICityService cityService,
            IStateProvinceService provinceService,
            ICountryService countryService,
            ISourceService sourceService,
            IAccountService accountService,
            ICompanyService companyService,
            ICompanyDivisionService companyDivisionService,
            ICompanyDepartmentService companyDepartmentService,
            ICompanyContactService companyContactService,
            ICompanyBillingService companyBillingService,
            ICandidateService candidatesService,
            ICandidateAddressService candidateAddressService,
            ICandidateKeySkillService candidateSkillService,
            ISkillService skillService,
            ICompanyCandidateService companyCandidateService,
            IJobOrderService jobOrderService,
            ICandidateJobOrderService candidateJobOrderService,
            ICandidateJobOrderStatusHistoryService candidateJobOrderStatusHistoryService,
            IShiftService shiftService,
            ITransportationService transportationService,
            IIntersectionService intersectionService,
            CandidateWorkTimeSettings candidateWorkTimeSettings,
            IWorkTimeService workTimeService,
            IWorkflowMessageService workflowMessageService,
            ICandidateBankAccountService candidateBankAccountService
            )
        {
            _webHelper = webHelper;
            _workContext = workContext;
            _languageService = languageService;
            _candidateSettings = candidateSettigns;
            _salutationService = salutationService;
            _genderService = genderService;
            
            _activityLogService = activityLogService;
            _localizationService = localizationService;
            _addressTypeService = addressTypeAddress;
            _cityService = cityService;
            _provinceService = provinceService;
            _countryService = countryService;
            _sourceService = sourceService;
            _accountService = accountService;
            _companyService = companyService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _companyContactService = companyContactService;
            _companyBillingService = companyBillingService;
            _candidatesService = candidatesService;
            _candidateAddressservice = candidateAddressService;
            _canidateSkillService = candidateSkillService;
            _skillService = skillService;
            _companyCandidateService = companyCandidateService;
            _jobOrderService = jobOrderService;
            _candidateJobOrderService = candidateJobOrderService;
            _candidateJobOrderStatusHistoryService = candidateJobOrderStatusHistoryService;
            _shiftService = shiftService;
            _transportationService = transportationService;
            _intersectionService = intersectionService;
            _candidateWorkTimeSettings = candidateWorkTimeSettings;
            _workTimeService = workTimeService;
            _workflowMessageService = workflowMessageService;
            _candidateBankAccountService = candidateBankAccountService;
        }

        #endregion


        #region Utilities

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string ConvertColumnToString(object columnValue)
        {
            if (columnValue == null)
                return null;

            return Convert.ToString(columnValue);
        }


        #endregion


        #region Import Work Time

        public IList<string> ImportWorkTimeFromXlsx(Stream stream, out IList<string> warnings, bool isStdTmplt = true)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // customer Excel file
            if (!isStdTmplt)
                return this.ImportWorkTimeFromCustomerXlsxTypeII(stream, out warnings);

            // to store error messages
            IList<string> result = new List<string>();
            warnings = Enumerable.Empty<string>().ToList();

            // Let's begin
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Add("No worksheet found");
                    return result;
                }
                var maxRow = worksheet.Dimension.End.Row;
                var allCells = worksheet.Cells;

                // start date
                var startDateString = allCells[3, 1].Value.ToString().Split(' ')[2];
                var startDate = DateTime.Parse(startDateString);
                if (startDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    result.Add("The week start date in Row 3 is not Sunday, Please double check.");
                    return result;
                }

                // end date
                var endDate = new DateTime();
                if (startDate != null)
                    endDate = startDate.AddDays(6);
                else
                {
                    result.Add("Cannot get Start Date from the sheet");
                    return result;
                }

                //the columns
                var properties = new string[]
                {
                    "EmployeeName",
                    "BadgeId",
                    "EmployeeId",
                    "Position",
                    "PositionId",
                    "Shift",
                    "Type",
                    "Sun",
                    "Mon",
                    "Tue",
                    "Wed",
                    "Thu",
                    "Fri",
                    "Sat",
                    "TotalHrs",
                };

                // get companies need to import employee number too
                var companiesToUpdate = Enumerable.Empty<int>();
                var companies = _candidateWorkTimeSettings.CompaniesToImportEmployeeNumber;
                if (!String.IsNullOrWhiteSpace(companies))
                    companiesToUpdate = companies.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x));
                var allCandidates = Enumerable.Empty<Tuple<int, string>>().ToList();
                var candidatesToUpdate = Enumerable.Empty<Tuple<int, string>>().ToList();

                #region validation

                for (int iRow = 6; iRow < maxRow; iRow++)
                {
                    string employeeName = ConvertColumnToString(allCells[iRow, GetColumnIndex(properties, "EmployeeName")].Value);
                    int candidateId = Convert.ToInt32(allCells[iRow, GetColumnIndex(properties, "BadgeId")].Value);
                    string position = ConvertColumnToString(allCells[iRow, GetColumnIndex(properties, "Position")].Value);
                    int positionId = Convert.ToInt32(allCells[iRow, GetColumnIndex(properties, "PositionId")].Value);

                    //confirm employee Id and name
                    if (candidateId == 0)
                        result.Add(string.Format("Row {0}: Candidate Id is blank or invalid.", iRow));
                    else
                    {
                        Candidate candidate = _candidatesService.GetCandidateById(candidateId);
                        if (candidate != null)
                        {
                            var lastName = candidate.LastName ?? null;
                            var firstName = candidate.FirstName ?? null;
                            var fullName = "";
                            if (!String.IsNullOrWhiteSpace(lastName))
                                fullName += lastName.Trim();
                            fullName += ", ";
                            if (!String.IsNullOrWhiteSpace(firstName))
                                fullName += firstName.Trim();
                            if (!fullName.ToLower().StartsWith(employeeName.ToLower()))
                                result.Add(string.Format("Row {0}: Candidate Name is incorrect.", iRow));
                            else
                                allCandidates.Add(new Tuple<int, string>(candidate.Id, candidate.EmployeeId));
                        }
                        else
                            result.Add(string.Format("Row {0}: Candidate does not exist.", iRow));
                    }

                    //confirm job order
                    var jobOrder = new JobOrder();
                    if (positionId == 0)
                        result.Add(string.Format("Row {0}: Position Id is blank or invalid.", iRow));
                    else
                    {
                        jobOrder = _jobOrderService.GetJobOrderById(positionId);
                        if (jobOrder == null)
                            result.Add(string.Format("Row {0}: Job Order does not exist.", iRow));
                        else
                        {
                            // not in date range of job order
                            if ((jobOrder.EndDate.HasValue && startDate > jobOrder.EndDate.Value) || endDate < jobOrder.StartDate)
                                result.Add(string.Format("Row {0}: The start/end date are not in the date range of of job order [{1}].", iRow, positionId));

                            // if LocationId assgined
                            if (jobOrder.CompanyLocationId == 0)
                                result.Add(string.Format("Row {0}: The location Id of job order [{1}] is not assgined.", iRow, positionId));
                        }
                    }

                    //confirm employee is placed in the job order
                    if (candidateId != 0 && positionId != 0 && jobOrder != null)
                    {
                        if (!_candidateJobOrderService.IsCandidatePlacedInJobOrderWithinDateRange(positionId, candidateId, startDate, endDate))
                            result.Add(string.Format("Row {0}: Candidate [{1}] is not placed in the job order [{2}], on anyday from [{3}] to [{4}].",
                                                        iRow, candidateId, positionId, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")));
                        else
                        {
                            var sunHrsCellIndex = GetColumnIndex(properties, "Sun");
                            for (int i = 0; i < 7; i++)
                            {
                                var currentDate = startDate.AddDays(i).Date;
                                if (currentDate >= jobOrder.StartDate.Date && currentDate <= (jobOrder.EndDate.HasValue ? jobOrder.EndDate.Value.Date : endDate))
                                {
                                    var cellValue = allCells[iRow, sunHrsCellIndex + i].Value;
                                    if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
                                    {
                                        var currentDateHrs = Convert.ToDecimal(cellValue);
                                        if (currentDateHrs > 0 && !_candidateJobOrderService.IsCandidatePlacedInJobOrderWithinDateRange(positionId, candidateId, currentDate, currentDate))
                                            result.Add(string.Format("Row {0}: Candidate [{1}] is not placed in the job order [{2}], on [{3}].", iRow, candidateId, positionId, currentDate.ToString("yyyy-MM-dd")));
                                    }
                                }
                            }
                        }
                    }
                }

                if (result.Count > 0)
                    return result;

                #endregion

                // import starts now
                for (int iRow = 6; iRow < maxRow; iRow++)
                {
                    string employeeName = ConvertColumnToString(allCells[iRow, GetColumnIndex(properties, "EmployeeName")].Value);
                    int candidateId = Convert.ToInt32(allCells[iRow, GetColumnIndex(properties, "BadgeId")].Value);
                    var employeeNo = ConvertColumnToString(allCells[iRow, GetColumnIndex(properties, "EmployeeId")].Value);
                    string position = ConvertColumnToString(allCells[iRow, GetColumnIndex(properties, "Position")].Value);
                    int positionId = Convert.ToInt32(allCells[iRow, GetColumnIndex(properties, "PositionId")].Value);
                    var jobOrder = _jobOrderService.GetJobOrderById(positionId);

                    // collect candidates to update
                    if (companiesToUpdate.Contains(jobOrder.CompanyId) && !String.IsNullOrWhiteSpace(employeeNo) && allCandidates.First(x => x.Item1 == candidateId).Item2 != employeeNo)
                        candidatesToUpdate.Add(new Tuple<int, string>(candidateId, employeeNo));

                    var sunHrsCellIndex = GetColumnIndex(properties, "Sun");
                    for (int i = 0; i < 7; i++)
                    {
                        var currentDate = startDate.AddDays(i).Date;
                        if (currentDate >= jobOrder.StartDate.Date && currentDate <= (jobOrder.EndDate.HasValue ? jobOrder.EndDate.Value.Date : endDate))
                        {
                            var cellValue = allCells[iRow, sunHrsCellIndex + i].Value;
                            if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue.ToString()))
                            {
                                var currentDateHrs = Convert.ToDecimal(cellValue);

                                // Add/Update CandidateWorkTime
                                _workTimeService.InsertOrUpdateWorkTime(positionId, candidateId, currentDate, currentDateHrs, WorkTimeSource.Import, logging: false);
                            }
                        }
                    }
                }

                // calculation of OT
                _workTimeService.CalculateOTforWorktimeWithinDateRange(startDate, endDate);

                // update Employee No.
                if (candidatesToUpdate.Any())
                    _candidatesService.UpdateEmployeeNumbers(candidatesToUpdate);
            }

            return result;

        }


        public IList<string> ImportWorkTimeFromCustomerXlsxTypeI(Stream stream, int companyId, int locationId = 0)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            IList<string> result = new List<string>();

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Add("No worksheet found");
                    return result;
                }

                var maxRow = worksheet.Dimension.End.Row;
                var allCells = worksheet.Cells;

                #region determin header row and locations of valid columns

                var headerCells = allCells[1, 1, 5, 20];
                var weekEndDateCell = (from cell in headerCells
                                       where cell.Value is string && (cell.Value.ToString().StartsWith("Weekend Date") || cell.Value.ToString().StartsWith("Date Range"))
                                       select cell).FirstOrDefault();
                var nameCell = (from cell in headerCells
                                where cell.Value is string && cell.Value.Equals("Employee Name")
                                select cell).FirstOrDefault();
                var idCell = (from cell in headerCells
                              where cell.Value is string && cell.Value.Equals("Emp ")
                              select cell).FirstOrDefault();
                var workDateCell = (from cell in headerCells
                                    where cell.Value is string && cell.Value.Equals("WorkDate")
                                    select cell).FirstOrDefault();
                var positionCell = (from cell in headerCells
                                    where cell.Value is string && cell.Value.Equals("Activity")
                                    select cell).FirstOrDefault();
                var totalHoursCell = (from cell in headerCells
                                      where cell.Value is string && cell.Value.Equals("Total ")
                                      select cell).FirstOrDefault();

                int headerRow = 0;
                if (weekEndDateCell != null && nameCell != null && idCell != null && nameCell.Start.Row == idCell.Start.Row)
                    headerRow = nameCell.Start.Row;
                else
                {
                    result.Add(string.Format("Cannot find header row from the work sheet {0}.", worksheet.Name));
                    return result;
                }

                #endregion

                // start & end date
                var startDateString = allCells[weekEndDateCell.Start.Row, weekEndDateCell.Start.Column].Value.ToString();
                int startingIndex = startDateString.IndexOf('[');
                int endingIndex = startDateString.IndexOf(']');
                startDateString = startDateString.Substring(startingIndex + 1, endingIndex - startingIndex - 1);

                var startDate = DateTime.Parse(startDateString);
                var endDate = new DateTime();
                if (startDate != null && startDate.DayOfWeek == DayOfWeek.Sunday)
                    endDate = startDate.AddDays(6);
                else
                {
                    result.Add("Cannot get correct Start or End Date from the sheet.");
                    return result;
                }

                var jobOrders = _jobOrderService.GetJobOrdersByDateRangeAsQueryable(startDate, endDate)
                                    .Where(x => x.CompanyId == companyId && x.JobOrderStatusId == (int)JobOrderStatusEnum.Active);
                if (jobOrders != null && locationId != 0)
                    jobOrders = jobOrders.Where(x => x.CompanyLocationId == locationId);

                if (jobOrders == null)
                {
                    result.Add("Job Orders are not created correctly.");
                    return result;
                }

                var activeJobOrders = jobOrders.Where(x => x.BillingRateCode.ToLower().StartsWith("auto_")).ToList();
                if (activeJobOrders == null || activeJobOrders.Count < 4)
                {
                    result.Add("Job Orders are not created correctly.");
                    return result;
                }
                else
                {
                    foreach (var jo in activeJobOrders)
                        if (jo.CompanyLocationId == 0)
                            result.Add("Job order [{0}] location Id is not assigned.");
                }

                #region validation of all Candidate Id and Name

                int iRow = headerRow;
                while (iRow < maxRow)
                {
                    // seek valid row
                    string employeeName = null;
                    for (int i = 0; i < 20; i++)
                    {
                        iRow++;
                        employeeName = ConvertColumnToString(allCells[iRow, nameCell.Start.Column].Value);
                        var position = ConvertColumnToString(allCells[iRow, positionCell.Start.Column].Value);
                        if (!String.IsNullOrWhiteSpace(employeeName) && !employeeName.Equals("Employee Name") &&
                            !String.IsNullOrWhiteSpace(position))
                            break;
                    }

                    // no valid row any more
                    if (employeeName == null)
                        break;

                    // seek same employee
                    int sameEmployeeNum = 0;
                    string trailName = string.Empty;
                    while (iRow < maxRow)
                    {
                        iRow++;
                        var nextEmployeeName = ConvertColumnToString(allCells[iRow, nameCell.Start.Column].Value);
                        var position = ConvertColumnToString(allCells[iRow, positionCell.Start.Column].Value);
                        if (!String.IsNullOrWhiteSpace(nextEmployeeName) && !nextEmployeeName.Equals("Employee Name") &&
                            !nextEmployeeName.Equals(employeeName + trailName))
                            if (!String.IsNullOrWhiteSpace(position))
                                break;
                            else
                                trailName = nextEmployeeName.Trim();
                        sameEmployeeNum++;
                    }
                    if (iRow != maxRow)
                    {
                        iRow--;
                        if (!String.IsNullOrWhiteSpace(trailName))
                            employeeName += trailName;
                    }
                    else
                        break;

                    int employeeId = Convert.ToInt32(allCells[iRow - sameEmployeeNum, idCell.Start.Column].Value);

                    if (employeeId == 0)
                        result.Add(string.Format("Candidate Id [{0}] from Row [{1}] to Row [{2}] is blank.", employeeId, iRow - sameEmployeeNum, iRow));
                    else
                    {
                        employeeName = employeeName.Replace(",  ", ", ").Trim();
                        Candidate candidate = _candidatesService.GetCandidateById(employeeId);
                        if (candidate == null)
                            result.Add(string.Format("Candidate Id [{0}] from Row [{1}] to Row [{2}] is invalid.", employeeId, iRow - sameEmployeeNum, iRow));
                        else
                        {
                            var lastName = candidate.LastName ?? null;
                            var firstName = candidate.FirstName ?? null;
                            var fullName = "";
                            if (!String.IsNullOrWhiteSpace(lastName))
                                fullName += lastName.Trim();
                            fullName += ", ";
                            if (!String.IsNullOrWhiteSpace(firstName))
                                fullName += firstName.Trim();
                            if (!fullName.ToLower().StartsWith(employeeName.ToLower()))
                                result.Add(string.Format("Candidate Name [{0}] and Id [{1}] from Row [{2}] to Row [{3}], does not match the name [{4}] in our database.", employeeName, employeeId, iRow - sameEmployeeNum, iRow, fullName));
                        }
                    }

                }

                #endregion

                if (result.Count > 0)
                    return result;

                iRow = headerRow;
                while (iRow < maxRow)
                {
                    // seek valid row
                    string employeeName = null;
                    for (int i = 0; i < 20; i++)
                    {
                        iRow++;
                        employeeName = ConvertColumnToString(allCells[iRow, nameCell.Start.Column].Value);
                        var position = ConvertColumnToString(allCells[iRow, positionCell.Start.Column].Value);
                        if (!String.IsNullOrWhiteSpace(employeeName) && !employeeName.Equals("Employee Name") &&
                            !String.IsNullOrWhiteSpace(position))
                            break;
                    }

                    // no valid row any more
                    if (employeeName == null)
                        break;

                    employeeName = employeeName.Replace(",  ", ", ").Trim();
                    int employeeId = Convert.ToInt32(allCells[iRow, idCell.Start.Column].Value);
                    DateTime workDate = DateTime.Parse(ConvertColumnToString(allCells[iRow, workDateCell.Start.Column].Value));

                    // check if multiple rates
                    int rowNum = 1;
                    int skipped = 0;
                    while (iRow < maxRow - skipped)
                    {
                        var nextDateString = ConvertColumnToString(allCells[iRow + skipped + 1, workDateCell.Start.Column].Value);
                        var nextIdString = ConvertColumnToString(allCells[iRow + skipped + 1, idCell.Start.Column].Value);
                        var nextId = 0;
                        var isIdStringNumeric = false;
                        if (!String.IsNullOrWhiteSpace(nextIdString))
                            isIdStringNumeric = int.TryParse(nextIdString, out nextId);
                        if (!isIdStringNumeric || String.IsNullOrWhiteSpace(nextDateString) || nextDateString.Equals("WorkDate"))
                        {
                            skipped++;
                        }
                        else if (nextId == employeeId && DateTime.Parse(nextDateString) == workDate)
                        {
                            rowNum++;
                            skipped++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    int firstRow = iRow;
                    int lastRow = firstRow + skipped;
                    int hourRow = lastRow, rateRow = firstRow;
                    for (int i = 0; i <= skipped; i++)
                    {
                        var position = ConvertColumnToString(allCells[firstRow + i, positionCell.Start.Column].Value);
                        if (!String.IsNullOrWhiteSpace(position))
                            if (position.Equals("WORK HOURS"))
                                hourRow = firstRow + i;
                            else if (rowNum == 2 && (position.Equals("DOLLAR INCREMENTS") || position.Equals("PREMIUM HOURS")))
                                rateRow = firstRow + i;
                    }
                    decimal totalHours = Convert.ToDecimal(allCells[hourRow, totalHoursCell.Start.Column].Value.ToString());

                    // choose job order according to rate
                    var jobOrder = new JobOrder();
                    switch (rowNum)
                    {
                        case 1:
                            jobOrder = activeJobOrders.Where(x => x.BillingRateCode.ToLower().StartsWith("auto_gl / d")).FirstOrDefault();
                            break;
                        case 2:
                            string pos = ConvertColumnToString(allCells[rateRow, positionCell.Start.Column].Value);
                            if (pos.Equals("DOLLAR INCREMENTS"))
                                jobOrder = activeJobOrders.Where(x => x.BillingRateCode.ToLower().StartsWith("auto_gls / d")).FirstOrDefault();
                            else if (pos.Equals("PREMIUM HOURS"))
                                jobOrder = activeJobOrders.Where(x => x.BillingRateCode.ToLower().StartsWith("auto_gl / a")).FirstOrDefault();
                            break;
                        case 3:
                            jobOrder = activeJobOrders.Where(x => x.BillingRateCode.ToLower().StartsWith("auto_gls / a")).FirstOrDefault();
                            break;
                        default:
                            result.Add(String.Format("Cannot process hours starting from Row [{0}]", firstRow));
                            break;
                    }

                    if (jobOrder == null || jobOrder.Id == 0)
                        result.Add(String.Format("Cannot find a job order for hours starting from Row [{0}]", firstRow));
                    else
                    {
                        int enteredBy = 0;
                        if (_workContext.CurrentAccount != null && enteredBy != _workContext.CurrentAccount.Id)
                            enteredBy = _workContext.CurrentAccount.Id;

                        // place into job order
                        _candidateJobOrderService.InsertOrUpdateCandidateJobOrder(jobOrder.Id, employeeId, workDate, (int)CandidateJobOrderStatusEnum.Placed, null, enteredBy, logging: false);

                        // insert or candidate work time
                        _workTimeService.InsertOrUpdateWorkTime(jobOrder.Id, employeeId, workDate, totalHours, WorkTimeSource.Import, logging: false);
                    }

                    iRow = lastRow;
                }

                // calculation of OT
                _workTimeService.CalculateOTforWorktimeWithinDateRange(startDate, endDate);

            }

            return result;

        }


        private class TimeSheetImport
        {
            public int CandidateId { get; set; }
            public string EmployeeId { get; set; }
            public DateTime WorkDate { get; set; }
            public string PayType { get; set; }
            public string DeptNum { get; set; }
            public decimal Hours { get; set; }
        }


        public IList<string> ImportWorkTimeFromCustomerXlsxTypeII(Stream stream, out IList<string> warnings)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            IList<string> result = new List<string>();
            warnings = Enumerable.Empty<string>().ToList();

            using (var xlPackage = new ExcelPackage(stream))
            {
                var allWorksheets = xlPackage.Workbook.Worksheets.Where(x => x.Name.EndsWith("GC - Data")).ToList();
                if (allWorksheets == null || allWorksheets.Count == 0)
                {
                    result.Add("No worksheet found.");
                    return result;
                }

                var CandidateIdMap = new Dictionary<string, int>();

                #region validation

                foreach (ExcelWorksheet worksheet in allWorksheets)
                {
                    var sheetDim = worksheet.Dimension;
                    if (sheetDim != null)
                    {
                        var maxRow = sheetDim.End.Row;
                        int firstRow = 2;
                        var allCells = worksheet.Cells;

                        #region determin header row and locations of valid columns

                        var headerCells = allCells[1, 1, 1, worksheet.Dimension.Columns];

                        var empIdCell = (from cell in headerCells
                                         where cell.Value is string && cell.Value.Equals("PayID")
                                         select cell).FirstOrDefault();
                        var firstNameCell = (from cell in headerCells
                                             where cell.Value is string && cell.Value.Equals("FirstName")
                                             select cell).FirstOrDefault();
                        var lastNameCell = (from cell in headerCells
                                            where cell.Value is string && cell.Value.Equals("LastName")
                                            select cell).FirstOrDefault();
                        var workDateCell = (from cell in headerCells
                                            where cell.Value is string && cell.Value.Equals("WorkDate")
                                            select cell).FirstOrDefault();
                        var payTypeCell = (from cell in headerCells
                                           where cell.Value is string && cell.Value.Equals("PayType")
                                           select cell).FirstOrDefault();
                        var hoursCell = (from cell in headerCells
                                         where cell.Value is string && cell.Value.Equals("Hours")
                                         select cell).FirstOrDefault();
                        var deptNumCell = (from cell in headerCells
                                           where cell.Value is string && cell.Value.Equals("Dept #")
                                           select cell).FirstOrDefault();
                        var cmpyCodeCell = (from cell in headerCells
                                            where cell.Value is string && cell.Value.Equals("Location")
                                            select cell).FirstOrDefault();

                        if (empIdCell == null || firstNameCell == null || lastNameCell == null || workDateCell == null &&
                            payTypeCell == null || hoursCell == null || deptNumCell == null || cmpyCodeCell == null)
                        {
                            result.Add("Cannot find valid header row.");
                            return result;
                        }

                        #endregion

                        var iRow = firstRow;
                        while (iRow <= maxRow)
                        {
                            var employeeId = ConvertColumnToString(allCells[iRow, empIdCell.Start.Column].Value);
                            var firstName = ConvertColumnToString(allCells[iRow, firstNameCell.Start.Column].Value);
                            var lastName = ConvertColumnToString(allCells[iRow, lastNameCell.Start.Column].Value);

                            // seek same employee
                            int sameEmployeeNum = 0;
                            while (iRow < maxRow)
                            {
                                var nextEmployeeId = ConvertColumnToString(allCells[iRow + 1, empIdCell.Start.Column].Value);
                                if (nextEmployeeId != employeeId)
                                    break;
                                sameEmployeeNum++;
                                iRow++;
                            }

                            if (String.IsNullOrWhiteSpace(employeeId))
                                result.Add(string.Format("{0},{1},{2},{3},[{4}]-[{5}],blank employee Id", worksheet.Name, string.Empty, firstName, lastName, iRow - sameEmployeeNum, iRow));
                            else
                            {
                                var candidate = _candidatesService.GetCandidateByVendorIdAndEmployeeId(1, employeeId);
                                if (candidate == null)
                                    result.Add(string.Format("{0},{1},{2},{3},[{4}]-[{5}],invalid employee Id", worksheet.Name, employeeId, firstName, lastName, iRow - sameEmployeeNum, iRow));
                                else
                                {
                                    if (!CommonHelper.TrimAndToLower(candidate.FirstName).StartsWith(CommonHelper.TrimAndToLower(firstName)) ||
                                        !CommonHelper.TrimAndToLower(candidate.LastName).StartsWith(CommonHelper.TrimAndToLower(lastName)))
                                        warnings.Add(string.Format("{0},{1},{2},{3},[{4}]-[{5}],not match names in DB,{6},{7},{8},{9}", worksheet.Name, employeeId, firstName, lastName, 
                                            iRow - sameEmployeeNum, iRow, candidate.FirstName, candidate.LastName, candidate.Id, CommonHelper.ToCanadianPhone(candidate.MobilePhone)));
                                    
                                    if (!CandidateIdMap.ContainsKey(employeeId))
                                        CandidateIdMap.Add(employeeId, candidate.Id);
                                }
                            }

                            iRow++;
                        }
                    }
                }

                #endregion

                if (result.Any())
                    return result;

                #region import

                // TODO: refactoring - read all rows, group by candidate & date and sum rate, then map job order by rate

                // paytypes
                // TODO: sum rate/premium, then match by rate
                var regularPayType = new List<string>() { _candidateWorkTimeSettings.WinnersRegularPayType };
                var afternoonPayType = new List<string>() { _candidateWorkTimeSettings.WinnersRegularPayType, _candidateWorkTimeSettings.WinnersAfternoonPayType }.OrderBy(x => x);
                var nightPayType = new List<string>() { _candidateWorkTimeSettings.WinnersRegularPayType, _candidateWorkTimeSettings.WinnersNightPayType }.OrderBy(x => x);
                var weekendRegularPayType = new List<string>() { _candidateWorkTimeSettings.WinnersRegularPayType, _candidateWorkTimeSettings.WinnersWeekendPayType }.OrderBy(x => x);

                // get departments with premium rate
                string[] shippingDeptList = null;
                var shippingDepts = _candidateWorkTimeSettings.WinnersDepartmentsWithShippingPremium;
                if (!String.IsNullOrWhiteSpace(shippingDepts))
                    shippingDeptList = shippingDepts.Split(',');
                string[] sortationDeptList = null;
                var sortationDepts = _candidateWorkTimeSettings.WinnersDepartmentsWithSortationPremium;
                if (!String.IsNullOrWhiteSpace(sortationDepts))
                    sortationDeptList = sortationDepts.Split(',');

                foreach (ExcelWorksheet worksheet in allWorksheets)
                {
                    var sheetDim = worksheet.Dimension;
                    if (sheetDim != null)
                    {
                        // for placement validation and alerts
                        var bannedIds = new List<int>();
                        var duplicates = new List<string>();

                        var maxRow = sheetDim.End.Row;
                        int firstRow = 2;
                        var allCells = worksheet.Cells;

                        #region determin header row and locations of valid columns

                        var headerCells = allCells[1, 1, 1, worksheet.Dimension.Columns];

                        var empIdCell = (from cell in headerCells
                                         where cell.Value is string && cell.Value.Equals("PayID")
                                         select cell).FirstOrDefault();
                        var firstNameCell = (from cell in headerCells
                                             where cell.Value is string && cell.Value.Equals("FirstName")
                                             select cell).FirstOrDefault();
                        var lastNameCell = (from cell in headerCells
                                            where cell.Value is string && cell.Value.Equals("LastName")
                                            select cell).FirstOrDefault();
                        var workDateCell = (from cell in headerCells
                                            where cell.Value is string && cell.Value.Equals("WorkDate")
                                            select cell).FirstOrDefault();
                        var payTypeCell = (from cell in headerCells
                                           where cell.Value is string && cell.Value.Equals("PayType")
                                           select cell).FirstOrDefault();
                        var hoursCell = (from cell in headerCells
                                         where cell.Value is string && cell.Value.Equals("Hours")
                                         select cell).FirstOrDefault();
                        var deptNumCell = (from cell in headerCells
                                           where cell.Value is string && cell.Value.Equals("Dept #")
                                           select cell).FirstOrDefault();
                        var cmpyCodeCell = (from cell in headerCells
                                            where cell.Value is string && cell.Value.Equals("Location")
                                            select cell).FirstOrDefault();

                        if (empIdCell == null || firstNameCell == null || lastNameCell == null || workDateCell == null &&
                            payTypeCell == null || hoursCell == null || deptNumCell == null || cmpyCodeCell == null)
                        {
                            result.Add("Cannot find valid header row.");
                            return result;
                        }

                        #endregion

                        #region Get start/end dates, company, job orders (from first row)

                        var startDate = DateTime.Parse(ConvertColumnToString(allCells[firstRow, workDateCell.Start.Column].Value));
                        if (startDate == null)
                        {
                            warnings.Add(String.Format("Cannot get week start date from [{0}]", worksheet.Name));
                            continue;
                        }
                        startDate = startDate.AddDays(DayOfWeek.Sunday - startDate.DayOfWeek);
                        var endDate = startDate.AddDays(6);

                        var companyCode = ConvertColumnToString(allCells[firstRow, cmpyCodeCell.Start.Column].Value);
                        if (String.IsNullOrWhiteSpace(companyCode))
                        {
                            warnings.Add(String.Format("Cannot get company code from [{0}]", worksheet.Name));
                            continue;
                        }
                        var company = _companyService.GetCompanyByCode(companyCode);
                        if (company == null)
                        {
                            warnings.Add(String.Format("Cannot get company by code from [{0}]", worksheet.Name));
                            continue;
                        }

                        var jobOrders = _jobOrderService.GetJobOrdersByDateRangeAsQueryable(startDate, endDate)
                                        .Where(x => x.CompanyId == company.Id && x.JobOrderStatusId == (int)JobOrderStatusEnum.Active);
                        if (jobOrders == null)
                        {
                            warnings.Add(String.Format("Job Orders are not created correctly for {0}", company.CompanyName));
                            continue;
                        }

                        var activeJobOrders = jobOrders.Where(x => x.BillingRateCode.ToLower().StartsWith("auto_")).ToList();
                        if (activeJobOrders == null || activeJobOrders.Count < 6)
                        {
                            warnings.Add(String.Format("Job Orders are not created correctly for {0}", company.CompanyName));
                            continue;
                        }

                        #endregion

                        #region read and consolidate all rows

                        var iRow = firstRow;
                        var timeSheets = new List<TimeSheetImport>();
                        while (iRow <= maxRow)
                        {
                            var employeeId = ConvertColumnToString(allCells[iRow, empIdCell.Start.Column].Value);
                            timeSheets.Add(new TimeSheetImport()
                            {
                                CandidateId = CandidateIdMap.ContainsKey(employeeId) ? CandidateIdMap[employeeId] : 0,
                                EmployeeId = employeeId,
                                WorkDate = DateTime.Parse(ConvertColumnToString(allCells[iRow, workDateCell.Start.Column].Value)),
                                PayType = ConvertColumnToString(allCells[iRow, payTypeCell.Start.Column].Value),
                                DeptNum = ConvertColumnToString(allCells[iRow, deptNumCell.Start.Column].Value),
                                Hours = Convert.ToDecimal(allCells[iRow, hoursCell.Start.Column].Value.ToString()),
                            });

                            iRow++;
                        }

                        // aggregate PayType by WorkDate
                        var aggregated = timeSheets.GroupBy(x => new { x.CandidateId, x.WorkDate, x.DeptNum, x.PayType })
                            .Select(g => new TimeSheetImport()
                            {
                                CandidateId = g.Key.CandidateId,
                                EmployeeId = g.First().EmployeeId,
                                WorkDate = g.Key.WorkDate,
                                PayType = g.Key.PayType,
                                DeptNum = g.Key.DeptNum,
                                Hours = g.Sum(x => x.Hours)
                            });
                        aggregated = aggregated.GroupBy(x => new { x.CandidateId, x.WorkDate, x.DeptNum })
                            .Select(g => new TimeSheetImport()
                            {
                                CandidateId = g.Key.CandidateId,
                                EmployeeId = g.First().EmployeeId,
                                WorkDate = g.Key.WorkDate,
                                PayType = g.Aggregate("", (a, b) => (a == "" ? "" : a + ",") + b.PayType),
                                DeptNum = g.Key.DeptNum,
                                Hours = g.First().Hours
                            });

                        // last working day within this week
                        var lastWorkingDays = timeSheets.Where(x => x.WorkDate <= endDate).GroupBy(x => x.CandidateId).Select(g => new
                        {
                            CandidateId = g.Key,
                            LastWorkingDay = g.Where(x => x.WorkDate <= endDate).Max(x => x.WorkDate)
                        });

                        // forced out-of-scope date to be the last working day
                        var forced = aggregated.Join(lastWorkingDays, a => a.CandidateId, l => l.CandidateId, (a, l) => new TimeSheetImport()
                        {
                            CandidateId = a.CandidateId,
                            EmployeeId = a.EmployeeId,
                            WorkDate = a.WorkDate <= l.LastWorkingDay ? a.WorkDate : l.LastWorkingDay,
                            PayType = a.PayType,
                            DeptNum = a.DeptNum,
                            Hours = a.Hours
                        });

                        // sum for the same WorkDate, PayType, and DeptNum
                        var summarized = forced.GroupBy(x => new { x.CandidateId, x.WorkDate, x.PayType, x.DeptNum })
                            .Select(g => new TimeSheetImport()
                            {
                                CandidateId = g.Key.CandidateId,
                                EmployeeId = g.First().EmployeeId,
                                WorkDate = g.Key.WorkDate,
                                PayType = g.Key.PayType,
                                DeptNum = g.Key.DeptNum,
                                Hours = g.Sum(x => x.Hours)
                            });

                        #endregion

                        #region import of rows

                        int lastCandidateId = 0;
                        JobOrder lastJobOrder = null;
                        DateTime lastWorkDate = DateTime.MinValue;
                        decimal lastHours = 0m;

                        foreach (var timeSheet in summarized)
                        {
                            // get shift code
                            var payType = timeSheet.PayType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).OrderBy(x => x);
                            var dayCode = string.Empty;
                            var shiftCode = string.Empty;
                            if (payType.SequenceEqual(regularPayType))
                                shiftCode = "d";
                            else if (payType.SequenceEqual(afternoonPayType))
                                shiftCode = "a";
                            else if (payType.SequenceEqual(nightPayType))
                                shiftCode = "n";
                            else if (payType.SequenceEqual(weekendRegularPayType))
                            {
                                dayCode = "w";
                                shiftCode = "d";
                            }

                            // get rate code
                            var rateCode = "UNDEFINED";
                            if (shippingDepts != null && shippingDepts.Contains(timeSheet.DeptNum))
                                rateCode = String.Concat("auto_gls", dayCode, " / " , shiftCode);
                            else if (sortationDepts != null && sortationDepts.Contains(timeSheet.DeptNum))
                                rateCode = String.Concat("auto_glsort", dayCode, " / ", shiftCode);
                            else
                                rateCode = String.Concat("auto_gl", dayCode, " / ", shiftCode);

                            // get job order
                            var jobOrder = activeJobOrders.Where(x => x.BillingRateCode.ToLower().StartsWith(rateCode)).FirstOrDefault();
                            if (jobOrder == null || jobOrder.Id == 0)
                                result.Add(String.Format("{0}: Cannot find a job order: {1};{2};{3};{4};{5}", worksheet.Name, 
                                    timeSheet.EmployeeId, timeSheet.WorkDate.ToShortDateString(), timeSheet.PayType, timeSheet.DeptNum, timeSheet.Hours));
                            else
                            {
                                int enteredBy = 0;
                                if (_workContext.CurrentAccount != null && enteredBy != _workContext.CurrentAccount.Id)
                                    enteredBy = _workContext.CurrentAccount.Id;

                                #region placement validation

                                if (!bannedIds.Contains(timeSheet.CandidateId) &&
                                    _candidatesService.IsCandidateBannedByCompanyAndDateRange(timeSheet.CandidateId, company.Id, timeSheet.WorkDate, timeSheet.WorkDate))
                                    bannedIds.Add(timeSheet.CandidateId);

                                var duplicate = _candidateJobOrderService.AnyOtherPlacementWithinDateRange(jobOrder.Id, timeSheet.CandidateId, timeSheet.WorkDate, timeSheet.WorkDate);
                                if (!String.IsNullOrWhiteSpace(duplicate))
                                    duplicates.Add(String.Concat(timeSheet.WorkDate.ToShortDateString(), ": ", duplicate));

                                #endregion

                                // place into job order
                                _candidateJobOrderService.InsertOrUpdateCandidateJobOrder(jobOrder.Id, timeSheet.CandidateId, timeSheet.WorkDate, (int)CandidateJobOrderStatusEnum.Placed, timeSheet.WorkDate, enteredBy, logging: false);

                                // insert or update candidate work time
                                if (timeSheet.CandidateId == lastCandidateId && jobOrder == lastJobOrder && timeSheet.WorkDate == lastWorkDate)
                                    timeSheet.Hours += lastHours;
                                _workTimeService.InsertOrUpdateWorkTime(jobOrder.Id, timeSheet.CandidateId, timeSheet.WorkDate, timeSheet.Hours, WorkTimeSource.Import, logging: false);

                                lastCandidateId = timeSheet.CandidateId;
                                lastJobOrder = jobOrder;
                                lastWorkDate = timeSheet.WorkDate;
                                lastHours = timeSheet.Hours;
                            }
                        }

                        #endregion

                        // calculation of OT
                        _workTimeService.CalculateOTforWorktimeWithinDateRange(startDate, endDate);

                        // alert
                        if (bannedIds.Any() || duplicates.Any())
                        {
                            var receivers = activeJobOrders.Select(x => x.RecruiterId).Distinct().ToList();
                            _workflowMessageService.SendTimeSheetImportPlacementValidationAlert(company.CompanyName, bannedIds, duplicates, receivers, _workContext.CurrentAccount.Email);
                        }
                    }
                }

                #endregion
            }

            return result;
        }

        #endregion


        #region Import Candidates

        public IList<string> ImportCandidateFromXlsx(Stream stream, int companyId)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // to store error messages
            IList<string> result = new List<string>();

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var allWorksheets = xlPackage.Workbook.Worksheets;
                if (allWorksheets == null && allWorksheets.Count == 0)
                {
                    result.Add("No worksheet found");
                    return result;
                }

                foreach (ExcelWorksheet worksheet in allWorksheets)
                {
                    var allCells = worksheet.Cells;

                    #region determin header row and locations of valid columns

                    int maxHearderRows = 20;
                    int maxColumns = 20;
                    var idCell = (from cell in allCells[1, 1, maxHearderRows, maxColumns]
                                  where cell.Value is string && (cell.Value.ToString().Trim().ToLower().Equals("gc id") || cell.Value.ToString().Trim().ToLower().Equals("id"))
                                  select cell).FirstOrDefault();
                    var nameCell = (from cell in allCells[1, 1, maxHearderRows, maxColumns]
                                    where cell.Value is string && (cell.Value.ToString().Trim().ToLower().Equals("worker's name") || cell.Value.ToString().Trim().ToLower().Equals("name"))
                                    select cell).FirstOrDefault();
                    var positionCell = (from cell in allCells[1, 1, maxHearderRows, maxColumns]
                                        where cell.Value is string && cell.Value.ToString().Trim().ToLower().Equals("position")
                                        select cell).FirstOrDefault();
                    var startDateCell = (from cell in allCells[1, 1, maxHearderRows, maxColumns]
                                         where cell.Value is string && (cell.Value.ToString().Trim().ToLower().Equals("start date"))
                                         select cell).FirstOrDefault();
                    var endDateCell = (from cell in allCells[1, 1, maxHearderRows, maxColumns]
                                       where cell.Value is string && (cell.Value.ToString().Trim().ToLower().Equals("end date"))
                                       select cell).FirstOrDefault();
                    var reasonCell = (from cell in allCells[1, 1, maxHearderRows, maxColumns]
                                      where cell.Value is string && cell.Value.ToString().Trim().ToLower().Equals("reason for leave")
                                      select cell).FirstOrDefault();
                    var noteCell = (from cell in allCells[1, 1, maxHearderRows, maxColumns]
                                    where cell.Value is string && (cell.Value.ToString().Trim().ToLower().StartsWith("note"))
                                    select cell).FirstOrDefault();

                    int headerRow = 0;
                    if (nameCell != null && startDateCell != null && nameCell.Start.Row == startDateCell.Start.Row)
                        headerRow = nameCell.Start.Row;
                    else
                    {
                        // result.Add(string.Format("Cannot find header row from the work sheet {0}.", worksheet.Name));
                        continue;
                    }

                    #endregion

                    int iRow = headerRow + 1;
                    while (true)
                    {
                        //stop if there no key data
                        bool allColumnsAreEmpty = true;
                        for (var i = 2; i <= 4; i++)
                        {
                            if (allCells[iRow, i].Value != null && !String.IsNullOrWhiteSpace(allCells[iRow, i].Value.ToString()))
                            {
                                allColumnsAreEmpty = false;
                                break;
                            }
                        }

                        if (allColumnsAreEmpty)
                            break;

                        int candidateId = 0;
                        if (idCell != null)
                        {
                            var candidateIdString = ConvertColumnToString(allCells[iRow, idCell.Start.Column].Value).Trim();
                            candidateId = Int32.TryParse(candidateIdString, out candidateId) ? candidateId : 0;
                        }
                        string candidateName = ConvertColumnToString(allCells[iRow, nameCell.Start.Column].Value).Trim();

                        if (candidateName != null)
                        {
                            IList<Candidate> canList = _candidatesService.SearchCandidates(candidateName, 10, true);
                            if (canList != null && canList.Count > 0)
                            {
                                foreach (var can in canList)
                                {
                                    if (candidateId == 0)
                                        candidateId = can.Id;
                                    else if (candidateId == can.Id)
                                    {
                                        candidateName = can.GetFullName();
                                        break;
                                    }
                                    else if (candidateName == can.GetFullName())
                                    {
                                        candidateId = can.Id;
                                        break;
                                    }
                                }
                            }
                        }

                        var candidate = _candidatesService.GetCandidateById(candidateId);

                        // candidate information not available
                        if (candidateId == 0 || candidateName == null || candidate == null)
                        {
                            iRow++;
                            continue;
                        }
                        if (!candidate.IsEmployee && !(candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started) && !(candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Finished))
                        {
                            result.Add(candidateName + " is not onboarded.");
                            iRow++;
                            continue;
                        }
                        string position = ConvertColumnToString(allCells[iRow, positionCell.Start.Column].Value);

                        var startDateString = ConvertColumnToString(allCells[iRow, startDateCell.Start.Column].Value);
                        var startDate = _TryDateTimeParse(startDateString);

                        string endDateString = null;
                        DateTime? endDate = null;
                        if (endDateCell != null)
                        {
                            endDateString = ConvertColumnToString(allCells[iRow, endDateCell.Start.Column].Value);
                            endDate = _TryDateTimeParse(endDateString);
                        }

                        string reasonForLeave = null;
                        if (reasonCell != null)
                            reasonForLeave = ConvertColumnToString(allCells[iRow, reasonCell.Start.Column].Value);

                        string note = null;
                        if (noteCell != null)
                            note = ConvertColumnToString(allCells[iRow, noteCell.Start.Column].Value); ;

                        // no update from import
                        if (_companyCandidateService.GetCompanyCandidatesByCompanyIdAndCandidateId(companyId, candidateId).LastOrDefault() == null)
                        {
                            var companyCandidate = new CompanyCandidate()
                            {
                                CompanyId = companyId,
                                CandidateId = candidateId,
                                Position = position,
                                StartDate = startDate == null ? DateTime.Today.AddDays(-14) : (DateTime)startDate,
                                EndDate = endDate ?? null,
                                ReasonForLeave = reasonForLeave,
                                Note = note,
                                CreatedOnUtc = DateTime.UtcNow,
                                UpdatedOnUtc = DateTime.UtcNow,
                            };

                            _companyCandidateService.InsertCompanyCandidate(companyCandidate);
                        }

                        //next row
                        iRow++;
                    }
                }
            }

            return result;

        }


        private DateTime? _TryDateTimeParse(string s)
        {
            if (s == null)
                return null;

            var dt = new DateTime?();
            try
            {
                dt = DateTime.Parse(s);
            }
            catch (FormatException)
            {
                try
                {
                    dt = DateTime.FromOADate(double.Parse(s));
                }
                catch (FormatException)
                {
                    dt = null;
                }
            }

            return dt;
        }

        #endregion


        #region Import Vendor Candidates

        public ImportResult ImportVendorCandidateFromXlsx(Stream stream, int vendorId)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            IList<string> result = new List<string>();
            var importResult = new ImportResult()
            {
                Attempted = 0,
                Imported = 0,
                NotImported = 0
            };

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Add("No worksheet found");
                    importResult.ErrorMsg = result;
                    return importResult;
                }

                var properties = new string[]
                {
                    "EmployeeId",
                    "Username",
                    "Password",
                    "Salutation",
                    "Gender",
                    "FirstName",
                    "LastName",
                    "MiddleName",
                    "HomePhone",
                    "MobilePhone",
                    "EmergencyPhone",
                    "Email",
                    "BirthDate",
                    "Language",
                    "Education",
                    "SocialInsuranceNumber",
                    "SINExpiryDate",
                    "SINExtensionSubmissionDate",
                    "WorkPermit",
                    "WorkPermitExpiry",
                    "Shift",
                    "Transportation",
                    "LicencePlate",
                    "MajorIntersection1",
                    "MajorIntersection2",
                    "PreferredWorkLocation",
                    "Note",
                };

                result = ValidateImportTemplate(worksheet, properties);

                if (result.Count > 0)
                {
                    importResult.ErrorMsg = result;
                    return importResult;
                }

                importResult = _ImportVendorCandidateAccount(worksheet, properties, vendorId);
            }

            return importResult;
        }


        private string _Column2String(object column)
        {
            var result = ConvertColumnToString(column);

            return String.IsNullOrEmpty(result) || result == "NULL" ? null : result.Trim();
        }


        private DateTime? _Column2DateTime(object column)
        {
            return _TryDateTimeParse(_Column2String(column));
        }


        public IList<string> ValidateImportTemplate(ExcelWorksheet worksheet, string[] properties)
        {
            IList<string> result = new List<string>();

            if (worksheet == null || properties == null || properties.Length <= 0)
                result.Add("Invalid input paratmeters.");

            else
            {
                var errMsg = new List<string>();
                var maxColumn = worksheet.Dimension.End.Column;
                var maxRow = worksheet.Dimension.End.Row;
                var allCells = worksheet.Cells;

                // check columns
                if (maxColumn < properties.Length)
                    errMsg.Add("Some columns are missing");
                else
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var header = _Column2String(allCells[1, i + 1].Value);
                        if (String.IsNullOrEmpty(header) || header != properties[i])
                            errMsg.Add(String.Format("[{0}] name is not correct", _Column2String(allCells[1, i + 1])));
                    }
                }

                if (errMsg.Count > 0)
                    result.Add("Column: " + String.Join(", ", errMsg));
            }

            return result;
        }


        private ImportResult _ImportVendorCandidateAccount(ExcelWorksheet worksheet, string[] properties, int vendorId)
        {
            var importResult = new ImportResult();
            IList<string> result = new List<string>();
            var maxRow = worksheet.Dimension.End.Row;
            var allCells = worksheet.Cells;

            for (int iRow = 2; iRow <= maxRow; iRow++)
            {
                var errMsg = new List<string>();
                importResult.Attempted++;

                var candidate = _candidatesService.GetNewCandidateEntity();

                candidate.EmployeeId = _Column2String(allCells[iRow, GetColumnIndex(properties, "EmployeeId")].Value);
                if (String.IsNullOrWhiteSpace(candidate.EmployeeId))
                    errMsg.Add("EmployeeId is not valid");
                else if (_candidatesService.GetCandidateByVendorIdAndEmployeeId(vendorId, candidate.EmployeeId) != null)
                    errMsg.Add("EmployeeId is duplicated in system");

                candidate.Email = _Column2String(allCells[iRow, GetColumnIndex(properties, "Email")].Value);
                if (!String.IsNullOrEmpty(candidate.Email))
                {
                    candidate.Email = candidate.Email.Trim();
                    if (!CommonHelper.IsValidEmail(candidate.Email))
                        candidate.Email = null;
                    else if (_candidatesService.GetCandidateByEmail(candidate.Email) != null)
                        errMsg.Add("Email is duplicated in system");
                }
                candidate.GenderId = _genderService.GetGenderIdByName(_Column2String(allCells[iRow, GetColumnIndex(properties, "Gender")].Value));
                if (candidate.GenderId == 0)
                    errMsg.Add("Gender is not valid");

                candidate.LastName = _Column2String(allCells[iRow, GetColumnIndex(properties, "LastName")].Value);
                if (String.IsNullOrWhiteSpace(candidate.LastName))
                    errMsg.Add("LastName is empty");

                candidate.SocialInsuranceNumber = CommonHelper.ExtractNumericText(_Column2String(allCells[iRow, GetColumnIndex(properties, "SocialInsuranceNumber")].Value));
                candidate.SINExpiryDate = _Column2DateTime(allCells[iRow, GetColumnIndex(properties, "SINExpiryDate")].Value);
                if (!String.IsNullOrWhiteSpace(candidate.SocialInsuranceNumber))
                {
                    if (!CommonHelper.IsValidCanadianSin(candidate.SocialInsuranceNumber))
                        errMsg.Add("Social Insurance Number is not valid");
                    else if (candidate.SocialInsuranceNumber.StartsWith("9") && !candidate.SINExpiryDate.HasValue)
                        errMsg.Add("SINExpiryDate is not valid");
                }

                if (errMsg.Count > 0)
                {
                    importResult.NotImported++;
                    result.Add(String.Format("Row [{0}] : ", iRow) + String.Join(" | ", errMsg));
                    continue;
                }

                // continue to import
                candidate.Username = _Column2String(allCells[iRow, GetColumnIndex(properties, "UserName")].Value);
                if (string.IsNullOrWhiteSpace(candidate.Username))
                    errMsg.Add("UserName cannot be empty!");
                else if (_candidatesService.GetCandidateByUsername(candidate.Username.Trim()) != null)
                    errMsg.Add("UserName is duplicated in system");

                // Imported password has to be in plain text
                candidate.Password = _Column2String(allCells[iRow, GetColumnIndex(properties, "Password")].Value);
                if (String.IsNullOrWhiteSpace(candidate.Password))
                    candidate.Password = Guid.NewGuid().ToString();

                var salutationId = _salutationService.GetSalutationIdByName(_Column2String(allCells[iRow, GetColumnIndex(properties, "Salutation")].Value));
                if (salutationId > 0)
                    candidate.SalutationId = salutationId;

                // candidateService.RegisterCandidate will cleanup these fields
                candidate.FirstName = _Column2String(allCells[iRow, GetColumnIndex(properties, "FirstName")].Value);
                candidate.MiddleName = _Column2String(allCells[iRow, GetColumnIndex(properties, "MiddleName")].Value);
                candidate.HomePhone = _Column2String(allCells[iRow, GetColumnIndex(properties, "HomePhone")].Value);
                candidate.MobilePhone = _Column2String(allCells[iRow, GetColumnIndex(properties, "MobilePhone")].Value);
                candidate.EmergencyPhone = _Column2String(allCells[iRow, GetColumnIndex(properties, "EmergencyPhone")].Value);

                candidate.BirthDate = _Column2DateTime(allCells[iRow, GetColumnIndex(properties, "BirthDate")].Value);
                candidate.Education = _Column2String(allCells[iRow, GetColumnIndex(properties, "Education")].Value);
                var langId = _languageService.GetLanguageIdByName(_Column2String(allCells[iRow, GetColumnIndex(properties, "Language")].Value));
                if (langId > 0)
                    candidate.LanguageId = langId;

                candidate.SINExtensionSubmissionDate = _Column2DateTime(allCells[iRow, GetColumnIndex(properties, "SINExtensionSubmissionDate")].Value);

                candidate.WorkPermit = _Column2String(allCells[iRow, GetColumnIndex(properties, "WorkPermit")].Value);
                candidate.WorkPermitExpiry = _Column2DateTime(allCells[iRow, GetColumnIndex(properties, "WorkPermitExpiry")].Value);

                var shiftId = _shiftService.GetShiftIdByName(_Column2String(allCells[iRow, GetColumnIndex(properties, "Shift")].Value));
                if (shiftId > 0)
                    candidate.ShiftId = shiftId;
                var xpotationId = _transportationService.GetTransportationIdByName(_Column2String(allCells[iRow, GetColumnIndex(properties, "Transportation")].Value));
                if (xpotationId > 0)
                    candidate.TransportationId = xpotationId;
                candidate.LicencePlate = _Column2String(allCells[iRow, GetColumnIndex(properties, "LicencePlate")].Value);
                var xsection1 = _Column2String(allCells[iRow, GetColumnIndex(properties, "MajorIntersection1")].Value);
                if (_intersectionService.GetIntersectionIdByName(xsection1) > 0)
                    candidate.MajorIntersection1 = xsection1;
                var xsection2 = _Column2String(allCells[iRow, GetColumnIndex(properties, "MajorIntersection2")].Value);
                if (_intersectionService.GetIntersectionIdByName(xsection2) > 0)
                    candidate.MajorIntersection2 = xsection2;
                candidate.PreferredWorkLocation = _Column2String(allCells[iRow, GetColumnIndex(properties, "PreferredWorkLocation")].Value);
                candidate.Note = _Column2String(allCells[iRow, GetColumnIndex(properties, "Note")].Value);


                candidate.FranchiseId = vendorId;
                candidate.SourceId = _sourceService.GetSourceIdByName("Other");
                candidate.OwnerId = _workContext.CurrentAccount.Id;
                candidate.EnteredBy = _workContext.CurrentAccount.Id;


                string registerResult = _candidatesService.RegisterCandidate(candidate, false, false);
                if (!String.IsNullOrWhiteSpace(registerResult))
                {
                    importResult.NotImported++;
                    result.Add(String.Format("Row [{0}] : Candidate registration fails. Error={1}", iRow, registerResult));
                }
                else
                {
                    // onboard needed for MSP candidates
                    var isEmployee = _workContext.CurrentFranchise.IsDefaultManagedServiceProvider ? false : true;
                    _candidatesService.ActivateCandidate(candidate, isEmployee);

                    _activityLogService.InsertActivityLog("AddNewCandidate", _localizationService.GetResource("ActivityLog.AddNewCandidate"), candidate.GetFullName());
                }
            }

            importResult.Imported = importResult.Attempted - importResult.NotImported;
            importResult.ErrorMsg = result;

            return importResult;
        }


        public ImportResult ImportVendorCandidateAddressFromXlsx(Stream stream, int vendorId)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            IList<string> result = new List<string>();
            var importResult = new ImportResult()
            {
                Attempted = 0,
                Imported = 0,
                NotImported = 0
            };

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Add("No worksheet found");
                    importResult.ErrorMsg = result;
                    return importResult;
                }

                var properties = new string[]
                {
                    "EmployeeId",
                    "AddressType",
                    "UnitNumber",
                    "AddressLine1",
                    "AddressLine2",
                    "AddressLine3",
                    "City",
                    "Province",
                    "Country",
                    "PostalCode"
                };

                result = ValidateImportTemplate(worksheet, properties);

                if (result.Count > 0)
                {
                    importResult.ErrorMsg = result;
                    return importResult;
                }

                importResult = _ImportVendorCandidateAddress(worksheet, properties, vendorId);
            }

            return importResult;
        }


        private ImportResult _ImportVendorCandidateAddress(ExcelWorksheet worksheet, string[] properties, int vendorId)
        {
            var importResult = new ImportResult();
            IList<string> result = new List<string>();
            var maxRow = worksheet.Dimension.End.Row;
            var allCells = worksheet.Cells;

            for (int iRow = 2; iRow <= maxRow; iRow++)
            {
                var errMsg = new List<string>();
                importResult.Attempted++;

                var candidate = new Candidate();
                var employeeId = _Column2String(allCells[iRow, GetColumnIndex(properties, "EmployeeId")].Value);
                if (String.IsNullOrEmpty(employeeId))
                    errMsg.Add("EmployeeId is not valid");
                else
                    candidate = _candidatesService.GetCandidateByVendorIdAndEmployeeId(vendorId, employeeId);
                if (candidate == null)
                    errMsg.Add("Account does not exist");

                var addressType = _Column2String(allCells[iRow, GetColumnIndex(properties, "AddressType")].Value);
                var addTypeId = _addressTypeService.GetAddressTypeIdByName(addressType);
                if (addTypeId == 0)
                    errMsg.Add("AddressType is not valid");

                var address1 = _Column2String(allCells[iRow, GetColumnIndex(properties, "AddressLine1")].Value);
                if (String.IsNullOrEmpty(address1))
                    errMsg.Add("AddressLine1 is empty");

                var cityId = _cityService.GetCityIdByName(_Column2String(allCells[iRow, GetColumnIndex(properties, "City")].Value));
                if (cityId == 0)
                    errMsg.Add("City is not valid");

                var stateProvinceId = _provinceService.GetStateProvinceIdByName(_Column2String(allCells[iRow, GetColumnIndex(properties, "Province")].Value));
                if (stateProvinceId == 0)
                    errMsg.Add("Province is not valid");

                var countryId = _countryService.GetCountryIdByName(_Column2String(allCells[iRow, GetColumnIndex(properties, "Country")].Value));
                if (countryId == 0)
                    errMsg.Add("Country is not valid");

                var zipCode = _Column2String(allCells[iRow, GetColumnIndex(properties, "PostalCode")].Value);
                if (!CommonHelper.IsUsOrCanadianZipCode(zipCode))
                    errMsg.Add("PostalCode is not valid");

                if (errMsg.Count > 0)
                {
                    importResult.NotImported++;
                    result.Add(String.Format("Row [{0}] : ", iRow) + String.Join(" | ", errMsg));
                    continue;
                }

                var candidateId = candidate.Id;
                var canAddress = _candidateAddressservice.GetCandidateAddressByCandidateIdAndType(candidateId, addTypeId);
                if (canAddress == null)
                    canAddress = new CandidateAddress();

                canAddress.CandidateId = candidate.Id;
                canAddress.AddressTypeId = addTypeId;
                canAddress.UnitNumber = _Column2String(allCells[iRow, GetColumnIndex(properties, "UnitNumber")].Value);
                canAddress.AddressLine1 = address1;
                canAddress.AddressLine2 = _Column2String(allCells[iRow, GetColumnIndex(properties, "AddressLine2")].Value);
                canAddress.AddressLine3 = _Column2String(allCells[iRow, GetColumnIndex(properties, "AddressLine3")].Value);
                canAddress.CityId = cityId;
                canAddress.StateProvinceId = stateProvinceId;
                canAddress.CountryId = countryId;
                canAddress.PostalCode = CommonHelper.ExtractAlphanumericText(zipCode);
                canAddress.IsActive = true;
                canAddress.IsDeleted = false;
                canAddress.EnteredBy = _workContext.CurrentAccount.Id;
                canAddress.DisplayOrder = 0;
                canAddress.CreatedOnUtc = DateTime.UtcNow;
                canAddress.UpdatedOnUtc = canAddress.CreatedOnUtc;

                if (canAddress.Id == 0)
                {
                    _candidateAddressservice.InsertCandidateAddress(canAddress);
                    _activityLogService.InsertActivityLog("AddNewCandidateAddress", _localizationService.GetResource("ActivityLog.AddNewCandidateAddress"), addTypeId);
                }
                else
                {
                    _candidateAddressservice.UpdateCandidateAddress(canAddress);
                    _activityLogService.InsertActivityLog("UpdateCandidateAddress", _localizationService.GetResource("ActivityLog.UpdateCandidateAddress"), addTypeId);
                }

                // update search keys
                _candidatesService.UpdateCandidateSearchKeys(candidate);
            }

            importResult.Imported = importResult.Attempted - importResult.NotImported;
            importResult.ErrorMsg = result;

            return importResult;
        }


        public ImportResult ImportVendorCandidateSkillFromXlsx(Stream stream, int vendorId)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            IList<string> result = new List<string>();
            var importResult = new ImportResult()
            {
                Attempted = 0,
                Imported = 0,
                NotImported = 0
            };

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Add("No worksheet found");
                    importResult.ErrorMsg = result;
                    return importResult;
                }

                var properties = new string[]
                {
                    "EmployeeId",
                    "KeySkill",
                    "YearsOfExperience",
                    "LastUsedDate",
                    "Note"
                };

                result = ValidateImportTemplate(worksheet, properties);

                if (result.Count > 0)
                {
                    importResult.ErrorMsg = result;
                    return importResult;
                }

                importResult = _ImportVendorCandidateSkill(worksheet, properties, vendorId);
            }

            return importResult;
        }


        private ImportResult _ImportVendorCandidateSkill(ExcelWorksheet worksheet, string[] properties, int vendorId)
        {
            var importResult = new ImportResult();
            IList<string> result = new List<string>();
            var maxRow = worksheet.Dimension.End.Row;
            var allCells = worksheet.Cells;

            for (int iRow = 2; iRow <= maxRow; iRow++)
            {
                var errMsg = new List<string>();
                importResult.Attempted++;

                var candidate = new Candidate();
                var employeeId = _Column2String(allCells[iRow, GetColumnIndex(properties, "EmployeeId")].Value);
                if (String.IsNullOrEmpty(employeeId))
                    errMsg.Add("EmployeeId is not valid");
                else
                    candidate = _candidatesService.GetCandidateByVendorIdAndEmployeeId(vendorId, employeeId);
                if (candidate == null)
                    errMsg.Add("Account does not exist");

                var keySkill = _Column2String(allCells[iRow, GetColumnIndex(properties, "KeySkill")].Value);
                if (_skillService.GetSkillIdByName(keySkill) == 0)
                    errMsg.Add("KeySkill is not valid");

                var years = Convert.ToDecimal(allCells[iRow, GetColumnIndex(properties, "YearsOfExperience")].Value);
                if (years == 0)
                    errMsg.Add("YearsOfExperience is not valid");

                var lastDate = _Column2DateTime(allCells[iRow, GetColumnIndex(properties, "LastUsedDate")].Value);
                if (lastDate == null || lastDate > DateTime.Today)
                    errMsg.Add("LastUsedDate is not valid");

                if (errMsg.Count > 0)
                {
                    importResult.NotImported++;
                    result.Add(String.Format("Row [{0}] : ", iRow) + String.Join(" | ", errMsg));
                    continue;
                }

                var candidateId = candidate.Id;
                var canSkill = _canidateSkillService.GetCandidateKeySkillsByCandidateId(candidateId)
                               .Where(x => x.KeySkill == keySkill).OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefault();
                if (canSkill == null)
                    canSkill = new CandidateKeySkill();

                canSkill.CandidateId = candidateId;
                canSkill.KeySkill = keySkill;
                canSkill.YearsOfExperience = years;
                canSkill.LastUsedDate = lastDate;
                canSkill.Note = _Column2String(allCells[iRow, GetColumnIndex(properties, "Note")].Value);
                canSkill.IsDeleted = false;
                canSkill.CreatedOnUtc = DateTime.UtcNow;
                canSkill.UpdatedOnUtc = canSkill.CreatedOnUtc;

                if (canSkill.Id == 0)
                {
                    _canidateSkillService.InsertCandidateKeySkill(canSkill);
                    _activityLogService.InsertActivityLog("AddNewCandidateKeySkill", _localizationService.GetResource("ActivityLog.AddNewCandidateKeySkill"), canSkill.KeySkill);
                }
                else
                {
                    _canidateSkillService.UpdateCandidateKeySkill(canSkill);
                    _activityLogService.InsertActivityLog("UpdateCandidateKeySkill", _localizationService.GetResource("ActivityLog.UpdateCandidateKeySkill"), canSkill.KeySkill);
                }

                // update search keys
                _candidatesService.UpdateCandidateSearchKeys(candidate);
            }

            importResult.Imported = importResult.Attempted - importResult.NotImported;
            importResult.ErrorMsg = result;

            return importResult;
        }

        public ImportResult ImportVendorCandidateBankAccountFromXlsx(Stream stream, int vendorId)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            IList<string> result = new List<string>();
            var importResult = new ImportResult()
            {
                Attempted = 0,
                Imported = 0,
                NotImported = 0
            };

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Add("No worksheet found");
                    importResult.ErrorMsg = result;
                    return importResult;
                }

                var properties = new string[]
                {
                    "EmployeeId",
                    "Institution Number",
                    "Transit Number",
                    "Account Number",
                    "Note"
                };

                result = ValidateImportTemplate(worksheet, properties);

                if (result.Count > 0)
                {
                    importResult.ErrorMsg = result;
                    return importResult;
                }

                importResult = _ImportVendorCandidateBankAccount(worksheet, properties, vendorId);
            }

            return importResult;
        }

        private ImportResult _ImportVendorCandidateBankAccount(ExcelWorksheet worksheet, string[] properties, int vendorId)
        {
            var importResult = new ImportResult();
            IList<string> result = new List<string>();
            var maxRow = worksheet.Dimension.End.Row;
            var allCells = worksheet.Cells;

            for (int iRow = 2; iRow <= maxRow; iRow++)
            {
                var errMsg = new List<string>();
                importResult.Attempted++;

                var candidate = new Candidate();
                var employeeId = _Column2String(allCells[iRow, GetColumnIndex(properties, "EmployeeId")].Value);
                if (String.IsNullOrEmpty(employeeId))
                    errMsg.Add("EmployeeId is not valid");
                else
                    candidate = _candidatesService.GetCandidateByVendorIdAndEmployeeId(vendorId, employeeId);
                if (candidate == null)
                    errMsg.Add("Account does not exist");

                var institutionNumber = _Column2String(allCells[iRow, GetColumnIndex(properties, "Institution Number")].Value);
                if (String.IsNullOrWhiteSpace(institutionNumber)||institutionNumber.Length>4||institutionNumber.Any(x=>!Char.IsNumber(x)))
                    errMsg.Add("Institution Number is not valid");

                var transitNumber = _Column2String(allCells[iRow, GetColumnIndex(properties, "Transit Number")].Value);
                if (String.IsNullOrWhiteSpace(transitNumber) || transitNumber.Length > 5 || transitNumber.Any(x => !Char.IsNumber(x)))
                    errMsg.Add("Transit Number is not valid");

                var accountNumber = _Column2String(allCells[iRow, GetColumnIndex(properties, "Account Number")].Value);
                if (String.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length > 17 || accountNumber.Any(x => !Char.IsNumber(x)))
                    errMsg.Add("Account Number is not valid");

                if (errMsg.Count > 0)
                {
                    importResult.NotImported++;
                    result.Add(String.Format("Row [{0}] : ", iRow) + String.Join(" | ", errMsg));
                    continue;
                }

                var candidateId = candidate.Id;
                var canBankAccount = _candidateBankAccountService.GetAllCandidateBankAccountsByCandidateId(candidateId)
                                        .Where(x=>x.IsActive&&x.InstitutionNumber==institutionNumber
                                                &&x.TransitNumber==transitNumber&&x.AccountNumber==accountNumber).FirstOrDefault();

                if (canBankAccount == null)
                    canBankAccount = new CandidateBankAccount();

                canBankAccount.CandidateId = candidateId;
                canBankAccount.InstitutionNumber = institutionNumber;
                canBankAccount.TransitNumber = transitNumber;
                canBankAccount.AccountNumber = accountNumber;
                canBankAccount.Note = _Column2String(allCells[iRow, GetColumnIndex(properties, "Note")].Value);
                canBankAccount.IsDeleted = false;
                canBankAccount.IsActive = true;
                canBankAccount.EnteredBy = _workContext.CurrentAccount.Id;
                canBankAccount.DisplayOrder = 0;
                canBankAccount.CreatedOnUtc = DateTime.UtcNow;
                canBankAccount.UpdatedOnUtc = canBankAccount.CreatedOnUtc;

                if (canBankAccount.Id == 0)
                {
                    _candidateBankAccountService.Insert(canBankAccount);
                    _activityLogService.InsertActivityLog("AddNewCandidateBankAccount", _localizationService.GetResource("ActivityLog.AddNewCandidateBankAccount"), accountNumber);
                }
                else
                {
                    _candidateBankAccountService.Update(canBankAccount);
                    _activityLogService.InsertActivityLog("UpdateCandidateBankAccount", _localizationService.GetResource("ActivityLog.UpdateCandidateBankAccount"), accountNumber);
                }
            }

            importResult.Imported = importResult.Attempted - importResult.NotImported;
            importResult.ErrorMsg = result;

            return importResult;
        }
        #endregion

    }
}
