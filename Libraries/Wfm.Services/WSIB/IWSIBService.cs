using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.WSIBS;

namespace Wfm.Services.WSIBS
{
    public interface IWSIBService
    {
        #region CRUD
        void Create(WSIB entity);
        WSIB Retrieve(int id);
        void Update(WSIB entity);
        void Delete(WSIB entity);
        #endregion

        #region Method
        IQueryable<WSIB> GetAllWSIBs();

        IQueryable<WSIB> GetAllWSIBsByProvinceId(int provinceId);

        bool IsOverlappingOrNot(WSIB entity, bool excludeSelf);
        #endregion
    }
}
