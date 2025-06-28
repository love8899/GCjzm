using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public interface IDNRReasonService
    {
        #region CRUD
        void Create(DNRReason entity);
        DNRReason Retrieve(int id);
        void Update(DNRReason entity);
        void Delete(DNRReason entity);
        #endregion

        #region List
        List<SelectListItem> GetAllDNRReasonsForDropDownList();

        IQueryable<DNRReason> GetAllDNRReasons();
        #endregion
    }
}
