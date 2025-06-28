using System;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Candidates;
using Wfm.Services.Accounts;
using Wfm.Services.Security;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.JobOrders;
using System.Collections.Generic;


namespace Wfm.Admin.Models.Candidate
{
    public class CandidateSchedule_BL
    {
        #region Fields

        private readonly IAccountService _accountServices;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IJobOrderService _jobOrderService;
        #endregion

        #region Ctor

        public CandidateSchedule_BL(IAccountService accountService, 
                                    ICandidateJobOrderService candidateJobOrderService,
                                    IPermissionService permissionService,
                                    IWorkContext workContext,
                                    IJobOrderService jobOrderService)
        {
            _accountServices = accountService;
            _candidateJobOrderService = candidateJobOrderService;
            _permissionService = permissionService;
            _workContext = workContext;
            _jobOrderService = jobOrderService;
        }
        
        #endregion


        public IList<CandidateScheduleModel> GetAllPlacementsByCandidateIdAsQueryable(Account account, int candidateId)
        {
            var allowChangePlacement = _permissionService.Authorize(StandardPermissionProvider.ManageCandidatePlacement);
            var isRecruiter = account.IsRecruiterOrRecruiterSupervisor();
            var placements = _candidateJobOrderService.GetCandidateJobOrderByCandidateIdAsQueryable(candidateId);
            var recruiters = _accountServices.GetAllRecruitersAsQueryable(account);

            //var result = placements.Join(recruiters, p => p.JobOrder.RecruiterId, r => r.Id, (p, r) => new CandidateScheduleModel
            var result = (from p in placements
                         from r in recruiters.Where(r => r.Id == p.JobOrder.RecruiterId).DefaultIfEmpty()
                         select new CandidateScheduleModel
                         {
                             CandidateJobOrderId = p.Id,
                             CandidateId = p.CandidateId,
                             CompanyId = p.JobOrder.CompanyId,
                             CompanyName = p.JobOrder.Company.CompanyName,
                             LocationId = p.JobOrder.CompanyLocationId,
                             DepartmentId = p.JobOrder.CompanyDepartmentId,
                             ContactId = p.JobOrder.CompanyContactId,
                             JobOrderId = p.JobOrderId,
                             JobOrderGuid=p.JobOrder.JobOrderGuid,
                             Title = p.JobOrderId.ToString(),
                             Description = p.JobOrder.JobTitle,
                             RecruiterName = r != null ? r.FirstName + " " + r.LastName : String.Empty,
                             Start = p.StartDate,
                             //StartTimezone = TimeZoneInfo.Local.DisplayName,
                             //StartTimezone = "EST",
                             End = p.EndDate.HasValue ? p.EndDate.Value : DateTime.MaxValue,
                             //EndTimezone = "EST",
                             //Timezone = "EST",
                             IsAllDay = true,
                             StatusId = p.CandidateJobOrderStatusId,
                             ReadOnly = !allowChangePlacement|| p.JobOrder.FranchiseId!=account.FranchiseId||(account.Id != p.JobOrder.OwnerId && account.Id != p.JobOrder.RecruiterId),
                         }).ToList();
            for (int i = 0; i < result.Count(); i++)
            {
                int placedCount = _candidateJobOrderService.GetNumberOfPlacedCandidatesByJobOrder(result[i].JobOrderId, result[i].Start);
                JobOrderOpening[] _openingChanges;
                int openingAvailable = _jobOrderService.GetJobOrderOpeningAvailable(result[i].JobOrderId, result[i].Start, out _openingChanges);
                result[i].AvailableOpening = openingAvailable - placedCount;
            }
            return result;
        }


        public string CreateOrSavePlacements(CandidateScheduleModel placement)
        {
            var newCjo = new CandidateJobOrder()
            {
                Id = placement.CandidateJobOrderId,
                CandidateId = placement.CandidateId,
                JobOrderId = placement.JobOrderId,
                StartDate = placement.Start,
                EndDate = placement.End < DateTime.MaxValue.Date ? placement.End : (DateTime?)null,
                CandidateJobOrderStatusId = placement.StatusId,
                EnteredBy = _workContext.CurrentAccount.Id
            };

            return _candidateJobOrderService.CreateOrSavePlacements(newCjo, true);
        }

    }

}
