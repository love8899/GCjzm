using System.Linq;
using System.Collections.Generic;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Messages;
using System;


namespace Wfm.Services.Messages
{
    public partial interface IClientNotificationService
    {
        void DeleteClientNotification(ClientNotification clientNotification);

        void InsertClientNotification(ClientNotification clientNotification);

        void UpdateClientNotification(ClientNotification clientNotification);

        ClientNotification GetClientNotificationById(int clientNotificationId);

        IQueryable<ClientNotification> GetAllClientNotificationsByMessageAndCompany(int messageTemplateId, int companyId);

        IQueryable<ClientNotification> GetAllClientNotificationsByCompany(int companyId);

        IList<Account> GetClientRecipientsAllowed(int messageTemplateId, int companyId, int locationId = 0, int departmentId = 0);

        bool AllowSendingEmailToAccount(int messageTemplateId, int companyId,Account account);

        void SetupDefaultNotifications(int companyId);

        void DeleteAllClientNotificationsByCompanyGuid(Guid? guid);
    }
}
