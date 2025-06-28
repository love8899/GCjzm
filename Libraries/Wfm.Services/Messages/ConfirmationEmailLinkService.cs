using System;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Messages;

namespace Wfm.Services.Messages
{
    public class ConfirmationEmailLinkService:IConfirmationEmailLinkService
    {
        #region Field
        private readonly IRepository<ConfirmationEmailLink> _confirmationEmailLinkRepository;
        #endregion

        #region CTOR
        public ConfirmationEmailLinkService(IRepository<ConfirmationEmailLink> confirmationEmailLinkRepository)
        {
            _confirmationEmailLinkRepository = confirmationEmailLinkRepository;
        }
        #endregion

        #region CRUD
        public void Create(ConfirmationEmailLink entity)
        {
            if (entity == null)
                throw new ArgumentNullException("ConfirmationEmailLink");
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _confirmationEmailLinkRepository.Insert(entity);
        }

        public ConfirmationEmailLink Retrieve(int id)
        {
            var result = _confirmationEmailLinkRepository.GetById(id);
            return result;
        }

        public void Update(ConfirmationEmailLink entity)
        {
            if (entity == null)
                throw new ArgumentNullException("ConfirmationEmailLink");
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _confirmationEmailLinkRepository.Update(entity);
        }



        public void Delete(ConfirmationEmailLink entity)
        {
            if (entity == null)
                throw new ArgumentNullException("ConfirmationEmailLink");
            _confirmationEmailLinkRepository.Delete(entity);
        }
        #endregion
        public ConfirmationEmailLink GetByGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return null;
            var result = _confirmationEmailLinkRepository.Table.Where(x => x.ConfirmationEmailLinkGuid == guid).FirstOrDefault();
            return result;
        }
    }
}
