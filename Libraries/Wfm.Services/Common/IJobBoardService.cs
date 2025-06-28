using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public interface IJobBoardService
    {
        #region CRUD
        void Create(JobBoard entity);
        JobBoard Retrieve(int id);
        void Update(JobBoard entity);
        void Delete(JobBoard entity);
        #endregion

        #region Method
        IQueryable<JobBoard> GetAllJobBoardsAsQuerable(bool showInactive = false, bool showHidden = false);
        IList<SelectListItem> GetAllJobBoards();
        bool IsDuplicate(JobBoard entity);
        #endregion
    }
}
