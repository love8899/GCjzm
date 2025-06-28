using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Accounts;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Core.Tests.Domain.Accounts
{
    [TestFixture]
    public class AccountTests
    {
        [Test]
        public void Can_check_IsInAccountRole()
        {
            var account = new Account()
            {
                /*AccountRoles = new List<AccountRole>()
                {
                    new AccountRole()
                    {
                        Active = true,
                        Name = "Test name 1",
                        SystemName = "Test system name 1",
                    },
                    new AccountRole()
                    {
                        Active = false,
                        Name = "Test name 2",
                        SystemName = "Test system name 2",
                    },
                }*/
            };

            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = true,
                AccountRoleName = "Test name 1",
                SystemName = "Test system name 1",
            });
            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = false,
                AccountRoleName = "Test name 2",
                SystemName = "Test system name 2",
            });
            account.IsInAccountRole("Test system name 1", false).ShouldBeTrue();
            account.IsInAccountRole("Test system name 1", true).ShouldBeTrue();

            account.IsInAccountRole("Test system name 2", false).ShouldBeTrue();
            account.IsInAccountRole("Test system name 2", true).ShouldBeFalse();

            account.IsInAccountRole("Test system name 3", false).ShouldBeFalse();
            account.IsInAccountRole("Test system name 3", true).ShouldBeFalse();
        }
        [Test]
        public void Can_check_whether_account_is_admin()
        {
            var account = new Account();

            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = true,
                AccountRoleName = "Registered",
                SystemName = AccountRoleSystemNames.Registered
            });
            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = true,
                AccountRoleName = "Guests",
                SystemName = AccountRoleSystemNames.Guests
            });

            account.IsAdministrator().ShouldBeFalse();

            account.AccountRoles.Add(
                new AccountRole()
                {
                    IsActive = true,
                    AccountRoleName = "Administrators",
                    SystemName = AccountRoleSystemNames.Administrators
                });
            account.IsAdministrator().ShouldBeTrue();
        }
        [Test]
        public void Can_check_whether_account_is_forum_moderator()
        {
            var account = new Account();

            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = true,
                AccountRoleName = "Registered",
                SystemName = AccountRoleSystemNames.Registered
            });
            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = true,
                AccountRoleName = "Guests",
                SystemName = AccountRoleSystemNames.Guests
            });

            account.IsForumModerator().ShouldBeFalse();

            account.AccountRoles.Add(
                new AccountRole()
                {
                    IsActive = true,
                    AccountRoleName = "ForumModerators",
                    SystemName = AccountRoleSystemNames.ForumModerators
                });
            account.IsForumModerator().ShouldBeTrue();
        }
        [Test]
        public void Can_check_whether_account_is_guest()
        {
            var account = new Account();

            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = true,
                AccountRoleName = "Registered",
                SystemName = AccountRoleSystemNames.Registered
            });

            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = true,
                AccountRoleName = "Administrators",
                SystemName = AccountRoleSystemNames.Administrators
            });

            account.IsGuest().ShouldBeFalse();

            account.AccountRoles.Add(
                new AccountRole()
                {
                    IsActive = true,
                    AccountRoleName = "Guests",
                    SystemName = AccountRoleSystemNames.Guests

                }
                );
            account.IsGuest().ShouldBeTrue();
        }
        [Test]
        public void Can_check_whether_account_is_registered()
        {
            var account = new Account();
            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = true,
                AccountRoleName = "Administrators",
                SystemName = AccountRoleSystemNames.Administrators
            });

            account.AccountRoles.Add(new AccountRole()
            {
                IsActive = true,
                AccountRoleName = "Guests",
                SystemName = AccountRoleSystemNames.Guests
            });

            account.IsRegistered().ShouldBeFalse();

            account.AccountRoles.Add(
                new AccountRole()
                {
                    IsActive = true,
                    AccountRoleName = "Registered",
                    SystemName = AccountRoleSystemNames.Registered
                });
            account.IsRegistered().ShouldBeTrue();
        }



        [Test]
        public void New_account_has_clear_password_type()
        {
            var account = new Account();
            account.PasswordFormat.ShouldEqual(PasswordFormat.Clear);
        }

        //[Test]
        //public void Can_add_address()
        //{
        //    var account = new Account();
        //    var address = new Address { Id = 1 };

        //    account.Addresses.Add(address);

        //    account.Addresses.Count.ShouldEqual(1);
        //    account.Addresses.First().Id.ShouldEqual(1);
        //}
        
        //[Test]
        //public void Can_remove_address_assigned_as_billing_address()
        //{
        //    var account = new Account();
        //    var address = new Address { Id = 1 };

        //    account.Addresses.Add(address);
        //    account.BillingAddress  = address;

        //    account.BillingAddress.ShouldBeTheSameAs(account.Addresses.First());

        //    account.RemoveAddress(address);
        //    account.Addresses.Count.ShouldEqual(0);
        //    account.BillingAddress.ShouldBeNull();
        //}

    }
}
