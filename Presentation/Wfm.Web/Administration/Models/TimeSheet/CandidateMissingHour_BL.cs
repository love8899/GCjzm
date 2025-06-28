using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wfm.Services.TimeSheet;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Messages;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.ExportImport;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;


namespace Wfm.Admin.Models.TimeSheet
{
    public class CandidateMissingHour_BL
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IMissingHourService _missingHourService;
        private readonly IJobOrderService _jobOrderService;

        #endregion

        #region Ctor

        public CandidateMissingHour_BL(
            ILogger logger,
            ILocalizationService localizationService,
            IMissingHourService missingHourService,
            IJobOrderService jobOrderService)
        {
            _logger = logger;
            _localizationService = localizationService;
            _missingHourService = missingHourService;
            _jobOrderService = jobOrderService;
        }

        #endregion


        public IQueryable<CandidateMissingHour> GetAllCandidateMissingHour(int companyId = 0, int candidateId = 0, DateTime? jobStartDate = null, DateTime? jobEndDate = null, 
            Account account = null, bool includeProcessed = false)
        {
            var missingHours = _missingHourService.GetAllCandidateMissingHourAsQueryable(isAccountBased: true)
                               .Where(x => includeProcessed || x.CandidateMissingHourStatusId != (int)CandidateMissingHourStatus.Processed);

            if (companyId > 0)
                missingHours = missingHours.Where(x => x.JobOrder.CompanyId == companyId);
            if (candidateId > 0)
                missingHours = missingHours.Where(x => x.CandidateId == candidateId);
            if (jobStartDate.HasValue)
                missingHours = missingHours.Where(x => x.WorkDate >= jobStartDate);
            if (jobEndDate.HasValue)
                missingHours = missingHours.Where(x => x.WorkDate <= jobEndDate);

            if (account != null && account.IsRecruiterOrRecruiterSupervisor())
            {
                var jobOrderIds = _jobOrderService.GetAllJobOrdersAsQueryable(account).Where(x => x.OwnerId == account.Id || x.RecruiterId == account.Id)
                                  .Select(x => x.Id);
                missingHours = missingHours.Where(x => jobOrderIds.Contains(x.JobOrderId));
            }
            
            return missingHours;
        }


        public string SaveMissingHour(CandidateMissingHourModel model)
        {
            var result = String.Empty;

            try
            {
                result = _missingHourService.InsertOrUpdateMissingHour(model.JobOrderId, model.CandidateId, model.WorkDate, model.NewHours, model.Note);
            }
            catch (WfmException ex)
            {
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                result = _localizationService.GetResource("Common.UnexpectedError");
                _logger.Error(ex2.Message, ex2);
            }

            return result;
        }


        public string ChangeMissingHourStatus(CandidateMissingHourModel model, int accountId)
        {
            var result = String.Empty;

            try
            {
                var missingHour = _missingHourService.GetMissingHourById(model.Id);
                if (missingHour == null)
                    result = "The missing hour does not exist.";

                // skip approved
                //else if (missingHour.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Approved)
                //    result = "The missing hour is approved already.";

                // same status
                else if (model.CandidateMissingHourStatusId == missingHour.CandidateMissingHourStatusId)
                    result = "The status must be different.";

                else
                {
                    missingHour.CandidateMissingHourStatusId = model.CandidateMissingHourStatusId;

                    if (missingHour.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Approved)
                    {
                        missingHour.ApprovedBy = accountId;
                        missingHour.ApprovedOnUtc = DateTime.UtcNow;
                    }
                    
                    missingHour.Note = model.Note;

                    _missingHourService.UpdateCandidateMissingHour(missingHour);
                }
            }
            catch (WfmException ex)
            {
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                result = _localizationService.GetResource("Common.UnexpectedError");
                _logger.Error(ex2.Message, ex2);
            }

            return result;
        }


        public string ProcessMissingHour(ICandidateJobOrderService _candidateJobOrderService, IWorkTimeService _workTimeService, IRepository<CandidateWorkTimeLog> _workTimeLogRepository, 
                        int missingHourId, int accountId, out string warning)
        {
            var result = String.Empty;
            warning = String.Empty;

            try
            {
                var missingHour = _missingHourService.GetMissingHourById(missingHourId);
                if (missingHour == null)
                    result = "The missing hour does not exist.";

                // create/update placement
                warning = _CreateOrUpdatePlacement(_candidateJobOrderService, missingHour, accountId);

                // generate/approve worktime, and insert worktime log
                if (missingHour.OrigHours == 0)
                {
                    var workTimeId = _workTimeService.InsertOrUpdateWorkTime(missingHour.JobOrderId, missingHour.CandidateId, missingHour.WorkDate, missingHour.NewHours, WorkTimeSource.Manual, "missing hour", overwriteOrig: true);
                    if (workTimeId == 0)
                        result = "Cannot generate the timesheet.";
                    else
                    {
                        // TODO: check if the worktime is not for this invoice term???
                        _InsertWorkTimeLog(_workTimeLogRepository, workTimeId, missingHour.OrigHours, missingHour.NewHours);
                    }
                }

                // change status
                missingHour.CandidateMissingHourStatusId = (int)CandidateMissingHourStatus.Processed;
                missingHour.ProcessedBy = accountId;
                missingHour.ProcessedOnUtc =  DateTime.UtcNow;

                _missingHourService.UpdateCandidateMissingHour(missingHour);
            }
            catch (WfmException ex)
            {
                result = ex.Message;
            }
            catch (Exception ex2)
            {
                result = _localizationService.GetResource("Common.UnexpectedError");
                _logger.Error(ex2.Message, ex2);
            }

            return result;
        }


        public IQueryable<CandidateMissingHour> GetMissingHourByIds(string selectedIds)
        {
            if (String.IsNullOrWhiteSpace(selectedIds))
                return Enumerable.Empty<CandidateMissingHour>().AsQueryable();

            var ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x));
            var missingHours = GetAllCandidateMissingHour(includeProcessed: true).Where(x => ids.Contains(x.Id));

            return missingHours;
        }


        public QueuedEmailModel AskForApprovalEmailDraft(IExportManager _exportManager, IAccountService _accountService, 
                                    IWorkflowMessageService _workflowMessageService, IFranchiseService _franchiseService,
                                    Account account, string ids)
        {
            var missingHours = GetMissingHourByIds(ids).Where(x => x.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.PendingApproval);
            if (!missingHours.Any())
                return null;

            var missingHour = missingHours.FirstOrDefault();
            var supervisors = missingHours.Select(x => x.JobOrder.CompanyContact).Distinct().Select(x => x.Email);
            var hrManagers = _accountService.GetAllCompanyHrAccountsByCompany(missingHour.JobOrder.CompanyId).Select(x => x.Email);
            var vendorName = _franchiseService.GetFranchiseById(missingHour.JobOrder.FranchiseId).FranchiseName;
            var singleRecord = missingHours.Count() == 1;

            var To = singleRecord ? (missingHour.JobOrder.CompanyContact != null ? missingHour.JobOrder.CompanyContact.Email : string.Empty) : String.Join(";", hrManagers);
            var CC = singleRecord ? String.Join(";", hrManagers) : String.Join(";", supervisors);
            CC = String.IsNullOrWhiteSpace(CC) ? account.Email : String.Concat(CC, ";", account.Email);

            var msgTmpltName = singleRecord ? "MissingHour.AskForApprovalMessage(Single)" : "MissingHour.AskForApprovalMessage";
            var messageTemplate = _workflowMessageService.GetActiveMessageTemplate(msgTmpltName, 1);
            var subject = _UpdateMissingHourMsgSubject(missingHour, vendorName, messageTemplate.Subject);
            var body = messageTemplate.Body;
            if (singleRecord)
                body = _UpdateMissingHourMsgBody(missingHour, body);

            var model = new QueuedEmailModel()
            {
                EmailAccountId = messageTemplate.EmailAccountId,
                Priority = 5,
                From = account.Email,
                FromName = account.FullName,
                FromAccountId = account.Id,
                To = To,
                CC = CC,
                Subject = subject,
                Body = body,
                MessageCategoryId = messageTemplate.MessageCategoryId
            };

            if (!singleRecord)
            {
                byte[] attachmentBytes = null;
                var attachmentName = _GetMissingHourExcelFile(_exportManager, missingHours, out attachmentBytes);
                model.AttachmentFileName = attachmentName;
                model.AttachmentFile = attachmentBytes;
            }

            return model;
        }


        public string SendAskForApprovalEmail(IQueuedEmailService _queuedEmailService, IAccountService _accountService, QueuedEmailModel model, string ids)
        {
            var result = String.Empty;
            var email = model.ToEntity();
            
            // set FROM
            var sender = _accountService.GetAccountByEmail(email.From);
            if (sender == null)
                result = String.Format("From address [{0}] is invalid.", email.From);
            else
            {
                email.FromName = sender.FullName;
                email.FromAccountId = sender.Id;
            }

            // set TO
            var receivers = email.To.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (receivers.Count() == 1)
            {
                var receiver = _accountService.GetAccountByEmail(receivers.First());
                if (receiver != null)
                {
                    email.ToName = receiver.FullName;
                    email.ToAccountId = receiver.Id;
                }
            }

            if (!String.IsNullOrWhiteSpace(result))
                return result;

            // set ReplyTo
            email.ReplyTo = email.From;
            email.ReplyToName = email.FromName;

            // attachment
            email.AttachmentFile = email.AttachmentFile == null || email.AttachmentFile.Length == 0 ? null : email.AttachmentFile;
            email.AttachmentFile2 = null;
            email.AttachmentFile3 = null;

            email.CreatedOnUtc = email.UpdatedOnUtc = DateTime.UtcNow;
            _queuedEmailService.InsertQueuedEmail(email);
            
            var missingHours = GetMissingHourByIds(ids).ToList();
            foreach (var missingHour in missingHours)
            {
                missingHour.LastAskForApprovalOnUtc = DateTime.UtcNow;
                _missingHourService.UpdateCandidateMissingHour(missingHour);
            }

            return result;
        }


        private string _CreateOrUpdatePlacement(ICandidateJobOrderService _candidateJobOrderService, CandidateMissingHour missingHour, int enteredBy)
        {
            return _candidateJobOrderService.CreateOrSavePlacements(new CandidateJobOrder()
            {
                CandidateId = missingHour.CandidateId,
                JobOrderId = missingHour.JobOrderId,
                StartDate = missingHour.WorkDate,
                EndDate = missingHour.WorkDate,
                CandidateJobOrderStatusId = (int)CandidateJobOrderStatusEnum.Placed,
                EnteredBy = enteredBy
            });
        }


        private void _InsertWorkTimeLog(IRepository<CandidateWorkTimeLog> workTimeLogRepository, int workTimeId, decimal origHours, decimal newHOurs)
        {
            workTimeLogRepository.Insert(new CandidateWorkTimeLog()
            {
                CandidateWorkTimeId = workTimeId,
                OriginalHours = origHours,
                NewHours = newHOurs,
                CreatedOnUtc = DateTime.UtcNow
            });
        }


        private string _GetMissingHourExcelFile(IExportManager _exportManager, IEnumerable<CandidateMissingHour> missingHours, out byte[] bytes)
        {
            var fileName = String.Empty;
            using (var stream = new MemoryStream())
            {
                fileName = _exportManager.ExportMissingHourToXlsx(stream, missingHours, toClient: true);
                bytes = stream.ToArray();
            }

            return fileName;
        }


        private string _UpdateMissingHourMsgSubject(CandidateMissingHour missingHour, string vendorName, string subject)
        {
            var result = subject.Replace("%MissingHour.CompanyName%", missingHour.JobOrder.Company.CompanyName);
            result = result.Replace("%MissingHour.EmployeeId%", missingHour.Candidate.Id.ToString());
            result = result.Replace("%MissingHour.EmployeeName%", missingHour.Candidate.GetFullName());
            result = result.Replace("%MissingHour.VendorName%", vendorName);

            return result;
        }


        private string _UpdateMissingHourMsgBody(CandidateMissingHour missingHour, string body)
        {
            var result = body.Replace("%MissingHour.EmployeeId%", missingHour.Candidate.Id.ToString());
            result = result.Replace("%MissingHour.EmployeeName%", missingHour.Candidate.GetFullName());
            result = result.Replace("%MissingHour.ReportDate%", missingHour.CreatedOnUtc.Value.ToShortDateString());
            var deptName = missingHour.JobOrder.CompanyDepartmentId == 0 ? string.Empty : missingHour.JobOrder.Company.CompanyDepartments.Where(x => x.Id == missingHour.JobOrder.CompanyDepartmentId).FirstOrDefault().DepartmentName;
            result = result.Replace("%MissingHour.Department%", deptName);
            result = result.Replace("%MissingHour.Shift%", missingHour.JobOrder.Shift.ShiftName);
            var supervisor = missingHour.JobOrder.CompanyContact == null ? string.Empty : missingHour.JobOrder.CompanyContact.FullName;
            result = result.Replace("%MissingHour.Supervisor%", supervisor);
            var workDateTime = String.Format("{0} to {1}, {2}", missingHour.JobOrder.StartTime.ToString("h:mmtt"), missingHour.JobOrder.EndTime.ToString("h:mmtt"), missingHour.WorkDate.ToShortDateString());
            result = result.Replace("%MissingHour.DateTime%", workDateTime);
            result = result.Replace("%MissingHour.NewHours%", missingHour.NewHours.ToString("n"));

            return result;
        }

    }
}
