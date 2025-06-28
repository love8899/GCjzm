using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Services.Employees
{
    public class EmployeePayrollTemplateService : IEmployeePayrollTemplateService
    {
        #region Constants

        private const string EMPLOYEEPAYROLLTEMPLATE_PATTERN_KEY = "Wfm.EmployeePayrollTemplate.";

        #endregion

        #region Fields

        private readonly IRepository<EmployeePayrollTemplate> _employeePayrollTemplateRepository;
        private readonly IRepository<Payroll_Item> _payrollItemRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public EmployeePayrollTemplateService(ICacheManager cacheManager, IRepository<EmployeePayrollTemplate> employeeTD1Repository, IRepository<Payroll_Item> payrollItemRepository)
        {
            _cacheManager = cacheManager;
            _employeePayrollTemplateRepository = employeeTD1Repository;
            _payrollItemRepository = payrollItemRepository;
        }

        #endregion

        #region CRUD

        public void InsertEmployeePayrollTemplate(EmployeePayrollTemplate template)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            _employeePayrollTemplateRepository.Insert(template);

            _cacheManager.RemoveByPattern(EMPLOYEEPAYROLLTEMPLATE_PATTERN_KEY);
        }

        public void UpdateEmployeePayrollTemplate(EmployeePayrollTemplate template)
        {
            if (template == null)
                throw new ArgumentNullException("employeeTD1");

            _employeePayrollTemplateRepository.Update(template);

            _cacheManager.RemoveByPattern(EMPLOYEEPAYROLLTEMPLATE_PATTERN_KEY);
        }

        public void DeleteEmployeePayrollTemplate(EmployeePayrollTemplate template)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            _employeePayrollTemplateRepository.Delete(template);

            _cacheManager.RemoveByPattern(EMPLOYEEPAYROLLTEMPLATE_PATTERN_KEY);
        }

        #endregion


        #region Employee Payroll Template

        public EmployeePayrollTemplate GetEmployeePayrollTemplateById(int id)
        {
            return _employeePayrollTemplateRepository.GetById(id);
        }

        #endregion


        #region LIST

        public IQueryable<EmployeePayrollTemplate> GetAllEmployeePayrollTemplates()
        {
            return _employeePayrollTemplateRepository.Table;
        }


        public IQueryable<EmployeePayrollTemplate> GetAllEmployeePayrollTemplatesByEmployeeId(int employeeId)
        {
            return GetAllEmployeePayrollTemplates().Where(x => x.EmployeeId == employeeId);
        }


        public IList<EmployeePayrollTemplate> PrepareEmployeePayrollTemplates(int franchiseId, int employeeId)
        {
            var existingResult = GetAllEmployeePayrollTemplatesByEmployeeId(employeeId).ToList();
            var existingItemIds = existingResult.Select(x => x.PayrollItemId);
            var allItemIds = GetPayTemplateItems(franchiseId).Select(x => x.ID);
            var emptyResult = allItemIds.Except(existingItemIds).Select(x => new EmployeePayrollTemplate()
                {
                    EmployeeId = employeeId,
                    PayrollItemId = x
                });

            return existingResult.Union(emptyResult).ToList();
        }

        #endregion


        #region Payroll Items

        public IList<Payroll_Item> GetPayTemplateItems(int franchiseId)
        {
            return _payrollItemRepository.TableNoTracking.Where(x => x.TypeID == 1 && x.SubTypeId == 2 && x.FranchiseId == franchiseId).ToList();
        }

        #endregion
    }
}
