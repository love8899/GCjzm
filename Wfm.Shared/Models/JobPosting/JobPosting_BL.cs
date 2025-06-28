using System;
using System.Linq;
using System.Collections.Generic;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using JP_Core = Wfm.Core.Domain.JobPosting;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.JobPosting;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.TimeSheet;
using Wfm.Shared.Extensions;


namespace Wfm.Shared.Models.JobPosting
{
    public class JobPosting_BL
    {
        #region Fields

        private readonly ICandidateService _candidateService;
        private readonly ICandidateBlacklistService _candidateBlacklistService;
        private readonly IAccountService _accountService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IJobPostService _jobPostService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly ICompanyService _companyService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IWorkTimeService _workTimeService;
        private readonly IShiftService _shiftService;
        private readonly IFranchiseService _franchiseService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly IJobOrderTypeService _jobOrderTypeService;
        #endregion


        #region Ctor

        public JobPosting_BL(ICandidateService candidateService,
                             ICandidateBlacklistService candidateBlacklistService,
                             IAccountService accountService,
                             IRecruiterCompanyService recruiterCompanyService,
                             IJobPostService jobPostService,
                             IJobOrderService jobOrderService,
                             ICompanyDivisionService companyDivisionService,
                             ICompanyDepartmentService companyDepartmentService,
                             ICompanyBillingService companyBillingService,
                             ICompanyVendorService companyVendorService,
                             ICompanyService companyService,
                             ICandidateJobOrderService candidateJobOrderService,
                             IWorkTimeService workTimeService,
                             IShiftService shiftService,
                             IFranchiseService franchiseService,
                             ILocalizationService localizationService,
                             IActivityLogService activityLogService,
                             IWorkflowMessageService workflowMessageService,
                             IWorkContext workContext,
                             IJobOrderTypeService jobOrderTypeService)
        {
            _candidateService = candidateService;
            _candidateBlacklistService = candidateBlacklistService;
            _accountService = accountService;
            _recruiterCompanyService = recruiterCompanyService;
            _jobPostService = jobPostService;
            _jobOrderService = jobOrderService;
            _companyDivisionService = companyDivisionService;
            _companyDepartmentService = companyDepartmentService;
            _companyBillingService = companyBillingService;
            _companyVendorService = companyVendorService;
            _companyService = companyService;
            _candidateJobOrderService = candidateJobOrderService;
            _workTimeService = workTimeService;
            _shiftService = shiftService;
            _franchiseService = franchiseService;
            _localizationService = localizationService;
            _activityLogService = activityLogService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _jobOrderTypeService = jobOrderTypeService;
        }

        #endregion


        public JobPostingEditModel CreateNewJobPost(int? companyId = null)
        {
            var model = new JobPostingEditModel();
            model.JobCategoryId = 10;   // Industrial and Manufacturing
            model.IsPublished = false;
            model.JobPostingStatusId = (int)JobOrderStatusEnum.Active;
            model.StartDate = DateTime.Today;
            model.NumberOfOpenings = 1;
            model.EnteredBy = _workContext.CurrentAccount.Id;
            model.FranchiseId = _workContext.CurrentFranchise.Id;
            model.IsSubmitted = true;
            model.SubmittedOnUtc = DateTime.UtcNow;
            model.CreatedOnUtc = DateTime.UtcNow;
            model.UpdatedOnUtc = DateTime.UtcNow;
            model.MondaySwitch = true;
            model.TuesdaySwitch = true;
            model.WednesdaySwitch = true;
            model.ThursdaySwitch = true;
            model.FridaySwitch = true;
            var type = _jobOrderTypeService.GetJobOrderTypeByTypeName("Temporary");
            model.JobTypeId = type == null ? 1 : type.Id;
            if (companyId.HasValue)
            {
                model.CompanyId = companyId.Value;
                model.CompanyName = _companyService.GetCompanyById(companyId.Value).CompanyName;
                model.IsSubmitted = false;
                model.SubmittedOnUtc = null;
            }

            return model;
        }


        public Guid SaveJobPost(JobPostingEditModel model)
        {
            var entity = model.ToEntity();
            _jobPostService.InsertJobPost(entity);           
            return entity.JobPostingGuid;
        }


        public IQueryable<JobPostingPublishModel> GetPublishStatus(Guid? guid, DateTime refDate)
        {
            var jobPosting = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (jobPosting == null)
                return null;
            var jobOrders = _jobOrderService.GetAllJobOrdersByJobPostingIdAsQueryable(_workContext.CurrentAccount, jobPosting.Id);
            var vendors = _companyVendorService.GetAllCompanyVendorsByCompanyId(jobPosting.CompanyId).Select(x => x.Vendor);
            var openings = _jobOrderService.GetAllJobOrderOpeningsByDate(_workContext.CurrentAccount, refDate);
            var rateCodes = GetAllBillingRateCodesByJobPosting(jobPosting, refDate);

            var placedNums = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(jobPosting.CompanyId, refDate)
                             .GroupBy(grp => grp.JobOrderId)
                             .Select(x => new
                             {
                                 JobOrderId = x.Key,
                                 Placed = x.Count()
                             });

            var result = from v in vendors
                         from j in jobOrders.Where(j => j.FranchiseId == v.Id).DefaultIfEmpty()
                         from o in openings.Where(o => o.JobOrderId == j.Id).DefaultIfEmpty()
                         from r in rateCodes.Where(r => r.FranchiseId == v.Id).DefaultIfEmpty()
                         from p in placedNums.Where(p => p.JobOrderId == j.Id).DefaultIfEmpty()
                         select new JobPostingPublishModel()
                         {
                             JobPostingId = jobPosting.Id,
                             VendorId = v.Id,
                             VendorName = v.FranchiseName,
                             VendorGuid=v.FranchiseGuid,
                             CompanyId = jobPosting.CompanyId,
                             Opening = o != null ? o.OpeningNumber : 0,
                             BillingRateCode = r != null ? r.Position.Name + " / " + r.ShiftCode : null,
                             JobOrderId = j != null ? j.Id : 0,
                             Placed = p != null ? p.Placed : 0,
                             Shortage = (o != null ? o.OpeningNumber : 0) - (p != null ? p.Placed : 0),
                             JobOrderGuid = j!=null? j.JobOrderGuid : Guid.Empty
                         };

            return result.OrderByDescending(x => x.Opening).ThenBy(x => x.VendorId).Distinct();
        }


        public IQueryable<CompanyBillingRate> GetAllBillingRateCodesByJobPosting(JP_Core.JobPosting jobPosting, DateTime refDate)
        {        
            var shiftCode = _shiftService.GetShiftById(jobPosting.ShiftId).ShiftName;
            return _companyBillingService.GetCompanyBillingRatesByCompanyIdAndLocationIdAndRateCodeAndDate(jobPosting.CompanyId, jobPosting.CompanyLocationId, jobPosting.PositionId, shiftCode, refDate);
        }


        public string PublishJobPosting(JobPostingPublishModel model, DateTime refDate, out int jobOrderId)
        {
            var result = String.Empty;
            jobOrderId = 0;
            var jobOrder = _jobOrderService.GetJobOrderByJobPostingIdAndVendorId(model.JobPostingId, model.VendorId);

            if (jobOrder == null && model.Opening > 0)
            {
                var jobPosting = _jobPostService.RetrieveJobPost(model.JobPostingId);
                var vendorAccounts = _accountService.GetAllAccountsAsQueryable(_workContext.CurrentAccount).Where(x => x.FranchiseId == model.VendorId );

                var vendorOwner = vendorAccounts.Where(x=> x.AccountRoles.Where(y => y.SystemName == AccountRoleSystemNames.VendorAdministrators).Any()).FirstOrDefault();

                if (vendorOwner == null)
                {
                    result = String.Format("The admin account for {0} cannot be found. A Supervisor or recruiter account will be picked as the owner of created job orders.", model.VendorName);

                    var recruiters = _recruiterCompanyService.GetAllRecruitersByCompanyId(model.CompanyId).Where(x => x.Account.FranchiseId == model.VendorId);

                    // Since we can't find the vendor admin, we will pick the first vendor's supervisor accoutn as the owner
                    var recruiter = recruiters.Where(x => x.Account.AccountRoles.Where(y => y.SystemName == AccountRoleSystemNames.VendorRecruiterSupervisors).Any())
                                    .FirstOrDefault();

                    // Our second fall-back plan would be a vendor recruiter role
                    if (recruiter == null)
                    {
                        recruiter = recruiters.Where(x => x.Account.AccountRoles.Where(y => y.SystemName == AccountRoleSystemNames.VendorRecruiters).Any())
                                    .FirstOrDefault();
                    }

                    if (recruiter != null)
                        vendorOwner = vendorAccounts.Where(x => x.Id == recruiter.AccountId).First();
                }

                if (vendorOwner != null)
                {
                    jobOrder = AutoMapper.Mapper.Map<Wfm.Core.Domain.JobOrders.JobOrder>(jobPosting);
                    jobOrder.JobPostingId = model.JobPostingId;
                    jobOrder.BillingRateCode = model.BillingRateCode + " / " + model.VendorId;
                    jobOrder.SchedulePolicyId = jobPosting.SchedulePolicyId;
                    jobOrder.JobOrderTypeId = jobPosting.JobTypeId;
                    jobOrder.JobOrderCategoryId = jobPosting.JobCategoryId;
                    jobOrder.JobOrderStatusId = (int)JobOrderStatusEnum.Active;
                    jobOrder.IsPublished = false;
                    jobOrder.IsPublished = false;
                    jobOrder.IsHot = false;
                    jobOrder.AllowSuperVisorModifyWorkTime = true;
                    jobOrder.RecruiterId = vendorOwner.Id;
                    jobOrder.OwnerId = vendorOwner.Id;
                    jobOrder.EnteredBy = _workContext.CurrentAccount.Id;
                    jobOrder.FranchiseId = model.VendorId;
                    jobOrder.IsDeleted = false;
                    jobOrder.IncludeHolidays = jobPosting.IncludeHolidays;
                    jobOrder.MondaySwitch = jobPosting.MondaySwitch;
                    jobOrder.TuesdaySwitch = jobPosting.TuesdaySwitch;
                    jobOrder.WednesdaySwitch = jobPosting.WednesdaySwitch;
                    jobOrder.ThursdaySwitch = jobPosting.ThursdaySwitch;
                    jobOrder.FridaySwitch = jobPosting.FridaySwitch;
                    jobOrder.SaturdaySwitch = jobPosting.SaturdaySwitch;
                    jobOrder.SundaySwitch = jobPosting.SundaySwitch;

                    _jobOrderService.InsertJobOrder(jobOrder);
                    jobOrderId = jobOrder.Id;
                }
                else
                {
                    result = String.Format("There are no accounts defined for {0} that can be used as job order owner. This vendor cannnot be used in the publish task.", model.VendorName);
                }
            }

            if (jobOrder != null)
                _jobOrderService.UpdateJobOrderOpenings(jobOrder.Id, refDate, model.Opening, null);

            return result;
        }


        public void UpdateJobPostingPublishStatus(int jobPostingId)
        {
            var jobPosting = _jobPostService.RetrieveJobPost(jobPostingId);

            // if not submitted yet
            if (!jobPosting.IsSubmitted)
            {
                jobPosting.IsSubmitted = true;
                jobPosting.SubmittedBy = _workContext.CurrentAccount.Id;
                jobPosting.SubmittedOnUtc = DateTime.UtcNow;
            }

            jobPosting.IsPublished = true;
            jobPosting.PublishedOnUtc = DateTime.UtcNow;

            _jobPostService.UpdateJobPost(jobPosting);
        }


        public string CloseJobOrdersByJobPosting(JP_Core.JobPosting jobPosting, DateTime CloseDate, int CloseReason)
        {
            string result = null;

            var jobOrders = _jobPostService.GetAllJobOrdersByJobPostingId(jobPosting.Id).ToList();
            var jobOrderIds = jobOrders.Select(x => x.Id).ToList();

            // check if all work time (till today) are approved
            if (CloseDate <= DateTime.Today && !_workTimeService.IfAllJobOrderWorkTimeApproved(jobOrderIds, CloseDate))
                result += string.Format("There are time sheets for the job posting that are not approved yet. Please approve or void them first.");

            else
            {
                jobPosting.EndDate = CloseDate;
                jobPosting.CloseReason = CloseReason;
                jobPosting.JobPostingStatusId = (int)JobOrderStatusEnum.Closed;
                _jobPostService.UpdateJobPost(jobPosting);

                foreach (var jobOrder in jobOrders)
                    this.CloseJobOrder(jobOrder, CloseDate, CloseReason);
            }

            return result;
        }


        public void CloseJobOrder(JobOrder jobOrder, DateTime CloseDate, int CloseReason)
        {
            jobOrder.EndDate = CloseDate;
            jobOrder.JobOrderCloseReason = (JobOrderCloseReasonCode)CloseReason;
            _jobOrderService.UpdateJobOrder(jobOrder);

            //Change all placed candidates end date 
            _candidateJobOrderService.SetEndDateAfterClosingJobOrder(jobOrder.Id, CloseDate);

            //log
            _activityLogService.InsertActivityLog("SetJobOrderEndDate", _localizationService.GetResource("ActivityLog.SetJobOrderEndDate"), jobOrder.Id);
        }


        public JobPostingEditModel GetCopyOfJobPosting(Guid? guid, out string errors, bool byClient = false)
        {
            errors = String.Empty;

            var jobPosting = _jobPostService.RetrieveJobPostingByGuid(guid);
            if (jobPosting == null)
            {
                errors = "The job posting does not exist!";
                return null;
            }

            JobPostingEditModel model = jobPosting.ToEditModel();
            model.Id = 0;
            //model.ShiftId = 0;
            model.CompanyName = _companyService.GetCompanyById(model.CompanyId).CompanyName;
            model.StartDate = DateTime.Today.AddDays(1);
            //model.StartTime = DateTime.Today;
            model.EndDate = null;
            //model.EndTime = DateTime.Today;
            model.NumberOfOpenings = 0;
            model.IsPublished = false;
            model.FranchiseId = _franchiseService.GetDefaultMSPId();
            model.IsDeleted = false;
            model.JobPostingStatusId = (int)JobOrderStatusEnum.Active;
            model.CreatedOnUtc = DateTime.UtcNow;
            model.UpdatedOnUtc = model.CreatedOnUtc;
            model.PublishedOnUtc = null;
            model.SubmittedOnUtc = DateTime.UtcNow;

            if (byClient)
            {
                model.IsSubmitted = false;
                model.SubmittedOnUtc = null;
            }

            return model;
        }


        public IList<PlacementRejectionModel> JobOrderPlacedCandidateList(Account account, int jobOrderId, DateTime refDate)
        {
            var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
            var candidatesBanned = _candidateBlacklistService.GetAllCandidateBlacklistsByCompanyIdAndDate(jobOrder.CompanyId, refDate);
            var placement = _candidateJobOrderService.GetCandidateJobOrderByJobOrderIdAndDateRange(jobOrderId, refDate, refDate)
                            .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed);

            var result = from p in placement
                         from c in candidatesBanned.Where(c => (c.Candidate.SocialInsuranceNumber != null && p.Candidate.SocialInsuranceNumber == c.Candidate.SocialInsuranceNumber) || 
                                                               (c.Candidate.BirthDate.HasValue && p.Candidate.BirthDate == c.Candidate.BirthDate && 
                                                                p.Candidate.FirstName == c.Candidate.FirstName && p.Candidate.LastName == c.Candidate.LastName))
                                                   .Take(1).DefaultIfEmpty()
                         select new PlacementRejectionModel
                         {
                             CandidateGuid = p.Candidate.CandidateGuid,
                             CandidateId = p.CandidateId,
                             CandidateName = (p.Candidate.FirstName ?? String.Empty) + " " + (p.Candidate.LastName ?? String.Empty),
                             Email = p.Candidate.Email,
                             HomePhone = p.Candidate.HomePhone,
                             MobilePhone = p.Candidate.MobilePhone,
                             AssociatedGuid = c != null ? c.Candidate.CandidateGuid : (Guid?)null,
                             AssociatedId = c != null ? c.CandidateId : (int?)null,
                             IsBanned = c != null,
                             BannedReason = c != null ? c.BannedReason : null,
                             JobOrderId = p.JobOrderId,
                             EmployeeId=p.Candidate.EmployeeId,
                         };

            return result.ToList();
        }


        public string RemoveCandidateFromPlacement(int jobOrderId, int candidateId, DateTime refDate, string reason, string comment)
        {
            var result = String.Empty;

            var placements = _candidateJobOrderService.GetAllCandidateJobOrdersByJobOrderIdAndCandidateIdAndDate(jobOrderId, candidateId, refDate);

            foreach (var cjo in placements.ToList())
            {
                var msg = _candidateJobOrderService.RemovePlacement(cjo);
                if (!String.IsNullOrEmpty(msg))
                    result += msg + "\r\n";
                else
                {
                    _activityLogService.InsertActivityLog("RejectPlacement", _localizationService.GetResource("ActivityLog.RejectPlacement"),
                                                           candidateId, jobOrderId, cjo.StartDate.ToString("yyyy-MM-dd"), reason, comment);
                    // navigation properties () were cleared by the deletion above !!!
                    cjo.Candidate = _candidateService.GetCandidateById(cjo.CandidateId);
                    cjo.JobOrder = _jobOrderService.GetJobOrderById(cjo.JobOrderId);
                    _workflowMessageService.SendNotificationToRecruiterForPlacmentRejection(cjo, reason, comment, _workContext.CurrentAccount, languageId: 1);
                }
            }

            return result;
        }


        public void SendEmailNotificationForPublishJobPosting(List<string> jobOrderIds, int jobPostingId, bool toClient = true)  
        {
            var jobPosting = _jobPostService.RetrieveJobPost(jobPostingId);

            if (toClient)
                _workflowMessageService.SendJobPostingPublishNotification(jobPosting, jobOrderIds, languageId: 1);
            
            _workflowMessageService.SendJobPostingPublishNotificationToVendors(jobOrderIds);
        }

    }

}
