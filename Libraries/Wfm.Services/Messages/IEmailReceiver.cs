using System.Collections.Generic;
using MailKit;
using MimeKit;
using Wfm.Core.Domain.Messages;


namespace Wfm.Services.Messages
{
    public partial interface IEmailReceiver
    {
        IList<MimeMessage> GetEmails(EmailAccount emailAccount, UniqueIdRange range, ref List<UniqueId> uniqueIds);
    }
}
