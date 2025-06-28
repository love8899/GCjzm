using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Companies;
using Wfm.Services.Candidates;
using Wfm.Services.Features;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.Tasks;
using Wfm.Services.TimeSheet;
using Wfm.Services.Logging;
using Wfm.Services.Messages;


namespace Wfm.Services.ExportImport
{
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IFeatureService _featureService;
        private readonly IAccountService _accountService;
        private readonly ICompanyService _companyService;
        private readonly IFranchiseService _franchiseService;
        private readonly ICandidateService _candidatesService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyContactService _companyContactService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ICandidateDirectHireStatusHistoryService _directHireStatusHistoryService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidateAvailabilityService _availabilityService;
        private readonly IOTRulesForJobOrderService _otRulesForJobOrderService;
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly CommonSettings _commonSettings;
        private readonly IWorkContext _workContext;
        private readonly IWorkTimeService _workTimeService;
        private readonly IFranciseAddressService _franchiseAddressService;
        private readonly ILogger _logger;
        private readonly IEmailAccountService _emailAccountService;
        #endregion

        #region Ctor

        public ExportManager(
            IWebHelper webHelper,
            IFeatureService featureService,
            IAccountService accountService,
            ICompanyService companyService,
            IFranchiseService franchiseService,
            ICandidateService candidatesService,
            ICompanyDivisionService companyDivisionService,
            ICompanyContactService companyContactService,
            ICompanyCandidateService companyCandidateService,
            IJobOrderService jobOrderService,
            ICandidateDirectHireStatusHistoryService directHireStatusHistoryService,
            ICandidateJobOrderService candidateJobOrderService,
            ICandidateAvailabilityService availabilityService,
            IOTRulesForJobOrderService otRulesForJobOrderService,
            ICandidatePictureService candidatePictureService,
            CommonSettings commonSettings,
            IWorkContext workContext,
            IWorkTimeService workTimeService,
            IFranciseAddressService franchiseAddressService,
            ILogger logger, IEmailAccountService emailAccountService
            )
        {
            _webHelper = webHelper;
            _featureService = featureService;
            _accountService = accountService;
            _companyService = companyService;
            _franchiseService = franchiseService;
            _candidatesService = candidatesService;
            _companyDivisionService = companyDivisionService;
            _companyContactService = companyContactService;
            _companyCandidateService = companyCandidateService;
            _availabilityService = availabilityService;
            _jobOrderService = jobOrderService;
            _directHireStatusHistoryService = directHireStatusHistoryService;
            _candidateJobOrderService = candidateJobOrderService;
            _otRulesForJobOrderService = otRulesForJobOrderService;
            _candidatePictureService = candidatePictureService;
            _commonSettings = commonSettings;
            _workContext = workContext;
            _workTimeService = workTimeService;
            _franchiseAddressService = franchiseAddressService;
            _logger = logger;
            _emailAccountService = emailAccountService;
        }

        #endregion


        #region Utilities
        private class Column
        {
            public string Name { get; set; }
            public int Width { get; set; }
            public ExcelHorizontalAlignment Alignment { get; set; }
        }

        protected virtual Image GetLogo(int? franchiseId = null)
        {
            return _franchiseService.GetFranchiseLogo(franchiseId ?? _workContext.CurrentFranchise.Id);
            //string logoPath = Path.Combine(_webHelper.MapPath("~/Content/Images/"), "hd_logo.png");
            //Image logo = Image.FromFile(logoPath);
            
        }


        private void AssignCellValueAndAlignment(ExcelRange cell, string value, ExcelHorizontalAlignment H_Alignement, ref int col, bool? highlight = false)
        {
            cell.Value = value;
            cell.Style.HorizontalAlignment = H_Alignement;
            if (highlight.HasValue && highlight.Value)
            {
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightCyan);
            }
            col++;
        }


        private void SetPrinterToLegalSize(ExcelWorksheet  worksheet,
                                            decimal TopMargin = 2.032M / 2.54M, 
                                            decimal RightMargin = 2.032M / 2.54M,
                                            decimal BottomMargin = 2.032M / 2.54M,
                                            decimal LeftMargin = 2.032M / 2.54M
                                           )
        {
            // Printer settings
            worksheet.PrinterSettings.PaperSize = ePaperSize.Legal;
            worksheet.PrinterSettings.TopMargin = TopMargin;
            worksheet.PrinterSettings.RightMargin = RightMargin;
            worksheet.PrinterSettings.BottomMargin = BottomMargin;
            worksheet.PrinterSettings.LeftMargin = LeftMargin;
            worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
        }

        private void SetPrinterToLetterSize(ExcelWorksheet worksheet,
                                            decimal TopMargin = 2.032M / 2.54M,
                                            decimal RightMargin = 2.032M / 2.54M,
                                            decimal BottomMargin = 2.032M / 2.54M,
                                            decimal LeftMargin = 2.032M / 2.54M
                                           )
        {
            // Printer settings
            worksheet.PrinterSettings.PaperSize = ePaperSize.Letter;
            worksheet.PrinterSettings.TopMargin = TopMargin;
            worksheet.PrinterSettings.RightMargin = RightMargin;
            worksheet.PrinterSettings.BottomMargin = BottomMargin;
            worksheet.PrinterSettings.LeftMargin = LeftMargin;
            worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
            worksheet.PrinterSettings.FitToPage = true;
        }

        private void SetWorkSheetTitle(ExcelWorksheet  worksheet, string title, string lastCellName, int row)
        {
            worksheet.Row(row).Height = 55;

            var titleCells = String.Concat("A1:", lastCellName);
            worksheet.Cells[titleCells].Merge = true;
            worksheet.Cells[titleCells].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[titleCells].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //worksheet.Cells["A1:N1"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
            worksheet.Cells[row, 1].Value = title;
            worksheet.Cells[row, 1].Style.Font.Size = 18;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DarkGreen);
            worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));
        }


        private void SetWorkSheetColumns(ExcelWorksheet  worksheet, Column[] columns, int firstCol, int row)
        {
            for (int i = firstCol; i < columns.Length; i++)
            {
                worksheet.Cells[row, i + 1 - firstCol].Value = columns[i].Name;
                worksheet.Cells[row, i + 1 - firstCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, i + 1 - firstCol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                worksheet.Cells[row, i + 1 - firstCol].Style.Font.Bold = true;
                worksheet.Cells[row, i + 1 - firstCol].Style.WrapText = true;

                worksheet.Column(i + 1 - firstCol).Width = columns[i].Width;
                worksheet.Cells[row, i + 1].Style.HorizontalAlignment = columns[i].Alignment;
            }

            worksheet.Row(row).Height = 22;
            worksheet.Row(row).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        #endregion



        #region Export AttandentList


        public string ExportAttandentListToXlsxByDateRange(Stream stream, JobOrder jobOrder, DateTime startDate, DateTime endDate)
        {
            if (jobOrder == null)
                throw new ArgumentNullException("jobOrder");

            var company = _companyService.GetCompanyById(jobOrder.CompanyId);
            if (company == null)
                throw new ArgumentNullException("jobOrder.CompanyId");

            // export
            return ExportAtetanceListForJobOrders(stream, jobOrder.Id.ToString(), startDate, endDate, company.Id);
        }


        public string ExportAtetanceListForJobOrders(Stream stream, string ids, DateTime startDate, DateTime endDate, int companyId)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Company company = _companyService.GetCompanyById(companyId);

            string fileName = string.Empty;
            string franchiseName = _workContext.CurrentFranchise.FranchiseName;
            // Let's begin
            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Employee Time Sheets");

                // Printer settings
                this.SetPrinterToLetterSize(worksheet);
                worksheet.PrinterSettings.FitToHeight = 10;
                worksheet.PrinterSettings.RepeatRows = new ExcelAddress("1:5");
                // initial row
                int row = 1;

                // Logo and title
                // *****************************
                this.SetWorkSheetTitle(worksheet, franchiseName + " Employee Time Sheets", "Q1", row);

                // next row
                row++;

                // Client information
                // *****************************
                worksheet.Row(row).Height = 25;
                worksheet.Cells["A2:Q2"].Merge = true;
                worksheet.Cells["A2:Q2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:Q2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //worksheet.Cells["A2:N2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                string clientInfo = "Client Name : " + company.CompanyName;
                worksheet.Cells[row, 1].Value = clientInfo;
                worksheet.Cells[row, 1].Style.Font.Size = 12;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DimGray);
                worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                // next row
                row++;

                // Week #
                // *****************************
                worksheet.Row(row).Height = 30;
                worksheet.Cells["A3:Q3"].Merge = true;
                worksheet.Cells["A3:Q3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:Q3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:Q3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);

                // get week start date and end date - Sunday to Saturday
                //DateTime fromDate = startDate.HasValue ? (DateTime)startDate : jobOrder.StartDate;
                //int delta = DayOfWeek.Sunday - fromDate.DayOfWeek;
                DateTime sunday = startDate;
                DateTime saturday = endDate;

                string weekHeader = "Week of " + sunday.ToString("yyyy-MMM-dd") + " to " + saturday.ToString("yyyy-MMM-dd");
                worksheet.Cells[row, 1].Value = weekHeader;
                worksheet.Cells[row, 1].Style.Font.Size = 13;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DarkOrange);
                worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                // file name
                string companyName = Regex.Replace(company.CompanyName.Trim(), @"[^a-zA-Z0-9]", "_").Trim('_');
                string fileNameString = Regex.Replace(companyName, @"[_]{2,}", @"_");
                fileName = string.Format("{0}_{1}_{2}_{3}{4}", "Employee_Timesheets", sunday.ToString("yyyy-MMM-dd"), saturday.ToString("yyyy-MMM-dd"), fileNameString, ".xlsx");

                // next row
                row++;

                // Headers
                // *****************************
                var properties = new Column[]
                    {
                        new Column(){ Name="Employee Name",//A
                                      Width= 25,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Badge Id", // B
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee #",//C
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Position",//D
                                      Width= 30,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Job Order",//E
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Shift",//F
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name= "Type",//G
                                      Width= 8,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Sun",//H
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Mon",//I
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Tue",//J
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Wed",//K
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Thu",//L
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Fri",//M
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Sat",//N
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Total Hrs",//O
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Reg. Hrs",//P
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="OT",//Q
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right}
                    };

                // fromat column header
                this.SetWorkSheetColumns(worksheet, properties, 0, row);

                // next row
                row++;

                // subProperties
                var subProperties = new string[]
                {
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    sunday.AddDays(0).ToString("dd-MMM"),
                    sunday.AddDays(1).ToString("dd-MMM"),
                    sunday.AddDays(2).ToString("dd-MMM"),
                    sunday.AddDays(3).ToString("dd-MMM"),
                    sunday.AddDays(4).ToString("dd-MMM"),
                    sunday.AddDays(5).ToString("dd-MMM"),
                    sunday.AddDays(6).ToString("dd-MMM"),
                    "",
                    "",
                    ""
                };
                for (int i = 0; i < subProperties.Length; i++)
                {
                    worksheet.Cells[row, i + 1].Value = subProperties[i];
                    worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[row, i + 1].Style.Font.Color.SetColor(Color.DarkBlue);
                    worksheet.Cells[row, i + 1].Style.Font.Bold = false;

                    if (i >= 3) worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                // next row
                row++;

                // Content
                // *****************************
                var jobOrderIds = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x));
                foreach (var jobOrderId in jobOrderIds)
                {
                    var OTRule = _otRulesForJobOrderService.GetOTRulesForJobOrder(jobOrderId).FirstOrDefault();
                    decimal OTThreshold;
                    if (OTRule == null || OTRule.OvertimeCode != "WEEKLY")
                        OTThreshold = 0;
                    else
                        OTThreshold = OTRule.Threshold;

                    var candidates = _candidateJobOrderService.GetActualDailyPlacement(jobOrderId, startDate, endDate)
                        .GroupBy(x => new { Candidate = x.Candidate, JobOrder = x.JobOrder }).Select(g => new
                        {
                            CandidateId = g.Key.Candidate.Id,
                            CandidateName = String.Concat(g.Key.Candidate.LastName, ", ", g.Key.Candidate.FirstName),
                            EmployeeId = g.Key.Candidate.EmployeeId,
                            JobOrderId = g.Key.JobOrder.Id,
                            JobTitle = g.Key.JobOrder.JobTitle,
                            StartTime = g.Key.JobOrder.StartTime,
                            Sun = g.Any(x => x.StartDate.DayOfWeek == DayOfWeek.Sunday),
                            Mon = g.Any(x => x.StartDate.DayOfWeek == DayOfWeek.Monday),
                            Tue = g.Any(x => x.StartDate.DayOfWeek == DayOfWeek.Tuesday),
                            Wed = g.Any(x => x.StartDate.DayOfWeek == DayOfWeek.Wednesday),
                            Thu = g.Any(x => x.StartDate.DayOfWeek == DayOfWeek.Thursday),
                            Fri = g.Any(x => x.StartDate.DayOfWeek == DayOfWeek.Friday),
                            Sat = g.Any(x => x.StartDate.DayOfWeek == DayOfWeek.Saturday),
                        });

                    foreach (var c in candidates.OrderBy(x => x.CandidateId))
                    {
                        int col = 1;

                        worksheet.Cells[row, col].Value = c.CandidateName;
                        col++;

                        worksheet.Cells[row, col].Value = c.CandidateId.ToString("00000000");
                        col++;

                        worksheet.Cells[row, col].Value = c.EmployeeId;
                        col++;

                        worksheet.Cells[row, col].Value = c.JobTitle;
                        col++;

                        worksheet.Cells[row, col].Value = c.JobOrderId;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        col++;

                        worksheet.Cells[row, col].Value = c.StartTime.ToString("hh:mm tt");
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        col++;

                        // reserve for type
                        AssignCellValueAndAlignment(worksheet.Cells[row, col], "", ExcelHorizontalAlignment.Right, ref col);

                        AssignCellValueAndAlignment(worksheet.Cells[row, col], "", ExcelHorizontalAlignment.Right, ref col, c.Sun);

                        AssignCellValueAndAlignment(worksheet.Cells[row, col], "", ExcelHorizontalAlignment.Right, ref col, c.Mon);

                        AssignCellValueAndAlignment(worksheet.Cells[row, col], "", ExcelHorizontalAlignment.Right, ref col, c.Tue);

                        AssignCellValueAndAlignment(worksheet.Cells[row, col], "", ExcelHorizontalAlignment.Right, ref col, c.Wed);

                        AssignCellValueAndAlignment(worksheet.Cells[row, col], "", ExcelHorizontalAlignment.Right, ref col, c.Thu);

                        AssignCellValueAndAlignment(worksheet.Cells[row, col], "", ExcelHorizontalAlignment.Right, ref col, c.Fri);

                        AssignCellValueAndAlignment(worksheet.Cells[row, col], "", ExcelHorizontalAlignment.Right, ref col, c.Sat);

                        // function
                        worksheet.Cells[row, col].Formula = "SUM(H" + row.ToString() + ":N" + row.ToString() + ")";
                        col++;

                        worksheet.Cells[row, col].Formula = "IF(O" + row.ToString() + "<=" + OTThreshold.ToString() + ",O" + row.ToString() + "," + OTThreshold.ToString() + ")";
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        col++;

                        worksheet.Cells[row, col].Formula = "O" + row.ToString() + "-P" + row.ToString();
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        col++;

                        // row height
                        worksheet.Row(row).Height = 18;

                        // Next row
                        row++;
                    }
                }

                // border
                // *****************************
                worksheet.Cells["A1:Q" + (row - 1).ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);

                // Total row
                // *****************************
                worksheet.Cells[row, 1].Value = "Total";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                if (row > 6) //check added to avoid circular reference
                {
                    // function
                    worksheet.Cells[row, properties.Count() - 1].Formula = "SUM(P6:P" + (row - 1).ToString() + ")";
                    worksheet.Cells[row, properties.Count() - 2].Formula = "SUM(O6:O" + (row - 1).ToString() + ")";
                    worksheet.Cells[row, properties.Count()].Formula = "SUM(Q6:Q" + (row - 1).ToString() + ")";
                }
                // Next row
                row++;

                // add logo to row 1
                // *****************************
                // *** Put it at the end to avoid resizing ***
                //Image logo = GetLogo();
                //var picture = worksheet.Drawings.AddPicture("Logo", logo);
                //picture.SetSize(188, 68);
                // *** end of logo *** 

                xlPackage.Save();

            }

            return fileName;
        }


        public string ExportDailyAttendanceConfirmationListToXlsx(Stream stream, int companyId, DateTime refDate, int[] candidateJobOrderIds)
        {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                if (companyId == 0)
                    throw new ArgumentNullException("jobOrder");

                if (candidateJobOrderIds == null)
                    throw new ArgumentNullException("candidates");

                Company company = _companyService.GetCompanyById(companyId);
                if (company == null)
                    return null;
                string franchiseName = _workContext.CurrentFranchise.FranchiseName;
                string fileName = string.Empty;

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var candidateJobOrders = _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable().Where(x => candidateJobOrderIds.Contains(x.Id));
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Daily Attendance");

                    // Printer settings
                    this.SetPrinterToLetterSize(worksheet, TopMargin: 1.032M / 2.54M, RightMargin: 1.032M / 2.54M, BottomMargin: 1.032M / 2.54M, LeftMargin: 1.032M / 2.54M);
                    worksheet.PrinterSettings.VerticalCentered = true;
                    worksheet.PrinterSettings.HorizontalCentered = true;

                    // initial row
                    int row = 1;

                    // Logo and title
                    // *****************************
                    this.SetWorkSheetTitle(worksheet, franchiseName + " Employee Daily Attendance", "L1", row);

                    // next row
                    row++;

                    // Client information
                    // *****************************
                    worksheet.Row(row).Height = 25;
                    worksheet.Cells["A2:L2"].Merge = true;
                    worksheet.Cells["A2:L2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A2:L2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //worksheet.Cells["A2:H2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                    
                    // file name
                    string companyName = Regex.Replace(company.CompanyName.Trim(), @"[^a-zA-Z0-9]", "_").Trim('_');
                    string fileNameString = Regex.Replace(companyName, @"[_]{2,}", @"_");

                    if (candidateJobOrders.Select(x => x.JobOrderId).Distinct().Count() > 1)
                    {
                        worksheet.Cells[row, 1].Value = "Client Name : " + company.CompanyName;
                        fileName = string.Format("{0}_{1}_{2}{3}", "Daily_Attendance", refDate.ToString("yyyy-MMM-dd"), fileNameString, ".xlsx");
                    }
                    else
                    {
                        var jobOrder = candidateJobOrders.FirstOrDefault().JobOrder;
                        if (jobOrder.CompanyContactId > 0)
                            worksheet.Cells[row, 1].Value = "Client Name : " + company.CompanyName + "  Supervisor Name : " + jobOrder.CompanyContact.FullName;
                        else
                        {
                            var HR = _accountService.GetClientCompanyHRAccount(company.Id);
                            if (HR == null)
                                worksheet.Cells[row, 1].Value = "Client Name : " + company.CompanyName;
                            else
                                worksheet.Cells[row, 1].Value = "Client Name : " + company.CompanyName + "  Supervisor Name : " + HR.FullName;
                        }
                        fileName = string.Format("{0}_{1}_{2}_{3}{4}", "Daily_Attendance", refDate.ToString("yyyy-MMM-dd"), jobOrder.Id,fileNameString, ".xlsx");
                    }
                    worksheet.Cells[row, 1].Style.Font.Size = 12;
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DimGray);
                    worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                    // next row
                    row++;

                    // Date
                    // *****************************
                    worksheet.Row(row).Height = 30;
                    worksheet.Cells["A3:L3"].Merge = true;
                    worksheet.Cells["A3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells["A3:L3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                    worksheet.Cells[row, 1].Value = refDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 1].Style.Font.Size = 13;
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DarkOrange);
                    worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));



                    // next row
                    row++;

                    // Headers
                    // *****************************
                    var properties = new Column[]
                    {
                        new Column(){ Name="Number",//A
                                      Width= 8,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Photo",
                                      Width= 8,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee Name",//B  
                                      Width= 25,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee #",//C
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Position",//D
                                      Width= 40,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Shift",//E
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name= "Job Order",//F
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Start",//G
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="End",//H
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Shift Duration",//I
                                      Width= 13,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee's Signature",//J
                                      Width= 28,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Comments",//K
                                      Width= 18,  Alignment= ExcelHorizontalAlignment.Left}
                    };

                    // fromat column header
                    this.SetWorkSheetColumns(worksheet, properties, 0, row);


                    // next row
                    row++;

                    for (int i = 0; i < candidateJobOrderIds.Length; i++)
                    {

                        int col = 1;
                        int candidateJobOrderId = candidateJobOrderIds[i];
                        Candidate candidate = _candidatesService.GetCandidateById(_candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable().Where(c => c.Id == candidateJobOrderId).Select(a => a.CandidateId).FirstOrDefault());
                        JobOrder jo = _jobOrderService.GetJobOrderById(_candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable().Where(c => c.Id == candidateJobOrderId).Select(a => a.JobOrderId).FirstOrDefault());
                        var defaultCandidatePicture = _candidatePictureService.GetCandidatePicturesByCandidateId(candidate.Id, 1).FirstOrDefault();
                        string path = _candidatePictureService.GetThumbLocalPath(defaultCandidatePicture);

                        worksheet.Cells[row, col].Value = row - 4;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        col++;

                        Image file = Image.FromFile(path);
                        var picture = worksheet.Drawings.AddPicture("Image"+candidate.Id,file);
                        picture.SetSize(55, 68);  
                        picture.SetPosition(row-1,0,col-1,0);//position for the image
                        col++;

                        worksheet.Cells[row, col].Value = String.Concat(candidate.LastName , ", " , candidate.FirstName);
                        col++;

                        worksheet.Cells[row, col].Value = candidate.Id.ToString("00000000");
                        col++;

                        worksheet.Cells[row, col].Value = jo.JobTitle;
                        worksheet.Cells[row, col].Style.WrapText = true;
                        col++;

                        worksheet.Cells[row, col].Value = jo.Shift.ShiftName;
                        col++;

                        worksheet.Cells[row, col].Value = jo.Id;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        col++;

                        worksheet.Cells[row, col].Value = jo.StartTime.ToString("hh:mm tt");
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        col++;

                        worksheet.Cells[row, col].Value = jo.EndTime.ToString("hh:mm tt");
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        col++;

                        // row height
                        worksheet.Row(row).Height = 44;

                        // Next row
                        row++;

                    }

                    //Total row
                    worksheet.Cells[row, 1].Value = "Total";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    // function
                    worksheet.Cells[row, 9].Formula = "SUM(J5:J" + (row - 1).ToString() + ")";
                    row++;

                    // border
                    // *****************************
                    worksheet.Cells["A1:L" + (row - 1).ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);
                    worksheet.Cells["A5:L" + (row - 1).ToString()].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells["A5:L" + (row - 1).ToString()].Style.Border.Bottom.Color.SetColor(Color.DarkGray);
                    worksheet.Cells["A5:L" + (row - 1).ToString()].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells["A5:L" + (row - 1).ToString()].Style.Border.Left.Color.SetColor(Color.DarkGray);
                    worksheet.Cells["A5:L" + (row - 1).ToString()].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells["A5:L" + (row - 1).ToString()].Style.Border.Right.Color.SetColor(Color.DarkGray);
                    worksheet.Cells["A5:L" + (row - 1).ToString()].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells["A5:L" + (row - 1).ToString()].Style.Border.Top.Color.SetColor(Color.DarkGray);

                    
                    row++;
                    worksheet.Cells[row, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[row, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[row, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[row, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    worksheet.Cells[row, 2].Style.Border.Bottom.Color.SetColor(Color.Black);
                    worksheet.Cells[row, 6].Style.Border.Bottom.Color.SetColor(Color.Black);
                    worksheet.Cells[row, 7].Style.Border.Bottom.Color.SetColor(Color.Black);
                    worksheet.Cells[row, 8].Style.Border.Bottom.Color.SetColor(Color.Black);
                    worksheet.Cells[row, 10].Style.Border.Bottom.Color.SetColor(Color.Black);
                    row++;

                    worksheet.Cells[row, 2].Value = "Print Supervisor Name";
                    worksheet.Cells[row, 6].Value = "Signature - Supervisor";
                    worksheet.Cells[row, 10].Value = "Date";
                    row++;


                    // add logo to row 1
                    // *****************************
                    // *** Put it at the end to avoid resizing ***
                    //Image logo = GetLogo();
                    //var logopicture = worksheet.Drawings.AddPicture("Logo", logo);
                    //logopicture.SetSize(188, 68);
                    // *** end of logo *** 

                    xlPackage.Save();
                }

                return fileName;
            
        }
        public string ExportDialyAttendantListToXlsx(Stream stream, int companyId, DateTime refDate, int[] candidateJobOrderIds) 
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (companyId == 0)
                throw new ArgumentNullException("jobOrder");

            if (candidateJobOrderIds == null)
                throw new ArgumentNullException("candidates");

            var address = _franchiseAddressService.GetAllFranchiseAddressForPublicSite().Where(x => x.IsHeadOffice).FirstOrDefault();
            string primaryPhone = string.Empty;
            if (address==null)
                _logger.Error("Current Franchise head office address has not been set up yet!");
            else
                primaryPhone = address.PrimaryPhone;

            var accountingEmail = _emailAccountService.GetEmailAccountByFranchiseIdAndCode(_workContext.CurrentFranchise.Id, "Accounting");
            string email = string.Empty;
            if (accountingEmail == null)
                _logger.Error("There is no Accounting email!");
            else
                email = accountingEmail.Email;

            Company company = _companyService.GetCompanyById(companyId);
            if (company == null)
                return null;
            string franchiseName = _workContext.CurrentFranchise.FranchiseName;
            string fileName = string.Empty;

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Daily Attendance");

                // Printer settings
                this.SetPrinterToLetterSize(worksheet, TopMargin: 1.032M / 2.54M, RightMargin: 1.032M / 2.54M, BottomMargin: 1.032M / 2.54M, LeftMargin: 1.032M / 2.54M);
                worksheet.PrinterSettings.VerticalCentered = true;
                worksheet.PrinterSettings.HorizontalCentered = true;

                // initial row
                int row = 1;

                // Logo and title
                // *****************************
                this.SetWorkSheetTitle(worksheet, franchiseName + " Employee Daily Attendance", "M1", row);

                // next row
                row++;

                // Client information
                // *****************************
                worksheet.Row(row).Height = 25;
                worksheet.Cells["A2:M2"].Merge = true;
                worksheet.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:M2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //worksheet.Cells["A2:H2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);

                worksheet.Cells[row, 1].Value = "Client Name : " + company.CompanyName;
                worksheet.Cells[row, 1].Style.Font.Size = 12;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DimGray);
                worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                // next row
                row++;

                // Date
                // *****************************
                worksheet.Row(row).Height = 30;
                worksheet.Cells["A3:M3"].Merge = true;
                worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:M3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                worksheet.Cells[row, 1].Value = refDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 1].Style.Font.Size = 13;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DarkOrange);
                worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                // file name
                string companyName = Regex.Replace(company.CompanyName.Trim(), @"[^a-zA-Z0-9]", "_").Trim('_');
                string fileNameString = Regex.Replace(companyName, @"[_]{2,}", @"_");
                fileName = string.Format("{0}_{1}_{2}{3}", "Daily_Attendance", refDate.ToString("yyyy-MMM-dd"), fileNameString, ".xlsx");

                // next row
                row++;

                // Headers
                // *****************************
                var properties = new Column[]
                    {
                        new Column(){ Name="Number",//A
                                      Width= 8,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee Name", //B
                                      Width= 25,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee #",//C
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name= "Status",//D
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Total Hours",//E
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Position",//F
                                      Width= 40,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name= "Shift",//G
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Job Order",//H
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Start",//I
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="End",//J
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Shift Duration",//K
                                      Width= 13,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee's Signature",//L
                                      Width= 28,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Comments",//M
                                      Width= 18,  Alignment= ExcelHorizontalAlignment.Left}

                    };

                // fromat column header
                this.SetWorkSheetColumns(worksheet, properties, 0, row);


                // next row
                row++;

                for (int i=0;i<candidateJobOrderIds.Length;i++)
                {
             
                            int col = 1;
                            int candidateJobOrderId = candidateJobOrderIds[i];
                            Candidate candidate = _candidatesService.GetCandidateById(_candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable().Where(c => c.Id == candidateJobOrderId).Select(a => a.CandidateId).FirstOrDefault());
                            JobOrder jo = _jobOrderService.GetJobOrderById(_candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable().Where(c => c.Id == candidateJobOrderId).Select(a => a.JobOrderId).FirstOrDefault());
                            decimal totalHours = _workTimeService.GetCandidateTotalHoursForCompany(candidate.Id, jo.CompanyId);
                            worksheet.Cells[row, col].Value = row - 4;
                            worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            col++;

                            worksheet.Cells[row, col].Value = candidate.LastName + ", " + candidate.FirstName; 
                            col++;

                            worksheet.Cells[row, col].Value = candidate.EmployeeId;
                            col++;
                    
                            worksheet.Cells[row, col].Value = totalHours>0?"Returning":"New";
                            col++;

                            worksheet.Cells[row, col].Value = totalHours;
                            col++;

                            worksheet.Cells[row, col].Value = jo.JobTitle;
                            worksheet.Cells[row, col].Style.WrapText = true;
                            col++;

                            worksheet.Cells[row, col].Value = jo.Shift.ShiftName;
                            col++;

                            worksheet.Cells[row, col].Value = jo.Id;
                            worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            col++;

                            worksheet.Cells[row, col].Value = jo.StartTime.ToString("hh:mm tt");
                            worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            col++;

                            worksheet.Cells[row, col].Value = jo.EndTime.ToString("hh:mm tt");
                            worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            col++;

                            // row height
                            worksheet.Row(row).Height = 30;

                            // Next row
                            row++;
                           
                }

                //Total row
                worksheet.Cells[row, 1].Value = "Total";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                // function
                worksheet.Cells[row, 11].Formula = "SUM(K5:K" + (row-1).ToString() + ")";
                row++;

                // border
                // *****************************
                worksheet.Cells["A1:M" + (row - 1).ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);
                worksheet.Cells["A5:M" + (row - 1).ToString()].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells["A5:M" + (row - 1).ToString()].Style.Border.Bottom.Color.SetColor(Color.DarkGray);
                worksheet.Cells["A5:M" + (row - 1).ToString()].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                worksheet.Cells["A5:M" + (row - 1).ToString()].Style.Border.Left.Color.SetColor(Color.DarkGray);
                worksheet.Cells["A5:M" + (row - 1).ToString()].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                worksheet.Cells["A5:M" + (row - 1).ToString()].Style.Border.Right.Color.SetColor(Color.DarkGray);
                worksheet.Cells["A5:M" + (row - 1).ToString()].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                worksheet.Cells["A5:M" + (row - 1).ToString()].Style.Border.Top.Color.SetColor(Color.DarkGray);

                // Next row
                worksheet.Cells[row, 2].Value = "Please Note: Client must complete time sheets in full. Hours must be signed, dated and confirmed by the worksite supervisor.";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Size = 10;
                row++;

                worksheet.Cells[row, 2].Value = "The client will be invoiced according to the hours recorded on the time sheets - so make sure the hours are accurate.";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Size = 10;
                row++;

                worksheet.Cells[row, 2].Value = franchiseName + " will not calculate lunch, so any deduction of hours must be calculated and recorded on the time sheet by the client.";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Size = 10;
                row++;

                worksheet.Cells[row, 2].Value = "Your time sheet is due back via fax or email every Monday by 3 p.m. Please fax or email time chart(s) to:";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Size = 10;
                row++;

                worksheet.Cells[row, 2].Value = String.Concat(primaryPhone," or ",email);
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Size = 10;
                row++;

                worksheet.Cells[row, 2].Value = "Comments (Ranking 1-5: 5 represents highest score for employee's performance while 1 represents lowest score.)";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Font.Size = 10;
                row++;

                row++;
                worksheet.Cells[row, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Medium; 
                worksheet.Cells[row, 2].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells[row, 8].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells[row, 9].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells[row, 10].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells[row, 12].Style.Border.Bottom.Color.SetColor(Color.Black);
                row++;

                worksheet.Cells[row, 2].Value = "Print Supervisor Name";
                worksheet.Cells[row, 8].Value = "Signature - Supervisor";
                worksheet.Cells[row, 12].Value = "Date";
                row++;

                row++;
                worksheet.Cells[row, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                worksheet.Cells[row, 2].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells[row, 8].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells[row, 12].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells[row, 8].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells[row, 10].Style.Border.Bottom.Color.SetColor(Color.Black);
                row++;

                worksheet.Cells[row, 2].Value = "Print Name - Commission Rep";
                worksheet.Cells[row, 8].Value = "Signature - Commission Rep";
                worksheet.Cells[row, 12].Value = "Date";
                row++;


                // add logo to row 1
                // *****************************
                // *** Put it at the end to avoid resizing ***
                //Image logo = GetLogo();
                //var picture = worksheet.Drawings.AddPicture("Logo", logo);
                //picture.SetSize(188, 68);
                // *** end of logo *** 

                xlPackage.Save();
            }

            return fileName;
        }

        #endregion


        #region Export CandidateWorkTime

        public void ExportCandidateWorkTimeToXlsx(Stream stream, IList<CandidateWorkTime> candidateWorkTime)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string franchiseName = _workContext.CurrentFranchise.FranchiseName;
            string fileName = string.Empty;
            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Employee Time Sheet Approval");
                //var candidateJobOrders = _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable().Where(x => candidateJobOrderIds.Contains(x.Id));


                // Printer settings
                this.SetPrinterToLetterSize(worksheet, TopMargin: 1.032M / 2.54M, RightMargin: 1.032M / 2.54M, BottomMargin: 1.032M / 2.54M, LeftMargin: 1.032M / 2.54M);
                worksheet.PrinterSettings.VerticalCentered = true;
                worksheet.PrinterSettings.HorizontalCentered = true;

                // initial row
                int row = 1;

                // Logo and title
                // *****************************
                this.SetWorkSheetTitle(worksheet, franchiseName + " Employee Daily Time Sheets", "O1", row);

                // next row
                row++;


                // Headers
                // *****************************
                var properties = new Column[]
                    {
                        new Column(){ Name="No.",//A
                                      Width= 5,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Id", //B
                                      Width= 13,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Name",//C 
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Job Order",//D
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Position",//E
                                      Width= 30,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Date",//F
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name= "Status",//G
                                      Width= 18,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Approved By",//H
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Punch In",//I
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Punch Out",//J
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Clock Hours",//K
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Gross Hours",//L
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Adjustment (Min)",//M
                                      Width= 18,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Billable Hours",//N
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Notes",//O
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left}
                    };

                // fromat column header
                this.SetWorkSheetColumns(worksheet, properties, 0, row);
                
                row++;

                foreach (var p in candidateWorkTime)
                {
                    int col = 1;

                    worksheet.Cells[row, col].Value = row - 2;
                    col++;

                    worksheet.Cells[row, col].Value = p.Candidate.EmployeeId;
                    col++;

                    worksheet.Cells[row, col].Value = String.Concat(p.Candidate.LastName, " ", p.Candidate.FirstName);
                    col++;

                    worksheet.Cells[row, col].Value = p.JobOrderId;
                    col++;

                    worksheet.Cells[row, col].Value = p.JobOrder.JobTitle;
                    col++;

                    worksheet.Cells[row, col].Value = p.JobStartDateTime.ToShortDateString();           // ToString("yyyy-MM-dd");
                    col++;

                    worksheet.Cells[row, col].Value = p.CandidateWorkTimeStatus.ToString();
                    col++;
                    
                    worksheet.Cells[row, col].Value = p.ApprovedByName;
                    col++;

                    if (p.ClockIn != null)
                    {
                        worksheet.Cells[row, col].Value = p.ClockIn.Value.ToString("yyyy-dd-MM HH:mm:ss");
                    }
                    else
                        worksheet.Cells[row, col].Value = "";
                    col++;

                    if (p.ClockOut != null)
                    {
                        worksheet.Cells[row, col].Value = p.ClockOut.Value.ToString("yyyy-dd-MM HH:mm:ss");
                    }
                    else
                        worksheet.Cells[row, col].Value = "";
                    col++;

                    worksheet.Cells[row, col].Value = p.ClockTimeInHours;
                    col++;

                    worksheet.Cells[row, col].Value = p.GrossWorkTimeInHours;
                    col++;

                    worksheet.Cells[row, col].Value = p.AdjustmentInMinutes;
                    col++;

                    worksheet.Cells[row, col].Value = p.NetWorkTimeInHours;
                    col++;

                    worksheet.Cells[row, col].Value = p.Note;
                    col++;

                    row++;
                }
                xlPackage.Save();

            }

        }


        public string ExportWorkTimeChangesAfterInvoiceToXlsx(Stream stream, IList<InvoiceUpdateDetail> timeCharts)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string fileName = "Invoice_Update_Details.xlsx";

            // Let's begin
            using (var xlPackage = new ExcelPackage(stream))
            {
                var sortedTimeCharts = timeCharts.GroupBy(x => new { x.CompanyId, x.FranchiseId, x.WeekOfYear, x.Year,x.BillingTaxRate }).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var item in sortedTimeCharts.Keys.OrderBy(x => x.CompanyId).ThenBy(x => x.FranchiseId).ThenBy(x => x.Year).ThenBy(x => x.WeekOfYear).ToList())
                {
                    var vendor = _franchiseService.GetFranchiseById(item.FranchiseId);
                    var vendorName = vendor != null ? vendor.FranchiseName : "Unknown Vendor";
                    var company = _companyService.GetCompanyById(item.CompanyId);
                    var companyName = company != null ? company.CompanyName : "Unknown Company";
                    string[] dates = DateService.WeekDateOfWeek(item.Year, item.WeekOfYear);
                    var worksheetName = String.Format("{0}{1}{2:d2}_{3}_{4}", item.Year, item.WeekOfYear, (int)((item.BillingTaxRate ?? 0m) * 100), item.FranchiseId, companyName);

                    // get handle to the existing worksheet
                    var worksheet = xlPackage.Workbook.Worksheets.Add(worksheetName);

                    // Printer settings
                    this.SetPrinterToLegalSize(worksheet);
                    worksheet.PrinterSettings.Scale = 75;

                    // initial row
                    int row = 1;

                    // Logo and title
                    // *****************************
                    this.SetWorkSheetTitle(worksheet, vendorName + Environment.NewLine + "Invoice Update Details", "T1", row);
                    worksheet.Cells[row, 1].Style.WrapText = true;

                    // next row
                    row++;

                    // Headers
                    // *****************************
                    var properties = new Column[]
                    {
                        new Column(){ Name="Employee Name",//A
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee #", // B
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Department",//C
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Position",//D
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Shift",//E
                                      Width= 11,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Sun",//F
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Mon",//G
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name= "Tue",//H
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Wed",//I
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Thu",//J
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Fri",//K
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Sat",//L
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Total Hrs",//M
                                      Width= 11,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Reg Hrs",//N
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Reg Billing Rate",//O
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Reg Amount",//P
                                      Width= 13,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="OT Hrs",//Q
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="OT Billing Rate",//R
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="OT Amount",//S
                                      Width= 13,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Total",//T
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Right}
                    };

                    // fromat column header
                    this.SetWorkSheetColumns(worksheet, properties, 0, row);

                    worksheet.Row(row).Height = 30;

                    // next row
                    row++;

                    var subHeader = new string[]
                    {
                        "",
                        "",
                        "",
                        "",
                        "",
                        dates[0],
                        dates[1],
                        dates[2],
                        dates[3],
                        dates[4],
                        dates[5],
                        dates[6],
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        ""
                    };
                    for (int i = 0; i < subHeader.Length; i++)
                    {
                        worksheet.Cells[row, i + 1].Value = subHeader[i];
                        worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                        worksheet.Cells[row, i + 1].Style.Font.Bold = false;
                    }

                    // next row
                    row++;

                    foreach (var b in sortedTimeCharts[item].OrderBy(b => b.EmployeeLastName))
                    {
                        int col = 1;
                        //A
                        worksheet.Cells[row, col].Value = String.Concat(b.EmployeeLastName, ", ", b.EmployeeFirstName);
                        col++;
                        //B
                        worksheet.Cells[row, col].Value = b.EmployeeId;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        col++;
                        //C
                        worksheet.Cells[row, col].Value = b.DepartmentName;
                        col++;
                        //D
                        worksheet.Cells[row, col].Value = b.PositionCode;
                        col++;
                        //E
                        worksheet.Cells[row, col].Value = b.ShiftCode;
                        col++;
                        //F
                        worksheet.Cells[row, col].Value = b.Sunday;
                        col++;
                        //G
                        worksheet.Cells[row, col].Value = b.Monday;
                        col++;
                        //H
                        worksheet.Cells[row, col].Value = b.Tuesday;
                        col++;
                        //I
                        worksheet.Cells[row, col].Value = b.Wednesday;
                        col++;
                        //J
                        worksheet.Cells[row, col].Value = b.Thursday;
                        col++;
                        //K
                        worksheet.Cells[row, col].Value = b.Friday;
                        col++;
                        //L
                        worksheet.Cells[row, col].Value = b.Saturday;
                        col++;
                        //M
                        worksheet.Cells[row, col].Value = b.SubTotalHours;
                        col++;
                        //N
                        worksheet.Cells[row, col].Value = b.RegularHours;
                        col++;
                        //O
                        worksheet.Cells[row, col].Value = b.RegularBillingRate;
                        worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        col++;
                        //P
                        worksheet.Cells[row, col].Value = b.RegularHours * b.RegularBillingRate;
                        worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        col++;
                        //Q
                        worksheet.Cells[row, col].Value = b.OTHours;
                        col++;
                        //R
                        worksheet.Cells[row, col].Value = b.OvertimeBillingRate;
                        worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        col++;
                        //S
                        worksheet.Cells[row, col].Value = b.OTHours * b.OvertimeBillingRate;
                        worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        col++;
                        //T
                        worksheet.Cells[row, col].Value = b.RegularHours * b.RegularBillingRate + b.OTHours * b.OvertimeBillingRate;
                        worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        col++;

                        // row height
                        worksheet.Row(row).Height = 18;

                        // Next row
                        row++;
                    }

                    // border
                    // *****************************
                    worksheet.Cells["A1:T" + row.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);

                    row++;

                }

                xlPackage.Save();

            }
            return fileName;

        }


        public string ExportMissingHourToXlsx(Stream stream, IEnumerable<CandidateMissingHour> missingHours, bool toClient = false)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string fileName = "Missing_Hour.xlsx";

            using (var xlPackage = new ExcelPackage(stream))
            {
                string worksheetName = "Missing Hour";
                var worksheet = xlPackage.Workbook.Worksheets.Add(worksheetName);

                // Printer settings
                this.SetPrinterToLegalSize(worksheet, RightMargin: 1.4M / 2.54M, LeftMargin: 1.4M / 2.54M);

                // Headers
                // *****************************
                var properties = new Column[11]
                    {
                        new Column(){ Name="No.",//A
                                      Width= 5,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Report Date", // B
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee ID",//C
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee Name",//D
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Shift",//E
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Working Date&Time\r\nmay be missed",//F
                                      Width= 30,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name= "Working Hours\rmay be missed",//G
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Company",//H
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Department",//I
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Status",//J
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Entered by",//K
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Right},
                    };

                // fromat column header
                this.SetWorkSheetColumns(worksheet, properties, 0, 1);


                if (toClient)
                {
                    properties[8].Name = "Approved by";
                    properties[9].Name = "Notes";
                }

                // initial row
                int row = 1;
                int col = 1;

                worksheet.Row(row).Height = 30;
                worksheet.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                foreach (var item in missingHours.OrderBy(x => x.CandidateId).ThenBy(x => x.WorkDate))
                {
                    row++;
                    col = 1;
                    worksheet.Row(row).Height = 18;
                    worksheet.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    worksheet.Cells[row, col].Value = row - 1;
                    col++;
                    
                    worksheet.Cells[row, col].Value = item.CreatedOnUtc.Value.ToLocalTime().ToShortDateString();
                    col++;
                    
                    worksheet.Cells[row, col].Value = item.Candidate.EmployeeId;
                    col++;
                    
                    worksheet.Cells[row, col].Value = item.Candidate.GetFullName();
                    col++;
                    
                    worksheet.Cells[row, col].Value = item.JobOrder.Shift.ShiftName;
                    col++;
                    
                    worksheet.Cells[row, col].Value = String.Concat(item.JobOrder.StartTime.ToString("h:mmtt"), " to ", item.JobOrder.EndTime.ToString("h:mmtt"),
                                                                ", ", item.WorkDate.ToShortDateString());
                    col++;
                    
                    worksheet.Cells[row, col].Value = item.NewHours;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    col++;

                    worksheet.Cells[row, col].Value = item.JobOrder.CompanyId > 0 ? item.JobOrder.Company.CompanyName : string.Empty;
                    col++;
                    
                    worksheet.Cells[row, col].Value = item.JobOrder.CompanyDepartmentId > 0 ?
                        item.JobOrder.Company.CompanyDepartments.Where(x => x.Id == item.JobOrder.CompanyDepartmentId).FirstOrDefault().DepartmentName :
                        String.Empty;
                    col++;

                    if (!toClient)
                    {
                        worksheet.Cells[row, col].Value = ((CandidateMissingHourStatus)item.CandidateMissingHourStatusId).ToString();
                        col++;

                        worksheet.Cells[row, col].Value = item.EnteredBy > 0 ? _accountService.GetAccountById(item.EnteredBy).FullName : String.Empty;
                    }
                    else
                        col++;
                }

                // border
                worksheet.Cells[1, 1, row, col].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);

                xlPackage.Save();
            }
            return fileName;

        }

        #endregion


        #region Export EmployeeTimeChartHistory

        public string ExportEmployeeTimeChartToXlsxForAdmin(Stream stream, IList<EmployeeTimeChartHistory> timeCharts, 
            DateTime begin, DateTime end, int companyId = 0, int vendorId = 0, bool withRates = false)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string fileName = string.Empty;
            //CompanyId = 0 means timecharts can contain time charts from multiple companies
            if (companyId == 0)
            {
                fileName = String.Format("EmployeeTimeCharts_{0}-{1}", begin.ToString("MMM dd, yyyy"), end.ToString("MMM dd, yyyy"));
            }
            //CompanyId != 0 means timecharts only contain time charts from one company
            else
            {
                string companyName = _companyService.GetCompanyById(companyId).CompanyName;
                fileName = String.Format("{0}_{1}-{2}", companyName, begin.ToString("MMM dd, yyyy"), end.ToString("MMM dd, yyyy"));
            }
            
            var defVendor = _franchiseService.GetFranchiseById(_franchiseService.GetDefaultMSPId());
            var vendorName = defVendor.FranchiseName;
            if (vendorId > 0 && vendorId != defVendor.Id)
            {
                var currentVendor = _franchiseService.GetFranchiseById(vendorId);
                vendorName = currentVendor.FranchiseName;

                if (!currentVendor.IsLinkToPublicSite)
                    fileName += String.Format("_{0}", vendorName);
            }
            fileName += ".xlsx";

            // invoicing enabled, and include rates
            var invoicingEnabled = _featureService.IsFeatureEnabled("Admin", "Invoicing") && withRates;

            //Create Headers and format them
            var properties = new List<string>
                    {
                        "Employee Name",
                        "Badge ID",
                        "Employee #",
                        "Department",
                        "Position",
                        "Shift",
                        "Sun",
                        "Mon",
                        "Tue",
                        "Wed",
                        "Thu",
                        "Fri",
                        "Sat",
                        "Total Hrs",
                        "Reg Hrs"
                    };
            // column width
            var columnWidth = new List<int>
                    {
                        20,
                        10,
                        10,
                        15,
                        15,
                        11,
                        7,
                        7,
                        7,
                        7,
                        7,
                        7,
                        7,
                        11,
                        10
                    };

            if (invoicingEnabled)
            {
                properties.Add("Reg Billing Rate");
                properties.Add("Reg Amount");
                columnWidth.Add(10);
                columnWidth.Add(13);
            }
            properties.Add("OT Hrs");
            columnWidth.Add(10);
            if (invoicingEnabled)
            {
                properties.Add("OT Billing Rate");
                properties.Add("OT Amount");
                properties.Add("Total");
                columnWidth.Add(10);
                columnWidth.Add(13);
                columnWidth.Add(15);
            }
            var maxCol = properties.Count;

            // Let's begin
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true;

                var sortedTimeCharts = timeCharts.GroupBy(x => new { x.WeekOfYear, x.Year, x.CompanyName, x.BillingTaxRate }).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var item in sortedTimeCharts.Keys.OrderBy(x => x.CompanyName).ThenBy(x => x.WeekOfYear).ThenBy(x => x.Year).ToList())
                {
                    string worksheetName = String.Format("{0}{1}{2}{3}", item.Year, item.WeekOfYear, (int)(item.BillingTaxRate * 100), item.CompanyName);
                    // get handle to the existing worksheet
                    var worksheet = xlPackage.Workbook.Worksheets.Add(worksheetName);

                    // Printer settings
                    worksheet.PrinterSettings.Scale = invoicingEnabled ? 57 : 82;
                    worksheet.PrinterSettings.Orientation = eOrientation.Landscape;

                    // initial row
                    int row = 1;

                    // Logo and title
                    // *****************************
                    worksheet.Row(1).Height = 55;
                    worksheet.Cells[row, 1, row, maxCol].Merge = true;
                    worksheet.Cells[row, 1, row, maxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 1, row, maxCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[row, 1].Value = vendorName + Environment.NewLine + "Weekly Time Sheets";
                    worksheet.Cells[row, 1].Style.WrapText = true;
                    worksheet.Cells[row, 1].Style.Font.Size = 18;
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DarkGreen);
                    worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));
                 //////   this.SetWorkSheetTitle(worksheet, vendorName + Environment.NewLine + "Weekly Time Sheets", "L1", row);

                    // next row
                    row++;
                    DateTime startDate = DateService.FirstDateOfWeek(item.Year, item.WeekOfYear);
                    worksheet.Cells[row, 1].Value = String.Concat(item.CompanyName, " Week of :", startDate.ToString("MMM-dd"), "-", startDate.AddDays(6).ToString("MMM-dd"), ",", item.Year);
                    row++;
                    worksheet.Cells[row, 1].Value = DateTime.Today.ToString("MMMM dd,yyyy");
                    worksheet.Cells[row, 2].Value = "Invoice:";
                    row++;

                    for (int i = 0; i < maxCol; i++)
                    {
                        worksheet.Cells[row, i + 1].Value = properties[i];
                        worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                        worksheet.Cells[row, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[row, i + 1].Style.WrapText = true;

                        worksheet.Column(i + 1).Width = columnWidth[i];
                        if (i >= 6)
                            worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    worksheet.Row(row).Height = 30;
                    worksheet.Row(row).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // next row
                    row++;

                    for (int i = 1; i <= maxCol; i++)
                    {
                        worksheet.Cells[row, i].Value = i >= 7 && i <= 13 ? startDate.AddDays(i - 7).ToString("dd-MMM") : null;
                        worksheet.Cells[row, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                        worksheet.Cells[row, i].Style.Font.Bold = false;
                    }

                    // next row
                    row++;

                    foreach (var b in sortedTimeCharts[item].OrderBy(b => b.EmployeeLastName))
                    {
                        int col = 1;
                        //A
                        worksheet.Cells[row, col].Value = String.Concat(b.EmployeeLastName, ", ", b.EmployeeFirstName);
                        if (!String.IsNullOrWhiteSpace(b.Note))
                        {
                            worksheet.Cells[row, col].AddComment(b.Note.Trim(), "GC");
                        }
                        col++;

                        //B
                        worksheet.Cells[row, col].Value = b.EmployeeId;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        col++;
                        //C
                        worksheet.Cells[row, col].Value = b.EmployeeNumber;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        col++;

                        //D
                        worksheet.Cells[row, col].Value = b.DepartmentName;
                        col++;
                        //E
                        worksheet.Cells[row, col].Value = b.PositionCode;
                        col++;
                        //F
                        worksheet.Cells[row, col].Value = b.ShiftCode;
                        col++;
                        //G
                        worksheet.Cells[row, col].Value = b.Sunday;
                        if (!b.SundayStatus && b.Sunday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //H
                        worksheet.Cells[row, col].Value = b.Monday;
                        if (!b.MondayStatus && b.Monday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //I
                        worksheet.Cells[row, col].Value = b.Tuesday;
                        if (!b.TuesdayStatus && b.Tuesday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //J
                        worksheet.Cells[row, col].Value = b.Wednesday;
                        if (!b.WednesdayStatus && b.Wednesday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //K
                        worksheet.Cells[row, col].Value = b.Thursday;
                        if (!b.ThursdayStatus && b.Thursday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //L
                        worksheet.Cells[row, col].Value = b.Friday;
                        if (!b.FridayStatus && b.Friday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //M
                        worksheet.Cells[row, col].Value = b.Saturday;
                        if (!b.SaturdayStatus && b.Saturday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //N
                        worksheet.Cells[row, col].Value = b.SubTotalHours;
                        col++;
                        //O
                        worksheet.Cells[row, col].Value = b.RegularHours;
                        col++;
                        
                        if (invoicingEnabled)
                        {
                            worksheet.Cells[row, col].Value = b.RegularBillingRate;
                            worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                            col++;
                            worksheet.Cells[row, col].Value = b.RegularHours * b.RegularBillingRate;
                            worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                            col++;
                        }

                        worksheet.Cells[row, col].Value = b.OTHours;
                        col++;

                        if (invoicingEnabled)
                        {
                            worksheet.Cells[row, col].Value = b.OvertimeBillingRate;
                            worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                            col++;
                            worksheet.Cells[row, col].Value = b.OTHours * b.OvertimeBillingRate;
                            worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                            col++;
                            worksheet.Cells[row, col].Value = b.RegularHours * b.RegularBillingRate + b.OTHours * b.OvertimeBillingRate;
                            worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                            col++;
                        }

                        // row height
                        worksheet.Row(row).Height = 18;
                        if ((row % 2) != 0)
                        {
                            using (ExcelRange Rng = worksheet.Cells[row,1,row,maxCol] )
                            {
                                Rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                Rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(237,241,248));
                            }  
                        }

                        // Next row
                        row++;
                    }

                    // border
                    // *****************************
                    worksheet.Cells[1, 1, row, maxCol].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);

                    // Total row
                    // *****************************
                    int rowNum = row - 1;
                    worksheet.Cells[row, 1].Value = "Total";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    // function
                    if (invoicingEnabled)
                    {
                        worksheet.Cells[row, maxCol - 7].Formula = String.Format("SUM({0}6:{0}{1})", (char)('A' + maxCol - 7 - 1), rowNum);
                        worksheet.Cells[row, maxCol - 6].Formula = String.Format("SUM({0}6:{0}{1})", (char)('A' + maxCol - 6 - 1), rowNum);
                        worksheet.Cells[row, maxCol - 4].Formula = String.Format("SUM({0}6:{0}{1})", (char)('A' + maxCol - 4 - 1), rowNum);
                        worksheet.Cells[row, maxCol - 3].Formula = String.Format("SUM({0}6:{0}{1})", (char)('A' + maxCol - 3 - 1), rowNum);
                        worksheet.Cells[row, maxCol - 1].Formula = String.Format("SUM({0}6:{0}{1})", (char)('A' + maxCol - 1 - 1), rowNum);
                    }
                    else
                    {
                        worksheet.Cells[row, maxCol - 2].Formula = String.Format("SUM({0}6:{0}{1})", (char)('A' + maxCol - 2 - 1), rowNum);
                        worksheet.Cells[row, maxCol - 1].Formula = String.Format("SUM({0}6:{0}{1})", (char)('A' + maxCol - 1 - 1), rowNum);
                        worksheet.Cells[row, maxCol].Formula = String.Format("SUM({0}6:{0}{1})", (char)('A' + maxCol - 1), rowNum);
                    }
                    row++;

                    if (invoicingEnabled)
                    {
                        worksheet.Cells[row, maxCol - 1].Value = "Credit Hours Subtotal:";
                        worksheet.Cells[row, maxCol - 1].Style.Font.Bold = true;
                        worksheet.Cells[row, maxCol - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        // function
                        worksheet.Cells[row, maxCol].Formula = String.Format("SUM({0}6:{0}{1})", (char)('A' + maxCol - 1), rowNum);
                        worksheet.Cells[row, maxCol].Style.Numberformat.Format = "$#,##0.00";
                        row++;

                        var taxRate = item.BillingTaxRate.GetValueOrDefault();
                        worksheet.Cells[row, maxCol - 1].Value = String.Format("HST({0:P2}):", taxRate);
                        worksheet.Cells[row, maxCol - 1].Style.Font.Bold = true;
                        worksheet.Cells[row, maxCol - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        // function
                        worksheet.Cells[row, maxCol].Formula = String.Format("{0}{1}*{2}", (char)('A' + maxCol - 1), row - 1, taxRate);
                        worksheet.Cells[row, maxCol].Style.Numberformat.Format = "$#,##0.00";
                        row++;

                        worksheet.Cells[row, maxCol - 1].Value = "Total:";
                        worksheet.Cells[row, maxCol - 1].Style.Font.Bold = true;
                        worksheet.Cells[row, maxCol - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        // function
                        worksheet.Cells[row, maxCol].Formula = String.Format("{0}{1}+{0}{2}", (char)('A' + maxCol - 1), row - 2, row - 1);
                        worksheet.Cells[row, maxCol].Style.Numberformat.Format = "$#,##0.00";
                        row++;
                    }

                    // Next row
                    worksheet.Cells[row, 1].Value = "*The amount in red is a pending hour, which is not approved yet.";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    row++;

                    // add logo to row 1
                    // *****************************
                    // *** Put it at the end to avoid resizing ***
                    //Image logo = GetLogo();
                    //var picture = worksheet.Drawings.AddPicture("Logo", logo);
                    //picture.SetSize(188, 68);
                    // *** end of logo *** 
                }

                xlPackage.Save();

            }
            return fileName;
        }


        public string ExportEmployeeTimeChartToXlsxForReminder(Stream stream, IList<EmployeeTimeChartHistory> timeCharts)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string fileName = "EmployeeTimeCharts.xlsx";

            string franchiseName = _franchiseService.GetDefaultMSPName();

            // Let's begin
            using (var xlPackage = new ExcelPackage(stream))
            {
                var sortedTimeCharts = timeCharts.GroupBy(x => new { x.WeekOfYear, x.Year, x.CompanyName, x.BillingTaxRate }).ToDictionary(x => x.Key, x => x.ToList());
                if (sortedTimeCharts.Count == 1)
                {
                    fileName = sortedTimeCharts.Keys.Single().CompanyName + "_Week_" + sortedTimeCharts.Keys.Single().WeekOfYear + "_Year_" + sortedTimeCharts.Keys.Single().Year + "_time_charts.xlsx";
                }
                foreach (var item in sortedTimeCharts.Keys.OrderBy(x => x.CompanyName).ThenBy(x => x.WeekOfYear).ThenBy(x => x.Year).ToList())
                {
                    // get handle to the existing worksheet
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Week " + item.WeekOfYear + "/" + item.Year + " " + item.CompanyName);


                    // Printer settings
                    this.SetPrinterToLegalSize(worksheet);

                    // initial row
                    int row = 1;


                    // Logo and title
                    // *****************************
                    this.SetWorkSheetTitle(worksheet, franchiseName + " Employee Time Sheets", "P1", row);


                    // next row
                    row++;
                    DateTime startDate = DateService.FirstDateOfWeek(item.Year, item.WeekOfYear);
                    worksheet.Cells[row, 1].Value =  String.Concat(item.CompanyName , " Week of :" , startDate.ToString("MMM-dd") , "-" , startDate.AddDays(6).ToString("MMM-dd") , "," ,item.Year);
                    row++;
                    worksheet.Cells[row, 1].Value = DateTime.Today.ToString("MMMM dd,yyyy");
                    worksheet.Cells[row, 2].Value = "Invoice:";
                    row++;

                    // Headers
                    // *****************************
                    var properties = new Column[]
                    {
                        new Column(){ Name="Employee Name",//A
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee #", // B
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Department",//C
                                      Width= 13,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Contact",//D
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Position",//E
                                      Width= 28,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Shift",//F
                                      Width= 8,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name= "Sun",//G
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Mon",//H
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Tue",//I
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Wed",//J
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Thu",//K
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Fri",//L
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Sat",//M
                                      Width= 7,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Total Hrs",//N
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Reg Hrs",//O
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="OT Hrs",//P
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Right}
                    };

                    // fromat column header
                    this.SetWorkSheetColumns(worksheet, properties, 0, row);
                    
                    // next row
                    row++;


                    var subHeader = new string[]
                    {
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        startDate.ToString("dd-MMM"),
                        startDate.AddDays(1).ToString("dd-MMM"),
                        startDate.AddDays(2).ToString("dd-MMM"),
                        startDate.AddDays(3).ToString("dd-MMM"),
                        startDate.AddDays(4).ToString("dd-MMM"),
                        startDate.AddDays(5).ToString("dd-MMM"),
                        startDate.AddDays(6).ToString("dd-MMM"),
                        "",
                        "",
                        //"",
                        "",
                        //"",
                        //""
                    };
                    for (int i = 0; i < subHeader.Length; i++)
                    {
                        worksheet.Cells[row, i + 1].Value = subHeader[i];
                        worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                        worksheet.Cells[row, i + 1].Style.Font.Bold = false;
                    }


                    // next row
                    row++;


                    foreach (var b in sortedTimeCharts[item].OrderBy(b => b.EmployeeLastName))
                    {
                        int col = 1;
                        //A
                        worksheet.Cells[row, col].Value = String.Concat(b.EmployeeLastName , ", " , b.EmployeeFirstName);
                        col++;
                        //B
                        worksheet.Cells[row, col].Value = b.EmployeeId;
                        col++;
                        //C
                        worksheet.Cells[row, col].Value = b.DepartmentName;
                        col++;
                        //D
                        worksheet.Cells[row, col].Value = b.ContactName;
                        col++;
                        //E
                        worksheet.Cells[row, col].Value = b.JobTitle;
                        worksheet.Cells[row, col].Style.WrapText = true;
                        col++;
                        //F
                        worksheet.Cells[row, col].Value = b.ShiftCode;
                        col++;
                        //G
                        worksheet.Cells[row, col].Value = b.Sunday;
                        if (!b.SundayStatus && b.Sunday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //H
                        worksheet.Cells[row, col].Value = b.Monday;
                        if (!b.MondayStatus && b.Monday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //I
                        worksheet.Cells[row, col].Value = b.Tuesday;
                        if (!b.TuesdayStatus && b.Tuesday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //J
                        worksheet.Cells[row, col].Value = b.Wednesday;
                        if (!b.WednesdayStatus && b.Wednesday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //K
                        worksheet.Cells[row, col].Value = b.Thursday;
                        if (!b.ThursdayStatus && b.Thursday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //L
                        worksheet.Cells[row, col].Value = b.Friday;
                        if (!b.FridayStatus && b.Friday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //M
                        worksheet.Cells[row, col].Value = b.Saturday;
                        if (!b.SaturdayStatus && b.Saturday > 0)
                            worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.Red);
                        col++;
                        //N
                        worksheet.Cells[row, col].Value = b.SubTotalHours;
                        col++;
                        //O
                        worksheet.Cells[row, col].Value = b.RegularHours;
                        col++;

                        //P
                        worksheet.Cells[row, col].Value = b.OTHours;
                        col++;



                        // row height
                        worksheet.Row(row).Height = 30;



                        // Next row
                        row++;

                    }



                    // border
                    // *****************************
                    worksheet.Cells["A1:P" + row.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);




                    // Total row
                    // *****************************
                    int count = row - 1;
                    worksheet.Cells[row, 1].Value = "SubTotal";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    // function
                    worksheet.Cells[row, properties.Count() - 2].Formula = "SUM(N6:N" + count.ToString() + ")";
                    worksheet.Cells[row, properties.Count() - 1].Formula = "SUM(O6:O" + count.ToString() + ")";
                    worksheet.Cells[row, properties.Count()].Formula = "SUM(P6:P" + count.ToString() + ")";
                    row++;

                    worksheet.Cells[row, 1].Value = "*The amount in red is a pending hour, which is not approved yet.";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    row++;



                    // add logo to row 1
                    // *****************************
                    // *** Put it at the end to avoid resizing ***
                    //Image logo = GetLogo();
                    //var picture = worksheet.Drawings.AddPicture("Logo", logo);
                    //picture.SetSize(188, 68);
                    // *** end of logo *** 

                }

                xlPackage.Save();

            }
            return fileName;
        }

        public string ExportEmployeeTimeChartToXlsxForClient(Stream stream, IList<EmployeeTimeChartHistory> timeCharts)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            //var sortedbyCompanyTimeCharts = timeCharts.GroupBy(x => x.CompanyName).ToDictionary(x => x.Key, x => x.ToList());
            //List<string> fileNames = new List<string>();
            //foreach (var companyName in sortedbyCompanyTimeCharts.Keys)
            //{
            string fileName = "EmployeeTimeSheet.xlsx";

            // Let's begin
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true;

                //// file name
                //fileName = string.Format("{0}_{1}_{2}_{3}{4}", "Employee_Timecharts", payperiod.StartDate.AddDays(0).ToString("yyyy-MMM-dd"), payperiod.StartDate.AddDays(6).ToString("yyyy-MMM-dd"), fileNameString, ".xlsx");

                var sortedTimeCharts = timeCharts.GroupBy(x => new { x.WeekOfYear, x.Year, x.CompanyName, x.BillingTaxRate }).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var item in sortedTimeCharts.Keys.OrderBy(x => x.CompanyName).ThenBy(x => x.WeekOfYear).ThenBy(x => x.Year).ToList())
                {
                    // get handle to the existing worksheet
                    var worksheet = xlPackage.Workbook.Worksheets.Add(String.Concat( "Week " , item.WeekOfYear , "_" , item.Year , " " , item.CompanyName," ",item.BillingTaxRate));

                    // Printer settings
                    this.SetPrinterToLegalSize(worksheet);

                    // initial row
                    int row = 1;

                    // column headers
                    var properties = new string[]
                    {
                        "Vendor",
                        "Employee Name",
                        "Employee #",
                        "Department",
                        "Position",
                        "Shift",
                        "Sun",
                        "Mon",
                        "Tue",
                        "Wed",
                        "Thu",
                        "Fri",
                        "Sat",
                        "Total Hrs",
                        "Reg Hrs",
                        //"Regular Billing Rate",
                        "OT Hrs",
                        "Approved By"
                        //"Overtime Billing Rate",
                        //"Subtotal"
                    };

                    // column width
                    var columnWidth = new int[]
                    {
                        30,
                        15,
                        10,
                        13,
                        28,
                        8,
                        7,
                        7,
                        7,
                        7,
                        7,
                        7,
                        7,
                        9,
                        9,
                        //20,
                        9,
                        20
                        //10
                    };

                    // show or hide vendor
                    var displayVendor = _commonSettings.DisplayVendor;
                    var firstCol = displayVendor ? 0 : 1;

                    var sheetWidth = properties.Length - firstCol;
                    var lastCol = ((char)('A' + sheetWidth - 1)).ToString();
                    var sheetRange = "A1:" + lastCol;
                    var firstRowRange = sheetRange + "1";
                    var totalHrsCol = ((char)('A' + sheetWidth - 1 - 3)).ToString();
                    var regHrsCol = ((char)('A' + sheetWidth - 1 - 2)).ToString();
                    var OTHrsCol = ((char)('A' + sheetWidth - 1 - 1)).ToString();

                    // Logo and title
                    // *****************************
                    worksheet.Row(1).Height = 55;
                    worksheet.Cells[firstRowRange].Merge = true;
                    worksheet.Cells[firstRowRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[firstRowRange].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[row, 1].Value = "Employee Time Sheet";
                    worksheet.Cells[row, 1].Style.Font.Size = 18;
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DarkGreen);
                    worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                   ////// this.SetWorkSheetTitle(worksheet, " Employee Time Sheet", "L1", row);

                    // next row
                    row++;
                    DateTime startDate = DateService.FirstDateOfWeek(item.Year, item.WeekOfYear);
                    worksheet.Cells[row, 1].Value = String.Concat(item.CompanyName , " Week of :" , startDate.ToString("MMM-dd") , "-" , startDate.AddDays(6).ToString("MMM-dd"), "," , item.Year);
                    row++;
                    worksheet.Cells[row, 1].Value = DateTime.Today.ToString("MMMM dd,yyyy");
                    worksheet.Cells[row, 2].Value = "Invoice:";
                    row++;
                    // get pay period info
                    //var firstBillingChart = billingCharts.FirstOrDefault();
                    //var payperiod = _payPeriodService.GetPayPeriodById(firstBillingChart.PayPeriodId);

                    // fromat column header
                    for (int i = firstCol; i < properties.Length; i++)
                    {
                        worksheet.Cells[row, i + 1 - firstCol].Value = properties[i];
                        worksheet.Cells[row, i + 1 - firstCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, i + 1 - firstCol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                        worksheet.Cells[row, i + 1 - firstCol].Style.Font.Bold = true;

                        worksheet.Column(i + 1 - firstCol).Width = columnWidth[i];
                        if (i >= 6) worksheet.Cells[row, i + 1 - firstCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    worksheet.Row(row).Height = 22;
                    worksheet.Row(row).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // next row
                    row++;

                    var subHeader = new string[]
                    {
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        startDate.ToString("dd-MMM"),
                        startDate.AddDays(1).ToString("dd-MMM"),
                        startDate.AddDays(2).ToString("dd-MMM"),
                        startDate.AddDays(3).ToString("dd-MMM"),
                        startDate.AddDays(4).ToString("dd-MMM"),
                        startDate.AddDays(5).ToString("dd-MMM"),
                        startDate.AddDays(6).ToString("dd-MMM"),
                        "",
                        "",
                        //"",
                        "",
                        ""
                        //""
                    };
                    for (int i = firstCol; i < subHeader.Length; i++)
                    {
                        worksheet.Cells[row, i + 1 - firstCol].Value = subHeader[i];
                        worksheet.Cells[row, i + 1 - firstCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, i + 1 - firstCol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                        worksheet.Cells[row, i + 1 - firstCol].Style.Font.Bold = false;
                    }

                    // next row
                    row++;
                    var firstDataRow = row;
                    var firstDataRowStr = firstDataRow.ToString();

                    foreach (var b in sortedTimeCharts[item].OrderBy(b => b.EmployeeLastName))
                    {
                        int col = 1;
                        
                        if (displayVendor)
                        {
                            //Vendor
                            worksheet.Cells[row, col].Value = b.FranchiseName;
                            worksheet.Cells[row, col].Style.WrapText = true;
                            col++;
                        }
                        
                        //A
                        worksheet.Cells[row, col].Value = String.Concat(b.EmployeeLastName , ", " , b.EmployeeFirstName);
                        worksheet.Cells[row, col].Style.WrapText = true;
                        col++;
                        //B
                        worksheet.Cells[row, col].Value = b.EmployeeNumber;
                        col++;

                        //C
                        worksheet.Cells[row, col].Value = b.DepartmentName;
                        col++;
                        //D
                        worksheet.Cells[row, col].Value = b.JobTitle;
                        worksheet.Cells[row, col].Style.WrapText = true;
                        col++;
                        //E
                        worksheet.Cells[row, col].Value = b.ShiftCode;
                        col++;
                        //F
                        worksheet.Cells[row, col].Value = b.Sunday;
                        col++;
                        //G
                        worksheet.Cells[row, col].Value = b.Monday;
                        col++;
                        //H
                        worksheet.Cells[row, col].Value = b.Tuesday;
                        col++;
                        //I
                        worksheet.Cells[row, col].Value = b.Wednesday;
                        col++;
                        //J
                        worksheet.Cells[row, col].Value = b.Thursday;
                        col++;
                        //K
                        worksheet.Cells[row, col].Value = b.Friday;
                        col++;
                        //L
                        worksheet.Cells[row, col].Value = b.Saturday;
                        col++;
                        //M
                        worksheet.Cells[row, col].Value = b.SubTotalHours;
                        col++;
                        //N
                        worksheet.Cells[row, col].Value = b.RegularHours;
                        col++;
                        ////O
                        //worksheet.Cells[row, col].Value = b.RegularBillingRate;
                        //worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        //col++;
                        //O
                        worksheet.Cells[row, col].Value = b.OTHours;
                        col++;
                        //Q
                        worksheet.Cells[row, col].Value = b.ApprovedBy;
                        worksheet.Cells[row, col].Style.WrapText = true;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        col++;
                        //R
                        //worksheet.Cells[row, col].Value = b.RegularHours * b.RegularBillingRate + b.OTHours * b.OvertimeBillingRate;
                        //worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        //col++;

                        // row height
                        worksheet.Row(row).Height = 30;

                        // Next row
                        row++;
                    }

                    // border
                    // *****************************
                    worksheet.Cells[sheetRange + row.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);

                    // Total row
                    // *****************************
                    int count = row - 1;
                    worksheet.Cells[row, 1].Value = "Total";
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    // function
                    worksheet.Cells[row, properties.Count() - 3 - firstCol].Formula = "SUM(" + totalHrsCol + firstDataRowStr + ":" + totalHrsCol + count.ToString() + ")";
                    worksheet.Cells[row, properties.Count() - 2 - firstCol].Formula = "SUM(" + regHrsCol + firstDataRowStr + ":" + regHrsCol + count.ToString() + ")";
                    worksheet.Cells[row, properties.Count() - 1 - firstCol].Formula = "SUM(" + OTHrsCol + firstDataRowStr + ":" + OTHrsCol + count.ToString() + ")";
                    row++;

                    //worksheet.Cells[row, properties.Count() - 1].Value = "Total OT Hours:";
                    //worksheet.Cells[row, properties.Count() - 1].Style.Font.Bold = true;
                    //worksheet.Cells[row, properties.Count() - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //// function
                    //worksheet.Cells[row, properties.Count()].Formula = "SUM(M4:M" + count.ToString() + ")";
                    //row++;

                    //worksheet.Cells[row, properties.Count() - 1].Value = "Credit Hours Subtotal:";
                    //worksheet.Cells[row, properties.Count() - 1].Style.Font.Bold = true;
                    //worksheet.Cells[row, properties.Count() - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //// function
                    //worksheet.Cells[row, properties.Count()].Formula = "SUM(R6:R" + count.ToString() + ")";
                    //worksheet.Cells[row, properties.Count()].Style.Numberformat.Format = "$#,##0.00";
                    //row++;

                    //worksheet.Cells[row, properties.Count() - 1].Value = "HST(" + item.BillingTaxRate.ToString("P2") + "):";
                    //worksheet.Cells[row, properties.Count() - 1].Style.Font.Bold = true;
                    //worksheet.Cells[row, properties.Count() - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //// function
                    //worksheet.Cells[row, properties.Count()].Formula = "R" + (row - 1) + "*" + item.BillingTaxRate;
                    //worksheet.Cells[row, properties.Count()].Style.Numberformat.Format = "$#,##0.00";
                    //row++;

                    //worksheet.Cells[row, properties.Count() - 1].Value = "Total:";
                    //worksheet.Cells[row, properties.Count() - 1].Style.Font.Bold = true;
                    //worksheet.Cells[row, properties.Count() - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //// function
                    //worksheet.Cells[row, properties.Count()].Formula = "R" + (row - 1) + "+ R" + (row - 2);
                    //worksheet.Cells[row, properties.Count()].Style.Numberformat.Format = "$#,##0.00";
                    //row++;

                    // Next row
                    //row++;



                    // add logo to row 1
                    // *****************************
                    // *** Put it at the end to avoid resizing ***
                    //Image logo = GetLogo();
                    //var picture = worksheet.Drawings.AddPicture("Logo", logo);
                    //picture.SetSize(188, 68);
                    // *** end of logo *** 

                }

                xlPackage.Save();

            }
            return fileName;
        }
        
        #endregion


        #region Export CompanyBillingRate

        public string ExportCompanyBillingRate(Stream stream, IList<CompanyBillingRate> companyBillingRates)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            string fileName = "CompanyBillingRate.xlsx";

            // invoicing enabled?
            var invoicingEnabled = _featureService.IsFeatureEnabled("Admin", "Invoicing");

            //Create Headers and format them
            var properties = new List<string>
                {
                    "Company",
                    "Location",
                    "Rate Code",
                    "Position",
                    "Shift",
                };
            // column width
            var columnWidth = new List<int>
                {
                    15,
                    12,
                    13,
                    13,
                    5
                };
            if (invoicingEnabled)
            {
                properties.Add("Reg. Billing Rate");
                columnWidth.Add(11);
            }
            properties.Add("Reg. Pay Rate");
            columnWidth.Add(9);
            if (invoicingEnabled)
            {
                properties.Add("OT Billing Rate");
                columnWidth.Add(11);
            }
            properties.Add("OT Pay Rate");
            columnWidth.Add(9);
            if (invoicingEnabled)
            {
                properties.Add("Billing Tax Rate");
                columnWidth.Add(9);
            }
            properties.AddRange(new List<string> { "Weekly Hours", "Effective Date", "Deactivated Date" });
            columnWidth.AddRange(new List<int> { 9, 16, 16 });
            var maxCol = properties.Count;

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Company Billing Rate");
               
                // Printer settings
                this.SetPrinterToLegalSize(worksheet);

                // initial row
                int row = 1;

                // Logo and title
                // *****************************
                worksheet.Row(1).Height = 55;
                worksheet.Cells[row, 1, row, maxCol].Merge = true;
                worksheet.Cells[row, 1, row, maxCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[row, 1, row, maxCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, 1].Value = "Company Billing Rates";
                worksheet.Cells[row, 1].Style.Font.Size = 18;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DarkGreen);
                worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

              ////  this.SetWorkSheetTitle(worksheet, " Company Billing Rates", "L1", row);

                row++;

                for (int i = 0; i < maxCol; i++)
                {
                    worksheet.Cells[row, i + 1].Value = properties[i];
                    worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                    worksheet.Cells[row, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[row, i + 1].Style.WrapText = true;
                    worksheet.Column(i + 1).Width = columnWidth[i];
                    if (i >= 4) worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                worksheet.Row(row).Height = 33;
                worksheet.Row(row).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // next row
                row++;

                foreach (var item in companyBillingRates)
                {  
                    int col = 1;
                    //A
                    worksheet.Cells[row, col].Value = _companyService.GetCompanyById(item.CompanyId).CompanyName;
                    worksheet.Cells[row, col].Style.WrapText = true;
                    col++;
                    //B
                    worksheet.Cells[row, col].Value = _companyDivisionService.GetCompanyLocationById(item.CompanyLocationId).LocationName;
                    worksheet.Cells[row, col].Style.WrapText = true;
                    col++;
                    //C
                    worksheet.Cells[row, col].Value = item.RateCode;
                    worksheet.Cells[row, col].Style.WrapText = true;
                    col++;
                    //D
                    worksheet.Cells[row, col].Value = item.Position.Name;
                    worksheet.Cells[row, col].Style.WrapText = true;
                    col++;
                    //E
                    worksheet.Cells[row, col].Value = item.ShiftCode;
                    col++;
                    
                    if (invoicingEnabled)
                    {
                        worksheet.Cells[row, col].Value = item.RegularBillingRate;
                        worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        col++;
                    }

                    worksheet.Cells[row, col].Value = item.RegularPayRate;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                    col++;

                    if (invoicingEnabled)
                    {
                        worksheet.Cells[row, col].Value = item.OvertimeBillingRate;
                        worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";
                        col++;
                    }

                    worksheet.Cells[row, col].Value = item.OvertimePayRate;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "$#,##0.00";    
                    col++;

                    if (invoicingEnabled)
                    {
                        worksheet.Cells[row, col].Value = item.BillingTaxRate;
                        worksheet.Cells[row, col].Style.Numberformat.Format = "0.00%";
                        col++;
                    }

                    worksheet.Cells[row, col].Value = item.WeeklyWorkHours;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0.00"; 
                    col++;

                    worksheet.Cells[row, col].Value = item.EffectiveDate;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "yyyy-mm-dd hh:mm"; 
                    col++;

                    worksheet.Cells[row, col].Value = item.DeactivatedDate;
                    worksheet.Cells[row, col].Style.Numberformat.Format = "yyyy-mm-dd hh:mm"; 
                    col++;

                    // row height
                    worksheet.Row(row).Height = 30;

                    // Next row
                    row++;
                }

                // border
                // *****************************
                worksheet.Cells[1, 1, row, maxCol].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);
                row++;

                // add logo to row 1
                // *****************************
                // *** Put it at the end to avoid resizing ***
                //Image logo = GetLogo();
                //var picture = worksheet.Drawings.AddPicture("Logo", logo);
                //picture.SetSize(188, 68);
                // *** end of logo *** 
                
                xlPackage.Save();
            }
            return fileName;

        }

        #endregion


        #region Export CompanyCandidate

        public string ExportCompanyCandidateToXlsx(Stream stream, int companyId, int vendorId, IList<CompanyCandidate> companyCandidates)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (companyId == 0) throw new ArgumentNullException("companyId");

            if (companyCandidates == null) throw new ArgumentNullException("companyCandidates");

            var company = _companyService.GetCompanyById(companyId);
            if (company == null) return null;

            var vendor = _franchiseService.GetFranchiseById(vendorId);
            if (vendor == null) return null;

            var refDate = DateTime.Today;
            string fileName = string.Empty;

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Employee List");

                // Printer settings
                this.SetPrinterToLetterSize(worksheet, TopMargin: 1.032M / 2.54M, RightMargin: 1.032M / 2.54M, BottomMargin: 1.032M / 2.54M, LeftMargin: 1.032M / 2.54M);
                worksheet.PrinterSettings.FitToHeight = 0;
                worksheet.PrinterSettings.FitToWidth = 1;
                worksheet.PrinterSettings.VerticalCentered = false;
                worksheet.PrinterSettings.HorizontalCentered = true;

                // initial row, and column
                int row = 1, col = 1;

                // Logo and title
                // *****************************
                this.SetWorkSheetTitle(worksheet, "Employee List - " + vendor.FranchiseName, "S1", row);

                // next row
                row++;

                // Client information
                // *****************************
                worksheet.Row(row).Height = 25;
                worksheet.Cells["A2:U2"].Merge = true;
                worksheet.Cells["A2:U2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:U2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[row, col].Value = "Client Name : " + company.CompanyName;
                worksheet.Cells[row, col].Style.Font.Size = 12;
                worksheet.Cells[row, col].Style.Font.Bold = true;
                worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.DimGray);
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                // next row
                row++;

                // Date
                // *****************************
                worksheet.Row(row).Height = 30;
                worksheet.Cells["A3:U3"].Merge = true;
                worksheet.Cells["A3:U3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:U3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:U3"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                worksheet.Cells[row, col].Value = refDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, col].Style.Font.Size = 13;
                worksheet.Cells[row, col].Style.Font.Bold = true;
                worksheet.Cells[row, col].Style.Font.Color.SetColor(Color.DarkOrange);
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                // file name
                string companyName = Regex.Replace(company.CompanyName.Trim(), @"[^a-zA-Z0-9]", "_").Trim('_');
                string fileNameString = Regex.Replace(companyName, @"[_]{2,}", @"_");
                fileName = string.Format("{0}_{1}_{2}{3}", "Employee_List", refDate.ToString("yyyy-MMM-dd"), fileNameString, ".xlsx");

                // Headers
                row++;
                worksheet.Row(row).Height = 22;
                worksheet.Cells["A4:U4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:U4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A4:U4"].Style.Font.Bold = true;
                worksheet.Cells["A4:U4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:U4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));

                worksheet.Cells["A4:H4"].Merge = true;
                worksheet.Cells["A4:H4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                worksheet.Cells[row, col].Value = "Persoanl Information";

                col += 6;
                worksheet.Cells["I4:K4"].Merge = true;
                worksheet.Cells["I4:K4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                worksheet.Cells[row, col].Value = "Work Preference";

                col += 3;
                worksheet.Cells["L4:P4"].Merge = true;
                worksheet.Cells["L4:P4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                worksheet.Cells[row, col].Value = "Status";

                col += 5;
                worksheet.Cells["Q4:U4"].Merge = true;
                worksheet.Cells["Q4:U4"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                worksheet.Cells[row, col].Value = "Latest Job";

                row++;
                // Headers
                // *****************************
                var properties = new Column[]
                    {
                        new Column(){ Name="Id",//A
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Name", // B
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Gender",//C
                                      Width= 6,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Home Phone",//D
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Mobile Phone",//E
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Trans",//F
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Intersection1",//G
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Intersection2",//H
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name= "Location",//I
                                      Width= 14,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Position",//J
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Shift",//K
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Start Date",//L
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="End Date",//M
                                      Width= 12,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Status",//N
                                      Width= 9,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Reason for Leave",//O
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Note",//P
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Location",//Q
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Position",//R
                                      Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Job Title",//S
                                      Width= 25,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Shift Time",//T
                                      Width= 13,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Supervisor",//U
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left}
                    };

                // fromat column header
                this.SetWorkSheetColumns(worksheet, properties, 0, row);
                
                worksheet.View.FreezePanes(row + 1, 2);

                // Content
                foreach (var c in companyCandidates)
                {
                    row++;
                    col = 1;
                    //worksheet.Row(row).Height = 22;

                    var isActive = !c.EndDate.HasValue || c.EndDate >= refDate;

                    worksheet.Cells[row, col].Value = c.CandidateId;
                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[row, col].Style.Indent = 1;
                    
                    col++;

                    AssignCellValueAndAlignment(worksheet.Cells[row, col], String.Concat(c.Candidate.LastName , ", " , c.Candidate.FirstName), ExcelHorizontalAlignment.Left, ref col);

                    worksheet.Cells[row, col].Value = c.Candidate.Gender.GenderName != "Not specified" ? c.Candidate.Gender.GenderName.Substring(0, 1) : "";
                    col++;

                    AssignCellValueAndAlignment(worksheet.Cells[row, col], CommonHelper.ToCanadianPhone(c.Candidate.HomePhone), ExcelHorizontalAlignment.Left, ref col);

                    AssignCellValueAndAlignment(worksheet.Cells[row, col], CommonHelper.ToCanadianPhone(c.Candidate.MobilePhone), ExcelHorizontalAlignment.Left, ref col);

                    worksheet.Cells[row, col].Value = c.Candidate.TransportationId==null?"":c.Candidate.Transportation.TransportationName;
                    col++;
                    worksheet.Cells[row, col].Value = c.Candidate.MajorIntersection1;
                    col++;
                    worksheet.Cells[row, col].Value = c.Candidate.MajorIntersection2;
                    col++;

                    worksheet.Cells[row, col].Value = c.Candidate.PreferredWorkLocation;
                    col++;
                    worksheet.Cells[row, col].Value = c.Position;
                    col++;
                    worksheet.Cells[row, col].Value = c.Candidate.ShiftId==null?"":c.Candidate.Shift.ShiftName;

                    col++;
                    worksheet.Cells[row, col].Value = c.StartDate.ToString("yyyy-MM-dd");
                    col++;
                    worksheet.Cells[row, col].Value = c.EndDate.HasValue ? c.EndDate.Value.ToString("yyyy-MM-dd") : "";
                    col++;
                    worksheet.Cells[row, col].Value = isActive ? "Active" : "Inactive";
                    col++;
                    worksheet.Cells[row, col].Value = c.ReasonForLeave;
                    col++;
                    worksheet.Cells[row, col].Value = c.Note;

                    var cjo = _candidateJobOrderService.GetCandidateJobOrderByCandidateIdAsQueryable(c.CandidateId)
                              .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed &&
                                          x.JobOrder.CompanyId == companyId && x.JobOrder.FranchiseId == vendorId &&
                                          x.StartDate <= refDate)
                              .OrderByDescending(x => x.StartDate).FirstOrDefault();

                    if (cjo != null)
                    {
                        col++;
                        var location = _companyDivisionService.GetCompanyLocationById(cjo.JobOrder.CompanyLocationId);
                        worksheet.Cells[row, col].Value = location==null?"":location.LocationName;
                        col++;
                        worksheet.Cells[row, col].Value = cjo.JobOrder.BillingRateCode.Split('/')[0];
                        col++;
                        worksheet.Cells[row, col].Value = cjo.JobOrder.JobTitle;
                        worksheet.Cells[row, col].Style.WrapText = true;
                        col++;
                        worksheet.Cells[row, col].Value = String.Concat(cjo.JobOrder.StartTime.ToString("HH:mm"), " - " , cjo.JobOrder.EndTime.ToString("HH:mm"));
                        col++;
                        var supervisor = _companyContactService.GetCompanyContactById(cjo.JobOrder.CompanyContactId);
                        worksheet.Cells[row, col].Value = supervisor == null ? "" : supervisor.FirstName + " " + supervisor.LastName;
                    }
                }

                // border
                // *****************************
                worksheet.Cells["A1:U" + row.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells["A4:U" + row.ToString()].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:U" + row.ToString()].Style.Border.Bottom.Color.SetColor(Color.Black);
                worksheet.Cells["A4:U" + row.ToString()].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:U" + row.ToString()].Style.Border.Left.Color.SetColor(Color.Black);
                worksheet.Cells["A4:U" + row.ToString()].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:U" + row.ToString()].Style.Border.Right.Color.SetColor(Color.Black);
                worksheet.Cells["A4:U" + row.ToString()].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:U" + row.ToString()].Style.Border.Top.Color.SetColor(Color.Black);

                //// add logo to row 1
                //// *****************************
                //// *** Put it at the end to avoid resizing ***
                //Image logo = GetLogo();
                //var picture = worksheet.Drawings.AddPicture("Logo", logo);
                //picture.SetSize(188, 68);
                //// *** end of logo *** 

                xlPackage.Save();
            }

            return fileName;
        }

        #endregion


        #region Export Client Daily Attendance List
        public string ExportDailyAttendanceListForClient(Stream stream, IEnumerable<DailyAttendanceList> attendanceList, DateTime refDate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
        
            string fileName = "DailyAttendanceList.xlsx";  

            // Let's begin
            using (var xlPackage = new ExcelPackage(stream))
            {
                    // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Daily Attendance List");

                    // Printer settings
                    this.SetPrinterToLegalSize(worksheet);

                    // initial row
                    int row = 1;

                    // column headers
                    var properties = new string[]
                    {
                        "Vendor",
                        "Employee name", 
                        "Employee #",
                        "Is New",
                        "Total Hours Worked",
                        "Location",
                        "Department",
                        "Job Order ",
                        "Shift Start Time",
                        "Shift End Time", 
                        "Status"
                    };

                    // column width
                    var columnWidth = new int[]
                    {
                        30,
                        20,
                        15,
                        15,
                        15,
                        20,
                        20,
                        30,
                        10,
                        10,
                        15,
                    };

                    // show or hide vendor
                    var displayVendor = _commonSettings.DisplayVendor;
                    var firstCol = displayVendor ? 0 : 1;

                    var sheetWidth = properties.Length - firstCol;
                    var lastCol = ((char)('A' + sheetWidth - 1)).ToString();
                    var sheetRange = "A1:" + lastCol;
                    var firstRowRange = sheetRange + "1";
                   
                    // Logo and title
                    // *****************************
                    //worksheet.Row(1).Height = 55;
                    //worksheet.Cells[firstRowRange].Merge = true;
                    //worksheet.Cells[firstRowRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //worksheet.Cells[firstRowRange].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //worksheet.Cells[row, 1].Value = "Daily Attendance List - " + refDate.ToString("MM/dd/yyyy");
                    //worksheet.Cells[row, 1].Style.Font.Size = 18;
                    //worksheet.Cells[row, 1].Style.Font.Bold = true;
                    //worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DarkGreen);
                    //worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                    this.SetWorkSheetTitle(worksheet,  "Daily Attendance List - " + refDate.ToString("MM/dd/yyyy"), lastCol+"1", row);

                    // next row
                    row++;                 
                   
                  
                    // fromat column header
                    for (int i = firstCol; i < properties.Length; i++)
                    {
                        worksheet.Cells[row, i + 1 - firstCol].Value = properties[i];
                        worksheet.Cells[row, i + 1 - firstCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, i + 1 - firstCol].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
                        worksheet.Cells[row, i + 1 - firstCol].Style.Font.Bold = true;
                        worksheet.Cells[row, i + 1 - firstCol].Style.WrapText = true;

                        worksheet.Column(i + 1 - firstCol).Width = columnWidth[i];
                     
                    }

                    worksheet.Row(row).Height = 22;
                    worksheet.Row(row).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // next row
                    row++;

                    var firstDataRow = row;
                    var firstDataRowStr = firstDataRow.ToString();

                    foreach (var attendance in attendanceList)
                    {
                        int col = 1;
                        
                        if (displayVendor)
                        {
                            //Vendor
                            worksheet.Cells[row, col].Value = attendance.VendorName;
                            worksheet.Cells[row, col].Style.WrapText = true;
                            col++;
                        }

                        //A
                        worksheet.Cells[row, col].Value = String.Concat(attendance.EmployeeLastName, ", ", attendance.EmployeeFirstName);
                        worksheet.Cells[row, col].Style.WrapText = true;
                        col++;
                        //B
                        worksheet.Cells[row, col].Value = attendance.EmployeeId;
                        col++;
                        //Is New
                        worksheet.Cells[row, col].Value = attendance.TotalHoursWorked>0?"No":"Yes";
                        col++;
                        //total hours
                        worksheet.Cells[row, col].Value = attendance.TotalHoursWorked;
                        col++;
                        //C
                        worksheet.Cells[row, col].Value = attendance.Location;
                        worksheet.Cells[row, col].Style.WrapText = true;
                        col++;

                        //D
                        worksheet.Cells[row, col].Value = attendance.Department;
                        worksheet.Cells[row, col].Style.WrapText = true;
                        col++;

                        //E
                        worksheet.Cells[row, col].Value = attendance.JobTitleAndId;
                        worksheet.Cells[row, col].Style.WrapText = true;
                        col++;
                        //F
                        worksheet.Cells[row, col].Value = attendance.ShiftStartTime.ToShortTimeString();
                        col++;
                        //G
                        worksheet.Cells[row, col].Value = attendance.ShiftEndTime.ToShortTimeString();
                        col++;
                        //H
                        worksheet.Cells[row, col].Value = attendance.Status;
                        col++;
                        
                        // row height
                        worksheet.Row(row).Height = 30;

                        // Next row
                        row++;
                    }

                    // border
                    // *****************************
                    worksheet.Cells[sheetRange + row.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);
              
                xlPackage.Save();

            }
            return fileName;
        }
        #endregion


        #region Direct Placement Invoice

        public void DirectPlacementInvoiceXlsx(Stream stream, IList<CandidateDirectHireStatusHistory> placements)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Direct Placement Invoice");
                // initial row
                int row = 1;

                // Logo and title
                // *****************************
                this.SetWorkSheetTitle(worksheet, "Invoice for Direct Hire", "C1", row);


                // next row
                row++;

                // Client information
                // *****************************
                worksheet.Row(row).Height = 25;
                worksheet.Cells["A2:C2"].Merge = true;
                worksheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //worksheet.Cells["A2:N2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Gray);
                string clientInfo = String.Concat("Invoice Date: ",DateTime.Today.ToShortDateString());
                worksheet.Cells[row, 1].Value = clientInfo;
                worksheet.Cells[row, 1].Style.Font.Size = 12;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.Font.Color.SetColor(Color.DimGray);
                worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(245, 245, 245));

                // next row
                row++;

              


                // Headers
                // *****************************
                var properties = new Column[]
                    {
                        new Column(){ Name="Service",//A
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Description", // B
                                      Width= 60,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Amount",//C
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Right}
                    };

                // fromat column header
                this.SetWorkSheetColumns(worksheet, properties, 0, row);
                row++;
                // same job order    
                var jobOrder = _jobOrderService.GetJobOrderById(placements.FirstOrDefault().JobOrderId);
                var state = _companyDivisionService.GetCompanyLocationById(jobOrder.CompanyLocationId).StateProvince.Abbreviation;

                decimal totalAmount = 0m;
                foreach (var p in placements)
                {
                    int col = 1;

                    worksheet.Cells[row, col].Value = "Direct Placement";
                    col++;

                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Value = String.Format("{0} {1}", p.Candidate.FirstName, p.Candidate.LastName);
                    col++;


                    //var rate = _directHireStatusHistoryService.GetDirectHireJobOrderFeeAmount(jobOrder, p.Salary);

                    row++;
                    col = 1;

                    col++;
                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Value = String.Format("    - Company: {0}", p.JobOrder.Company.CompanyName);

                    row++;
                    col = 1;

                    col++;
                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Value = String.Format("    - Position: {0}", p.JobOrder.JobTitle);

                    row++;
                    col = 1;

                    col++;
                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Value = String.Format("    - Hire date: {0}", p.HiredDate.Value.ToShortDateString());


                    row++;
                    col = 1;

                    col++;

                    worksheet.Cells[row, col].Style.WrapText = true;
                    var formula = jobOrder.FeePercent.HasValue && jobOrder.FeePercent > 0 ?
                                  String.Format("    - Bill at {0}% of the annual salary", jobOrder.FeePercent) :
                                  "    - Bill at a fixed amount ";
                    worksheet.Cells[row, col].Value = formula;
                    col++;

                    var amount = jobOrder.FeePercent.HasValue && jobOrder.FeePercent > 0 ?
                           p.Salary*jobOrder.FeePercent/100m :
                            jobOrder.FeeAmount;
                    totalAmount += amount.HasValue ? amount.Value : 0;
                    worksheet.Cells[row, col].Value = amount;
                    col++;

                    row++;
                    row++;
                }

                worksheet.Cells[row, 1].Value = "";
                worksheet.Cells[row, 2].Style.WrapText = true;
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                var taxRate = 0.13m;

                worksheet.Cells[row, 2].Value = String.Format("HST ({0}) [{1}]", state,taxRate.ToString("p2"));

                // TODO: state/provice HST (Cannot get from BillingRateCode!!!)
                worksheet.Cells[row, 3].Style.Font.Bold = true;
                worksheet.Cells[row, 3].Value = totalAmount*taxRate;

                row++;
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = "Invoice Amount";
                worksheet.Cells[row, 3].Style.Font.Bold = true;
                worksheet.Cells[row, 3].Value =totalAmount*(1+taxRate);
                // add logo to row 1
                // *****************************
                // *** Put it at the end to avoid resizing ***
                Image logo = GetLogo(jobOrder.FranchiseId);
                if (logo != null)
                {
                    var logopicture = worksheet.Drawings.AddPicture("Logo", logo);
                    logopicture.SetSize(188, 68);
                }
                xlPackage.Save();
            }
        }

        #endregion


        #region Get OneWeekFollowUp excel
        public string GenerateOneWeekFollowUpExcel(Stream stream, List<OneWeekFollowUpReportData> data)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string fileName = "OneWeekFollowUpReminder.xlsx";

            // Headers
            // *****************************
            var properties = new Column[]
                    {
                        new Column(){ Name="Company",//A
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Location", // B
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="JobOrder",//C
                                      Width= 10,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Job Title",//D
                                      Width= 25,  Alignment= ExcelHorizontalAlignment.Left},

                        new Column(){ Name="Employee Id",//E
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Employee Name",//F
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name= "Email",//G
                                      Width= 20,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Home Phone",//H
                                      Width= 16,  Alignment= ExcelHorizontalAlignment.Right},

                        new Column(){ Name="Mobile Phone",//I
                                      Width= 16,  Alignment= ExcelHorizontalAlignment.Right}
                    };

           

            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("One Week Follow Up");

                // fromat column header
                this.SetWorkSheetColumns(worksheet, properties, 0, 1);

                int row = 2;
                foreach (var p in data)
                {
                    int col = 1;
                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Value = p.CompanyName;
                    col++;

                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Value = p.LocationName;
                    col++;

                    worksheet.Cells[row, col].Value = p.JobOrderId;
                    col++;

                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Value = p.JobTitle;
                    col++;

                    worksheet.Cells[row, col].Value = p.EmployeeId;
                    col++;

                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Value = p.EmployeeName;
                    col++;

                    worksheet.Cells[row, col].Style.WrapText = true;
                    worksheet.Cells[row, col].Value = p.Email;
                    col++;

                    worksheet.Cells[row, col].Value = p.HomePhone;
                    col++;

                    worksheet.Cells[row, col].Value = p.MobilePhone;
                    col++;

                    row++;
                }
                xlPackage.Save();
            }
            return fileName;
        }
        #endregion


        #region Export Employee Total Hours

        public string ExportEmployeeTotalHoursToXlsx(Stream stream, IEnumerable<EmployeeTotalHours> totalHours)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string fileName = "EmployeeTotalHours.xlsx";

            using (var xlPackage = new ExcelPackage(stream))
            {
                string worksheetName = "Employees";
                var worksheet = xlPackage.Workbook.Worksheets.Add(worksheetName);

                // Printer settings
                this.SetPrinterToLegalSize(worksheet, RightMargin: 1.4M / 2.54M, LeftMargin: 1.4M / 2.54M);

                // Headers
                var properties = new Column[]
                {
                    new Column(){ Name="No.",
                                    Width= 5,  Alignment= ExcelHorizontalAlignment.Left},

                    new Column(){ Name="Employee Id",
                                    Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                    new Column(){ Name="First Name",
                                    Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                    new Column(){ Name="Last Name",
                                    Width= 15,  Alignment= ExcelHorizontalAlignment.Left},

                    new Column(){ Name="Email",
                                    Width= 25,  Alignment= ExcelHorizontalAlignment.Right},

                    new Column(){ Name="Home Phone",
                                    Width= 15,  Alignment= ExcelHorizontalAlignment.Right},

                    new Column(){ Name= "Mobile Phone",
                                    Width= 15,  Alignment= ExcelHorizontalAlignment.Right},

                    new Column(){ Name="Total Hours",
                                    Width= 15,  Alignment= ExcelHorizontalAlignment.Right}
                };

                // fromat column header
                this.SetWorkSheetColumns(worksheet, properties, 0, 1);

                // initial row
                int row = 1;
                int col = 1;

                worksheet.Row(row).Height = 30;
                worksheet.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                foreach (var item in totalHours.OrderByDescending(x => x.TotalHours).ThenBy(x => x.CandidateId))
                {
                    row++;
                    col = 1;
                    worksheet.Row(row).Height = 18;
                    worksheet.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    worksheet.Cells[row, col].Value = row - 1;
                    col++;

                    worksheet.Cells[row, col].Value = item.CandidateId;
                    col++;

                    worksheet.Cells[row, col].Value = item.FirstName;
                    col++;

                    worksheet.Cells[row, col].Value = item.LastName;
                    col++;

                    worksheet.Cells[row, col].Value = item.Email;
                    col++;

                    worksheet.Cells[row, col].Value = item.HomePhone;
                    col++;

                    worksheet.Cells[row, col].Value = item.MobilePhone;
                    col++;

                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[row, col].Value = item.TotalHours.ToString("0.00");
                }

                // border
                worksheet.Cells[1, 1, row, col].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);

                xlPackage.Save();
            }

            return fileName;
        }

        #endregion


        #region Employee Seneiority

        public string ExportEmployeeSeniorityReportToXlsx(Stream stream, IEnumerable<EmployeeSeniority> employees, decimal years)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string fileName = "EmployeeSeniorityReport.xlsx";

            using (var xlPackage = new ExcelPackage(stream))
            {
                
                var worksheet = xlPackage.Workbook.Worksheets.Add(String.Format("{0} yrs", years));
                this.SetPrinterToLegalSize(worksheet);

                var properties = new Column[]
                {
                    new Column(){ Name = "Id", Width = 10,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "First Name", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Last Name", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "First Hire Date", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Last Hire Date", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Termination Date", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "ROE Date", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                };

                int row = 1;
                this.SetWorkSheetColumns(worksheet, properties, 0, row);

                foreach (var e in employees)
                {
                    row++;

                    int col = 1;
                    worksheet.Cells[row, col].Value = e.CandidateId;

                    col++;
                    worksheet.Cells[row, col].Value = e.FirstName;

                    col++;
                    worksheet.Cells[row, col].Value = e.LastName;

                    col++;
                    worksheet.Cells[row, col].Value = e.FirstHireDate.HasValue ? e.FirstHireDate.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.LastHireDate.HasValue ? e.LastHireDate.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.TerminationDate.HasValue ? e.TerminationDate.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.ROE_Date.HasValue ? e.ROE_Date.Value.ToShortDateString() : string.Empty;
                }

                worksheet.Cells["A1:G" + row.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);

                xlPackage.Save();
            }

            return fileName;
        }


        public string ExportEmployeeSeniorityReportToXlsx(Stream stream, IEnumerable<EmployeeSeniority> employees, string dateField)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string fileName = "EmployeeSeniorityReport.xlsx";
            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add(dateField);
                this.SetPrinterToLegalSize(worksheet);

                var properties = new Column[]
                {
                    new Column(){ Name = "Id", Width = 10,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "First Name", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Last Name", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Home Phone", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Mobile Phone", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Email", Width = 25,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Address Line1", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Address Line2", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "City", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Province", Width = 10,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Country", Width = 10,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Post Code", Width = 10,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "First Hire Date", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Last Hire Date", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "First Day Worked", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Last Day Worked", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Company", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Termination", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "ROE Issue Date", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "ROE Reason", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "DNR Date", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "DNR Reason", Width = 20,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "First Placement", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Last Placement", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                };

                int row = 1;
                this.SetWorkSheetColumns(worksheet, properties, 0, row);

                var companyNames = _companyService.GetAllCompaniesAsQueryable(null, true, true).Select(x => new
                    {
                        CompanyId = x.Id,
                        CompanyName = x.CompanyName
                    }).ToList();

                foreach (var e in employees)
                {
                    row++;

                    int col = 1;
                    worksheet.Cells[row, col].Value = e.CandidateId;

                    col++;
                    worksheet.Cells[row, col].Value = e.FirstName;

                    col++;
                    worksheet.Cells[row, col].Value = e.LastName;

                    col++;
                    worksheet.Cells[row, col].Value = CommonHelper.ToCanadianPhone(e.HomePhone);

                    col++;
                    worksheet.Cells[row, col].Value = CommonHelper.ToCanadianPhone(e.MobilePhone);

                    col++;
                    worksheet.Cells[row, col].Value = e.Email;

                    col++;
                    worksheet.Cells[row, col].Value = e.AddressLine1;

                    col++;
                    worksheet.Cells[row, col].Value = e.AddressLine2;

                    col++;
                    worksheet.Cells[row, col].Value = e.City;

                    col++;
                    worksheet.Cells[row, col].Value = e.Province;

                    col++;
                    worksheet.Cells[row, col].Value = e.Country;

                    col++;
                    worksheet.Cells[row, col].Value = e.PostalCode;

                    col++;
                    worksheet.Cells[row, col].Value = e.FirstHireDate.HasValue ? e.FirstHireDate.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.LastHireDate.HasValue ? e.LastHireDate.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.FirstDayWorked.HasValue ? e.FirstDayWorked.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.LastDayWorked.HasValue ? e.LastDayWorked.Value.ToShortDateString() : string.Empty;

                    col++;
                    var client = companyNames.FirstOrDefault(x => x.CompanyId == e.LastClientWorked);
                    worksheet.Cells[row, col].Value = client != null ? client.CompanyName : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.TerminationDate.HasValue ? e.TerminationDate.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.ROE_Date.HasValue ? e.ROE_Date.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.ROE_Reason;

                    col++;
                    worksheet.Cells[row, col].Value = e.DNR_Date.HasValue ? e.DNR_Date.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.DNR_Reason;

                    col++;
                    worksheet.Cells[row, col].Value = e.FirstPlacement.HasValue ? e.FirstPlacement.Value.ToShortDateString() : string.Empty;

                    col++;
                    worksheet.Cells[row, col].Value = e.LastPlacement.HasValue ? e.LastPlacement.Value.ToShortDateString() : string.Empty;
                }

                worksheet.Cells["A1:x" + row.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);

                xlPackage.Save();
            }

            return fileName;
        }

        #endregion


        #region Employee Availability

        public string ExportAvailableToXlsx(Stream stream, Guid companyGuid, DateTime refDate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var fileName = "AvailableCandidates.xlsx";

            var companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var pool = _companyCandidateService.GetCompanyCandidatesByAccountAndCompany(_workContext.CurrentAccount, companyId, refDate);
            var availability = _availabilityService.GetAllCandidateAvailability(refDate, refDate);
            var jobs = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(companyId, refDate);
            var candidates = from p in pool
                             join a in availability on p.CandidateId equals a.CandidateId
                             from j in jobs.Where(j => j.CandidateId == p.CandidateId).DefaultIfEmpty()
                             select new
                             {
                                 CandidateId = p.CandidateId,
                                 EmployeeId = p.Candidate.EmployeeId,
                                 FirstName = p.Candidate.FirstName,
                                 LastName = p.Candidate.LastName,
                                 HomePhone = p.Candidate.HomePhone,
                                 MobilePhone = p.Candidate.MobilePhone,
                                 Email = p.Candidate.Email,
                                 Available = a.Shift.ShiftName,
                                 JobOrder = j != null ? j.JobOrder.Id.ToString() : null,
                                 Company = j != null && j.JobOrder.Company != null ? j.JobOrder.Company.CompanyName : null,
                                 Shift = j != null && j.JobOrder.Shift != null ? j.JobOrder.Shift.ShiftName : null,
                             };

            using (var xlPackage = new ExcelPackage(stream))
            {

                var worksheet = xlPackage.Workbook.Worksheets.Add(refDate.ToString("yyyy-MM-dd"));
                this.SetPrinterToLegalSize(worksheet);

                var properties = new Column[]
                {
                    new Column(){ Name = "Id", Width = 10,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Employee #", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "First Name", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Last Name", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Home Phone", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Mobile Phone", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Email", Width = 30,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Available", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Job Order", Width = 10,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Company", Width = 30,  Alignment = ExcelHorizontalAlignment.Left},
                    new Column(){ Name = "Shift", Width = 15,  Alignment = ExcelHorizontalAlignment.Left},
                };

                int row = 1;
                this.SetWorkSheetColumns(worksheet, properties, 0, row);

                foreach (var c in candidates.AsEnumerable())
                {
                    row++;

                    int col = 1;
                    worksheet.Cells[row, col].Value = c.CandidateId.ToString();

                    col++;
                    worksheet.Cells[row, col].Value = c.EmployeeId;

                    col++;
                    worksheet.Cells[row, col].Value = c.FirstName;

                    col++;
                    worksheet.Cells[row, col].Value = c.LastName;

                    col++;
                    worksheet.Cells[row, col].Value = c.HomePhone;

                    col++;
                    worksheet.Cells[row, col].Value = c.MobilePhone;

                    col++;
                    worksheet.Cells[row, col].Value = c.Email;

                    col++;
                    worksheet.Cells[row, col].Value = c.Available;

                    col++;
                    worksheet.Cells[row, col].Value = c.JobOrder;

                    col++;
                    worksheet.Cells[row, col].Value = c.Company;

                    col++;
                    worksheet.Cells[row, col].Value = c.Shift;
                }

                worksheet.Cells["A1:K" + row.ToString()].Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.DarkGray);

                xlPackage.Save();
            }

            return fileName;
        }

        #endregion
    }

}
