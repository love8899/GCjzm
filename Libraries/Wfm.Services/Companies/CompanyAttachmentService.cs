using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;


namespace Wfm.Services.Companies
{
    public class CompanyAttachmentService:ICompanyAttachmentService
    {
        #region Field

        private readonly IRepository<CompanyAttachment> _companyAttachmentRepository;
        private readonly ICompanyService _companyService;
        private readonly IRepository<RecruiterCompany> _companyRecruiters;

        #endregion

        #region CTOR

        public CompanyAttachmentService(IRepository<CompanyAttachment> companyAttachmentRepository, ICompanyService companyService, IRepository<RecruiterCompany> companyRecruiters)
        {
            _companyService = companyService;
            _companyAttachmentRepository = companyAttachmentRepository;
            _companyRecruiters = companyRecruiters;
        }

        #endregion


        #region CRUD
        public void Create(CompanyAttachment entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyAttachment");
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _companyAttachmentRepository.Insert(entity);
        }

        public CompanyAttachment Retrieve(int id)
        {
            var entity = _companyAttachmentRepository.GetById(id);
            return entity;            
        }
        public CompanyAttachment Retrieve(Guid? guid)
        {
            if (!guid.HasValue || guid == Guid.Empty)
                return null;
            var entity = _companyAttachmentRepository.Table.Where(x => x.CompanyAttachmentGuid == guid).FirstOrDefault();
            return entity;
        }
        public void Update(CompanyAttachment entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyAttachment");
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _companyAttachmentRepository.Update(entity);
        }

        public void Delete(CompanyAttachment entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyAttachment");
            _companyAttachmentRepository.Delete(entity);
        }
        #endregion


        #region Method

        public CompanyAttachment GetCompanyAttachmentById(int id)
        {
            return _companyAttachmentRepository.GetById(id);
        }


        public IQueryable<CompanyAttachment> GetAllCompanyAttachmentsByCompanyGuid(Account account, Guid? guid)
        {
            var query = _companyAttachmentRepository.Table.Where(x => x.Company.CompanyGuid == guid);

            // recruiters can access only non-restricted attachments or their own attachments
            if (account.IsRecruiterOrRecruiterSupervisor())
                query = query.Where(x => !x.IsRestricted || (x.EnteredBy.HasValue && x.EnteredBy == account.Id));

            return query;
        }


        public bool IsCompanyAttachmentAccessible(CompanyAttachment companyAttachment, Account account)
        {
            return  account.IsAdministrator() || account.IsPayrollAdministrator() ||
                    !companyAttachment.IsRestricted || (companyAttachment.EnteredBy.HasValue && companyAttachment.EnteredBy == account.Id);
        }


        public bool IsCompanyAttachmentDeletable(CompanyAttachment companyAttachment, Account account)
        {
            return account.IsAdministrator() || account.IsPayrollAdministrator() ||
                   (companyAttachment.EnteredBy.HasValue && companyAttachment.EnteredBy == account.Id);
        }
        
        #endregion
    }
}
