using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Payroll;
using System.Data.Entity;

namespace Wfm.Services.Payroll
{
    public class TaxFormBoxService:ITaxFormBoxService
    {
        #region Field
        private readonly IRepository<TaxFormBox> _taxFormBoxRepository;

        #endregion

        #region CTOR
        public TaxFormBoxService(IRepository<TaxFormBox> taxFormBoxRepository)
        {
            _taxFormBoxRepository = taxFormBoxRepository;
        }
        #endregion

        #region CRUD       

        public void Create(TaxFormBox entity)
        {
            if (entity == null)
                throw new ArgumentNullException("TaxFormBox");
            _taxFormBoxRepository.Insert(entity);
        }

        public TaxFormBox Retrieve(int id)
        {
            if (id == 0)
                return null;
            return _taxFormBoxRepository.GetById(id);
        }

        public void Update(TaxFormBox entity)
        {
            if (entity == null)
                throw new ArgumentNullException("TaxFormBox");
            _taxFormBoxRepository.Update(entity);
        }

        public void Delete(TaxFormBox entity)
        {
            if (entity == null)
                throw new ArgumentNullException("TaxFormBox");
            _taxFormBoxRepository.Delete(entity);
        }
        #endregion
        public IQueryable<TaxFormBox> GetAllTaxFormBoxes(int year,string type)
        {            
            if (!String.Equals(type, "EARNING") && !String.Equals(type, "DEDUCTION") && !String.Equals(type, "BENEFIT") && !String.Equals(type, "TAX") && !String.Equals(type, "INTERNAL"))
            {
                return null;
            }
            var result = _taxFormBoxRepository.Table.Where(x => x.Year == year).Include(x=>x.PayrollItems);
            switch (type)
            {
                case "EARNING":
                    result = result.Where(x=>x.Earnings); break;
                case "DEDUCTION":
                    result = result.Where(x=>x.Deductions); break;
                case "BENEFIT":
                    result = result.Where(x=>x.Benefits); break;
                case "TAX":
                    result = result.Where(x=>x.Taxes); break;
                case "INTERNAL":
                   result = result.Where(x=>x.Internals); break;
                default:
                    break;
            }
            return result;

        }
    }
}
