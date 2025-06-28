using System;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Logging;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests.Logging
{
    [TestFixture]
    public class LogPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_log()
        {
            var log = new Log
            {
                LogLevel = LogLevel.Error,
                ShortMessage = "ShortMessage1",
                FullMessage = "FullMessage1",
                IpAddress = "127.0.0.1",
                PageUrl = "http://www.someUrl1.com",
                ReferrerUrl = "http://www.someUrl2.com",
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };

            var fromDb = SaveAndLoadEntity(log);
            fromDb.ShouldNotBeNull();
            fromDb.LogLevel.ShouldEqual(LogLevel.Error);
            fromDb.ShortMessage.ShouldEqual("ShortMessage1");
            fromDb.FullMessage.ShouldEqual("FullMessage1");
            fromDb.IpAddress.ShouldEqual("127.0.0.1");
            fromDb.PageUrl.ShouldEqual("http://www.someUrl1.com");
            fromDb.ReferrerUrl.ShouldEqual("http://www.someUrl2.com");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_log_with_customer()
        {
            var log = new Log
            {
                LogLevel = LogLevel.Error,
                ShortMessage = "ShortMessage1",
                Account = GetTestAccount(),
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };

            var fromDb = SaveAndLoadEntity(log);
            fromDb.ShouldNotBeNull();
            fromDb.LogLevel.ShouldEqual(LogLevel.Error);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.Account.ShouldNotBeNull();
            //fromDb.Account.AdminComment.ShouldEqual("some comment here");
        }

        protected Account GetTestAccount()
        {
            return new Account
            {
                AccountGuid = Guid.NewGuid(),
                //AdminComment = "some comment here",
                IsActive = true,
                IsDeleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                //LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }
    }
}
