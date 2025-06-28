using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Data;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Services.Franchises
{
    public class FranchiseAddressService:IFranciseAddressService
    {
        #region Field
        private readonly IRepository<FranchiseAddress> _franchiseAddressRepository;
        private readonly IFranchiseService _franchiseService;
        #endregion

        #region Ctor
        public FranchiseAddressService(IRepository<FranchiseAddress> franchiseAddressRepository, IFranchiseService franchiseService)
        {
            _franchiseAddressRepository = franchiseAddressRepository;
            _franchiseService = franchiseService;
        }
        #endregion

        #region CRUD
        public void Create(FranchiseAddress entity)
        {
            if (entity == null)
                throw new ArgumentNullException("FranchiseAddress");

            _franchiseAddressRepository.Insert(entity);
        }

        public FranchiseAddress Retrieve(int id)
        {
            if (id == 0)
                return null;
            var address = _franchiseAddressRepository.GetById(id);
            return address;
        }

        public void Update(FranchiseAddress entity)
        {
            if (entity == null)
                throw new ArgumentNullException("FranchiseAddress");
            _franchiseAddressRepository.Update(entity);

        }

        public void Delete(FranchiseAddress entity)
        {
            if (entity == null)
                throw new ArgumentNullException("FranchiseAddress");
            _franchiseAddressRepository.Delete(entity);
        }
        #endregion

        #region Method
        

        public IList<FranchiseAddress> GetAllFranchiseAddressByFranchiseGuid(Guid? guid, bool showInactive = false)
        {
            var franchise = _franchiseService.GetFranchiseByGuid(guid);
            if (franchise == null) 
                return null;
            var query = _franchiseAddressRepository.Table.Where(x => x.FranchiseId == franchise.Id);
            
            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);        

            return query.ToList();
        }

        public IList<FranchiseAddress> GetAllFranchiseAddressForPublicSite()
        {          
            var query = _franchiseAddressRepository.Table.Where(x => x.IsActive && x.IsDeleted==false);
            query = query.Where(a => a.Franchise.IsLinkToPublicSite);          

            return query.ToList();
        }


        #endregion
    }
}
