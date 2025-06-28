using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Messages;


namespace Wfm.Admin.Models.Messages
{
    public class Message_BL
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IMessageService _messageService;

        #endregion


        #region Ctor

        public Message_BL(IWorkContext workContext, IMessageService messageService)
        {
            _workContext = workContext;
            _messageService = messageService;
        }

        #endregion


        public IQueryable<Message> GetAllMessagesByCriteria(MessageCriteria criteria)
        {
            var messages = _messageService.GetMessagesByAccount(_workContext.CurrentAccount.Id,false,false);
            var emptyResult = new List<Message>().AsQueryable();

            if (!String.IsNullOrEmpty(criteria.SubjectKeyword))
                messages = messages.Where(x => x.MessageHistory.Subject.Contains(criteria.SubjectKeyword));

            if (criteria.Categories == null || criteria.Categories.Count == 0)
                messages = emptyResult;
            else
                messages = messages.Where(x => criteria.Categories.Contains(x.MessageHistory.MessageCategory.CategoryName));

            if (criteria.Status == null || criteria.Status.Count == 0)
                messages = emptyResult;
            else
                messages = messages.Where(x => criteria.Status.Contains(x.IsRead ? 1 : 0));

            if (!criteria.WithCC)
                messages = messages.Where(x => x.MessageHistory.ToAccountId == _workContext.CurrentAccount.Id || x.MessageHistory.MailTo == _workContext.CurrentAccount.Email);

            return messages;
        }


        public void GetPreviousAndNextMessages(int id, MessageCriteria criteria, out Message prevMsg, out Message nextMsg, out int currIndex, out int total)
        {
            var messages = GetAllMessagesByCriteria(criteria).ToList();
            prevMsg = messages.TakeWhile(x => x.Id != id).LastOrDefault();
            nextMsg = messages.SkipWhile(x => x.Id != id).Skip(1).FirstOrDefault();
            currIndex = messages.FindIndex(x => x.Id == id);
            total = messages.Count();
        }


        public void GetPreviousAndNextMessageIds(int id, MessageCriteria criteria, out int prevMsgId, out int nextMsgId, out int currIndex, out int total)
        {
            Message prevMsg, nextMsg;
            GetPreviousAndNextMessages(id, criteria, out prevMsg, out nextMsg, out currIndex, out total);

            prevMsgId = prevMsg != null ? prevMsg.Id : 0;
            nextMsgId = nextMsg != null ? nextMsg.Id : 0;
        }


        public void MarkMessageReadStatus(Message message, bool isRead)
        {
            if (message != null && !message.IsCandidate && message.AccountId == _workContext.CurrentAccount.Id)
            {
                message.IsRead = isRead;
                message.ReadOnUtc = isRead ? DateTime.UtcNow : (DateTime?)null;
                _messageService.UpdateMessage(message);
            }
        }

    }

}
