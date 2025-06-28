using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Messages;


namespace Wfm.Admin.Models.Messages
{
    public class ClientNotificationMapping_BL
    {
        #region Fields

        private readonly IClientNotificationService _clientNotificationService;

        #endregion


        #region Ctor

        public ClientNotificationMapping_BL(IClientNotificationService clientNotificationService)
        {
            _clientNotificationService = clientNotificationService;
        }

        #endregion


        public ClientNotificationMappingModel GetClientNotificationMappingByCompany(int companyId)
        {
            var mapping = _clientNotificationService.GetAllClientNotificationsByCompany(companyId).ToList();

            var model = new ClientNotificationMappingModel();
            model.AvailableNotifications = mapping.Select(x => x.MessageTemplate).Distinct().Select(x => new Tuple<int, string>(x.Id, x.TagName)).ToList();
            model.AvailableAccountRoles = mapping.Select(x => x.AccountRole).Distinct().Select(x => new Tuple<int, string, string>(x.Id, x.HeaderTitle, x.SystemName)).ToList();

            return model;
        }


        public IList<IDictionary<string, object>> GetNotificationRoleMaps(int companyId)
        {
            var mapping = _clientNotificationService.GetAllClientNotificationsByCompany(companyId).ToList();
            var result = new List<IDictionary<string, object>>();

            var availableNotifications = mapping.Select(x => x.MessageTemplate).Distinct().Select(x => new Tuple<int, string>(x.Id, x.TagName)).ToList();
            var availableAccountRoles = mapping.Select(x => x.AccountRole).Distinct().Select(x => new Tuple<int, string, string>(x.Id, x.HeaderTitle, x.SystemName)).ToList();

            foreach (var n in availableNotifications)
            {
                dynamic notificationMap = new ExpandoObject();
                var notificationMapDic = notificationMap as IDictionary<string, Object>;
                notificationMapDic.Add("NotificationName", n.Item2);
                foreach (var ar in availableAccountRoles)
                {
                    var record = mapping.Where(x => x.MessageTemplateId == n.Item1 && x.AccountRoleId == ar.Item1).FirstOrDefault();
                    notificationMapDic.Add(ar.Item3, record != null ? record.IsActive : false);
                }
                result.Add(_SerializeExpando(notificationMap));
            }

            return result;
        }


        public void SaveClientNotificationMapping(int companyId, string notificationNames, FormCollection form)
        {
            var mapping = _clientNotificationService.GetAllClientNotificationsByCompany(companyId).ToList();
            // only notifications in current page;
            var notifications = notificationNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var accountRoles = mapping.Select(x => x.AccountRole).Distinct().Select(x => new Tuple<int, string>(x.Id, x.AccountRoleName)).ToList();

            foreach (var ar in accountRoles)
            {
                string formKey = "allow_" + ar.Item1;
                var allowedNotificationTagNames = form[formKey] != null ? form[formKey].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                foreach (var n in notifications)
                {
                    var mappingByMsg = mapping.Where(x => x.MessageTemplate.TagName == n);
                    var msgTmpltId = mappingByMsg.FirstOrDefault().MessageTemplateId;
                    var record = mappingByMsg.Where(x => x.AccountRoleId == ar.Item1).FirstOrDefault();
                    if (record != null)
                    {
                        record.IsActive = allowedNotificationTagNames.Contains(n);
                        record.UpdatedOnUtc = DateTime.UtcNow;
                        _clientNotificationService.UpdateClientNotification(record);
                    }
                    else
                    {
                        record = new ClientNotification()
                        {
                            MessageTemplateId = msgTmpltId,
                            CompanyId = companyId,
                            AccountRoleId = ar.Item1,
                            IsActive = allowedNotificationTagNames.Contains(n),
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow
                        };
                        _clientNotificationService.InsertClientNotification(record);
                    }
                }
            }
        }


        public IDictionary<string, object> _SerializeExpando(object obj)
        {
            var result = new Dictionary<string, object>();
            var dictionary = obj as IDictionary<string, object>;
            foreach (var item in dictionary)
                result.Add(item.Key, item.Value);
            return result;
        }
    }
}
