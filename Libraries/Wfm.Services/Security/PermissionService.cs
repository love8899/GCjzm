using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.Security;
using Wfm.Services.Accounts;


namespace Wfm.Services.Security
{
    public partial class PermissionService :IPermissionService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : account role ID
        /// {1} : permission system name
        /// </remarks>
        /// 
        private const string PERMISSIONS_ALLOWED_KEY = "Wfm.permission.allowed-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PERMISSIONS_PATTERN_KEY = "Wfm.permission.";
        #endregion

        #region Fields

        private readonly IRepository<PermissionRecord> _permissionPecordRepository;
        private readonly IRepository<Franchise> _franchiseRepository;
        private readonly IAccountService _accountService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="permissionPecordRepository">Permission repository</param>
        /// <param name="accountService">Account service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="cacheManager">Cache manager</param>
        public PermissionService(
            IRepository<PermissionRecord> permissionPecordRepository,
            IRepository<Franchise> franchiseRepository,
            IAccountService accountService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._permissionPecordRepository = permissionPecordRepository;
            this._franchiseRepository = franchiseRepository;
            this._accountService = accountService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="accountRole">Account role</param>
        /// <returns>true - authorized; otherwise, false</returns>
        protected virtual bool Authorize(string permissionRecordSystemName, AccountRole accountRole)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            string key = string.Format(PERMISSIONS_ALLOWED_KEY, accountRole.Id, permissionRecordSystemName);
            return _cacheManager.Get(key, () =>
            {
                foreach (var permission1 in accountRole.PermissionRecords)
                    if (permission1.SystemName.Equals(permissionRecordSystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }


        #endregion

        #region Methods

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void DeletePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionPecordRepository.Delete(permission);

            _cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }


        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordById(int permissionId)
        {
            if (permissionId == 0)
                return null;

            return _permissionPecordRepository.GetById(permissionId);
        }


        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in _permissionPecordRepository.Table
                        where pr.SystemName == systemName
                        orderby pr.Id
                        select pr;

            var permissionRecord = query.FirstOrDefault();
            return permissionRecord;
        }


        public virtual PermissionRecord GetPermissionRecordByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;

            var query = from pr in _permissionPecordRepository.Table
                        where pr.Name == name
                        orderby pr.Id
                        select pr;

            return query.FirstOrDefault();
        }


        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IList<PermissionRecord> GetAllPermissionRecords()
        {
            var query = from pr in _permissionPecordRepository.Table
                        orderby pr.Name
                        select pr;
            var permissions = query.ToList();
            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void InsertPermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionPecordRepository.Insert(permission);

            _cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }


        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void UpdatePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            _permissionPecordRepository.Update(permission);

            _cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }


        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void InstallPermissions(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 == null)
                {
                    //new permission (install it)
                    permission1 = new PermissionRecord()
                    {
                        Name = permission.Name,
                        SystemName = permission.SystemName,
                        Category = permission.Category,
                    };


                    //default account role mappings
                    var defaultPermissions = permissionProvider.GetDefaultPermissions();
                    foreach (var defaultPermission in defaultPermissions)
                    {
                        var accountRole = _accountService.GetAccountRoleBySystemName(defaultPermission.AccountRoleSystemName);
                        if (accountRole == null)
                        {
                            //new role (save it)
                            accountRole = new AccountRole()
                            {
                                AccountRoleName = defaultPermission.AccountRoleSystemName,
                                IsActive = true,
                                SystemName = defaultPermission.AccountRoleSystemName
                            };
                            _accountService.InsertAccountRole(accountRole);
                        }


                        var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                                      where p.SystemName == permission1.SystemName
                                                      select p).Any();
                        var mappingExists = (from p in accountRole.PermissionRecords
                                             where p.SystemName == permission1.SystemName
                                             select p).Any();
                        if (defaultMappingProvided && !mappingExists)
                        {
                            permission1.AccountRoles.Add(accountRole);
                        }
                    }

                    //save new permission
                    InsertPermissionRecord(permission1);
                }
            }
        }

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void UninstallPermissions(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordBySystemName(permission.SystemName);
                if (permission1 != null)
                {
                    DeletePermissionRecord(permission1);
                }
            }
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission)
        {
            return Authorize(permission, _workContext.CurrentAccount);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="account">Account</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission, Account account)
        {
            if (permission == null)
                return false;

            if (account == null)
                return false;

            return Authorize(permission.SystemName, account);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName)
        {
            return Authorize(permissionRecordSystemName, _workContext.CurrentAccount);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <param name="account">Account</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName, Account account)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            if (account == null)
                return false;

            var accountRoles = account.AccountRoles.Where(cr => cr.IsActive);
            foreach (var role in accountRoles)
                if (Authorize(permissionRecordSystemName, role))
                    //yes, we have such permission
                    return true;

            //no permission found
            return false;
        }


        public virtual TypeOfAccountEnum GetTypeOfAccount(Account account)
        {
            account = account == null ? account : _workContext.CurrentAccount;
            var defaultMSP = _franchiseRepository.Table.Where(x => x.IsDefaultManagedServiceProvider).FirstOrDefault();
            
            if (account.IsClientAccount)
                return TypeOfAccountEnum.Client;
            else
            {
                if (defaultMSP != null && account.FranchiseId == defaultMSP.Id)
                    return TypeOfAccountEnum.ManagedServiceProvider;
                else
                    return TypeOfAccountEnum.Vendor;
            }
        }

        #endregion

    }
}
