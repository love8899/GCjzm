using System;
using Twilio;
using Wfm.Core.Infrastructure;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Candidates;


namespace WcfServices.Messages
{
    public class MessageBL
    {
        public void SendMassMessage(string SelectedIds, string body, string from = null)
        {
            // get and set Twilio AccountSid and AuthToken
            var _smsSettings = EngineContext.Current.Resolve<SMSSettings>();
            var accountSid = _smsSettings.AccountSid;
            var authToken = _smsSettings.AuthToken;
            from = !String.IsNullOrWhiteSpace(from) ? from : _smsSettings.FromNumber;

            // instantiate a new Twilio Rest Client
            var client = new TwilioRestClient(accountSid, authToken);

            var IdList = SelectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var _candidateService = EngineContext.Current.Resolve<ICandidateService>();
            foreach (var id in IdList)
            {
                var candidate = _candidateService.GetCandidateById(Convert.ToInt32(id));
#if !DEBUG
                if (candidate != null && !String.IsNullOrWhiteSpace(candidate.MobilePhone))
                    client.SendMessage(from, candidate.MobilePhone, body);
#endif
            }
        }

    }
}