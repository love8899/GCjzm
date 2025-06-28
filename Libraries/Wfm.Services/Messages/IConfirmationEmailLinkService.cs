using System;
using Wfm.Core.Domain.Messages;

namespace Wfm.Services.Messages
{
    public interface IConfirmationEmailLinkService
    {
        #region CRUD
        void Create(ConfirmationEmailLink entity);
        ConfirmationEmailLink Retrieve(int id);
        void Update(ConfirmationEmailLink entity);
        void Delete(ConfirmationEmailLink entity);
        #endregion

        #region Method
        ConfirmationEmailLink GetByGuid(Guid? guid);
        #endregion
    }
}
