using System.Collections.Generic;
using Wfm.Core.Domain.Messages;

namespace Wfm.Services.Messages
{
    /// <summary>
    /// Message category service
    /// </summary>
    public partial interface IMessageCategoryService
    {
        void DeleteMessageCategory(MessageCategory messageCategory);

        void InsertMessageCategory(MessageCategory messageCategory);

        void UpdateMessageCategory(MessageCategory messageCategory);

        MessageCategory GetMessageCategoryById(int messageCategoryId);

        MessageCategory GetMessageCategoryByName(string messageCategoryName);

        IList<MessageCategory> GetAllMessageCategories(bool activeOnly = false);
    }
}
