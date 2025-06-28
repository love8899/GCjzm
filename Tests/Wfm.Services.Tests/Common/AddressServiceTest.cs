using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Wfm.Core.Domain.Common;
using Wfm.Core.Data;

namespace Wfm.Services.Tests.Common
{
    [TestFixture]
    public class AddressServiceTest
    {
        IRepository<City> _city;
        IRepository<StateProvince> _stateProvince;
        IRepository<Country> _country;

        [SetUp]
        public new void SetUp()
        {

            var city1 = new City
            {
                Id = 1,
                StateProvinceId =1,
                CityName ="Toronto"
            };

            var city2 = new City
            {
                Id = 2,
                StateProvinceId = 1,
                CityName = "Waterloo"
            };

            var stateProvince1 = new StateProvince
            {
                Id = 1,
                CountryId = 1,
                StateProvinceName = "ON"
            };

            var country1 = new Country
            {
                Id = 1,
                TwoLetterIsoCode = "CA",
                CountryName = "Canada"
            };


            _city = MockRepository.GenerateMock<IRepository<City>>();
            _city.Expect(x => x.Table).Return(new List<City>() { city1, city2 }.AsQueryable());

            _stateProvince = MockRepository.GenerateMock<IRepository<StateProvince>>();
            _stateProvince.Expect(x => x.Table).Return(new List<StateProvince>() { stateProvince1 }.AsQueryable());

            _country = MockRepository.GenerateMock<IRepository<Country>>();
            _country.Expect(x => x.Table).Return(new List<Country>() { country1 }.AsQueryable());


        }


    }

}
