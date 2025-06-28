using System;
using System.Linq;
using Twilio;
using Wfm.Core;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    public partial class TextMessageSender : ITextMessageSender
    {
        #region Fields

        private readonly SMSSettings _smsSettings;
        private readonly IMessageHistoryService _messageHistoryService;
        private readonly IWorkContext _workContext;
        private readonly IMessageCategoryService _messageCategoryService;
        #endregion


        #region Cotr

        public TextMessageSender(SMSSettings smsSettings, IMessageHistoryService messageHistoryService,
                                IWorkContext workContext, IMessageCategoryService messageCategoryService)
        {
            _smsSettings = smsSettings;
            _messageHistoryService = messageHistoryService;
            _workContext = workContext;
            _messageCategoryService = messageCategoryService;
        }

        #endregion

        public int SendTextMessage(string message, string numbers)
        {
            // get and set Twilio AccountSid and AuthToken
            var accountSid = _smsSettings.AccountSid;
            var authToken = _smsSettings.AuthToken;
            var from = _smsSettings.FromNumber;

            // instantiate a new Twilio Rest Client
            var client = new TwilioRestClient(accountSid, authToken);

            var numList = numbers.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => CommonHelper.ExtractNumericText(x));
            var done = numList.Count();

            //insert text message to MessageHistory
            var category = _messageCategoryService.GetMessageCategoryByName("Candidate");
            int categoryId = category == null ? 0 : category.Id;
            _messageHistoryService.InsertTextMessageToMessageHistory(String.Join(",",numList), message, _workContext.CurrentAccount,categoryId);
#if !DEBUG
            foreach (var num in numList)
                if (!String.IsNullOrWhiteSpace(num))
                    client.SendMessage(from, num, message);
                else
                    done--;
#endif
            return done;
        }

    }
}
