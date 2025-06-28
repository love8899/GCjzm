using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Events;
using Wfm.Services.Localization;

namespace Wfm.Services.Messages
{
    public partial class MessageCategoryService: IMessageCategoryService
    {
        #region Fields

        private readonly IRepository<MessageCategory> _messageCategoryRepository;

        #endregion


        #region Ctor

        public MessageCategoryService(
            IRepository<MessageCategory> messageCategoryRepository
            )
        {
            this._messageCategoryRepository = messageCategoryRepository;
        }

        #endregion


        #region Methods

        public virtual void DeleteMessageCategory(MessageCategory messageCategory)
        {
            if (messageCategory == null)
                throw new ArgumentNullException("messageCategory");

            _messageCategoryRepository.Delete(messageCategory);
        }


        public virtual void InsertMessageCategory(MessageCategory messageCategory)
        {
            if (messageCategory == null)
                throw new ArgumentNullException("messageCategory");

            _messageCategoryRepository.Insert(messageCategory);
        }


        public virtual void UpdateMessageCategory(MessageCategory messageCategory)
        {
            if (messageCategory == null)
                throw new ArgumentNullException("messageCategory");

            _messageCategoryRepository.Update(messageCategory);
        }


        public virtual MessageCategory GetMessageCategoryById(int messageCategoryId)
        {
            if (messageCategoryId == 0)
                return null;

            return _messageCategoryRepository.GetById(messageCategoryId);
        }


        public virtual MessageCategory GetMessageCategoryByName(string messageCategoryName)
        {
            if (string.IsNullOrWhiteSpace(messageCategoryName))
                throw new ArgumentException("messageCategoryName");

            var query = _messageCategoryRepository.Table;

            query = query.Where(x => x.CategoryName == messageCategoryName);
            query = query.OrderBy(x => x.Id);

            return query.FirstOrDefault();
        }


        public virtual IList<MessageCategory> GetAllMessageCategories(bool activeOnly = false)
        {
            var query = _messageCategoryRepository.Table;

                query = query.Where(x => !activeOnly || x.IsActive).OrderBy(x => x.CategoryName);

            return query.ToList();
        }

        #endregion
    }
}
