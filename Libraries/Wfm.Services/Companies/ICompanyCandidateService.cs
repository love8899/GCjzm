using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;


namespace Wfm.Services.Companies
{
    public partial interface ICompanyCandidateService
    {

        #region CRUD

        void InsertCompanyCandidate(CompanyCandidate companyCandidate);

        void UpdateCompanyCandidate(CompanyCandidate companyCandidate);

        void DeleteCompanyCandidate(CompanyCandidate companyCandidate);

        #endregion

        #region  CompanyCandidate

        CompanyCandidate GetCompanyCandidateById(int id);

        IList<CompanyCandidate> GetCompanyCandidatesByCompanyIdAndCandidateId(int companyId, int candidateId, DateTime? refDate = null, bool postdated = false);

        void UpdateCompanyCandidateRatingValue(int companyCandidateId, decimal rating);
        void AddCandidatesToCompanyIfNotYet(int companyId, int[] employeeIds, DateTime startDate);

        #endregion

        #region List

        IList<CompanyCandidate> GetCompanyCandidatePool(int companyId, DateTime? refDate = null, bool postdated = false);

        IQueryable<CompanyCandidate> GetCompanyCandidatesByCompanyIdAsQueryable(int companyId, DateTime? refDate = null, bool postdated = false);

        IQueryable<CompanyCandidate> GetCompanyCandidatesByAccountAndCompany(Account account, int companyId, DateTime refDate);

        //void GetAdditionalInfo(CompanyCandidate entry, ICompanyCandidatePriorityInfo priorityInfo);

        IQueryable<CompanyCandidatePoolVM> GetAvailablesFromCompanyCandidatePool(JobOrder jobOrder, DateTime refDate, bool includePlacedInOtherCompanies);

        IQueryable<CandidateWithAddress> GetAllCandidatesForCompanyPoolAsQueryable(Account account, int companyId, bool showInactive = false, bool showBanned = false, bool showHidden = false);

         IQueryable<CompanyCandidate> GetAllCompanyCandidatesAsQueryable(Account account);

        #endregion


        #region remove candidate

        string RemoveCandidateFromPool(CompanyCandidate companyCandidate, string reason, DateTime startDate);

        string RemoveCandidateFromAllPools(int candidateId, string reason, DateTime startDate, int? companyId = null);

        #endregion

    }

    public interface ICompanyCandidatePriorityInfo
    {
        DateTime? LastWorkingDate { get; set; }
        string LastWorkingShift { get; set; }
        string LastWorkingLocation { get; set; }
        decimal? TotalWorkingHours { get; set; }
    }
}
