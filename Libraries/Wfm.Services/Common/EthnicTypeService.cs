using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;
using Wfm.Core.Caching;
using Wfm.Services.Events;

namespace Wfm.Services.Common
{
    public class EthnicTypeService : IEthnicTypeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string ETHNICTYPES_ALL_KEY = "Wfm.ethnictype.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ETHNICTYPES_PATTERN_KEY = "Wfm.ethnictype.";

        #endregion

        #region Fields

        private readonly IRepository<EthnicType> _ethnicTypeRepository;
        private readonly ICacheManager _cacheManager;

        #endregion 

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EthnicTypeService"/> class.
        /// </summary>
        /// <param name="ethnicTypeRepository">The ethnicType repository.</param>
        public EthnicTypeService(ICacheManager cacheManager,
            IRepository<EthnicType> ethnicTypeRepository)
        {
            
            _cacheManager = cacheManager;
            _ethnicTypeRepository = ethnicTypeRepository;
        }

        #endregion 


        #region Methods

        /// <summary>
        /// Inserts the ethnicType.
        /// </summary>
        /// <param name="ethnicType">The ethnicType.</param>
        /// <exception cref="System.ArgumentNullException">ethnicType is null</exception>
        public void InsertEthnicType(EthnicType ethnicType)
        {
            if (ethnicType == null)
                throw new ArgumentNullException("ethnicType");

            _ethnicTypeRepository.Insert(ethnicType);

            //cache
            _cacheManager.RemoveByPattern(ETHNICTYPES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the ethnicType.
        /// </summary>
        /// <param name="ethnicType">The ethnicType.</param>
        /// <exception cref="System.ArgumentNullException">ethnicType</exception>
        public void UpdateEthnicType(EthnicType ethnicType)
        {
            if (ethnicType == null)
                throw new ArgumentNullException("ethnicType");

            _ethnicTypeRepository.Update(ethnicType);

            //cache
            _cacheManager.RemoveByPattern(ETHNICTYPES_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes the ethnicType.
        /// </summary>
        /// <param name="ethnicType">The ethnicType.</param>
        /// <exception cref="System.ArgumentNullException">ethnicType</exception>
        public void DeleteEthnicType(EthnicType ethnicType)
        {
            if (ethnicType == null)
                throw new ArgumentNullException("ethnicType");

            ethnicType.IsActive = false;
            _ethnicTypeRepository.Update(ethnicType);

            //cache
            _cacheManager.RemoveByPattern(ETHNICTYPES_PATTERN_KEY);
        }

        #endregion

        #region EthnicType

        /// <summary>
        /// Gets the ethnicType by ethnicTypeid.
        /// </summary>
        /// <param name="id">The ethnicType id.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">EthnicType is null</exception>
        public EthnicType GetEthnicTypeById(int id)
        {
            if (id == 0)
                return null;

            return _ethnicTypeRepository.GetById(id);
        }
        
        #endregion

        #region LIST

        /// <summary>
        /// Gets all ethnicTypes.
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public IList<EthnicType> GetAllEthnicTypes(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(ETHNICTYPES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _ethnicTypeRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(s => s.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(s => s.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.EthnicTypeName
                        select s;

                return query.ToList();
            });
        }

        #endregion

    }
}
