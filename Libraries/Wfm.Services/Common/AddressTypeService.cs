using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;
using Wfm.Core.Caching;

namespace Wfm.Services.Common
{
    public class AddressTypeService : IAddressTypeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : addresstype ID
        /// </remarks>
        private const string ADDRESSTYPES_BY_ID_KEY = "Wfm.addresstype.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string ADDRESSTYPES_ALL_KEY = "Wfm.addresstype.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ADDRESSTYPES_PATTERN_KEY = "Wfm.addresstype.";

        #endregion

        #region Fields

        private readonly IRepository<AddressType> _addressTypeRepository;
        private readonly ICacheManager _cacheManager;

        #endregion 

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressTypeService"/> class.
        /// </summary>
        /// <param name="addressTypeRepository">The addressType repository.</param>
        public AddressTypeService(ICacheManager cacheManager,
            IRepository<AddressType> addressTypeRepository
           )
        {
            
            _cacheManager = cacheManager;
            _addressTypeRepository = addressTypeRepository;
        }

        #endregion 

        #region Methods

        /// <summary>
        /// Inserts the addressType.
        /// </summary>
        /// <param name="addressType">The addressType.</param>
        /// <exception cref="System.ArgumentNullException">addressType is null</exception>
        public void InsertAddressType(AddressType addressType)
        {
            if (addressType == null)
                throw new ArgumentNullException("addressType");

            _addressTypeRepository.Insert(addressType);

            //cache
            _cacheManager.RemoveByPattern(ADDRESSTYPES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the addressType.
        /// </summary>
        /// <param name="addressType">The addressType.</param>
        /// <exception cref="System.ArgumentNullException">addressType</exception>
        public void UpdateAddressType(AddressType addressType)
        {
            if (addressType == null)
                throw new ArgumentNullException("addressType");

            _addressTypeRepository.Update(addressType);

            //cache
            _cacheManager.RemoveByPattern(ADDRESSTYPES_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes the addressType.
        /// </summary>
        /// <param name="addressType">The addressType.</param>
        /// <exception cref="System.ArgumentNullException">addressType</exception>
        public void DeleteAddressType(AddressType addressType)
        {
            if (addressType == null)
                throw new ArgumentNullException("addressType");

            addressType.IsDeleted = true;
            _addressTypeRepository.Update(addressType);

            //cache
            _cacheManager.RemoveByPattern(ADDRESSTYPES_PATTERN_KEY);
        }


        /// <summary>
        /// Gets the addressType by addressTypeid.
        /// </summary>
        /// <param name="id">The addressType id.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">AddressType is null</exception>
        public AddressType GetAddressTypeById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _addressTypeRepository.GetById(id);

            // Using caching
            string key = string.Format(ADDRESSTYPES_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _addressTypeRepository.GetById(id));
        }


        public int GetAddressTypeIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var addressType = _addressTypeRepository.Table.Where(x => x.AddressTypeName.ToLower() == name.ToLower()).FirstOrDefault();

            return addressType != null ? addressType.Id : 0;
        }

        
        /// <summary>
        /// Gets all addressTypes.
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public IList<AddressType> GetAllAddressTypes(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(ADDRESSTYPES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _addressTypeRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(s => s.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(s => s.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.AddressTypeName
                        select s;

                return query.ToList();
            });
        }
        
        #endregion

    }
}
