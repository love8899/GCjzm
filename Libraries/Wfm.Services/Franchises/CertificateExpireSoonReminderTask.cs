using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;

namespace Wfm.Services.Franchises
{
    public class CertificateExpireSoonReminderTask : IScheduledTask
    {
        #region Field
        private readonly IVendorCertificateService _vendorCertificateService;
        private readonly IWorkflowMessageService _workflowMessageService;
        #endregion

        #region Ctor
        public CertificateExpireSoonReminderTask(IVendorCertificateService vendorCertificateService, IWorkflowMessageService workflowMessageService)
        {
            _vendorCertificateService = vendorCertificateService;
            _workflowMessageService = workflowMessageService;
        }
        #endregion

        #region Method
        public virtual void Execute()
        {
            var certificates = _vendorCertificateService.GetAllLastestCertificates().Where(x=>DbFunctions.AddDays(x.GeneralLiabilityCertificateExpiryDate,-30)<DateTime.Today).ToList();
            foreach (var certificate in certificates)
            {
                _workflowMessageService.SendVendorCertificateExpireReminder(certificate.Description, certificate.FranchiseId, 1);
            }

        }
        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
        #endregion
    }
}
