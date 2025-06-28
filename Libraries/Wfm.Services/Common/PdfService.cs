using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Wfm.Core;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Features;
using Wfm.Services.Tasks;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Companies;
using System.Web;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Configuration;

namespace Wfm.Services.Common
{
    #region PDFPage

    public class PDFPage : iTextSharp.text.pdf.PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate template;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;

        #region Properties
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _HeaderLeft;
        public string HeaderLeft
        {
            get { return _HeaderLeft; }
            set { _HeaderLeft = value; }
        }

        private string _HeaderRight;
        public string HeaderRight
        {
            get { return _HeaderRight; }
            set { _HeaderRight = value; }
        }

        private Font _HeaderFont;
        public Font HeaderFont
        {
            get { return _HeaderFont; }
            set { _HeaderFont = value; }
        }

        private Font _FooterFont;
        public Font FooterFont
        {
            get { return _FooterFont; }
            set { _FooterFont = value; }
        }
        #endregion

        // we override the onOpenDocument method
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
                throw new WfmException("PDF Document Exception: " + de.Message);
            }
            catch (System.IO.IOException ioe)
            {
                throw new WfmException("PDF IO Exception: " + ioe.Message);
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 3);

            Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(bf, 3);
            cb.SetTextMatrix(pageSize.GetRight(70), pageSize.GetBottom(15));
            cb.ShowText(text);
            cb.EndText();

            cb.AddTemplate(template, pageSize.GetRight(70) + len, pageSize.GetBottom(15));
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            template.BeginText();
            template.SetFontAndSize(bf, 3);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }

    }

    #endregion

    public partial class PdfService : IPdfService
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IFeatureService _featureService;
        private readonly PdfSettings _pdfSettings;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly CommonSettings _commonSettings;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public PdfService(
            IWebHelper webHelper,
            IFeatureService featureService,
            ICompanyDivisionService companyDivisionService,
            PdfSettings pdfSettings,
            CommonSettings commonSettings,
            ISettingService settingService)
        {
            this._webHelper = webHelper;
            this._featureService = featureService;
            this._pdfSettings = pdfSettings;
            _companyDivisionService = companyDivisionService;
            _commonSettings = commonSettings;
            _settingService = settingService;
        }

        #endregion

        #region Utilities

        protected virtual Font GetFont()
        {
            //wfm supports unicode characters
            //wfm uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
            //It was downloaded from http://savannah.gnu.org/projects/freefont
            string fontPath = Path.Combine(_webHelper.MapPath("~/App_Data/Pdf/"), _pdfSettings.FontFileName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new Font(baseFont, 10, Font.NORMAL);
            return font;
        }

        #endregion

        #region Print Candidate

        public void PrintCandidatesToPdf(Stream output, IList<Candidate> candidates)
        {
            Document doc = new Document(new Rectangle(288f, 144f), 10, 10, 10, 10);
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());


            var pdfWriter = PdfWriter.GetInstance(doc, output);

            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Font font = new Font()
            {
                Size = 6
            };

            pdfWriter.PageEvent = new PDFPage()
            {
                HeaderFont = font,
                FooterFont = font,
            };

            doc.Open();

            Paragraph p = new Paragraph("Attendant List - " + DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss"));
            p.Alignment = Element.ALIGN_CENTER;

            doc.Add(p);
            doc.Add(new Paragraph(Environment.NewLine));

            int numOfColumns = 5;
            PdfPTable dataTable = new PdfPTable(numOfColumns);

            dataTable.DefaultCell.Padding = 3;

            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            dataTable.AddCell(new PdfPCell(new Phrase("Last name", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("First name", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Employee id", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Home phone", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Mobile phone", font)));


            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;

            foreach (var candidate in candidates)
            {

                dataTable.AddCell(new PdfPCell(new Phrase(candidate.LastName, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(candidate.FirstName, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(candidate.EmployeeId, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(candidate.HomePhone, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(candidate.MobilePhone, font)));
            }
            doc.Add(dataTable);

            doc.Close();
        }

        #endregion

        #region Print CandidateWorkTime

        public virtual void PrintCandidateWorkTimesToPdf(Stream output, IList<CandidateWorkTime> candidateWorkTime)
        {
            if (output == null)
                throw new ArgumentNullException("stream");

            if (candidateWorkTime == null)
                throw new ArgumentNullException("candidateWorkTime");

            var signatureCompanies = _settingService.GetSettingByKey<string>("SignatureCompanies").Split(',')?.Select(Int32.Parse)?.ToList();

            var isSignature = candidateWorkTime.Any( x => signatureCompanies.Contains(x.CompanyId));

            // Document doc = new Document(new Rectangle(288f, 144f), 10, 10, 10, 10);
            Document doc = new Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

            var pdfWriter = PdfWriter.GetInstance(doc, output);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);
            

            // Add header
            Paragraph p = new Paragraph("Employee Time Sheet Approval");
            p.Alignment = Element.ALIGN_CENTER;
            doc.Add(p);

            var firstWorkTime = candidateWorkTime.FirstOrDefault();
            Paragraph subHeader = new Paragraph(String.Concat( "Week " , firstWorkTime.WeekOfYear , ", " , firstWorkTime.Year));
            subHeader.Alignment = Element.ALIGN_CENTER;
            doc.Add(subHeader);

            doc.Add(new Paragraph(Environment.NewLine));

            int numOfColumns = isSignature ? 14 : 13;

            PdfPTable dataTable = new PdfPTable(numOfColumns);
            dataTable.TotalWidth = 820f;

            float[] widths;
            if (isSignature)
            {
                var witdhNo = 20f;
                var witdhId = 40f;
                var witdhName = 80f;
                var witdhJob = 38f;
                var witdhPosition = 80f;
                var witdhDate = 46f;
                var witdhStatus = 46f;
                var witdhClockIn = 50f;
                var witdhClockOut = 50f;
                var witdhClockHours = 30f;
                var witdhGrossHours = 30f;
                var witdhAdjustment = 30f;
                var witdhNetHours = 30f;
                var witdhSignature = 50f;

                widths = new float[] { witdhNo, witdhId, witdhName, witdhJob, witdhPosition, witdhDate, witdhStatus, witdhClockIn, witdhClockOut, witdhClockHours, witdhGrossHours, witdhAdjustment, witdhNetHours, witdhSignature };
            }
            else
            {
                widths = new float[] { 30f, 50f, 120f, 40f, 120f, 50f, 50f, 60f, 60f, 55f, 55f, 55f, 55f };
            }

            //float[] widths = new float[] { 30f, 50f, 120f, 40f, 120f, 60f, 40f, 60f, 60f, 55f, 55f, 55f, 55f };
            dataTable.LockedWidth = true;
            dataTable.SetWidths(widths);
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            dataTable.AddCell(new PdfPCell(new Phrase("No.", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Id", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Name", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Job Order", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Position", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Date", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Status", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("ClockIn", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("ClockOut", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Clock Hours", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Gross Hours", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Adjustment (Min)", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Net Hours", font)));

            if (isSignature)
            {
                dataTable.AddCell(new PdfPCell(new Phrase("Signature", font)));
            }

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;

            decimal totalClockHrs = 0;
            decimal totalGrossHours = 0;
            decimal totalAdjustmentMints = 0;
            decimal totalNetHours = 0;

            int i = 1;
            foreach (var item in candidateWorkTime)
            {
                dataTable.AddCell(new PdfPCell(new Phrase(i.ToString("00"), font)));

                dataTable.AddCell(new PdfPCell(new Phrase(item.Candidate.EmployeeId, font)));

                dataTable.AddCell(new PdfPCell(new Phrase(String.Concat(item.Candidate.LastName, " ", item.Candidate.FirstName), font)));

                dataTable.AddCell(new PdfPCell(new Phrase(item.JobOrderId.ToString(), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.JobOrder.JobTitle, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.JobStartDateTime.ToShortDateString(), font)));

                //  dataTable.AddCell(item.CandidateWorkTimeStatus.ToString());
                dataTable.AddCell(new PdfPCell(new Phrase(item.CandidateWorkTimeStatus.ToString(), font)));

                if (item.ClockIn != null)
                {
                    dataTable.AddCell(new PdfPCell(new Phrase(item.ClockIn.Value.ToString("yyyy-MM-dd HH:mm:ss"), font)));
                }
                else
                    dataTable.AddCell(new PdfPCell(new Phrase("", font)));

                if (item.ClockOut != null)
                {
                    dataTable.AddCell(new PdfPCell(new Phrase(item.ClockOut.Value.ToString("yyyy-MM-dd HH:mm:ss"), font)));
                }
                else
                    dataTable.AddCell(new PdfPCell(new Phrase("", font)));

                PdfPCell cell1 = new PdfPCell(new Phrase(item.ClockTimeInHours.ToString(), font));
                cell1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                dataTable.AddCell(cell1);

                PdfPCell cell2 = new PdfPCell(new Phrase(item.GrossWorkTimeInHours.ToString(), font));
                cell2.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                dataTable.AddCell(cell2);

                PdfPCell cell3 = new PdfPCell(new Phrase(item.AdjustmentInMinutes.ToString(), font));
                cell3.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                dataTable.AddCell(cell3);

                PdfPCell cell4 = new PdfPCell(new Phrase(item.NetWorkTimeInHours.ToString(), font));
                cell4.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                dataTable.AddCell(cell4);

                if (isSignature)
                {
                    var filePath = Path.Combine(HttpRuntime.AppDomainAppPath, "Signatures", $"{item.CompanyContactId}.png");
                    var cell5 = new PdfPCell(new Phrase("", font));
                    if (!String.IsNullOrEmpty(item.SignatureByName)  && File.Exists(filePath))
                    {
                        cell5 = new PdfPCell(iTextSharp.text.Image.GetInstance(filePath), true);
                    }
                    cell5.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    dataTable.AddCell(cell5);
                }

                totalClockHrs += item.ClockTimeInHours;
                totalGrossHours += item.GrossWorkTimeInHours;
                totalAdjustmentMints += item.AdjustmentInMinutes;
                totalNetHours += item.NetWorkTimeInHours;

                i++;
            }
            dataTable.AddCell(new PdfPCell(new Phrase("Total", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("", font)));
            PdfPCell total1 = new PdfPCell(new Phrase(totalClockHrs.ToString(), font));
            total1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            dataTable.AddCell(total1);
            
            PdfPCell total2 = new PdfPCell(new Phrase(totalGrossHours.ToString(), font));
            total2.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            dataTable.AddCell(total2);

            PdfPCell total3 = new PdfPCell(new Phrase(totalAdjustmentMints.ToString(), font));
            total3.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            dataTable.AddCell(total3);

            PdfPCell total4 = new PdfPCell(new Phrase(totalNetHours.ToString(), font));
            total4.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            dataTable.AddCell(total4);

            doc.Add(dataTable);

            doc.Close();
        }

        #endregion

        #region Print EmployeeTimeChartHistory
        public virtual void PrintEmployeeTimeChartHistoryToPdfForAdmin(Stream stream, IEnumerable<EmployeeTimeChartHistory> timeCharts, bool withRates = false)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (timeCharts == null)
                throw new ArgumentNullException("timeCharts");

            var firstTimeChart = timeCharts.FirstOrDefault();

            var pageSize = PageSize.A4.Rotate();

            var doc = new Document(pageSize);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);
            var pendingFont = GetFont();
            pendingFont.Color = BaseColor.RED;
            var footfont = GetFont();
            footfont.Size = 8;

            // invoicing enabled?
            var invoicingEnabled = _featureService.IsFeatureEnabled("Admin", "Invoicing") && withRates;

            var widths = new List<float> { 120f, 60f, 70f, 70f, 40f, 40f, 40f, 40f, 40f, 40f, 40f, 50f, 50f };
            widths.AddRange(invoicingEnabled ? new List<float> { 40f, 40f, 45f, 50f } : new List<float> { 40f });
            PdfPTable dataTable = new PdfPTable(widths.Count);
            dataTable.TotalWidth = 820f;
            dataTable.LockedWidth = true;
            dataTable.SetWidths(widths.ToArray());
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //dataTable.AddCell(new PdfPCell(new Phrase("No.", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Employee name", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Employee #", font)));
            //dataTable.AddCell(new PdfPCell(new Phrase("Department", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Position", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Shift", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Sun", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Mon", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Tue", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Wed", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Thu", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Fri", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Sat", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Subtotal Hrs", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Total Reg Hrs", font)));
            if (invoicingEnabled)
                dataTable.AddCell(new PdfPCell(new Phrase("Reg. Billing Rate", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("OT Hours", font)));
            if (invoicingEnabled)
            {
                dataTable.AddCell(new PdfPCell(new Phrase("OT Billing Rate", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("Subtotal", font)));
            }

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;

            var sortedTimeCharts = timeCharts.GroupBy(x => new { x.WeekOfYear, x.Year, x.CompanyName, x.BillingTaxRate })
                                            .ToDictionary(x => x.Key, x => x.ToList());
            //var sortedTimeCharts1 = timeCharts.GroupBy(x => new { x.WeekOfYear, x.Year, x.CompanyName,x.BillingTaxRate }); //var query = source.GroupBy(x => new { x.Column1, x.Column2 });

            //var sortedTimeCharts = sortedTimeCharts1.ToDictionary(x => x.Key, x => x.ToList());
            //foreach (var x in sortedTimeCharts.Keys.OrderBy(x => new { x.CompanyName, x.WeekOfYear, x.Year }))
            //{
            //    var xx = sortedTimeCharts[x];
            //}
            foreach (var item in sortedTimeCharts.Keys.OrderBy(x => x.CompanyName).ThenBy(x => x.WeekOfYear).ThenBy(x => x.Year))
            {
                dataTable.DeleteBodyRows();
                Paragraph mainHeader = new Paragraph(item.CompanyName + " Weekly Time Sheets - " + item.WeekOfYear);
                mainHeader.Alignment = Element.ALIGN_CENTER;
                doc.Add(mainHeader);
                string weekStart = DateService.FirstDateOfWeek(item.Year, item.WeekOfYear).ToString("MM/dd/yyyy");
                string weekEnd = DateService.FirstDateOfWeek(item.Year, item.WeekOfYear).AddDays(6).ToString("MM/dd/yyyy");
                //Paragraph subHeader = new Paragraph(item.CompanyName);
                //subHeader.Alignment = Element.ALIGN_LEFT;
                //doc.Add(subHeader);
                Paragraph subHeader1 = new Paragraph("Week of " + weekStart + " - " + weekEnd);
                subHeader1.Alignment = Element.ALIGN_CENTER;
                doc.Add(subHeader1);
                doc.Add(new Paragraph(Environment.NewLine));
                decimal totalHrs = 0;
                decimal totalRegularHours = 0;
                decimal totalOTHours = 0;
                decimal totalCredits = 0;



                //int i = 1;
                foreach (var timechart in sortedTimeCharts[item])
                {
                    decimal subtotal = (timechart.RegularBillingRate * timechart.RegularHours + timechart.OvertimeBillingRate * timechart.OTHours).Value;

                    //dataTable.AddCell(new PdfPCell(new Phrase(i.ToString("00"), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(String.Concat(timechart.EmployeeLastName , ", " , timechart.EmployeeFirstName), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.EmployeeNumber, font)));
                    //dataTable.AddCell(new PdfPCell(new Phrase(timechart.DepartmentName, font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.PositionCode, font)));
                    //dataTable.AddCell(new PdfPCell(new Phrase(billingChart.JobStartDateTime.ToString("HH:mm") + "-" + billingChart.JobEndDateTime.ToString("HH:mm"), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.ShiftCode, font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Sunday.ToString(), !timechart.SundayStatus && timechart.Sunday > 0 ? pendingFont : font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Monday.ToString(), !timechart.MondayStatus && timechart.Monday > 0 ? pendingFont : font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Tuesday.ToString(), !timechart.TuesdayStatus && timechart.Tuesday > 0 ? pendingFont : font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Wednesday.ToString(), !timechart.WednesdayStatus && timechart.Wednesday > 0 ? pendingFont : font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Thursday.ToString(), !timechart.ThursdayStatus && timechart.Thursday > 0 ? pendingFont : font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Friday.ToString(), !timechart.FridayStatus && timechart.Friday > 0 ? pendingFont : font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Saturday.ToString(), !timechart.SaturdayStatus && timechart.Saturday > 0 ? pendingFont : font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.SubTotalHours.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.RegularHours.ToString(), font)));
                    if (invoicingEnabled)
                        dataTable.AddCell(new PdfPCell(new Phrase(timechart.RegularBillingRate.GetValueOrDefault().ToString("C"), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.OTHours.ToString(), font)));
                    if (invoicingEnabled)
                    {
                        dataTable.AddCell(new PdfPCell(new Phrase(timechart.OvertimeBillingRate.GetValueOrDefault().ToString("C"), font)));
                        dataTable.AddCell(new PdfPCell(new Phrase(subtotal.ToString("C"), font)));
                    }
                    if (timechart.RegularHours != null)
                    {
                        totalRegularHours += timechart.RegularHours.Value;
                    }
                    if (timechart.OTHours != null)
                    {
                        totalOTHours += timechart.OTHours.Value;
                    }
                    if (timechart.SubTotalHours != null)
                    {
                        totalHrs += timechart.SubTotalHours.Value;
                    }
                    totalCredits += subtotal;
                    //i++;
                }
                //add a subtotal row 
                dataTable.AddCell(new PdfPCell(new Phrase("Total", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("".ToString(), font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase(totalHrs.ToString(), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(totalRegularHours.ToString(), font)));
                if (invoicingEnabled)
                    dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase(totalOTHours.ToString(), font)));
                if (invoicingEnabled)
                {
                    dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                    dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                }
                doc.Add(dataTable);
                if (invoicingEnabled)
                {
                    //Paragraph pFooter = new Paragraph("Total Regular Hours : " + totalRegularHours + " Total OT Hours: " + totalOTHours);
                    //pFooter.Alignment = Element.ALIGN_RIGHT;
                    //doc.Add(pFooter);
                    Paragraph pFooter1 = new Paragraph("Credit Hours Subtotal:" + totalCredits.ToString("C"));
                    pFooter1.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(pFooter1);
                    Paragraph pFooter2 = new Paragraph("HST(" + item.BillingTaxRate.GetValueOrDefault().ToString("P2") + "):" + (totalCredits * (item.BillingTaxRate.GetValueOrDefault())).ToString("C"));
                    pFooter2.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(pFooter2);
                    Paragraph pFooter3 = new Paragraph("Total:" + (totalCredits * (1 + item.BillingTaxRate.GetValueOrDefault())).ToString("C"));
                    pFooter3.Alignment = Element.ALIGN_RIGHT;
                    doc.Add(pFooter3);
                }
                Paragraph pFooter4 = new Paragraph("*The amount in red is a pending hour, which is not approved yet.");
                pFooter4.Alignment = Element.ALIGN_LEFT;
                pFooter4.Font = footfont;
                doc.Add(pFooter4);
                doc.NewPage();

            }
            doc.Close();
        }
        public virtual void PrintEmployeeTimeChartHistoryToPdfForClient(Stream stream, IEnumerable<EmployeeTimeChartHistory> timeCharts)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (timeCharts == null)
                throw new ArgumentNullException("timeCharts");

            var firstTimeChart = timeCharts.FirstOrDefault();

            var pageSize = PageSize.A4.Rotate();

            var doc = new Document(pageSize);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);

            // Add header
            //Paragraph mainHeader = new Paragraph("Weekly Time Chart");
            //mainHeader.Alignment = Element.ALIGN_CENTER;
            //doc.Add(mainHeader);

            //var payperiod = _payPeriodService.GetPayPeriodById(firstBillingChart.PayPeriodId);
            //Paragraph subHeader = new Paragraph("Week of " + payperiod.StartDate.ToString("MMM dd, yyyy") + " - " + payperiod.EndDate.ToString("MMM dd, yyyy"));
            //subHeader.Alignment = Element.ALIGN_CENTER;
            //doc.Add(subHeader);

            // show or hide vendor
            var displayVendor = _commonSettings.DisplayVendor;
            var firstCol = displayVendor ? 0 : 1;

            // Total columns
            int numOfColumns = 17 - firstCol;

            PdfPTable dataTable = new PdfPTable(numOfColumns);
            dataTable.TotalWidth = 785f;
            float[] origWidths = new float[] { 60f, 60f, 60f, 60f, 80f, 40f, 40f, 40f, 40f, 40f, 40f, 40f, 40f, 50f, 50f, 40f, 60f};
            float[] widths = origWidths.Skip(firstCol).Take(origWidths.Length - firstCol).ToArray();
            dataTable.LockedWidth = true;
            dataTable.SetWidths(widths);
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            if (displayVendor)
                dataTable.AddCell(new PdfPCell(new Phrase("Vendor", font)));
            
            dataTable.AddCell(new PdfPCell(new Phrase("Employee name", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Employee #", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Department", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Position", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Shift", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Sun", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Mon", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Tue", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Wed", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Thu", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Fri", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Sat", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Total Hours", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Total Reg Hrs", font)));
            //dataTable.AddCell(new PdfPCell(new Phrase("Reg. Billing Rate", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("OT Hours", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Approved By", font)));
            //dataTable.AddCell(new PdfPCell(new Phrase("OT Billing Rate", font)));
            //dataTable.AddCell(new PdfPCell(new Phrase("Subtotal", font)));

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;

            var sortedTimeCharts = timeCharts.GroupBy(x => new { x.WeekOfYear, x.Year, x.CompanyName })
                                            .ToDictionary(x => x.Key, x => x.ToList());
            //var sortedTimeCharts1 = timeCharts.GroupBy(x => new { x.WeekOfYear, x.Year, x.CompanyName,x.BillingTaxRate }); //var query = source.GroupBy(x => new { x.Column1, x.Column2 });

            //var sortedTimeCharts = sortedTimeCharts1.ToDictionary(x => x.Key, x => x.ToList());
            //foreach (var x in sortedTimeCharts.Keys.OrderBy(x => new { x.CompanyName, x.WeekOfYear, x.Year }))
            //{
            //    var xx = sortedTimeCharts[x];
            //}
            foreach (var item in sortedTimeCharts.Keys.OrderBy(x => x.CompanyName).ThenBy(x => x.WeekOfYear).ThenBy(x => x.Year))
            {
                dataTable.DeleteBodyRows();
                Paragraph mainHeader = new Paragraph(item.CompanyName + " Employee Time Sheet - " + item.WeekOfYear);
                mainHeader.Alignment = Element.ALIGN_CENTER;
                doc.Add(mainHeader);
                string weekStart = DateService.FirstDateOfWeek(item.Year, item.WeekOfYear).ToString("MM/dd/yyyy");
                string weekEnd = DateService.FirstDateOfWeek(item.Year, item.WeekOfYear).AddDays(6).ToString("MM/dd/yyyy");
                //Paragraph subHeader = new Paragraph(item.CompanyName);
                //subHeader.Alignment = Element.ALIGN_LEFT;
                //doc.Add(subHeader);
                Paragraph subHeader1 = new Paragraph("Week of " + weekStart + " - " + weekEnd);
                subHeader1.Alignment = Element.ALIGN_CENTER;
                doc.Add(subHeader1);
                doc.Add(new Paragraph(Environment.NewLine));
                decimal totalHrs = 0;
                decimal totalRegularHours = 0;
                decimal totalOTHours = 0;
                //decimal totalCredits = 0;

                //int i = 1;
                foreach (var timechart in sortedTimeCharts[item])
                {
                    //decimal subtotal = (timechart.RegularBillingRate * timechart.RegularHours + timechart.OvertimeBillingRate * timechart.OTHours).Value;

                    if (displayVendor)
                        dataTable.AddCell(new PdfPCell(new Phrase(timechart.FranchiseName, font)));

                    dataTable.AddCell(new PdfPCell(new Phrase(String.Concat(timechart.EmployeeLastName , ", " , timechart.EmployeeFirstName), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.EmployeeNumber, font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.DepartmentName, font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.JobTitle, font)));
                    //dataTable.AddCell(new PdfPCell(new Phrase(billingChart.JobStartDateTime.ToString("HH:mm") + "-" + billingChart.JobEndDateTime.ToString("HH:mm"), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.ShiftCode, font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Sunday.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Monday.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Tuesday.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Wednesday.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Thursday.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Friday.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.Saturday.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.SubTotalHours.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.RegularHours.ToString(), font)));
                   // dataTable.AddCell(new PdfPCell(new Phrase(timechart.RegularBillingRate.ToString("C"), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.OTHours.ToString(), font)));
                    dataTable.AddCell(new PdfPCell(new Phrase(timechart.ApprovedBy, font)));
                    //dataTable.AddCell(new PdfPCell(new Phrase(timechart.OvertimeBillingRate.ToString("C"), font)));
                    //dataTable.AddCell(new PdfPCell(new Phrase(subtotal.ToString("C"), font)));
                    if (timechart.RegularHours != null)
                    {
                        totalRegularHours += timechart.RegularHours.Value;
                    }
                    if (timechart.OTHours != null)
                    {
                        totalOTHours += timechart.OTHours.Value;
                    }
                    if (timechart.SubTotalHours != null)
                    {
                        totalHrs += timechart.SubTotalHours.Value;
                    }
                    //totalCredits += subtotal;
                    //i++;
                }
                //add a subtotal row 
                dataTable.AddCell(new PdfPCell(new Phrase("Total", font)));
                
                if (displayVendor)
                    dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("".ToString(), font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase(totalHrs.ToString(), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(totalRegularHours.ToString(), font)));
                //dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                dataTable.AddCell(new PdfPCell(new Phrase(totalOTHours.ToString(), font)));
                dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                //dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                //dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                doc.Add(dataTable);
                //Paragraph pFooter = new Paragraph("Total Regular Hours : " + totalRegularHours + " Total OT Hours: " + totalOTHours);
                //pFooter.Alignment = Element.ALIGN_RIGHT;
                //doc.Add(pFooter);
                //Paragraph pFooter1 = new Paragraph("Credit Hours Subtotal:" + totalCredits.ToString("C"));
                //pFooter1.Alignment = Element.ALIGN_RIGHT;
                //doc.Add(pFooter1);
                //Paragraph pFooter2 = new Paragraph("HST(" + item.BillingTaxRate.ToString("P2") + "):" + (totalCredits * item.BillingTaxRate).ToString("C"));
                //pFooter2.Alignment = Element.ALIGN_RIGHT;
                //doc.Add(pFooter2);
                //Paragraph pFooter3 = new Paragraph("Total:" + (totalCredits * (1 + item.BillingTaxRate)).ToString("C"));
                //pFooter3.Alignment = Element.ALIGN_RIGHT;
                //doc.Add(pFooter3);
                doc.NewPage();

            }
            doc.Close();
        }
        #endregion


        #region Export CompanyBillingRate

        public void ExportCompanyBillingRateToPDF(Stream stream, IEnumerable<CompanyBillingRate> companyBillingRates)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (companyBillingRates == null)
                throw new ArgumentNullException("companyBillingRates");

            var pageSize = PageSize.A4.Rotate();

            var doc = new Document(pageSize);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();

            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);

            // invoicing enabled?
            var invoicingEnabled = _featureService.IsFeatureEnabled("Admin", "Invoicing");

            var widths = new List<float> { 120f, 60f, 70f, 70f, 40f };
            widths.AddRange(invoicingEnabled ? new List<float> { 40f, 40f, 40f, 40f, 40f, 40f, 50f, 50f } : new List<float> { 40f, 40f, 40f, 50f, 50f });
            PdfPTable dataTable = new PdfPTable(widths.Count);
            dataTable.TotalWidth = 820f;
            dataTable.LockedWidth = true;
            dataTable.SetWidths(widths.ToArray());
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //dataTable.AddCell(new PdfPCell(new Phrase("No.", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Company", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Location", font)));
            //dataTable.AddCell(new PdfPCell(new Phrase("Department", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Rate code", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Position", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Shift", font)));
            if (invoicingEnabled)
                dataTable.AddCell(new PdfPCell(new Phrase("Reg. Billing rate", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Reg. Pay rate", font)));
            if (invoicingEnabled)
                dataTable.AddCell(new PdfPCell(new Phrase("OT Billing rate", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("OT Pay rate", font)));
            if (invoicingEnabled)
                dataTable.AddCell(new PdfPCell(new Phrase("Billing tax rate", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Weekly Hours", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Effective Date", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Deactivated Date", font)));
            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;
            Paragraph mainHeader = new Paragraph("Company Billing Rates");
            mainHeader.Alignment = Element.ALIGN_CENTER;
            doc.Add(mainHeader);
            doc.Add(new Paragraph(Environment.NewLine));
            foreach (var item in companyBillingRates)
            {
                string locationName = _companyDivisionService.GetCompanyLocationById(item.CompanyLocationId).LocationName;
                //dataTable.AddCell(new PdfPCell(new Phrase(i.ToString("00"), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.Company.CompanyName,font)));
                dataTable.AddCell(new PdfPCell(new Phrase(locationName,font)));
                //dataTable.AddCell(new PdfPCell(new Phrase(timechart.DepartmentName, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.RateCode, font)));
                //dataTable.AddCell(new PdfPCell(new Phrase(billingChart.JobStartDateTime.ToString("HH:mm") + "-" + billingChart.JobEndDateTime.ToString("HH:mm"), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.Position.Name, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.ShiftCode, font)));
                if (invoicingEnabled)
                    dataTable.AddCell(new PdfPCell(new Phrase(item.RegularBillingRate.ToString("C"), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.RegularPayRate.ToString("C"), font)));
                if (invoicingEnabled)
                    dataTable.AddCell(new PdfPCell(new Phrase(item.OvertimeBillingRate.ToString("C"), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.OvertimePayRate.ToString("C"), font)));
                if (invoicingEnabled)
                    dataTable.AddCell(new PdfPCell(new Phrase(item.BillingTaxRate.ToString("P2"), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.WeeklyWorkHours.ToString("N2"), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(item.EffectiveDate.ToString("yyyy-MM-dd hh:mm"), font)));
                if (item.DeactivatedDate.HasValue)
                {
                    dataTable.AddCell(new PdfPCell(new Phrase(item.DeactivatedDate.Value.ToString("yyyy-MM-dd hh:mm"), font)));
                }
                else 
                {
                    dataTable.AddCell(new PdfPCell(new Phrase("", font)));
                }
            }
            doc.Add(dataTable);
            doc.Close();
        }

        #endregion


        #region Print Daily Attendance List
        public virtual void PrintDailyAttendanceListToPdfForClient(Stream stream, IEnumerable<DailyAttendanceList> attendanceList, DateTime refDate, bool hideTotalHours)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (attendanceList == null)
                throw new ArgumentNullException("attendanceList");

            var pageSize = PageSize.A4.Rotate();
            var doc = new Document(pageSize);
            var pdfWriter = PdfWriter.GetInstance(doc, stream);
            doc.Open();
            //fonts
            var titleFont = GetFont();
            titleFont.SetStyle(Font.BOLD);
            titleFont.Color = BaseColor.BLACK;
            var font = GetFont();
            var attributesFont = GetFont();
            attributesFont.SetStyle(Font.ITALIC);

              // show or hide vendor
            var displayVendor = _commonSettings.DisplayVendor;
            var firstCol = displayVendor ? 0 : 1;

            float[] origWidths = new float[] { 60f, 80f, 60f, 60f, 60f, 80f, 80f, 100f, 60f, 60f, 60f };
            if (hideTotalHours)
                origWidths = new float[] { 60f, 80f, 60f, 60f, 80f, 80f, 100f, 60f, 60f, 60f };

            // Total columns
            int numOfColumns = origWidths.Length - firstCol;

            PdfPTable dataTable = new PdfPTable(numOfColumns);
            dataTable.TotalWidth = 785f;
            float[] widths = origWidths.Skip(firstCol).Take(numOfColumns).ToArray();
            dataTable.LockedWidth = true;
            dataTable.SetWidths(widths);
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            if (displayVendor)
                dataTable.AddCell(new PdfPCell(new Phrase("Vendor", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Employee name", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Employee #", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Is New", font)));
            if (!hideTotalHours)
                dataTable.AddCell(new PdfPCell(new Phrase("Total Hours Worked", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Location", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Department", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Job Order ", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Shift Start Time", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Shift End Time", font)));
            dataTable.AddCell(new PdfPCell(new Phrase("Status", font)));

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;

            dataTable.DeleteBodyRows();
            Paragraph mainHeader = new Paragraph(" Daily Attendance List - " + refDate.ToString("MM/dd/yyyy"));
            mainHeader.Alignment = Element.ALIGN_CENTER;
            doc.Add(mainHeader);
            doc.Add(new Paragraph(Environment.NewLine));
            foreach (var attendance in attendanceList)
            {
                
                if (displayVendor)
                    dataTable.AddCell(new PdfPCell(new Phrase(attendance.VendorName, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(String.Concat(attendance.EmployeeLastName, ", ", attendance.EmployeeFirstName), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(attendance.EmployeeId, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(attendance.TotalHoursWorked > 0 ? "No" : "Yes", font)));
                if (!hideTotalHours)
                    dataTable.AddCell(new PdfPCell(new Phrase(attendance.TotalHoursWorked.ToString(), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(attendance.Location, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(attendance.Department, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(attendance.JobTitleAndId, font)));
                dataTable.AddCell(new PdfPCell(new Phrase(attendance.ShiftStartTime.ToShortTimeString(), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(attendance.ShiftEndTime.ToShortTimeString(), font)));
                dataTable.AddCell(new PdfPCell(new Phrase(attendance.Status, font)));
            }

            doc.Add(dataTable);
            doc.NewPage();
            doc.Close();
        }
        #endregion


        #region Print Smart Card Bitmap to PDF

        public void PrintBitmapsToPdf(System.Drawing.Image frontImg, System.Drawing.Image backImg, Stream pdfStream)
        {
            var ratio = 72f / 300f;
            var frontSize = new Rectangle(frontImg.PhysicalDimension.Width * ratio, frontImg.PhysicalDimension.Height * ratio);
            var backSize = new Rectangle(backImg.PhysicalDimension.Width * ratio, backImg.PhysicalDimension.Height * ratio);

            using (var doc = new Document(frontSize))
            {
                var writer = PdfWriter.GetInstance(doc, pdfStream);
                var action = new PdfAction(PdfAction.PRINTDIALOG);
                writer.SetOpenAction(action);
                
                doc.SetMargins(0, 0, 0, 0);
                doc.Open();

                // avoid no page error
                doc.Add(new Paragraph(string.Empty));

                // front side
                var img = Image.GetInstance(frontImg, System.Drawing.Imaging.ImageFormat.Bmp);
                //img.ScalePercent(ratio * 100);
                img.ScaleToFit(frontSize);
                img.SetAbsolutePosition(0, 0);
                doc.Add(img);

                // back side
                doc.SetPageSize(backSize);
                doc.NewPage();
                img = iTextSharp.text.Image.GetInstance(backImg, System.Drawing.Imaging.ImageFormat.Bmp);
                //img.ScalePercent(ratio * 100);
                img.ScaleToFit(backSize);
                img.SetAbsolutePosition(0, 0);
                doc.Add(img);
               
                doc.Close();
            }
        }

        #endregion


        #region Encrypt

        public byte[] SecurePDF(byte[] bytes, string password)
        {
            byte[] result = null;

            using (var input = new MemoryStream(bytes))
            {
                using (var output = new MemoryStream())
                {
                    var reader = new PdfReader(input);
                    PdfEncryptor.Encrypt(reader, output, true, password, null, PdfWriter.ALLOW_SCREENREADERS);
                    result = output.ToArray();
                }
            }

            return result;
        }

        #endregion

    }
}
