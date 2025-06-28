using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Security;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests.Security
{
    [TestFixture]
    public class PermissionRecordPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_permissionRecord()
        {
            var permissionRecord = GetTestPermissionRecord();

            var fromDb = SaveAndLoadEntity(permissionRecord);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.SystemName.ShouldEqual("SystemName 2");
            fromDb.Category.ShouldEqual("Category 4");
        }

        [Test]
        public void Can_save_and_load_permissionRecord_with_customerRoles()
        {
            var permissionRecord = GetTestPermissionRecord();
            permissionRecord.AccountRoles.Add
                (
                    new AccountRole()
                    {
                        AccountRoleName = "Administrators",
                        SystemName = "Administrators"
                    }
                );


            var fromDb = SaveAndLoadEntity(permissionRecord);
            fromDb.ShouldNotBeNull();

            fromDb.AccountRoles.ShouldNotBeNull();
            (fromDb.AccountRoles.Count == 1).ShouldBeTrue();
            fromDb.AccountRoles.First().SystemName.ShouldEqual("Administrators");
        }

        protected PermissionRecord GetTestPermissionRecord()
        {
            return new PermissionRecord
            {
                Name = "Name 1",
                SystemName = "SystemName 2",
                Category = "Category 4",
            };
        }
    }
}
