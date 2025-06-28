using System.Collections.Generic;
using Wfm.Core.Domain.Media;

namespace Wfm.Services.Media
{
    public partial  interface IAttachmentTypeService
    {
        #region CRUD

        void InsertAttachmentType(AttachmentType attachmentType);

        void UpdateAttachmentType(AttachmentType attachmentType);

        void DeleteAttachmentType(AttachmentType attachmentType);

        #endregion

        #region AttachmentType

        AttachmentType GetAttachmentTypeById(int id);

        AttachmentType GetAttachmentTypeByFileExtension(string fileExtension);

        #endregion

        #region LIST

        IList<AttachmentType> GetAllAttachmentTypes(bool showInactive = false);

        #endregion

    }
}
