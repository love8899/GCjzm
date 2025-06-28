using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc.UI;
using Wfm.Admin.Extensions;
using Wfm.Core;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Web.Framework;


namespace Wfm.Admin.Models.Companies
{
    public class CompanyCandidate_BL
    {
        public DataSourceResult GetAllCandidatesForCompanyPool(IWorkContext _workContext, ICompanyService _companyService, ICompanyCandidateService _companyCandidateService, 
                                                               ICandidateBlacklistService _candidateBlacklistService, DataSourceRequest request, Guid companyGuid)
        {
            int companyId = _companyService.GetCompanyByGuid(companyGuid).Id;
            var candidates = _companyCandidateService.GetAllCandidatesForCompanyPoolAsQueryable(_workContext.CurrentAccount, companyId, true)
                .Where(x => x.IsActive && !x.IsBanned && !x.IsDeleted);
                //.PagedForCommand(request);

            var bannedByCompany = _candidateBlacklistService.GetAllCandidateBlacklistsByDate(DateTime.Today)
                                  .Where(x => !x.ClientId.HasValue || x.ClientId == companyId).Select(x => x.CandidateId);
            candidates = candidates.Where(x => !bannedByCompany.Contains(x.Id));
            
            IList<SearchCompanyCandidateModel> candidateModelList = new List<SearchCompanyCandidateModel>();

            foreach (var x in candidates.PagedForCommand(request))
            {
                var candidateModel = x.ToSearchCompanyCandidateModel();
                candidateModelList.Add(candidateModel);
            }

            var result = new DataSourceResult()
            {
                Data = candidateModelList,
                Total = candidates.Count()
            };

            return result;
        }

    }

}
