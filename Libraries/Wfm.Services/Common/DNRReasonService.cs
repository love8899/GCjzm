using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public class DNRReasonService:IDNRReasonService
    {
        #region Feild
        private readonly IRepository<DNRReason> _banReasonRepository;
        #endregion

        #region CTOR
        public DNRReasonService(IRepository<DNRReason> banReasonRepository)
        {
            _banReasonRepository = banReasonRepository;
        }
        #endregion

        #region CRUD
        public void Create(DNRReason entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            _banReasonRepository.Insert(entity);
        }

        public DNRReason Retrieve(int id)
        {
            if (id == 0)
                return null;
            return _banReasonRepository.GetById(id);
            //throw new NotImplementedException();
        }

        public void Update(DNRReason entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            _banReasonRepository.Update(entity);
        }

        public void Delete(DNRReason entity)
        {
            if (entity == null)
                throw new ArgumentNullException();
            entity.IsDeleted = true;
            _banReasonRepository.Update(entity);
        }

        #endregion
        
        public List<SelectListItem> GetAllDNRReasonsForDropDownList()
        {
            var result = _banReasonRepository.TableNoTracking.Where(x => !x.IsDeleted && x.IsActive).OrderBy(x=>x.Reason)
                             .Select(x => new SelectListItem() { Text = x.Reason, Value = x.Id.ToString() });
            return result.ToList();
        }

        public IQueryable<DNRReason> GetAllDNRReasons()
        {
            var result = _banReasonRepository.TableNoTracking.Where(x => !x.IsDeleted);
            return result;
        }
    }
}
