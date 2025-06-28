using System;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.Payroll;
using Wfm.Core.Infrastructure;
using Wfm.Services.Candidates;


namespace Wfm.Services.Payroll
{
    public partial class T4Helper
    {
        #region Fields

        public const int MIN_YEAR = 2015;
        public const int MAX_YEAR = 2018;

        public static string OWNER_PASSWORD { get { return "kJs89357Fm4Dmjd024"; } }

        private static readonly IWebHelper _webHelper = EngineContext.Current.Resolve<IWebHelper>();
        private static readonly IRepository<FranchiseAddress> _addresses = EngineContext.Current.Resolve<IRepository<FranchiseAddress>>();

        #endregion


        #region T4 PDF

        public static string GetEmployeeT4PDF(IPaymentHistoryService paymentHistoryService, Franchise employer, int taxYear, T4_Base t4, bool isEncrypted, string path, string template)
        {
            var fileName = _GetT4FileName(t4.Id);

            using (var fs = new FileStream(fileName, FileMode.Create))
                _GetEmployeeT4PDF(paymentHistoryService, employer, taxYear, t4, isEncrypted, path, template, fs);

            return fileName;
        }


        public static byte[] GetEmployeeT4PDFBytes(IPaymentHistoryService paymentHistoryService, Franchise employer, int taxYear, T4_Base t4, bool isEncrypted, string path, string template)
        {
            byte[] bytes;

            using (var ms = new MemoryStream())
            {
                _GetEmployeeT4PDF(paymentHistoryService, employer, taxYear, t4, isEncrypted, path, template, ms);
                bytes = ms.ToArray();
            }

            return bytes;
        }


        private static void _GetEmployeeT4PDF(IPaymentHistoryService paymentHistoryService, Franchise employer, int taxYear, T4_Base t4, bool isEncrypted, string path, string template, Stream stream)
        {
            using (PdfReader pdfReader = new PdfReader(Path.Combine(path, template)))
            {
                PdfReader.unethicalreading = true;

                using (PdfStamper pdfStamper = new PdfStamper(pdfReader, stream))
                {
                    if (isEncrypted)
                    {
                        string password;
                        if (paymentHistoryService.PayStub_Password(t4.CandidateId, out password))
                            pdfStamper.SetEncryption(true, password, OWNER_PASSWORD, PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING);
                    }

                    var pdfFormFields = pdfStamper.AcroFields;
                    _FillT4Slip(employer, taxYear, 1, t4, true, pdfFormFields);
                    _FillT4Slip(employer, taxYear, 2, t4, true, pdfFormFields);

                    if (taxYear >= 2018)
                    {
                        // According to CRA filing requirements, we should print this label on the top of cancelled or amended slips
                        _AddStatusLabel(pdfStamper, 1, 510, 762, t4.ReportTypeCode);
                    }

                    pdfStamper.FormFlattening = true;
                    pdfStamper.Close();
                }
            }
        }


        private static void _AddStatusLabel(PdfStamper stamper, int pageNum, float x, float y, string reportTypeCode)
        {
            var label = string.Empty;

            if (reportTypeCode == "C")
                label = "CANCELLED";
            else if (reportTypeCode == "A")
                label = "AMENDED";

            CommonHelper.AddTextToPdf(stamper, pageNum, x, y, label);
        }


        private static string _GetT4FileName(int t4Id)
        {
            StringBuilder fileName = new StringBuilder();
            fileName.Append(DateTime.Now.ToString("MMddyyyyhhmmss_"));
            fileName.Append(t4Id);
            fileName.Append(".pdf");

            return Path.Combine(_webHelper.GetTempPath(), fileName.ToString());
        }


        private static void _FillT4Slip(Franchise employer, int taxYear, int slipNumber, T4_Base t4, bool isEmployeeCopy, AcroFields pdfFormFields)
        {
            var franchiseAddress = _addresses.TableNoTracking.Where(x => x.FranchiseId == employer.Id).FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.IsHeadOffice);
            string employerAddress = String.Concat(franchiseAddress.AddressLine1, !String.IsNullOrWhiteSpace(franchiseAddress.AddressLine2) ? String.Concat("\n", franchiseAddress.AddressLine2.Trim()) : string.Empty);
            string employeeAddress = String.Concat(t4.AddressLine1, !String.IsNullOrWhiteSpace(t4.AddressLine2) ? String.Concat("\n", t4.AddressLine2.Trim()) : string.Empty);

            string prefix = String.Concat("form1[0].Page1[0].Slip", slipNumber, "[0].");

            pdfFormFields.SetField(String.Concat(prefix, "EmployersName[0].Slip1EmployersName[0]"),
                                   String.Concat(employer.FranchiseName, "\n", employerAddress, "\n", franchiseAddress.City.CityName, ", ", franchiseAddress.StateProvince.Abbreviation, " ", franchiseAddress.PostalCode));

            pdfFormFields.SetField(String.Concat(prefix, "Year[0].Slip1Year[0]"), taxYear.ToString());

            //if (!isEmployeeCopy)
            //    pdfFormFields.SetField(String.Concat(prefix, "EmployersAccount[0].Slip1Box54[0]"), employer.BusinessNumber);

            if (taxYear < 2018)
                pdfFormFields.SetField(String.Concat(prefix, "EmployeeSIN"), CommonHelper.ToPrettySocialInsuranceNumber(t4.SocialInsuranceNumber));
            else
                pdfFormFields.SetField(String.Concat(prefix, "Box12[0].Slip1Box12[0]"), t4.SocialInsuranceNumber);

            pdfFormFields.SetField(String.Concat(prefix, "Box28[0].CPP_CheckBox[0].Slip1CPP[0]"), t4.ExemptCPP_QPP.ToString());
            pdfFormFields.SetField(String.Concat(prefix, "Box28[0].EI_CheckBox[0].Slip1EI[0]"), t4.ExemptEI.ToString());
            pdfFormFields.SetField(String.Concat(prefix, "Box28[0].PPIP_CheckBox[0].Slip1PPIP[0]"), t4.ExemptPPIP.ToString());

            pdfFormFields.SetField(String.Concat(prefix, "Box10[0].Slip1Box10[0]"), t4.Box10_ProvinceCode);
            pdfFormFields.SetField(String.Concat(prefix, "Box29[0].Slip1Box29[0]"), t4.EmploymentCode != null ? t4.EmploymentCode : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box14[0].Slip1Box14[0]"), t4.EmploymentIncome > 0 ? t4.EmploymentIncome.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box22[0].Slip1Box22[0]"), t4.IncomeTax > 0 ? t4.IncomeTax.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box16[0].Slip1Box16[0]"), t4.CPP > 0 ? t4.CPP.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box24[0].Slip1Box24[0]"), t4.InsurableEarnings.ToString());
            pdfFormFields.SetField(String.Concat(prefix, "Box17[0].Slip1Box17[0]"), t4.QPP > 0 ? t4.QPP.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box26[0].Slip1Box26[0]"), t4.PensionableEarnings.ToString());
            pdfFormFields.SetField(String.Concat(prefix, "Box18[0].Slip1Box18[0]"), t4.EIPremium > 0 ? t4.EIPremium.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box44[0].Slip1Box44[0]"), t4.UnionPay > 0 ? t4.UnionPay.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box20[0].Slip1Box20[0]"), t4.RPP > 0 ? t4.RPP.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box46[0].Slip1Box46[0]"), t4.CharitableDonations > 0 ? t4.CharitableDonations.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box52[0].Slip1Box52[0]"), t4.PensionAdjustment > 0 ? t4.PensionAdjustment.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box50[0].Slip1Box50[0]"), t4.RPP_DPSPNumber);
            pdfFormFields.SetField(String.Concat(prefix, "Box55[0].Slip1Box55[0]"), t4.PPIPPremiums > 0 ? t4.PPIPPremiums.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Box56[0].Slip1Box56[0]"), t4.PPIPInsurableEarnings > 0 ? t4.PPIPInsurableEarnings.ToString() : "");
            pdfFormFields.SetField(String.Concat(prefix, "Employee[0].LastName[0].Slip1LastName[0]"), String.IsNullOrWhiteSpace(t4.LastName) ? "" : t4.LastName.ToUpper());
            pdfFormFields.SetField(String.Concat(prefix, "Employee[0].FirstName[0].Slip1FirstName[0]"), String.IsNullOrWhiteSpace(t4.FirstName) ? "" : t4.FirstName.ToUpper());
            pdfFormFields.SetField(String.Concat(prefix, "Employee[0].Initial[0].Slip1Initial[0]"), String.IsNullOrWhiteSpace(t4.Initials) ? "" : t4.Initials.ToUpper());

            var addrField = taxYear < 2018 ? "EmployeeAddress" : "Employee[0].Slip1Address[0]";
            pdfFormFields.SetField(String.Concat(prefix, addrField), String.Concat(employeeAddress, "\n", t4.City, ", ", t4.ProvinceCode, "\n", t4.Postalcode));

            if (t4.OtherInfoBox1Amount.HasValue && t4.OtherInfoBox1Amount > 0)
            {
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Box1[0].Slip1Box1[0]"), t4.OtherInfoBox1Code);
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Amount1[0].Slip1Amount1[0]"), t4.OtherInfoBox1Amount.ToString());
            }
            if (t4.OtherInfoBox2Amount.HasValue && t4.OtherInfoBox2Amount > 0)
            {
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Box2[0].Slip1Box2[0]"), t4.OtherInfoBox2Code);
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Amount2[0].Slip1Amount2[0]"), t4.OtherInfoBox2Amount.ToString());
            }
            if (t4.OtherInfoBox3Amount.HasValue && t4.OtherInfoBox3Amount > 0)
            {
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Box3[0].Slip1Box3[0]"), t4.OtherInfoBox3Code);
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Amount3[0].Slip1Amount3[0]"), t4.OtherInfoBox3Amount.ToString());
            }
            if (t4.OtherInfoBox4Amount.HasValue && t4.OtherInfoBox4Amount > 0)
            {
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Box4[0].Slip1Box4[0]"), t4.OtherInfoBox4Code);
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Amount4[0].Slip1Amount4[0]"), t4.OtherInfoBox4Amount.ToString());
            }
            if (t4.OtherInfoBox5Amount.HasValue && t4.OtherInfoBox5Amount > 0)
            {
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Box5[0].Slip1Box5[0]"), t4.OtherInfoBox5Code);
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Amount5[0].Slip1Amount5[0]"), t4.OtherInfoBox5Amount.ToString());
            }
            if (t4.OtherInfoBox6Amount.HasValue && t4.OtherInfoBox6Amount > 0)
            {
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Box6[0].Slip1Box6[0]"), t4.OtherInfoBox6Code);
                pdfFormFields.SetField(String.Concat(prefix, "OtherInformation[0].Amount6[0].Slip1Amount6[0]"), t4.OtherInfoBox6Amount.ToString());
            }

            // remove button
            var btnName = "form1[0].#pageSet[0].Page1[{0}].ClearData_EN[0]";
            pdfFormFields.RemoveField(String.Format(btnName, 0));
            pdfFormFields.RemoveField(String.Format(btnName, 1));
        }

        #endregion
    }
}
