using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Companies;


namespace Wfm.Services.TimeSheet
{
    public partial class MissingHourService : IMissingHourService
    {

        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IRepository<CandidateMissingHour> _missingHourRepository;
        private readonly IWorkTimeService _workTimeService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        
        #endregion

        #region Ctor

        public MissingHourService(
            IWorkContext workContext,
            IRepository<CandidateMissingHour> missingHourRepository,
            IWorkTimeService workTimeService,
            IRecruiterCompanyService recruiterCompanyService
            )
        {
            _workContext = workContext;
            _missingHourRepository = missingHourRepository;
            _workTimeService = workTimeService;
            _recruiterCompanyService = recruiterCompanyService;
        }

        #endregion


        #region Method

        public void InsertCandidateMissingHour(CandidateMissingHour candidateMissingHour)
        {
            if (candidateMissingHour == null)
                throw new ArgumentNullException("candidateMissingHour");

            _missingHourRepository.Insert(candidateMissingHour);
        }


        public void UpdateCandidateMissingHour(CandidateMissingHour candidateMissingHour)
        {
            if (candidateMissingHour == null)
                throw new ArgumentNullException("candidateMissingHour");

            candidateMissingHour.UpdatedOnUtc = DateTime.UtcNow;

            _missingHourRepository.Update(candidateMissingHour);
        }


        public void DeleteCandidateMissingHour(CandidateMissingHour candidateMissingHour)
        {
            if (candidateMissingHour == null)
                throw new ArgumentNullException("candidateMissingHour");

            _missingHourRepository.Delete(candidateMissingHour);
        }


        public string InsertOrUpdateMissingHour(int jobOrderId, int candidateId, DateTime startDate, decimal hours, string note)
        {
            var result = String.Empty;
            var candidateMissingHour = GetMissingHourByCandidateIdAndJobOrderIdAndJobStartDate(candidateId, jobOrderId, startDate);
            
            // update
            if (candidateMissingHour != null)
            {
                // skip processed or appoved or voided
                if (candidateMissingHour.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Processed ||
                    candidateMissingHour.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Approved ||
                    candidateMissingHour.CandidateMissingHourStatusId == (int)CandidateMissingHourStatus.Voided)
                    result = String.Format("There exist a missing hour record already {0}.",
                        ((CandidateMissingHourStatus)candidateMissingHour.CandidateMissingHourStatusId).ToString().ToLower());
                else
                {
                    candidateMissingHour.NewHours = CommonHelper.RoundUp(hours, 2);
                    candidateMissingHour.Note = note;

                    UpdateCandidateMissingHour(candidateMissingHour);
                }
            }

            // insert
            else
            {
                var candidateWorkTime = _workTimeService.GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(candidateId, jobOrderId, startDate);
                candidateMissingHour = new CandidateMissingHour()
                {
                    CandidateId = candidateId,
                    JobOrderId = jobOrderId,
                    CandidateMissingHourStatusId = (int)CandidateMissingHourStatus.PendingApproval,
                    WorkDate = startDate.Date,
                    OrigHours = candidateWorkTime != null ? candidateWorkTime.NetWorkTimeInHours : 0m,
                    NewHours = CommonHelper.RoundUp(hours, 2),
                    Note = note,
                    EnteredBy = _workContext.CurrentAccount.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };

                InsertCandidateMissingHour(candidateMissingHour);
            }

            return result;
        }


        public CandidateMissingHour GetMissingHourById(int id)
        {
            if (id == 0)
                return null;

            return _missingHourRepository.GetById(id);
        }


        public CandidateMissingHour GetMissingHourByCandidateIdAndJobOrderIdAndJobStartDate(int candidateId, int jobOrderId, DateTime jobStartDate)
        {
            var query = _missingHourRepository.Table;

            query = query.Where(x => x.CandidateId == candidateId &&
                                     x.JobOrderId == jobOrderId &&
                                     DbFunctions.TruncateTime(x.WorkDate) == jobStartDate.Date);

            return query.FirstOrDefault();
        }


        public IQueryable<CandidateMissingHour> GetAllCandidateMissingHourAsQueryable(bool isAccountBased = true)
        {
            var query = _missingHourRepository.Table;
           
            if (isAccountBased)
            {
                var ids = new List<int>();
                var account = _workContext.CurrentAccount;
                if (account.IsVendor())
                    query = query.Where(x => x.Candidate.FranchiseId == account.FranchiseId);
                if (account.IsVendor())
                {
                    query = query.Where(x => x.Candidate.FranchiseId == account.FranchiseId);
                    if (account.IsVendorRecruiter() || account.IsVendorRecruiterSupervisor())
                    {
                        ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                        query = query.Where(x => ids.Contains(x.JobOrder.CompanyId));
                    }
                }
                else
                {
                    if (account.IsMSPRecruiterSupervisor() || account.IsMSPRecruiter())
                    {
                        ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                        query = query.Where(x => ids.Contains(x.JobOrder.CompanyId));
                    }
                }
            }

            return query;
        }

        #endregion

    }
}
