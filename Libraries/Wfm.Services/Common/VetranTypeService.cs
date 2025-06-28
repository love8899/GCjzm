using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;
using Wfm.Core.Caching;

namespace Wfm.Services.Common
{
    public class VetranTypeService : IVetranTypeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string VETRANTYPES_ALL_KEY = "Wfm.vetrantype.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string VETRANTYPES_PATTERN_KEY = "Wfm.vetrantype.";

        #endregion

        #region Fields

        private readonly IRepository<VetranType> _vetranTypeRepository;
        private readonly ICacheManager _cacheManager;

        #endregion 

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="VetranTypeService"/> class.
        /// </summary>
        /// <param name="vetranTypeRepository">The vetranType repository.</param>
        public VetranTypeService(ICacheManager cacheManager,
            IRepository<VetranType> vetranTypeRepository)
        {
            
            _cacheManager = cacheManager;
            _vetranTypeRepository = vetranTypeRepository;
        }

        #endregion 

        #region CRUD

        /// <summary>
        /// Inserts the vetranType.
        /// </summary>
        /// <param name="vetranType">The vetranType.</param>
        /// <exception cref="System.ArgumentNullException">vetranType is null</exception>
        public void InsertVetranType(VetranType vetranType)
        {
            if (vetranType == null)
                throw new ArgumentNullException("vetranType");

            _vetranTypeRepository.Insert(vetranType);

            //cache
            _cacheManager.RemoveByPattern(VETRANTYPES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the vetranType.
        /// </summary>
        /// <param name="vetranType">The vetranType.</param>
        /// <exception cref="System.ArgumentNullException">vetranType</exception>
        public void UpdateVetranType(VetranType vetranType)
        {
            if (vetranType == null)
                throw new ArgumentNullException("vetranType");

            _vetranTypeRepository.Update(vetranType);

            //cache
            _cacheManager.RemoveByPattern(VETRANTYPES_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes the vetranType.
        /// </summary>
        /// <param name="vetranType">The vetranType.</param>
        /// <exception cref="System.ArgumentNullException">vetranType</exception>
        public void DeleteVetranType(VetranType vetranType)
        {
            if (vetranType == null)
                throw new ArgumentNullException("vetranType");

            vetranType.IsActive = false;
            _vetranTypeRepository.Update(vetranType);

            //cache
            _cacheManager.RemoveByPattern(VETRANTYPES_PATTERN_KEY);
        }

        #endregion

        #region VetranType

        /// <summary>
        /// Gets the vetranType by vetranTypeid.
        /// </summary>
        /// <param name="id">The vetranType id.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">VetranType is null</exception>
        public VetranType GetVetranTypeById(int id)
        {
            if (id == 0)
                return null;

            return _vetranTypeRepository.GetById(id);
        }

        #endregion

        #region LIST

        /// <summary>
        /// Gets all vetranTypes.
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public IList<VetranType> GetAllVetranTypes(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(VETRANTYPES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _vetranTypeRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(s => s.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(s => s.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.VetranTypeName
                        select s;

                return query.ToList();
            });
        }

        #endregion

    }
}
