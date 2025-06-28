using System;
using System.IO;
using System.Linq;
using Wfm.Core;
using Wfm.Services.Candidates;
using Wfm.Services.ExportImport;
using Wfm.Services.JobOrders;
using Wfm.Services.Messages;
using Wfm.Admin.Extensions;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Companies;
using System.Text;
using Wfm.Services.Logging;

namespace Wfm.Admin.Models.Messages
{
    public class QueuedMailModel_BL
    {
        private readonly IJobOrderService _jobOrderService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidateService _candidatesService;
        private readonly IExportManager _exportManager;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly ICompanyEmailTemplateService _companyEmailTemplateService;
        private readonly ILogger _loggger;
        public QueuedMailModel_BL(IJobOrderService jobOrderService,
                                  ICandidateJobOrderService candidateJobOrderService,
                                  ICandidateService candidatesService,
                                  IExportManager exportManager,
                                  IWorkflowMessageService workflowMessageService,
                                  IWorkContext workContext,
                                  ICompanyEmailTemplateService companyEmailTemplateService,
                                  ILogger loggger)
        {
            _jobOrderService = jobOrderService;
            _candidateJobOrderService = candidateJobOrderService;
            _candidatesService = candidatesService;
            _exportManager = exportManager;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _companyEmailTemplateService = companyEmailTemplateService;
            _loggger = loggger;
        }

        public QueuedEmailModel GetAttendanceListQueuedEmailModel(Guid? guid, DateTime? inquiryDate, string note, out string error)
        {
            var jo = _jobOrderService.GetJobOrderByGuid(guid);
            error = string.Empty;
            string fileName = string.Empty;
            string filePath = string.Empty;
            if (jo == null)
            {
                error = "The job order does not exist!";
                return null;
            }
            if (inquiryDate == null)
            {
                error = "Please select date!";
                return null;
            }

            //save attendance list file
            var fromDate = inquiryDate.Value.AddDays(DayOfWeek.Sunday - inquiryDate.Value.DayOfWeek);
            var toDate = fromDate.AddDays(6);
            var placedCandidates = _candidateJobOrderService.GetCandidateJobOrderByJobOrderIdAndDateRange(jo.Id, inquiryDate.Value, inquiryDate.Value);
            int[] ids = placedCandidates.Select(x => x.Id).ToArray();

            try
            {
                if (ids.Count() > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        fileName = _exportManager.ExportDailyAttendanceConfirmationListToXlsx(stream, jo.CompanyId, inquiryDate.Value, ids);
                        filePath = Path.GetTempPath();
                        System.IO.File.WriteAllBytes(filePath + fileName, stream.ToArray());

                    }
                    var email = _workflowMessageService.GetJobOrderPlacementEmail(jo, inquiryDate.Value, _workContext.CurrentFranchise, _workContext.WorkingLanguage.Id, fileName, filePath, note, out error);
                    return email.ToModel();
                }
                else
                {
                    error = String.Format("There are no candidates placed in Job Order {0} on {1}.", jo.Id, inquiryDate.Value.ToString("MM/dd/yyyy"));
                    return null;
                }

            }
            catch (Exception exc)
            {
                error = exc.Message;
                return null;
            }

        }

        public QueuedEmailModel GetPreviewEmailModel(string source, int jobOrderId, DateTime startDate, DateTime? endDate, int templateId, out string error)
        {
            error = string.Empty;
            QueuedEmail template = null;

            if (source.Equals("CMP"))
            {
                // For confirmation emails
                var companyTemplate = _companyEmailTemplateService.Retrieve(templateId);
                if (companyTemplate == null)
                {
                    error = "Template ID is invalid";
                    return null;
                }

                template = _workflowMessageService.GetCandidateConfirmationEmail(jobOrderId, 0, startDate, endDate, (int)CompanyEmailTemplateType.Confirmation, out  error,_workContext.WorkingLanguage.Id,true);
            }
            else
            {
                // for test emails
                template =  _workflowMessageService.GetCandidateRequiredTestsEmail(null, _workContext.WorkingLanguage.Id, 0);
            }

            if (error.Length > 0 || template == null)
                return null;

            return template.ToModel();
        }

        public void SendJobOrderConfirmationEmail(int jobOrderId, DateTime startDate, DateTime? endDate, int templateId, string selectedIds, out string error)
        {
            error = string.Empty;

            var jo = _jobOrderService.GetJobOrderById(jobOrderId);
            if (jo == null)
            {
                error = "The job order does not exist!";
                return;
            }

            StringBuilder errors = new StringBuilder();
            var ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

            foreach (int id in ids)
            {
                var email = _workflowMessageService.GetCandidateConfirmationEmail(jobOrderId, id, startDate, endDate, (int)CompanyEmailTemplateType.Confirmation, out error);
                if (String.IsNullOrEmpty(error) && email != null)
                    _workflowMessageService.SendConfirmationToEmployeeNotification(email);
                else
                    errors.AppendLine(error); 
            }

            if (errors.Length > 0)
                error = errors.ToString();
        }

        public QueuedEmailModel GetCandidateConfirmationEmail(Guid? candidateGuid, Guid? jobOrderGuid, DateTime from, DateTime? to, out string error)
        {
            error = string.Empty;
            var candidate = _candidatesService.GetCandidateByGuid(candidateGuid);
            if (candidate == null)
            {
                error = "The candidate does not exist!";
                return null;
            }

            var jobOrder = _jobOrderService.GetJobOrderByGuid(jobOrderGuid);
            if (jobOrder == null)
            {
                error = "The job order does not exist!";
                return null;
            }

            var email = _workflowMessageService.GetCandidateConfirmationEmail(jobOrder.Id, candidate.Id, from, to, (int)CompanyEmailTemplateType.Confirmation, out error);
            if (String.IsNullOrEmpty(error))
                return email.ToModel();
            else
                return null;
        }

        public void SendGeneralEmail(int templateId, string selectedIds, int? jobOrderId,DateTime? start,DateTime? end,out string error)
        {
            error = string.Empty;
            int counter = 0;

            StringBuilder errors = new StringBuilder();
            var ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

            foreach (int id in ids)
            {
                var candidate = _candidatesService.GetCandidateById(id);
                if (candidate != null && candidate.FranchiseId == _workContext.CurrentAccount.FranchiseId)
                {
                    int sent = _workflowMessageService.SendGeneralEmail(templateId,jobOrderId,candidate,start,end, 1,out error);
                    if (sent > 1)
                        counter++;
                    else
                        errors.AppendLine(error);
                }
                else
                    _loggger.Error(String.Format("Candidate {0} does not exist or Candidate does not belong to current franchise!", id));
            }
            if(errors.Length>0)
                _loggger.Error(errors.ToString());

            if (ids.Count != counter)
            {                
                error = String.Format("{0} emails were failed.", ids.Count - counter);
            }
        }
    }
}