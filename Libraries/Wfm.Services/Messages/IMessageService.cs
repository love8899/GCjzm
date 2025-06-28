using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    /// <summary>
    /// Message service
    /// </summary>
    public partial interface IMessageService
    {
        void DeleteMessage(Message message);

        void InsertMessage(Message message);

        void InsertMessagesForAllRecipients(MessageHistory messageHistory,bool byTextMessage=false);
        //void InsertTextMessageForCandidates(string candidateIds, string text, int accountId);

        void UpdateMessage(Message message);

        Message GetMessageById(int messageId);

        int GetTheNumberOfUnreadMessages(int accountId, bool IsCandidate = false, bool messageOnly = false);

        IQueryable<Message> GetMessagesByAccount(int accountId, bool IsCandidate = false, bool messageOnly = false);
    }
}
