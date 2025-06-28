using System;
using System.Linq;
using CanadianTaxTable.TaxTables;
using Common.TaxTables;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.TaxForm.RL1;
using Wfm.Services.Candidates;
using Wfm.Services.Configuration;


namespace Wfm.Services.Payroll
{

    public partial class RL1_2017Service : IRL1BaseService
    {

        #region Fields

        private const int _TaxYear = 2017;
        private readonly TaxTable objTaxTable = TaxTableUtilities.GetTaxTableForDate(new DateTime(_TaxYear, 12, 31));

        private readonly IWebHelper _webHelper;
        private readonly IRepository<Franchise> _franchises;
        private readonly IRepository<RL1_2017> _rl1s;
        private readonly IPaymentHistoryService _paymentHistoryService;
        private readonly ISettingService _settingService;

        #endregion


        #region Ctors

        public RL1_2017Service(
            IWebHelper webHelper,
            IRepository<Franchise> franchises,
            IRepository<RL1_2017> rl1s,
            IPaymentHistoryService paymentHistoryService,
            ISettingService settingService)
        {
            _webHelper = webHelper;
            _franchises = franchises;
            _rl1s = rl1s;
            _paymentHistoryService = paymentHistoryService;
            _settingService = settingService;
        }

        #endregion


        #region RL1

        public RL1_Base GetRL1DataById(int id)
        {
            return _rl1s.GetById(id);
        }


        public IQueryable<RL1_Base> GetAllRL1s(int? franchiseId = null)
        {
            return _rl1s.Table.Where(x => !franchiseId.HasValue || x.FranchiseId == franchiseId);
        }

        #endregion


        #region RL1 PDF

        public string GetEmployeeRL1PDF(int franchiseId, int rl1Id, bool isEncrypted, out int candidateId)
        {
            var employer = _franchises.GetById(franchiseId);
            var rl1 = GetRL1DataById(rl1Id);
            candidateId = rl1.CandidateId;

            var pdfTemplate = @"RL-1(2017-10)pre-copie2.pdf";
            // var setting = _settingService.GetSettingByKey<string>("RL1.PDF.Path", null, 0);
            var setting = "~/bin/Files/RL1_";
            var pdfTemplatePath = _webHelper.MapPath(String.Concat(setting, _TaxYear, "/"));

            return RL1Helper.GetRL1PDF(_paymentHistoryService, _TaxYear, employer, rl1, pdfTemplatePath, pdfTemplate, isEncrypted: isEncrypted, isEmployer: false);
        }


        public byte[] GetEmployeeRL1PDFBytes(int franchiseId, int rl1Id, bool isEncrypted, out int candidateId)
        {
            var employer = _franchises.GetById(franchiseId);
            var rl1 = GetRL1DataById(rl1Id);
            candidateId = rl1.CandidateId;

            var pdfTemplate = @"RL-1(2017-10)pre-copie2.pdf";
            // var setting = _settingService.GetSettingByKey<string>("RL1.PDF.Path", null, 0);
            var setting = "~/bin/Files/RL1_";
            var pdfTemplatePath = _webHelper.MapPath(String.Concat(setting, _TaxYear, "/"));

            return RL1Helper.GetRL1PDFBytes(_paymentHistoryService, _TaxYear, employer, rl1, pdfTemplatePath, pdfTemplate, isEncrypted: isEncrypted, isEmployer: false);
        }

        #endregion
    }
}
