using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;


namespace Wfm.Services.Companies
{
    public interface ICompanyAttachmentService
    {
        #region CRUD
        void Create(CompanyAttachment entity);
        CompanyAttachment Retrieve(int id);
        CompanyAttachment Retrieve(Guid? guid);
        void Update(CompanyAttachment entity);
        void Delete(CompanyAttachment entity);

        #endregion

        #region Method

        CompanyAttachment GetCompanyAttachmentById(int id);

        IQueryable<CompanyAttachment> GetAllCompanyAttachmentsByCompanyGuid(Account account, Guid? guid);

        bool IsCompanyAttachmentAccessible(CompanyAttachment companyAttachment, Account account);

        bool IsCompanyAttachmentDeletable(CompanyAttachment companyAttachment, Account account);

        #endregion
    }
}
