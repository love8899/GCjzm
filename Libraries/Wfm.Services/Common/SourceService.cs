using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Data;
using Wfm.Core.Caching;
using Wfm.Services.Events;

namespace Wfm.Services.Common
{
    public partial class SourceService : ISourceService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string SOURCES_ALL_KEY = "Wfm.source.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string SOURCES_PATTERN_KEY = "Wfm.source.";

        #endregion

        #region Fields

        private readonly IRepository<Source> _sourceRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public SourceService(ICacheManager cacheManager,
            IRepository<Source> sourceRepository)
        {
            _cacheManager = cacheManager;
            _sourceRepository = sourceRepository;
        }

        #endregion


        #region CRUD

        /// <summary>
        /// Inserts the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="System.ArgumentNullException">source is null</exception>
        public void InsertSource(Source source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            _sourceRepository.Insert(source);

            //cache
            _cacheManager.RemoveByPattern(SOURCES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public void UpdateSource(Source source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            _sourceRepository.Update(source);

            //cache
            _cacheManager.RemoveByPattern(SOURCES_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public void DeleteSource(Source source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            source.IsActive = false;
            _sourceRepository.Update(source);

            //cache
            _cacheManager.RemoveByPattern(SOURCES_PATTERN_KEY);
        }

        #endregion

        #region Source

        /// <summary>
        /// Gets the source by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Source GetSourceById(int id)
        {
            if (id == 0)
                return null;

            return _sourceRepository.GetById(id);
        }

        public int GetSourceIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var source = _sourceRepository.Table.Where(x => x.SourceName.ToLower() == name.ToLower()).FirstOrDefault();

            return source != null ? source.Id : 0;
        }

        #endregion

        #region LIST

        /// <summary>
        /// Gets all sources. Use to save into DropDownList
        /// </summary>
        /// <param name="showInactive"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public List<Source> GetAllSources(bool showInactive = false, bool showHidden = false)
        {
            //using cache
            //-----------------------------
            string key = string.Format(SOURCES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _sourceRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(s => s.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(s => s.IsDeleted == false);

                query = from s in query
                        orderby s.DisplayOrder, s.SourceName
                        select s;

                return query.ToList();
            });
        }

        #endregion

    }
}
