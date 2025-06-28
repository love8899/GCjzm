using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Media;
using Wfm.Core.Data;
using Wfm.Core.Caching;
using Wfm.Services.Events;

namespace Wfm.Services.Media
{
    public partial class DocumentTypeService : IDocumentTypeService
    {

        #region Fields

        private readonly IRepository<DocumentType> _documentTypeRepository;


        #endregion

        #region Ctors

        public DocumentTypeService(
            IRepository<DocumentType> documentTypeRepository

            )
        {
            _documentTypeRepository = documentTypeRepository;

        }

        #endregion


        #region CRUD

        public void InsertDocumentType(DocumentType documentType)
        {
            if (documentType == null)
                throw new ArgumentNullException("documentType");
            //insert
            _documentTypeRepository.Insert(documentType);

        }

        public void UpdateDocumentType(DocumentType documentType)
        {
            if (documentType == null)
                throw new ArgumentNullException("documentType");

            _documentTypeRepository.Update(documentType);
        }

        public void DeleteDocumentType(DocumentType documentType)
        {
            if (documentType == null)
                throw new ArgumentNullException("documentType");

            _documentTypeRepository.Delete(documentType);
        }

        #endregion

        #region DocumentType

        public DocumentType GetDocumentTypeById(int id)
        {
            if (id == 0)
                return null;

            return _documentTypeRepository.GetById(id);
        }

        public DocumentType GetDocumentTypeByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            return _documentTypeRepository.Table.Where(d=>d.InternalCode==code).FirstOrDefault();
        }


        #endregion

        #region LIST

        public IList<DocumentType> GetAllDocumentTypes(bool showInactive = false, bool showHidden = false)
        {
            var query = _documentTypeRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);

            // deleted
            if (!showHidden)
                query = query.Where(s => s.IsDeleted == false);

            query = from dt in query
                    orderby dt.TypeName, dt.CreatedOnUtc
                    select dt;

            return query.ToList();
        }





        #endregion

    }
}
