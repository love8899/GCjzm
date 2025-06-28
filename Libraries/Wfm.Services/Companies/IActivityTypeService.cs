using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Companies;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace Wfm.Services.Companies
{
    public interface IActivityTypeService
    {
        #region CRUD
        void Create(ActivityType entity);
        ActivityType Retrieve(int id);
        void Update(ActivityType entity);
        void Delete(ActivityType entity);
        #endregion

        #region Method
        IList<SelectListItem> GetAllActivityTypesForDropDownList();

        IList<ActivityType> GetAllActivityTypes();
        #endregion
    }
}
