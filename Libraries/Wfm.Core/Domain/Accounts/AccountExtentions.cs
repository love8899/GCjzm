using System;
using System.Linq;

namespace Wfm.Core.Domain.Accounts
{
    public static class AccountExtentions
    {
        #region Account role

        /// <summary>
        /// Gets a value indicating whether account is in a certain account role
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="accountRoleSystemName">Account role system name</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active account roles</param>
        /// <returns>Result</returns>
        public static bool IsInAccountRole(this Account account,
            string accountRoleSystemName, bool onlyActiveAccountRoles = true)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            if (String.IsNullOrEmpty(accountRoleSystemName))
                throw new ArgumentNullException("accountRoleSystemName");

            var result = account.AccountRoles
                .FirstOrDefault(ar => (!onlyActiveAccountRoles || ar.IsActive) && (ar.SystemName == accountRoleSystemName)) != null;
            return result;
        }


        /// <summary>
        /// Gets a value indicating whether account a search engine
        /// </summary>
        /// <param name="account">Account</param>
        /// <returns>Result</returns>
        public static bool IsSearchEngineAccount(this Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            if (!account.IsSystemAccount || String.IsNullOrEmpty(account.SystemName))
                return false;

            var result = account.SystemName.Equals(SystemAccountNames.SearchEngine, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the account is a built-in record for background tasks
        /// </summary>
        /// <param name="account">Account</param>
        /// <returns>Result</returns>
        public static bool IsBackgroundTaskAccount(this Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            if (!account.IsSystemAccount || String.IsNullOrEmpty(account.SystemName))
                return false;

            var result = account.SystemName.Equals(SystemAccountNames.BackgroundTask, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether account is administrator
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active account roles</param>
        /// <returns>Result</returns>
        public static bool IsAdministrator(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.Administrators, onlyActiveAccountRoles);
        }


        public static bool IsOperator(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.Operators, onlyActiveAccountRoles);
        }

        
        public static bool IsClient(this Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            if (account.IsSystemAccount)
                return false;

            return account.IsClientAccount;
        }

        /// <summary>
        /// Gets a value indicating whether account belongs to a vendor (account's franchise is not MSP)
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool IsVendor(this Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            return !account.Franchise.IsDefaultManagedServiceProvider;
        }

        /// <summary>
        /// Gets a value indicating whether account is administrator
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active account roles</param>
        /// <returns>Result</returns>
        public static bool IsPayrollAdministrator(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.PayrollSpecialist, onlyActiveAccountRoles) ||
                IsInAccountRole(account, AccountRoleSystemNames.PayrollAdministrators, onlyActiveAccountRoles);
        }

        /// <summary>
        /// Gets a value indicating whether account is a forum moderator
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active account roles</param>
        /// <returns>Result</returns>
        public static bool IsForumModerator(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.ForumModerators, onlyActiveAccountRoles);
        }

        public static bool IsClientAdministrator(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.ClientAdministrators, onlyActiveAccountRoles);
        }

        /// <summary>
        /// Gets a value indicating whether account is company administrator
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active account roles</param>
        /// <returns>Result</returns>
        public static bool IsCompanyAdministrator(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.CompanyAdministrators, onlyActiveAccountRoles);
        }

        public static bool IsCompanyHrManager(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.CompanyHrManagers, onlyActiveAccountRoles);
        }

        public static bool IsCompanyLocationManager(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.CompanyLocationManagers, onlyActiveAccountRoles);
        }

        public static bool IsCompanyDepartmentSupervisor(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.CompanyDepartmentSupervisors, onlyActiveAccountRoles);
        }

        public static bool IsCompanyDepartmentManager(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.CompanyDepartmentManager, onlyActiveAccountRoles);
        }
        //public static bool IsMSPAdmin(this Account account, bool onlyActiveAccountRoles = true)
        //{
        //    return IsInAccountRole(account, AccountRoleSystemNames.Administrators, onlyActiveAccountRoles);
        //}

        //public static bool IsMSPPayrollAdmin(this Account account, bool onlyActiveAccountRoles = true)
        //{
        //    return IsInAccountRole(account, AccountRoleSystemNames.PayrollAdministrators, onlyActiveAccountRoles);
        //}

        public static bool IsMSPRecruiterSupervisor(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.RecruiterSupervisors, onlyActiveAccountRoles);
        }

        public static bool IsMSPRecruiter(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.Recruiters, onlyActiveAccountRoles);
        }

        public static bool IsVendorAdmin(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.VendorAdministrators, onlyActiveAccountRoles);
        }

        public static bool IsVendorRecruiter(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.VendorRecruiters, onlyActiveAccountRoles);
        }

        public static bool IsVendorRecruiterSupervisor(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.VendorRecruiterSupervisors, onlyActiveAccountRoles);
        }

        public static bool IsRecruiterSupervisor(this Account account, bool onlyActiveAccountRoles = true)
        {
            return account.IsMSPRecruiterSupervisor() || account.IsVendorRecruiterSupervisor();
        }

        public static bool IsRecruiter(this Account account, bool onlyActiveAccountRoles = true)
        {
            return account.IsMSPRecruiter() || account.IsVendorRecruiter();
        }

        public static bool IsRecruiterOrRecruiterSupervisor(this Account account, bool onlyActiveAccountRoles = true)
        {
            return account.IsRecruiterSupervisor() || account.IsRecruiter();
        }

        /// <summary>
        /// Gets a value indicating whether account is registered
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active account roles</param>
        /// <returns>Result</returns>
        public static bool IsRegistered(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.Registered, onlyActiveAccountRoles);
        }

        /// <summary>
        /// Gets a value indicating whether account is guest
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="onlyActiveAccountRoles">A value indicating whether we should look only in active account roles</param>
        /// <returns>Result</returns>
        public static bool IsGuest(this Account account, bool onlyActiveAccountRoles = true)
        {
            return IsInAccountRole(account, AccountRoleSystemNames.Guests, onlyActiveAccountRoles);
        }


        public static bool IsAdminOrRecruiterSupervisor(this Account account, bool onlyActiveAccountRoles = true)
        {
            bool isAdmin = IsInAccountRole(account, AccountRoleSystemNames.Administrators, onlyActiveAccountRoles);
            bool isRecruiterSupervisor = IsInAccountRole(account, AccountRoleSystemNames.RecruiterSupervisors, onlyActiveAccountRoles);
            bool isVendorAdmin = IsInAccountRole(account, AccountRoleSystemNames.VendorAdministrators, onlyActiveAccountRoles);
            bool isVendorRecruiterSupervisor = IsInAccountRole(account, AccountRoleSystemNames.VendorRecruiterSupervisors, onlyActiveAccountRoles);
            return ( isAdmin||isRecruiterSupervisor||isVendorAdmin||isVendorRecruiterSupervisor);
        }

        #endregion

    }
}
