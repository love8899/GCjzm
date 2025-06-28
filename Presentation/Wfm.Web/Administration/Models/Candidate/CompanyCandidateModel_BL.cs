using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.ExportImport;


namespace Wfm.Admin.Models.Candidate
{
    public class CompanyCandidateModel_BL
    {
        public IQueryable<CompanyCandidate> GetCompanyCandidateList(Account account, int companyId, ICompanyCandidateService _companyCandidateService, ICandidateBlacklistService _candidateBlacklistService)
        {
            var companyCandidates = _companyCandidateService.GetCompanyCandidatesByCompanyIdAsQueryable(companyId);
            if (account.IsLimitedToFranchises)
                companyCandidates = companyCandidates.Where(x => x.Candidate.FranchiseId == account.FranchiseId);

            // exclude those banned candidates
            var bannedByCompany = _candidateBlacklistService.GetAllCandidateBlacklistsByDate(DateTime.Today)
                                  .Where(x => !x.ClientId.HasValue || x.ClientId == companyId).Select(x => x.CandidateId);
            companyCandidates = companyCandidates.Where(x => !bannedByCompany.Contains(x.CandidateId)&&!x.Candidate.UseForDirectPlacement);

            return companyCandidates;
        }


        public FileContentResult GetCompanyCandidateList(ICompanyCandidateService _companyCandidateService,
                                                         IExportManager _exportManager,
                                                         Account account, int companyId)
        {
            IList<CompanyCandidate> companyCandidates = _companyCandidateService.GetCompanyCandidatePool(companyId)
                                                        .Where(x => x.Candidate.IsActive && !x.Candidate.IsBanned && !x.Candidate.IsDeleted).ToList();
            if (account.IsLimitedToFranchises)
                companyCandidates = companyCandidates.Where(x => x.Candidate.FranchiseId == account.FranchiseId)
                                                     .OrderBy(x => x.CandidateId).ThenBy(x => x.StartDate).ToList();

            if (companyCandidates != null)
            {
                string fileName = string.Empty;
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    fileName = _exportManager.ExportCompanyCandidateToXlsx(stream, companyId, account.FranchiseId, companyCandidates);
                    bytes = stream.ToArray();
                }

                var file = new FileContentResult(bytes, "text/xls");
                file.FileDownloadName = fileName;

                return file;
            }

            return null;
        }

    }
}
