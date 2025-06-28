using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Employees;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.Messages;
using Wfm.Services.Logging;


namespace Wfm.Admin.Models.Employee
{
    public class Paystub_BL
    {
        #region Fields

        private readonly ICandidateService _candidateServcie;
        private readonly IRepository<EmployeePayrollSetting> _employeePayrollSettings;
        private readonly IPdfService _pdfService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ILogger _logger;

        #endregion


        #region Ctor

        public Paystub_BL(
            ICandidateService candidateServcie,
            IRepository<EmployeePayrollSetting> employeePayrollSettings,
            IPdfService pdfService,
            IWorkflowMessageService workflowMessageService,
            ILogger logger)
        {
            _candidateServcie = candidateServcie;
            _employeePayrollSettings = employeePayrollSettings;
            _pdfService = pdfService;
            _workflowMessageService = workflowMessageService;
            _logger = logger;
        }

        #endregion


        public string SendPaystub(int employeeId, byte[] paystub, DateTime payPeriodStart, DateTime payPeriodEnd)
        {
            var result = string.Empty;

            try
            {
                var candidate = _candidateServcie.GetCandidateById(employeeId);
                if (candidate == null)
                    return String.Format("PayStub_Password_and_Email():This employee {0} does not exist!", employeeId);

                var password = GetPaystubPassword(candidate.Id, candidate.SocialInsuranceNumber, candidate.BirthDate);
                var bytes = _pdfService.SecurePDF(paystub, password);
                var fileName = String.Concat(DateTime.Now.ToString("MMddyyyyHHmmss_"), employeeId, ".pdf");

                result = _workflowMessageService.SendPaystubToEmployee(candidate, bytes, payPeriodStart, payPeriodEnd, fileName);
            }
            catch (Exception ex)
            {
                _logger.Error("SendPayStub():", ex);
            }

            return result;
        }


        public string GetPaystubPassword(int candidateId, string sin = null, DateTime? birthDate = null)
        {
            var password = string.Empty;

            var payrollSetting = _employeePayrollSettings.TableNoTracking.Where(x => x.EmployeeId == candidateId).FirstOrDefault();
            if (payrollSetting == null)
                return String.Format("PayStub_Password_and_Email():This employee {0} payroll setting does not exist!", candidateId);

            password = payrollSetting.PayStubPassword;
            if (String.IsNullOrWhiteSpace(password))
            {
                if (String.IsNullOrWhiteSpace(sin) || !birthDate.HasValue)
                    return String.Format("PayStub_Password_and_Email():Employee's SIN or Date of birth is invalid! EmployeeId:{0}", candidateId);

                var last3sin = sin.Substring(sin.Length - 3);
                var birthMMdd = birthDate.Value.ToString("MMdd");
                if (String.IsNullOrWhiteSpace(last3sin) || String.IsNullOrWhiteSpace(birthMMdd))
                    return String.Format("PayStub_Password_and_Email():Employee's SIN or Date of birth is invalid! EmployeeId:{0}", candidateId);

                password = String.Concat(birthMMdd, last3sin);
            }

            return password;
        }

    }
}
