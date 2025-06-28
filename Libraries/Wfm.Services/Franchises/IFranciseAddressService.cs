using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Services.Franchises
{
    public partial interface IFranciseAddressService
    {
        #region CRUD
        void Create(FranchiseAddress entity);
        FranchiseAddress Retrieve(int id);
        void Update(FranchiseAddress entity);
        void Delete(FranchiseAddress entity);
        #endregion

        #region Method
      
        IList<FranchiseAddress> GetAllFranchiseAddressByFranchiseGuid(Guid? guid, bool showInactive = false);

        IList<FranchiseAddress> GetAllFranchiseAddressForPublicSite();

        #endregion
    }
}
