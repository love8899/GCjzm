using System.Linq;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Services.Payroll
{
    public interface IPayrollItemService
    {
        #region CRUD
        void Create(Payroll_Item entity);
        Payroll_Item Retrieve(int id);
        void Update(Payroll_Item entity);
        void Delete(Payroll_Item entity);
        #endregion

        #region Methods
        IQueryable<Payroll_Item> GetAllPayrollItems();
        #endregion
    }
}
