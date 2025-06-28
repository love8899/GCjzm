using System;
using System.Linq;
using CanadianTaxTable.TaxTables;
using Common.TaxTables;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.Payroll;
using Wfm.Services.Candidates;
using Wfm.Services.Configuration;


namespace Wfm.Services.Payroll
{
    public partial class T4_2019Service : IT4BaseService
    {
        #region Fields

        private const int _TaxYear = 2019;
        private readonly TaxTable objTaxTable = TaxTableUtilities.GetTaxTableForDate(new DateTime(_TaxYear, 12, 31));

        private readonly IWebHelper _webHelper;
        private readonly IRepository<Franchise> _franchises;
        private readonly IRepository<T4_2019> _t4s;
        private readonly IPaymentHistoryService _paymentHistoryService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctors

        public T4_2019Service(
            IWebHelper webHelper,
            IRepository<Franchise> franchises,
            IRepository<T4_2019> t4s,
            IPaymentHistoryService paymentHistoryService,
            ISettingService settingService)
        {
            _webHelper = webHelper;
            _franchises = franchises;
            _t4s = t4s;
            _paymentHistoryService = paymentHistoryService;
            _settingService = settingService;
        }

        #endregion


        #region T4

        public T4_Base GetT4DataById(int id)
        {
            return _t4s.GetById(id);
        }


        public IQueryable<T4_Base> GetAllT4s(int? franchiseId = null, bool includeSubmitted = true)
        {
            var result = _t4s.Table.Where(x => !franchiseId.HasValue || x.FranchiseId == franchiseId);
            if (!includeSubmitted)
                result = result.Where(x => x.IsSubmitted == false || !x.IsSubmitted.HasValue);

            return result;
        }

        #endregion


        #region T4 PDF


        public string GetEmployeeT4PDF(int franchiseId, int t4Id, bool isEncrypted, out int candidateId)
        {
            var employer = _franchises.GetById(franchiseId);
            var t4 = GetT4DataById(t4Id);
            candidateId = t4.CandidateId;

            var pdfTemplate = @"t4-fill-19e.pdf";
            // var path = _settingService.GetSettingByKey<string>("T4.PDF.Path", null, 0);
            var path = "~/bin/Files/T4_";
            var pdfTemplatePath = _webHelper.MapPath(string.Concat(path, _TaxYear, "/"));

            return T4Helper.GetEmployeeT4PDF(_paymentHistoryService, employer, _TaxYear, t4, isEncrypted, pdfTemplatePath, pdfTemplate);
        }


        public byte[] GetEmployeeT4PDFBytes(int franchiseId, int t4Id, bool isEncrypted, out int candidateId)
        {
            var employer = _franchises.GetById(franchiseId);
            var t4 = GetT4DataById(t4Id);
            candidateId = t4.CandidateId;

            var pdfTemplate = @"t4-fill-19e.pdf";
            // var path = _settingService.GetSettingByKey<string>("T4.PDF.Path", null, 0);
            var path = "~/bin/Files/T4_";
            var pdfTemplatePath = _webHelper.MapPath(string.Concat(path, _TaxYear, "/"));

            return T4Helper.GetEmployeeT4PDFBytes(_paymentHistoryService, employer, _TaxYear, t4, isEncrypted, pdfTemplatePath, pdfTemplate);
        }

        #endregion
    }
}
