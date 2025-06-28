using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Accounts;


namespace Wfm.Services.Messages
{
    public partial class ClientNotificationService: IClientNotificationService
    {
        #region Fields

        private readonly IRepository<ClientNotification> _clientNotificationRepository;
        private readonly IAccountService _accountService;
        private readonly IMessageTemplateService _messageTemplateService;

        #endregion


        #region Ctor

        public ClientNotificationService(IRepository<ClientNotification> clientNotificationRepository, IAccountService accountService, IMessageTemplateService messageTemplateService)
        {
            this._clientNotificationRepository = clientNotificationRepository;
            this._accountService = accountService;
            this._messageTemplateService = messageTemplateService;
        }

        #endregion


        #region Methods

        public void DeleteClientNotification(ClientNotification clientNotification)
        {
            if (clientNotification == null)
                throw new ArgumentNullException("clientNotification");

            _clientNotificationRepository.Delete(clientNotification);
        }


        public void InsertClientNotification(ClientNotification clientNotification)
        {
            if (clientNotification == null)
                throw new ArgumentNullException("clientNotification");

            _clientNotificationRepository.Insert(clientNotification);
        }


        public void UpdateClientNotification(ClientNotification clientNotification)
        {
            if (clientNotification == null)
                throw new ArgumentNullException("clientNotification");

            _clientNotificationRepository.Update(clientNotification);
        }


        public ClientNotification GetClientNotificationById(int clientNotificationId)
        {
            if (clientNotificationId == 0)
                return null;

            return _clientNotificationRepository.GetById(clientNotificationId);
        }


        public IQueryable<ClientNotification> GetAllClientNotificationsByMessageAndCompany(int messageTemplateId, int companyId)
        {
            var query = _clientNotificationRepository.Table;

            query = query.Where(x => x.MessageTemplateId == messageTemplateId && x.CompanyId == companyId);

            return query;
        }


        public IQueryable<ClientNotification> GetAllClientNotificationsByCompany(int companyId)
        {
            var query = _clientNotificationRepository.Table;

            query = query.Where(x => x.CompanyId == companyId);

            return query;
        }


        public IList<Account> GetClientRecipientsAllowed(int messageTemplateId, int companyId, int locationId = 0, int departmentId = 0)
        {
            var accountRoleIds = this.GetAllClientNotificationsByMessageAndCompany(messageTemplateId, companyId)
                                 .Where(x => x.IsActive).Select(x => x.AccountRoleId);

            var accounts = _accountService.GetAllClientAccountForTask().Where(x => x.CompanyId == companyId)
                           .Where(x => x.AccountRoles.Any(y => accountRoleIds.Contains(y.Id))).ToList();

            if (locationId > 0)
                accounts = accounts.Where(x => x.IsCompanyAdministrator() || x.IsCompanyHrManager() || 
                                               x.CompanyLocationId == locationId).ToList();
            if (departmentId > 0)
                accounts = accounts.Where(x => x.IsCompanyAdministrator() || x.IsCompanyHrManager() || x.IsCompanyLocationManager() || 
                                               x.CompanyDepartmentId == departmentId).ToList();

            return accounts;
        }

        public bool AllowSendingEmailToAccount(int messageTemplateId, int companyId,Account account)
        {
            if (account == null)
                return false;
            var contains = this.GetAllClientNotificationsByMessageAndCompany(messageTemplateId, companyId)
                                            .Where(x => x.IsActive).Select(x => x.AccountRole.AccountRoleName).Contains(account.AccountRoles.FirstOrDefault().AccountRoleName);
            return contains;
        }
        public void SetupDefaultNotifications(int companyId)
        {
            ClientNotificationDefaultMap.Create();
            var maps = ClientNotificationDefaultMap.DefaultMap;

            foreach (var map in maps)
            {
                var notification = _messageTemplateService.GetMessageTemplateByName(map.Key, 0);
                if (notification != null)
                {
                    var roles = _accountService.GetAllAccountRoles().Where(x => x.IsClientRole).ToList();
                    foreach (var role in roles)
                    {
                        var enabled = map.Value.Contains(role.SystemName);
                        var clientNotification = new ClientNotification()
                        {
                            MessageTemplateId = notification.Id,
                            CompanyId = companyId,
                            AccountRoleId = role.Id,
                            IsActive = enabled,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow
                        };
                        this.InsertClientNotification(clientNotification);
                    }
                }
            }
        }

        public void DeleteAllClientNotificationsByCompanyGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return;
            var notifications = _clientNotificationRepository.Table.Where(x => x.Company.CompanyGuid == guid);
            if (notifications.Count() > 0)
                _clientNotificationRepository.Delete(notifications);
        }
        #endregion
    }
}
