using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.WSIBS;

namespace Wfm.Services.WSIBS
{
    public class WSIBService:IWSIBService
    {
        #region Fields
        private readonly IRepository<WSIB> _wSIBRepository;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public WSIBService(IRepository<WSIB> wSIBRepository, IWorkContext workContext)
        {
            _wSIBRepository = wSIBRepository;
            _workContext = workContext;
        }
        #endregion

        #region CRUD
        public void Create(WSIB entity)
        {
            if (entity == null)
                throw new ArgumentNullException("WSIB");

            //insert
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            entity.UpdatedBy = _workContext.CurrentAccount.Id;
            entity.EnteredBy = _workContext.CurrentAccount.Id;
            _wSIBRepository.Insert(entity);
        }

        public WSIB Retrieve(int id)
        {
            if (id == 0)
                return null;
            return _wSIBRepository.GetById(id);
        }

        public void Update(WSIB entity)
        {
            if (entity == null)
                throw new ArgumentNullException("WSIB");
            entity.UpdatedOnUtc = DateTime.UtcNow;
            entity.UpdatedBy = _workContext.CurrentAccount.Id;
            _wSIBRepository.Update(entity);
        }

        public void Delete(WSIB entity)
        {
            if (entity == null)
                throw new ArgumentNullException("WSIB");
            _wSIBRepository.Delete(entity);
        }
        #endregion

        #region Method
        public IQueryable<WSIB> GetAllWSIBs()
        {
            return _wSIBRepository.Table;
        }

        public IQueryable<WSIB> GetAllWSIBsByProvinceId(int provinceId)
        {
            var query = GetAllWSIBs();
            query = query.Where(x => x.ProvinceId==provinceId);
            return query;
        }
        public bool IsOverlappingOrNot(WSIB entity,bool excludeSelf)
        {
            var similarWSIB = _wSIBRepository.TableNoTracking.Where(x => x.Code == entity.Code && x.ProvinceId == entity.ProvinceId);
            if (excludeSelf)
                similarWSIB = similarWSIB.Where(x => x.Id != entity.Id);
            if (similarWSIB.Count() > 0)
            {
                if (entity.EndDate == null)
                    return !similarWSIB.All(x => x.EndDate != null && x.EndDate < entity.StartDate);
                else
                    return similarWSIB.Any(x => entity.EndDate >= x.StartDate && (x.EndDate == null || x.EndDate >= entity.StartDate));
            }
            else
                return false;
        }
        #endregion
    }
}
