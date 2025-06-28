using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests.Directory
{
    [TestFixture]
    public class CountryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_country()
        {
            var country = new Country
            {
                CountryName = "United States",
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                IsActive = true,
                DisplayOrder = 1,
            };

            var fromDb = SaveAndLoadEntity(country);
            fromDb.ShouldNotBeNull();
            fromDb.CountryName.ShouldEqual("United States");
            fromDb.TwoLetterIsoCode.ShouldEqual("US");
            fromDb.ThreeLetterIsoCode.ShouldEqual("USA");
            fromDb.NumericIsoCode.ShouldEqual(1);
            fromDb.IsActive.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
        }

        [Test]
        public void Can_save_and_load_country_with_states()
        {
            var country = new Country
            {
                CountryName = "United States",
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                IsActive = true,
                DisplayOrder = 1
            };
            country.StateProvinces.Add
                (
                    new StateProvince
                    {
                        StateProvinceName = "California",
                        Abbreviation = "CA",
                        DisplayOrder = 1
                    }
                );
            var fromDb = SaveAndLoadEntity(country);
            fromDb.ShouldNotBeNull();
            fromDb.CountryName.ShouldEqual("United States");

            fromDb.StateProvinces.ShouldNotBeNull();
            (fromDb.StateProvinces.Count == 1).ShouldBeTrue();
            fromDb.StateProvinces.First().StateProvinceName.ShouldEqual("California");
        }

    }
}
