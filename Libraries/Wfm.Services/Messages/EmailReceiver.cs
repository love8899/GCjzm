using System.Collections.Generic;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    public partial class EmailReceiver : IEmailReceiver
    {
        #region Fields

        private readonly IEmailAccountService _emailAcccountService;

        #endregion


        #region Cotr

        public EmailReceiver(IEmailAccountService emailAccountService)
        {
            _emailAcccountService = emailAccountService;
        }

        #endregion


        public IList<MimeMessage> GetEmails(EmailAccount emailAccount, UniqueIdRange range, ref List<UniqueId> uniqueIds)
        {
            IList<MimeMessage> emails = Enumerable.Empty<MimeMessage>().ToList();

            using (var client = new ImapClient())
            {
                // accept all SSL certificates???
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(emailAccount.Host, emailAccount.Port, emailAccount.EnableSsl);

                client.Authenticate(emailAccount.Username, emailAccount.Password);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                uniqueIds = inbox.Fetch(range, MessageSummaryItems.UniqueId).Select(x => x.UniqueId).ToList();

                foreach (var id in uniqueIds)
                    emails.Add(inbox.GetMessage(id));

                client.Disconnect(true);
            }

            return emails;
        }
    }
}
