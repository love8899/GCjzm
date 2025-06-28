using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.Tasks;

namespace Wfm.Services.JobOrders
{
    public class RemindSendingPlacementTask : IScheduledTask
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly IJobOrderService _jobOrderService;
        private readonly IAccountService _accountService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IFranchiseService _franchiseService;
        private readonly IMessageHistoryService _messageHistoryService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICompanyService _companyService;
        #endregion

        #region Ctor
        public RemindSendingPlacementTask(ILogger logger, IJobOrderService jobOrderService, IAccountService accountService,
                                        ICandidateJobOrderService candidateJobOrderService,
                                        IMessageTemplateService messageTemplateService,
                                        IFranchiseService franchiseService,
                                        IMessageHistoryService messageHistoryService,
                                        IWorkflowMessageService workflowMessageService,
                                        ICompanyService companyService)
        {
            _logger = logger;
            _jobOrderService = jobOrderService;
            _accountService = accountService;
            _candidateJobOrderService = candidateJobOrderService;
            _messageTemplateService = messageTemplateService;
            _franchiseService = franchiseService;
            _messageHistoryService = messageHistoryService;
            _workflowMessageService = workflowMessageService;
            _companyService = companyService;
        }
        #endregion
        public virtual void Execute()
        {
            try
            {
                //1. Get all active job order during this period 
                DateTime refTime = DateTime.Now;
                int min = 15;
                IList<JobOrder> jobOrders = _jobOrderService.GetJobOrdersStartingSoon(refTime, min);
                foreach (var jobOrder in jobOrders)
                {
                    //
                    if (_accountService.GetClientCompanyHRAccount(jobOrder.CompanyId) != null || jobOrder.CompanyContactId > 0 )
                    {
                        //check whether an email has been sent to client
                        var messageTemplate = _messageTemplateService.GetMessageTemplateByName("JobOrderPlacementNotification", 0);
                        string subject = messageTemplate.Subject;
                        var company = jobOrder.Company != null ? jobOrder.Company : _companyService.GetCompanyByIdForScheduleTask(jobOrder.CompanyId);
                        subject = subject.Replace("%JobOrder.CompanyName%", company != null ? company.CompanyName : String.Empty);
                        subject = subject.Replace("%JobOrder.Id%", jobOrder.Id.ToString());
                        subject = subject.Replace("%Datetime.RefDate%", refTime.ToString("MM/dd/yyyy"));
                        subject = subject.Replace("%Franchise.Name%", _franchiseService.GetFranchiseById(jobOrder.FranchiseId).FranchiseName);
                        if (!_messageHistoryService.MessageSentOrNot(subject))
                            _workflowMessageService.SendRemindEmailPlacementNotification(jobOrder, refTime, 0);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error occurred while checking whether a placement is email sent or not. Error message : {0}", exc.Message), exc);
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }
    }
}
