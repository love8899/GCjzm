using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;

namespace Wfm.Services.Companies
{
    public class CompanyOvertimeRuleService:ICompanyOvertimeRuleService
    {
        #region Fileds
        private readonly IRepository<CompanyOvertimeRule> _companyOvertimeRuleRepository;
        private readonly ICompanyService _companyService;
        #endregion

        #region CTOR
        public CompanyOvertimeRuleService(IRepository<CompanyOvertimeRule> companyOvertimeRuleRepository, ICompanyService companyService)
        {
            _companyOvertimeRuleRepository = companyOvertimeRuleRepository;
            _companyService = companyService;
        }
        #endregion

        #region CRUD
        public void Create(CompanyOvertimeRule entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyOvertimeRule");

            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;

            _companyOvertimeRuleRepository.Insert(entity);
        }

        public CompanyOvertimeRule Retrieve(int id)
        {
            var rule = _companyOvertimeRuleRepository.GetById(id);
            return rule;
        }

        public void Update(CompanyOvertimeRule entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyOvertimeRule");

            entity.UpdatedOnUtc = DateTime.UtcNow;

            _companyOvertimeRuleRepository.Update(entity);
        }

        public void Delete(CompanyOvertimeRule entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyOvertimeRule");

            _companyOvertimeRuleRepository.Delete(entity);
        }
        #endregion

        #region Method
        public IList<CompanyOvertimeRule> GetAllCompanyOvertimeRuleByCompanyGuid(Guid? guid)
        {
            var company = _companyService.GetCompanyByGuid(guid);
            if(company == null)
                return new List<CompanyOvertimeRule>();
            var rules =  _companyOvertimeRuleRepository.Table.Where(x => x.Company.CompanyGuid == guid);
            return rules.ToList();
        }

        public IQueryable<CompanyOvertimeRule> GetAllCompanyOvertimeRules(bool showInactive = false)
        {
            var query =  _companyOvertimeRuleRepository.Table;
            if (!showInactive)
                query = query.Where(x => x.IsActive);
            return query;

        }

        public void DeleteAllCompanyOvertimeRulesByCompanyGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return;
            var rules = _companyOvertimeRuleRepository.Table.Where(x => x.Company.CompanyGuid == guid);
            if (rules.Count() > 0)
                _companyOvertimeRuleRepository.Delete(rules);
        }
        #endregion
    }
}
