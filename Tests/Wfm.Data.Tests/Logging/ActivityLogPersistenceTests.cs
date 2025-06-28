using System;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Logging;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests.Logging
{
    [TestFixture]
    public class ActivityLogPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_activityLogType()
        {
            var activityLogType = new ActivityLogType
                               {
                                   ActivityLogTypeName = "Name 1",
                                   IsActive = true,
                               };

            var fromDb = SaveAndLoadEntity(activityLogType);
            fromDb.ShouldNotBeNull();
            fromDb.ActivityLogTypeName.ShouldEqual("Name 1");
            fromDb.IsActive.ShouldEqual(true);
        }

        protected Account GetTestAccount()
        {
            return new Account
            {
                AccountGuid = Guid.NewGuid(),
                CreatedOnUtc = new DateTime(2010, 01, 01),
                //LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }
    }
}