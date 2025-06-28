using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;


namespace Wfm.Services.ExportImport
{
    public partial interface IImportManager
    {
        #region Import Work Time

        IList<string> ImportWorkTimeFromXlsx(Stream stream, out IList<string> warnings, bool isStdTmplt = true);

        IList<string> ImportWorkTimeFromCustomerXlsxTypeI(Stream stream, int companyId, int locationId = 0);

        IList<string> ImportWorkTimeFromCustomerXlsxTypeII(Stream stream, out IList<string> warnings);

        #endregion

        IList<string> ImportCandidateFromXlsx(Stream stream, int companyId);
        IList<string> ValidateImportTemplate(ExcelWorksheet worksheet, string[] properties);
        ImportResult ImportVendorCandidateFromXlsx(Stream stream, int vendorId);
        ImportResult ImportVendorCandidateAddressFromXlsx(Stream stream, int vendorId);
        ImportResult ImportVendorCandidateSkillFromXlsx(Stream stream, int vendorId);
        ImportResult ImportVendorCandidateBankAccountFromXlsx(Stream stream, int vendorId);
    }
}
