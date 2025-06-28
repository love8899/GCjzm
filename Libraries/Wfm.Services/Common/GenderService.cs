using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Data;
using Wfm.Core.Caching;

namespace Wfm.Services.Common
{
    public partial class GenderService : IGenderService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : gender ID
        /// </remarks>
        private const string GENDERS_BY_ID_KEY = "Wfm.gender.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string GENDERS_ALL_KEY = "Wfm.gender.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string GENDERS_PATTERN_KEY = "Wfm.gender.";

        #endregion

        #region Fields

        private readonly IRepository<Gender> _genderRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public GenderService(ICacheManager cacheManager,
            IRepository<Gender> genderRepository
            )
        {
            _cacheManager = cacheManager;
            _genderRepository = genderRepository;
        }

        #endregion


        #region CRUD

        /// <summary>
        /// Inserts the gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        /// <exception cref="System.ArgumentNullException">gender is null</exception>
        public void InsertGender(Gender gender)
        {
            if (gender == null)
                throw new ArgumentNullException("gender");

            _genderRepository.Insert(gender);

            //cache
            _cacheManager.RemoveByPattern(GENDERS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        /// <exception cref="System.ArgumentNullException">gender</exception>
        public void UpdateGender(Gender gender)
        {
            if (gender == null)
                throw new ArgumentNullException("gender");

            _genderRepository.Update(gender);

            //cache
            _cacheManager.RemoveByPattern(GENDERS_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes the gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        /// <exception cref="System.ArgumentNullException">gender</exception>
        public void DeleteGender(Gender gender)
        {
            if (gender == null)
                throw new ArgumentNullException("gender");

            gender.IsActive = false;
            _genderRepository.Update(gender);

            //cache
            _cacheManager.RemoveByPattern(GENDERS_PATTERN_KEY);
        }

        #endregion

        #region Gender

        /// <summary>
        /// Gets the gender by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Gender GetGenderById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _genderRepository.GetById(id);

            // Using caching
            string key = string.Format(GENDERS_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _genderRepository.GetById(id));
        }


        public int GetGenderIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var gender = _genderRepository.Table.Where(x => x.GenderName.ToLower() == name.ToLower()).FirstOrDefault();

            return gender != null ? gender.Id : 0;
        }

        #endregion

        #region LIST

        /// <summary>
        /// Gets all genders. Use to save into DropDownList
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public List<Gender> GetAllGenders(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(GENDERS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _genderRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(s => s.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(s => s.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.GenderName
                        select s;

                return query.ToList();
            });
        }


        #endregion

    }
}
