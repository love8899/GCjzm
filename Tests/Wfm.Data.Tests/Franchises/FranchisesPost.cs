using NUnit.Framework;
using Wfm.Tests;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Data.Tests.Franchises
{
    [TestFixture]
    public class FranchisesPost : PersistenceTest
    {
        [Test]
        public void Can_Save_and_load_FranchisesPost()
        {

            var franchiseAddress = new FranchiseAddress
            {
                UnitNumber = "5553",
                AddressLine1 = "North York Street",
                PostalCode = "N4G 1M9",
            };

            var franchise = new Franchise
            {
                FranchiseName = "GC",
                WebSite = "http://www.gc-employment.com",
                IsActive = true,
                ReasonForDisabled = "No Reason",
                EnteredBy = 1,
                CreatedOnUtc = System.DateTime.UtcNow,
                UpdatedOnUtc = System.DateTime.UtcNow
            };

            var fromDbFranchiseAddress = SaveAndLoadEntity(franchiseAddress);
            var fromDbFranchise = SaveAndLoadEntity(franchise);
          //  var fromDbFranchiseContact = SaveAndLoadEntity(franchiseContact);

            fromDbFranchise.ShouldNotBeNull();
        }
    }
}
