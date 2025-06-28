using System;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Messages;

namespace Wfm.Services.Messages
{
    public partial interface IMessageHistoryService
    {

        void InsertMessageHistory(MessageHistory messageHistory);

        void InsertMessageHistory(QueuedEmail queuedEmail, string note = null);
        void InsertTextMessageToMessageHistory(string phoneNumber, string textMessage, Account account, int messageCategoryId);

        void UpdateMessageHistory(MessageHistory messageHistory);

        void MoveQueuedEmailToMessageHistory(QueuedEmail queuedEmail, string note = null);

        bool MessageSentOrNot(string subject);

        MessageHistory GetMessageHistoryById(int messageHistoryId);

        IQueryable<MessageHistory> GetAllMessageHistoriesAsQueryable(Account account);

        IQueryable<MessageHistory> GetAllMessageHistoriesByAccountAsQueryable(Account account);

        IQueryable<MessageHistory> GetAllMessageHistoryByTimeRange(DateTime startTime, DateTime endTime, int? emailAccountId = null);

        int GetNumerOfMessagesByTimeRange(DateTime startTime, DateTime endTime, int? emailAccountId = null, bool byRecipient = true);

    }
}
