using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Services.Franchises
{
    public partial interface IFranchiseService
    {
        #region CRUD

        void InsertFranchise(Franchise franchise);

        void UpdateFranchise(Franchise franchise);

        void DeleteFranchise(Franchise franchise);

        #endregion

        #region Franchise

        Franchise GetFranchiseById(int id);

        Franchise GetFranchiseByGuid(Guid? guid);
        bool IsMSP(int id);

        int GetDefaultMSPId();

        string GetDefaultMSPName();
        Franchise GetPublicFranchise();
        #endregion

        #region LIST

        IList<Franchise> GetAllFranchises(bool showInactive = false, bool showHidden = false);

        IList<Franchise> GetAllFranchises(Account account, bool showInactive = false, bool showHidden = false);

        IQueryable<Franchise> GetAllFranchisesAsQueryable(Account account = null, bool showInactive = false, bool showHidden = false, bool defaultSortOrder = true);

        IQueryable<Franchise> GetAllVendorsAsQueryable(Account account, bool showInactive = false, bool showHidden = false);

        IList<System.Web.Mvc.SelectListItem> GetAllFranchisesAsSelectList(Account account, bool showInactive = false, bool showHidden = false, bool idVal = true);

        IList<System.Web.Mvc.SelectListItem> GetAllVendorsAsSelectList(Account account, bool showInactive = false, bool showHidden = false);
        byte[] GetPublicFranchiseLogo();
        #endregion

        #region Franchise Log
        Image GetFranchiseLogo(int franchiseId);
        #endregion
    }
}
