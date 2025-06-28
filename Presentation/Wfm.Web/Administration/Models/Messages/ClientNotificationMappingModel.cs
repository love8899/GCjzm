using System;
using System.Collections.Generic;
using Wfm.Admin.Models.Accounts;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Messages
{
    public class ClientNotificationMappingModel : BaseWfmModel
    {
        public ClientNotificationMappingModel()
        {
            AvailableNotifications = new List<Tuple<int, string>>();
            AvailableAccountRoles = new List<Tuple<int, string, string>>();
        }

        public List<Tuple<int, string>> AvailableNotifications { get; set; }
        public List<Tuple<int, string, string>> AvailableAccountRoles { get; set; }
    }
}