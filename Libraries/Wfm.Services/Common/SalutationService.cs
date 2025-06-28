using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Data;
using Wfm.Core.Caching;

namespace Wfm.Services.Common
{
    public partial class SalutationService : ISalutationService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : salutation ID
        /// </remarks>
        private const string SALUTATIONS_BY_ID_KEY = "Wfm.salutation.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string SALUTATIONS_ALL_KEY = "Wfm.salutation.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SALUTATIONS_PATTERN_KEY = "Wfm.salutation.";

        #endregion

        #region Fields

        private readonly IRepository<Salutation> _salutationRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public SalutationService(ICacheManager cacheManager,
            IRepository<Salutation> salutationRepository)
        {
            _cacheManager = cacheManager;
            _salutationRepository = salutationRepository;
        }

        #endregion


        #region CRUD

        /// <summary>
        /// Inserts the salutation.
        /// </summary>
        /// <param name="salutation">The salutation.</param>
        /// <exception cref="System.ArgumentNullException">salutation is null</exception>
        public void InsertSalutation(Salutation salutation)
        {
            if (salutation == null)
                throw new ArgumentNullException("salutation");

            _salutationRepository.Insert(salutation);

            //cache
            _cacheManager.RemoveByPattern(SALUTATIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the salutation.
        /// </summary>
        /// <param name="salutation">The salutation.</param>
        /// <exception cref="System.ArgumentNullException">salutation</exception>
        public void UpdateSalutation(Salutation salutation)
        {
            if (salutation == null)
                throw new ArgumentNullException("salutation");

            _salutationRepository.Update(salutation);

            //cache
            _cacheManager.RemoveByPattern(SALUTATIONS_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes the salutation.
        /// </summary>
        /// <param name="salutation">The salutation.</param>
        /// <exception cref="System.ArgumentNullException">salutation</exception>
        public void DeleteSalutation(Salutation salutation)
        {
            if (salutation == null)
                throw new ArgumentNullException("salutation");

            salutation.IsActive = false;
            _salutationRepository.Update(salutation);

            //cache
            _cacheManager.RemoveByPattern(SALUTATIONS_PATTERN_KEY);
        }

        #endregion

        #region Salutation

        /// <summary>
        /// Gets the salutation by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Salutation GetSalutationById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _salutationRepository.GetById(id);

            // Using caching
            string key = string.Format(SALUTATIONS_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _salutationRepository.GetById(id));
        }

        public int GetSalutationIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var salutation = _salutationRepository.Table.Where(x => x.SalutationName.ToLower() == name.ToLower()).FirstOrDefault();
            
            return salutation != null ? salutation.Id : 0;
        }

        #endregion

        #region LIST

        /// <summary>
        /// Gets all salutations. Use to save into DropDownList
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public List<Salutation> GetAllSalutations(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(SALUTATIONS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _salutationRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(s => s.IsActive == true);

                // deleted
                if (!showHidden)
                    query = query.Where(s => s.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.SalutationName
                        select s;

                return query.ToList();
            });
        }

        #endregion

    }
}
