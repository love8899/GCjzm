using System;
using Wfm.Services.ExportImport;


namespace Wfm.Admin.Models.Candidate
{
    public class VendorCandidateImportModel
    {
        public int VendorId { get; set; }
        public DateTime? AccountImportedOn { get; set; }
        public ImportResult AccountImportResult { get; set; }
        public DateTime? AddressImportedOn { get; set; }
        public ImportResult AddressImportResult { get; set; }
        public DateTime? SkillImportedOn { get; set; }
        public ImportResult SkillImportResult { get; set; }
        public DateTime? BankAccountImportedOn { get; set; }
        public ImportResult BankAccountImportResult { get; set; }
    }

}
