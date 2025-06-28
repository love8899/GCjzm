using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Services.Companies;
using Wfm.Web.Framework;
using Wfm.Admin.Extensions;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Admin.Models.CompanyContact
{
    public class CompanyContactViewModel_BL
    {
        public DataSourceResult GetCompanyContactViewModel(ICompanyContactService _companyContactService, IWorkContext _workContext, IRecruiterCompanyService _recruiterCompanyService, DataSourceRequest request)
        {
            var totalAccounts = _companyContactService.GetCompanyContactsAsQueryable(true);
            var clientAdmins = _companyContactService.GetCompanyClientAdminsAsQueryable(true);
            totalAccounts = totalAccounts.Union(clientAdmins).OrderBy(x => x.LastName).ThenBy(x => x.FirstName);

            if (_workContext.CurrentAccount.IsRecruiterOrRecruiterSupervisor())
            {
                List<int> companyIds = _recruiterCompanyService.GetCompanyIdsByRecruiterId(_workContext.CurrentAccount.Id);
                totalAccounts = totalAccounts.Where(x => companyIds.Contains(x.CompanyId));
            }
            else if (_workContext.CurrentAccount.IsClientAdministrator())
                totalAccounts = totalAccounts.Where(x => x.CompanyId == _workContext.CurrentAccount.CompanyId);
            var accounts = totalAccounts.PagedForCommand(request);

            List<CompanyContactViewModel> accountModelList = new List<CompanyContactViewModel>();
            foreach (var item in accounts)
            {
                CompanyContactViewModel accountModel = item.ToCompanyContactViewModel();
                accountModelList.Add(accountModel);
            }

            var result = new DataSourceResult()
            {
                Data = accountModelList,
                Total = accounts.TotalCount
            };
            return result;
        }
    }
}