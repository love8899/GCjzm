using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Models.Accounts;
using Wfm.Services.Security;
using Wfm.Services.Accounts;


namespace Wfm.Admin.Models.Security
{
    public class PermissionRoleMapping_BL
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IAccountService _accountService;

        #endregion


        #region Ctor

        public PermissionRoleMapping_BL(IPermissionService permissionService, IAccountService accountService)
        {
            _permissionService = permissionService;
            _accountService = accountService;
        }

        #endregion


        public PermissionMappingModel GetPermissionRoleMapping()
        {
            var model = new PermissionMappingModel();
            model.AvailablePermissions = _permissionService.GetAllPermissionRecords().Select(x => new Tuple<int, string>(x.Id, x.Name)).ToList();
            model.AvailableAccountRoles = _accountService.GetAllAccountRoles(true).OrderBy(x => x.IsClientRole).ThenBy(x => x.IsVendorRole)
                                          .Select(x => new Tuple<int, string, string>(x.Id, x.HeaderTitle, x.SystemName)).ToList();

            return model;
        }


        public IList<IDictionary<string, object>> GetPermissionRoleMaps()
        {
            var result = new List<IDictionary<string, object>>();

            var permissions = _permissionService.GetAllPermissionRecords().ToList();
            var accountRoles = _accountService.GetAllAccountRoles(true).OrderBy(x => x.IsClientRole).ThenBy(x => x.IsVendorRole)
                               .Select(x => new Tuple<int, string, string>(x.Id, x.HeaderTitle, x.SystemName)).ToList();

            foreach (var p in permissions)
            {
                dynamic permissionMap = new ExpandoObject();
                var permissionMapDic = permissionMap as IDictionary<string, Object>;
                permissionMapDic.Add("Permission", p.Name);
                foreach (var ar in accountRoles)
                    permissionMapDic.Add(ar.Item3, p.AccountRoles.Where(x => x.Id == ar.Item1).Any());
                result.Add(_SerializeExpando(permissionMap));
            }

            return result;
        }


        public void SavePermissionMapping(string permissionNames, FormCollection form)
        {
            // only permissions in current page;
            var permissions = permissionNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var accountRoles = _accountService.GetAllAccountRoles(true).OrderBy(x => x.IsClientRole).ThenBy(x => x.IsVendorRole).ToList();

            foreach (var ar in accountRoles)
            {
                string formKey = "allow_" + ar.Id;
                var allowedNotificationTagNames = form[formKey] != null ? form[formKey].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                foreach (var pr in permissions)
                {
                    var permission = _permissionService.GetPermissionRecordByName(pr);
                    if (permission != null)
                    {
                        bool allow = allowedNotificationTagNames.Contains(pr);
                        if (allow)
                        {
                            if (permission.AccountRoles.FirstOrDefault(x => x.Id == ar.Id) == null)
                            {
                                permission.AccountRoles.Add(ar);
                                _permissionService.UpdatePermissionRecord(permission);
                            }
                        }
                        else
                        {
                            if (permission.AccountRoles.FirstOrDefault(x => x.Id == ar.Id) != null)
                            {
                                permission.AccountRoles.Remove(ar);
                                _permissionService.UpdatePermissionRecord(permission);
                            }
                        }
                    }
                }
            }
        }


        private IDictionary<string, object> _SerializeExpando(object obj)
        {
            var result = new Dictionary<string, object>();
            var dictionary = obj as IDictionary<string, object>;
            foreach (var item in dictionary)
                result.Add(item.Key, item.Value);
            return result;
        }
    }
}
