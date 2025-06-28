using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Services.Payroll
{
    public class PayrollItemService:IPayrollItemService
    {
        private readonly IRepository<Payroll_Item> _payrollItemRepository;
        public PayrollItemService(IRepository<Payroll_Item> payrollItemRepository)
        {
            _payrollItemRepository = payrollItemRepository;
        }

        public IQueryable<Payroll_Item> GetAllPayrollItems()
        {
            var result = _payrollItemRepository.Table;
            return result;
        }
        #region CRUD      
        public void Create(Payroll_Item entity)
        {
            if (entity == null)
                throw new ArgumentNullException("PayrollItem");
            _payrollItemRepository.Insert(entity);
        }

        public Payroll_Item Retrieve(int id)
        {
            if (id == 0)
                return null;
            var entity = _payrollItemRepository.GetById(id);
            return entity;
        }

        public void Update(Payroll_Item entity)
        {
            if (entity == null)
                throw new ArgumentNullException("PayrollItem");
            _payrollItemRepository.Update(entity);
        }

        public void Delete(Payroll_Item entity)
        {
            if (entity == null)
                throw new ArgumentNullException("PayrollItem");
            _payrollItemRepository.Delete(entity);
        }
        #endregion
    }
}
