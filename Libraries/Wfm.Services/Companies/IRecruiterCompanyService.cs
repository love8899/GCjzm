using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Services.Companies
{
    public partial interface IRecruiterCompanyService
    {
        #region CRUD

        void InsertCompanyRecruiter(RecruiterCompany companyRecruiter);

        void UpdateCompanyRecruiter(RecruiterCompany companyRecruiter);

        void DeleteCompanyRecruiter(RecruiterCompany companyRecruiter);

        #endregion

        IQueryable<RecruiterCompany> GetAllRecruitersAsQueryable(Account account = null);

        IList<RecruiterCompany> GetAllRecruitersByCompanyId(int companyId);
        IList<RecruiterCompany> GetAllRecruitersByCompanyGuid(Guid? guid);
        void DeleteRecruiterCompanyById(int id);
        RecruiterCompany GetRecruiterCompanyById(int id);
        List<int> GetCompanyIdsByRecruiterId(int recruiterId);
        string GetAllRecruitersEmailByCompanyId(int companyId, int franchiseId);
        void DeleteCompanyRecruitersByCompanyGuid(Guid? guid);
        IList<SelectListItem> GetAllCompaniesByRecruiter(int id);
        IList<RecruiterCompany> GetAllRecruiterCompaniesByRecruiterId(int id);
        bool IsDuplicateConnection(int companyId, int recruiterId);
    }
}
