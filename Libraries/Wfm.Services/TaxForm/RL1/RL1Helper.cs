using System;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.TaxForm.RL1;
using Wfm.Core.Infrastructure;
using Wfm.Services.Candidates;


namespace Wfm.Services.Payroll
{
    public partial class RL1Helper
    {
        public const int MIN_YEAR = 2015;
        public const int MAX_YEAR = 2018;

        public static string OWNER_PASSWORD { get { return "kJs89357Fm4Dmjd024"; } }

        #region Fields

        private static readonly IWebHelper _webHelper = EngineContext.Current.Resolve<IWebHelper>();
        private static readonly IRepository<FranchiseAddress> _addresses = EngineContext.Current.Resolve<IRepository<FranchiseAddress>>();
        private static readonly IRepository<StateProvince> _provinces = EngineContext.Current.Resolve<IRepository<StateProvince>>();
        private static readonly IRepository<City> _cities = EngineContext.Current.Resolve<IRepository<City>>();

        #endregion


        public static string GetRL1SlipAuthorizationNumber(int year)
        {
            switch (year)
            {
                case 2015:
                    return "FS1501187";
                case 2016:
                    return "FS1601110";
                case 2017:
                    return "FS1701091";
                case 2018:
                    return "FS1801194";
                default:
                    return "FS9999999";
            }
        }


        #region RL1 PDF

        public static string GetRL1PDF(IPaymentHistoryService paymentHistoryService, int taxYear, Franchise employer, RL1_Base slip,
            string path, string template, bool isEncrypted, bool isEmployer)
        {
            var fileName = _GetRL1FileName(slip.Id);

            using (var fs = new FileStream(fileName, FileMode.Create))
                _GetEmployeeRL1PDF(paymentHistoryService, taxYear, employer, slip, path, template, isEncrypted, isEmployer, fs);

            return fileName;
        }


        public static byte[] GetRL1PDFBytes(IPaymentHistoryService paymentHistoryService, int taxYear, Franchise employer, RL1_Base slip,
            string path, string template, bool isEncrypted, bool isEmployer)
        {
            byte[] bytes;

            using (var ms = new MemoryStream())
            {
                _GetEmployeeRL1PDF(paymentHistoryService, taxYear, employer, slip, path, template, isEncrypted, isEmployer, ms);
                bytes = ms.ToArray();
            }

            return bytes;

        }


        private static void _GetEmployeeRL1PDF(IPaymentHistoryService paymentHistoryService, int taxYear, Franchise employer, RL1_Base slip,
            string path, string template, bool isEncrypted, bool isEmployer, Stream stream)
        {
            using (PdfReader pdfReader = new PdfReader(Path.Combine(path, template)))
            {
                PdfReader.unethicalreading = true;

                using (PdfStamper pdfStamper = new PdfStamper(pdfReader, stream))
                {
                    if (isEncrypted)
                    {
                        string password;
                        if (paymentHistoryService.PayStub_Password(slip.CandidateId, out password))
                            pdfStamper.SetEncryption(true, password, OWNER_PASSWORD, PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING);
                    }

                    _FillRL1Slip(taxYear, employer, slip, isEmployer, pdfStamper);

                    pdfStamper.FormFlattening = true;
                    pdfStamper.Close();
                }
            }
        }


        private static string _GetRL1FileName(int rl1Id)
        {
            StringBuilder fileName = new StringBuilder();
            fileName.Append(DateTime.Now.ToString("MMddyyyyhhmmss_"));
            fileName.Append(rl1Id);
            fileName.Append(".pdf");

            return Path.Combine(_webHelper.GetTempPath(), fileName.ToString());
        }


        private static void _FillRL1Slip(int taxYear, Franchise employer, RL1_Base slip, bool isEmployer, PdfStamper pdfStamper)
        {
            var pdfFormFields = pdfStamper.AcroFields;

            //Common Fields
            var fieldNamePrefix = taxYear == 2015 ? "rep_" : string.Empty;
            pdfFormFields.SetField(fieldNamePrefix + "codeReleve", slip.Code);

            if (slip.Code == "A")
            {
                pdfFormFields.SetField(fieldNamePrefix + "noDernier", Convert.ToString(slip.LastSequentialNum));
                pdfFormFields.SetFieldProperty("textModifie", "clrflags", PdfFormField.FLAGS_HIDDEN, null);
            }
            else if (slip.Code == "D")
            {
                pdfFormFields.SetFieldProperty("textAnnule", "clrflags", PdfFormField.FLAGS_HIDDEN, null);
            }

            var seqFieldName = taxYear == 2015 ? "SequentialNumber" : "noSEQ";
            pdfFormFields.SetField(seqFieldName, slip.SequentialNum);

            pdfFormFields.SetField(fieldNamePrefix + "caseA", slip.Box_A != 0 ? Convert.ToString(slip.Box_A) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseB", slip.Box_B != 0 ? Convert.ToString(slip.Box_B) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseC", slip.Box_C != 0 ? Convert.ToString(slip.Box_C) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseD", slip.Box_D != 0 ? Convert.ToString(slip.Box_D) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseE", slip.Box_E != 0 ? Convert.ToString(slip.Box_E) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseF", slip.Box_F != 0 ? Convert.ToString(slip.Box_F) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseG", Convert.ToString(slip.Box_G));
            pdfFormFields.SetField(fieldNamePrefix + "caseH", slip.Box_H != 0 ? Convert.ToString(slip.Box_H) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseI", Convert.ToString(slip.Box_I));
            pdfFormFields.SetField(fieldNamePrefix + "caseJ", slip.Box_J != 0 ? Convert.ToString(slip.Box_J) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseK", slip.Box_K != 0 ? Convert.ToString(slip.Box_K) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseL", slip.Box_L != 0 ? Convert.ToString(slip.Box_L) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseM", slip.Box_M != 0 ? Convert.ToString(slip.Box_M) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseN", slip.Box_N != 0 ? Convert.ToString(slip.Box_N) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseO", slip.Box_O != 0 ? Convert.ToString(slip.Box_O) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseP", slip.Box_P != 0 ? Convert.ToString(slip.Box_P) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseQ", slip.Box_Q != 0 ? Convert.ToString(slip.Box_Q) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseR", slip.Box_R != 0 ? Convert.ToString(slip.Box_R) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseS", slip.Box_S != 0 ? Convert.ToString(slip.Box_S) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseT", slip.Box_T != 0 ? Convert.ToString(slip.Box_T) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseU", slip.Box_U != 0 ? Convert.ToString(slip.Box_U) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseV", slip.Box_V != 0 ? Convert.ToString(slip.Box_V) : "");
            pdfFormFields.SetField(fieldNamePrefix + "caseW", slip.Box_W != 0 ? Convert.ToString(slip.Box_W) : "");

            var codeCaseOFieldName = taxYear == 2015 ? "CodeCaseO" : "code";
            pdfFormFields.SetField(codeCaseOFieldName, slip.Code_Case_O ?? string.Empty);

            if (!String.IsNullOrWhiteSpace(slip.SIN))
                pdfFormFields.SetField(fieldNamePrefix + "nas", slip.SIN.Insert(6, "    ").Insert(3, "    "));

            pdfFormFields.SetField(fieldNamePrefix + "ref", Convert.ToString(slip.ReferenceNumber));

            var infoCodeFieldName = taxYear == 2015 ? "rep_codeRenseignement" : "codeRensCompl";
            pdfFormFields.SetField(infoCodeFieldName + "1", slip.Additional_Info1_Code ?? string.Empty);
            pdfFormFields.SetField(infoCodeFieldName + "2", slip.Additional_Info2_Code ?? string.Empty);
            pdfFormFields.SetField(infoCodeFieldName + "3", slip.Additional_Info3_Code ?? string.Empty);
            pdfFormFields.SetField(infoCodeFieldName + "4", slip.Additional_Info4_Code ?? string.Empty);
            var infoValueFieldName = taxYear == 2015 ? "rep_infoRenseignement" : "donneeRensCompl";
            pdfFormFields.SetField(infoValueFieldName + "1", slip.Additional_Info1_Value != 0 ? Convert.ToString(slip.Additional_Info1_Value) : "");
            pdfFormFields.SetField(infoValueFieldName + "2", slip.Additional_Info2_Value != 0 ? Convert.ToString(slip.Additional_Info2_Value) : "");
            pdfFormFields.SetField(infoValueFieldName + "3", slip.Additional_Info3_Value != 0 ? Convert.ToString(slip.Additional_Info3_Value) : "");
            pdfFormFields.SetField(infoValueFieldName + "4", slip.Additional_Info4_Value != 0 ? Convert.ToString(slip.Additional_Info4_Value) : "");

            pdfFormFields.SetField("nom1", slip.LastName.Trim());
            pdfFormFields.SetField("prenom1", slip.FirstName);

            var employerName = employer.FranchiseName.Replace("&", "&amp;");
            var nameList = CommonHelper.SplitToLines(employerName, 30);
            var employerAddress = _addresses.TableNoTracking.Where(x => x.FranchiseId == employer.Id).FirstOrDefault(x => x.IsActive && !x.IsDeleted && x.IsHeadOffice);
            if (taxYear == 2015)
            {
                var employerNameAddress = employerName + Environment.NewLine +
                    employerAddress.AddressLine1 + " " + employerAddress.AddressLine2 + Environment.NewLine +
                    employerAddress.City.CityName + ", " + employerAddress.StateProvince.Abbreviation + " " + 
                    CommonHelper.ExtractAlphanumericText(employerAddress.PostalCode).ToUpper();
                pdfFormFields.SetField("rep_payeur", employerNameAddress);
            }
            else
            {
                pdfFormFields.SetField("nom2", employerName.Length > 30 ? nameList.ElementAt(0) : employerName);
                pdfFormFields.SetField("prenom2", employerName.Length > 30 ? nameList.ElementAt(1) : null);

                pdfFormFields.SetField("numero2", employerAddress.AddressLine1);
                pdfFormFields.SetField("rue2", employerAddress.AddressLine2);
                pdfFormFields.SetField("ville2", employerAddress.City.CityName);
            }

            var authNumFieldName = taxYear == 2015 ? "AuthorizationNumber" : "noFS";
            pdfFormFields.SetField(authNumFieldName, GetRL1SlipAuthorizationNumber(taxYear));

            var typeCodeFieldName = taxYear == 2015 ? "ModifyText" : "mention";
            if (slip.Code == "A")
                pdfFormFields.SetField(typeCodeFieldName, "Modifié".ToUpper());
            else if (slip.Code == "D")
                pdfFormFields.SetField(typeCodeFieldName, "Annulé".ToUpper());

            if (isEmployer)
            {
                // TODO
                return;
            }
            else
            {
                var xmlSeqFieldName = taxYear == 2015 ? "XMLSequentialNumber" : "noSEQ2";
                pdfFormFields.SetField(xmlSeqFieldName, slip.XMLSequentialNum ?? string.Empty);

                var city = _cities.GetById(slip.CityId).CityName;
                var province = _provinces.GetById(slip.StateProvinceId).Abbreviation;
                if (taxYear == 2015)
                {
                    pdfFormFields.SetField("rep_Identifiant1", slip.LastName + " " + slip.FirstName + Environment.NewLine + 
                        slip.AddressLine1 + " " + slip.AddressLine2 + Environment.NewLine + 
                        city + ", " + province + " " + slip.PostalCode);
                }
                else
                {
                    pdfFormFields.SetField("numero1", slip.AddressLine1);
                    pdfFormFields.SetField("rue1", _cities.GetById(slip.CityId).CityName);
                    pdfFormFields.SetField("ville1", String.Concat(_provinces.GetById(slip.StateProvinceId).Abbreviation, "  ", slip.PostalCode));

                    pdfFormFields.SetField("province2", String.Concat(employerAddress.StateProvince.Abbreviation, " ", CommonHelper.ExtractAlphanumericText(employerAddress.PostalCode).ToUpper()));
                }
            }
        }

        #endregion
    }
}
