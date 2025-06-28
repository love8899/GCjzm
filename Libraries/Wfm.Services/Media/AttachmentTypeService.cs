using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Media;
using Wfm.Core.Data;
using Wfm.Core.Caching;
using Wfm.Services.Events;

namespace Wfm.Services.Media
{
    public partial class AttachmentTypeService :IAttachmentTypeService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : attachment type ID
        /// </remarks>
        private const string ATTACHMENTTYPES_BY_ID_KEY = "Wfm.attachmenttype.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : attachment type name
        /// </remarks>
        private const string ATTACHMENTTYPES_BY_FILE_EXTENSION = "Wfm.attachmenttype.fileextension-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string ATTACHMENTTYPES_ALL_KEY = "Wfm.attachmenttype.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ATTACHMENTTYPES_PATTERN_KEY = "Wfm.attachmenttype.";

        #endregion

        #region Fields

        private readonly IRepository<AttachmentType> _attachmentTypeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion 

        #region Ctors

        public AttachmentTypeService(
            IRepository<AttachmentType> attachmentTypeRepository,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher
            )
        {
            _attachmentTypeRepository = attachmentTypeRepository;
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
        }

        #endregion


        #region CRUD

        public void InsertAttachmentType(AttachmentType attachmentType)
        {
            if (attachmentType == null)
                throw new ArgumentNullException("attachmentType");

            //insert
            _attachmentTypeRepository.Insert(attachmentType);

            //cache
            _cacheManager.RemoveByPattern(ATTACHMENTTYPES_PATTERN_KEY);
        }

        public void UpdateAttachmentType(AttachmentType attachmentType)
        {
            if (attachmentType == null)
                throw new ArgumentNullException("attachmentType");

            _attachmentTypeRepository.Update(attachmentType);

            //cache
            _cacheManager.RemoveByPattern(ATTACHMENTTYPES_PATTERN_KEY);
        }

        public void DeleteAttachmentType(AttachmentType attachmentType)
        {
            if (attachmentType == null)
                throw new ArgumentNullException("attachmentType");

            _attachmentTypeRepository.Delete(attachmentType);

            //cache
            _cacheManager.RemoveByPattern(ATTACHMENTTYPES_PATTERN_KEY);
        }

        #endregion

        #region AttachmentType

        public AttachmentType GetAttachmentTypeById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _attachmentTypeRepository.GetById(id);

            // Using caching
            string key = string.Format(ATTACHMENTTYPES_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _attachmentTypeRepository.GetById(id));
        }

        public AttachmentType GetAttachmentTypeByFileExtension(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                return null;

            // No caching
            //-----------------------------
            //var query = _attachmentTypeRepository.Table;

            //query = from at in query
            //        where at.FileExtensions.Contains(fileExtension.ToLower())
            //        select at;

            //return query.FirstOrDefault();


            // Using caching
            //-----------------------------
            string key = string.Format(ATTACHMENTTYPES_BY_FILE_EXTENSION, fileExtension);
            return _cacheManager.Get(key, () =>
            {
                var query = _attachmentTypeRepository.Table;

                query = from at in query
                        where at.FileExtensions.Contains(fileExtension.ToLower())
                        select at;

                return query.FirstOrDefault();
            });
        }

        #endregion

        #region LIST 
        
        public IList<AttachmentType> GetAllAttachmentTypes(bool showInactive = false)
        {
            //no cache
            //-----------------------------
            //var query = _attachmentTypeRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);

            //query = from at in query
            //        orderby at.TypeName, at.CreatedOnUtc
            //        select at;

            //return query.ToList();



            //using cache
            //-----------------------------
            string key = string.Format(ATTACHMENTTYPES_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _attachmentTypeRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);

                query = from at in query
                        orderby at.TypeName, at.CreatedOnUtc
                        select at;

                return query.ToList();
            });
        }

        #endregion

    }
}
