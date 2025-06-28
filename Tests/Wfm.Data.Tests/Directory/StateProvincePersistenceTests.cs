using Wfm.Core.Domain.Common;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests.Directory
{
    [TestFixture]
    public class StateProvincePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_stateProvince()
        {
            var stateProvince = new StateProvince
            {
                StateProvinceName = "California",
                Abbreviation = "CA",
                IsActive = true,
                DisplayOrder = 1,
                Country = new Country
                               {
                                   CountryName = "United States",
                                   TwoLetterIsoCode = "US",
                                   ThreeLetterIsoCode = "USA",
                                   NumericIsoCode = 1,
                                   IsActive = true,
                                   DisplayOrder = 1,
                               }
            };

            var fromDb = SaveAndLoadEntity(stateProvince);
            fromDb.ShouldNotBeNull();
            fromDb.StateProvinceName.ShouldEqual("California");
            fromDb.Abbreviation.ShouldEqual("CA");
            fromDb.IsActive.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);

            fromDb.Country.ShouldNotBeNull();
            fromDb.Country.CountryName.ShouldEqual("United States");
        }
    }
}
