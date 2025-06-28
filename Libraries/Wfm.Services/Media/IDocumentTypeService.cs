using System.Collections.Generic;
using Wfm.Core.Domain.Media;

namespace Wfm.Services.Media
{
    public partial  interface IDocumentTypeService 
    {
        #region CRUD

        void InsertDocumentType(DocumentType documentType);

        void UpdateDocumentType(DocumentType documentType);

        void DeleteDocumentType(DocumentType documentType);

        #endregion

        #region DocumentType

        DocumentType GetDocumentTypeById(int id);

        DocumentType GetDocumentTypeByCode(string code);

        #endregion 
         
        #region LIST

        IList<DocumentType> GetAllDocumentTypes(bool showInactive = false, bool showHidden = false); 

        #endregion

    }
}
