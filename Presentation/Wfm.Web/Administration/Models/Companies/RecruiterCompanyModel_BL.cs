using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Companies;
using Wfm.Admin.Extensions;

namespace Wfm.Admin.Models.Companies
{
    public class RecruiterCompanyModel_BL
    {
        public DataSourceResult GetRecruiterCompanyModelByCompanyId(Account account, int companyId,IRecruiterCompanyService _companyRecruiterService)
        {
            IList<RecruiterCompany> recruiters_company = _companyRecruiterService.GetAllRecruitersByCompanyId(companyId);
            if (account.IsLimitedToFranchises)
                recruiters_company = recruiters_company.Where(x => x.Account.FranchiseId == account.FranchiseId).ToList();
            List<RecruiterCompanyModel> models = new List<RecruiterCompanyModel>();
            foreach (var recruiter in recruiters_company)
            {
                RecruiterCompanyModel model = recruiter.ToModel();
                models.Add(model);
            }
            return new DataSourceResult() { Data = models, Total = models.Count };
        }

        public void AddNewRecruiterToCompany(int companyId, int recruiterId, IRecruiterCompanyService _companyRecruiterService)
        {
            RecruiterCompany recruiter_company = new RecruiterCompany();
            recruiter_company.AccountId = recruiterId;
            recruiter_company.CompanyId = companyId;
            //recruiter_company.IsActive = true;
            recruiter_company.CreatedOnUtc = DateTime.UtcNow;
            recruiter_company.UpdatedOnUtc = DateTime.UtcNow;
            _companyRecruiterService.InsertCompanyRecruiter(recruiter_company);
        }

        public void DeleteRecruiterCompany(string ids, IRecruiterCompanyService _companyRecruiterService)
        {
            string[] id_array = ids.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in id_array)
            {
                _companyRecruiterService.DeleteRecruiterCompanyById(Convert.ToInt32(id));
            }
        }
    }
}