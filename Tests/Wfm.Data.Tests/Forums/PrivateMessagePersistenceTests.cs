using System;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Forums;
using Wfm.Core.Domain.Franchises;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests.Forums
{
    [TestFixture]
    public class PrivateMessagePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_privatemessage()
        {
            var franchise = GetTestFranchise();
            var franchiseFromDb = SaveAndLoadEntity(franchise);
            franchiseFromDb.ShouldNotBeNull();

            var account1 = GetTestAccount();
            var account1FromDb = SaveAndLoadEntity(account1);
            account1FromDb.ShouldNotBeNull();

            var account2 = GetTestAccount();
            var account2FromDb = SaveAndLoadEntity(account2);
            account2FromDb.ShouldNotBeNull();

            var privateMessage = new PrivateMessage
            {
                Subject = "Private Message 1 Subject",
                Text = "Private Message 1 Text",
                IsDeletedByAuthor = false,
                IsDeletedByRecipient = false,
                IsRead = false,
                CreatedOnUtc = DateTime.UtcNow,
                FromAccountId = account1FromDb.Id,
                ToAccountId = account2FromDb.Id,
                FranchiseId = franchise.Id,
            };

            var fromDb = SaveAndLoadEntity(privateMessage);
            fromDb.ShouldNotBeNull();
            fromDb.Subject.ShouldEqual("Private Message 1 Subject");
            fromDb.Text.ShouldEqual("Private Message 1 Text");
            fromDb.IsDeletedByAuthor.ShouldBeFalse();
            fromDb.IsDeletedByRecipient.ShouldBeFalse();
            fromDb.IsRead.ShouldBeFalse();
        }

        protected Account GetTestAccount()
        {
            return new Account
            {
                AccountGuid = Guid.NewGuid(),
                //Note = "some comment here",
                IsActive = true,
                IsDeleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }

        protected Franchise GetTestFranchise()
        {
            return new Franchise
            {
                FranchiseName = "Franchise 1",
                WebSite = "http://www.test.com",
                DisplayOrder = 1
            };
        }
    }
}
