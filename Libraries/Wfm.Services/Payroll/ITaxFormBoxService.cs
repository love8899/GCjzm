using System.Linq;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Services.Payroll
{
    public interface ITaxFormBoxService
    {
        #region CRUD
        void Create(TaxFormBox entity);
        TaxFormBox Retrieve(int id);
        void Update(TaxFormBox entity);
        void Delete(TaxFormBox entity);
        #endregion

        #region Method
        IQueryable<TaxFormBox> GetAllTaxFormBoxes(int year,string type);
        #endregion
    }
}
