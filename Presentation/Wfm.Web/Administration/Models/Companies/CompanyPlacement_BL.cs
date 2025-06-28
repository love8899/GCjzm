using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.JobOrders;
using Wfm.Services.Security;


namespace Wfm.Admin.Models.Companies
{
    public class CompanyPlacement_BL
    {
        #region Fields

        private readonly IAccountService _accountService;
        private readonly ICandidateBlacklistService _candidateBlacklistService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly IJobOrderService _jobOrderService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICompanyContactService _companyContactService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CompanyPlacement_BL(
            IAccountService accountService,
            ICandidateBlacklistService candidateBlacklistService,
            ICompanyCandidateService companyCandidateService,
            IJobOrderService jobOrderService,
            ICompanyContactService companyContactService,
            IPermissionService permissionService,
            ICandidateJobOrderService candidateJobOrderService,
            IWorkContext workContext)
        {
            _accountService = accountService;
            _candidateBlacklistService = candidateBlacklistService;
            _companyCandidateService = companyCandidateService;
            _jobOrderService = jobOrderService;
            _candidateJobOrderService = candidateJobOrderService;
            _companyContactService = companyContactService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        #endregion


        public IList<PlacementSummaryModel> GetCompanyPlacementSummary(Account account, int companyId, DateTime refDate)
        {
            var jobOrders = _jobOrderService.GetJobOrdersByAccountAndCompany(account, companyId, refDate);

            var openings = _jobOrderService.GetAllJobOrderOpeningsByDate(account, refDate);

            var placedNums = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(companyId, refDate)
                             .GroupBy(grp => grp.JobOrderId)
                             .Select(x => new
                             {
                                 JobOrderId = x.Key,
                                 Placed = x.Count()
                             });

            var supervisors = _companyContactService.GetAllCompanyContactsAsQueryable();
            var HR = _accountService.GetClientCompanyHRAccount(companyId);
            var result = from j in jobOrders
                         from s in supervisors.Where(s => j.CompanyContactId == s.Id).DefaultIfEmpty()
                         from o in openings.Where(o => j.Id == o.JobOrderId).DefaultIfEmpty()
                         from p in placedNums.Where(p => j.Id == p.JobOrderId).DefaultIfEmpty()
                         select new PlacementSummaryModel
                         {
                             JobOrderId = j.Id,
                             JobOrderGuid = j.JobOrderGuid,
                             JobTitle = j.JobTitle,
                             Supervisor = s == null ? null : s.FirstName + " " + s.LastName,
                             RefDate = refDate,
                             Opening = o == null ? 0 : o.OpeningNumber,
                             Placed = p == null ? 0: p.Placed,
                             CanSendEmail = (s!=null || HR !=null) && (p!=null && p.Placed>0) 
                         };

            return result.ToList();
        }


        public IList<PlacementDetailsModel> JobOrderPipelinePlaced(int jobOrderId, DateTime refDate)
        {
            var placement = _candidateJobOrderService.GetCandidateJobOrderByJobOrderIdAndDateRange(jobOrderId, refDate, refDate)
                            .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed);

            var result = from p in placement
                         select new PlacementDetailsModel
                         {
                             CandidateGuid = p.Candidate.CandidateGuid,
                             CandidateId = p.CandidateId,
                             EmployeeId = p.Candidate.EmployeeId,
                             CandidateName = (p.Candidate.FirstName ?? String.Empty) + " " + (p.Candidate.LastName ?? String.Empty),
                             Email = p.Candidate.Email,
                             HomePhone = p.Candidate.HomePhone,
                             MobilePhone = p.Candidate.MobilePhone
                         };

            return result.ToList();
        }


        public IList<PlacementDetailsModel> GetCompanyPlacementDetails(Account account, int companyId, DateTime startDate, DateTime endDate)
        {
            var candidates = _companyCandidateService.GetCompanyCandidatesByAccountAndCompany(account, companyId, startDate);
            var bannedByCompany = _candidateBlacklistService.GetAllCandidateBlacklistsByDate(startDate)
                                  .Where(x => !x.ClientId.HasValue || x.ClientId == companyId).Select(x => x.CandidateId);
            candidates = candidates.Where(x => !bannedByCompany.Contains(x.CandidateId));

            // last week
            var lastWeekPlacement = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(companyId, startDate.AddDays(-7));
            // last before StartDate
            var lastPlacement = _candidateJobOrderService.GetLastCandidateJobOrdersBeforeDateAsQueryable(startDate);
            // current/next
            var nextPlacement = _candidateJobOrderService.GetFirstCandidateJobOrdersAfterDateAsQueryable(startDate);
            // available days
            var availableDays = _candidateJobOrderService.GetAvailableDaysOfCandidateWithinDataRange(startDate, endDate);
            var totalDays = (endDate - startDate).Days + 1;

            bool allowPlacement = _permissionService.Authorize(StandardPermissionProvider.ManageCandidatePlacement);
            bool isRecruiter = account.IsRecruiterOrRecruiterSupervisor();
            var result = from c in candidates
                         from l in lastPlacement.Where(l => l.CandidateId == c.CandidateId).DefaultIfEmpty()
                         from n in nextPlacement.Where(n => n.CandidateId == c.CandidateId).DefaultIfEmpty()
                         from a in availableDays.Where(a => a.CandidateId == c.CandidateId).DefaultIfEmpty()
                         select new PlacementDetailsModel
                         {
                             CandidateGuid = c.Candidate.CandidateGuid,
                             CandidateId = c.CandidateId,
                             CandidateName = (c.Candidate.FirstName ?? String.Empty) + " " + (c.Candidate.LastName ?? String.Empty),
                             Email = c.Candidate.Email,
                             HomePhone = c.Candidate.HomePhone,
                             MobilePhone = c.Candidate.MobilePhone,
                             Skills = c.Candidate.CandidateKeySkills.Select(x => x.KeySkill).ToList(),
                             StartDate = startDate,
                             EndDate = endDate,
                             LastJobOrder = l != null ? l.JobOrderId.ToString() + " (" + l.JobOrder.JobTitle + ")" : String.Empty,
                             LastCaniddateJobOrderId = l != null ? l.Id : 0,
                             Editable = allowPlacement && (n != null ? n.JobOrder.CompanyId == companyId && (!isRecruiter || n.JobOrder.OwnerId == account.Id || n.JobOrder.RecruiterId == account.Id) : true),
                             CurrentJobOrderId = n != null ? n.JobOrderId : 0,
                             CurrentCandidateJobOrderId = n != null ? n.Id : 0, 
                             AvailableDays = a != null ? a.AvailableDays : totalDays,
                             EmployeeId = c.Candidate.EmployeeId
                         };

            return result.ToList();
        }


        public void SavePlacementSummary(PlacementSummaryModel model)
        {
            var opening = model.Opening > 0 ? model.Opening : 0;
            _jobOrderService.UpdateJobOrderOpenings(model.JobOrderId, model.RefDate, model.Opening, null);
        }


        public string SavePlacementDetails(PlacementDetailsModel model)
        {
            if (model.CurrentCandidateJobOrderId > 0 && model.CurrentJobOrderId == 0)
            {
                //return _candidateJobOrderService.RemovePlacement(model.CurrentCandidateJobOrderId);
                var existingCjo = _candidateJobOrderService.GetCandidateJobOrderById(model.CurrentCandidateJobOrderId);
                return _candidateJobOrderService.SetCandidateJobOrderToNoStatus(existingCjo.CandidateId, existingCjo.JobOrderId, existingCjo.CandidateJobOrderStatusId, model.StartDate, model.EndDate);
            }
            else
            {
                var newCjo = new CandidateJobOrder()
                {
                    Id = model.CurrentCandidateJobOrderId,
                    CandidateId = model.CandidateId,
                    JobOrderId = model.CurrentJobOrderId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CandidateJobOrderStatusId = (int)CandidateJobOrderStatusEnum.Placed,
                    EnteredBy = _workContext.CurrentAccount.Id
                };

                return _candidateJobOrderService.CreateOrSavePlacements(newCjo, true);
            }
        }

    }
}
