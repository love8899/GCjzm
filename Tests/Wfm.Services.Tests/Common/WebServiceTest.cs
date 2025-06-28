using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Franchises;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.Accounts;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.Logging;
using Wfm.Tests;
namespace Wfm.Services.Tests.Common
{
    [TestFixture]
    public class WebServiceTest
    {
        private IWebService _webService;
        private ICompanyDivisionService _companyDivisionService;
        private IAccountService _accountService;
        private IWorkContext _workContext;
        private IFranciseAddressService _franchiseAddressService;
        private ILogger _logger;
        private IJobBoardService _jobBoardService;
        private IJobOrderService _jobOrderService;
        private JobOrder jo= new JobOrder();
        
        [SetUp]
        public new void SetUp()
        {
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _jobOrderService = MockRepository.GenerateMock<IJobOrderService>();
            _companyDivisionService = MockRepository.GenerateMock<ICompanyDivisionService>();
            _jobBoardService = MockRepository.GenerateMock<IJobBoardService>();
            _accountService = MockRepository.GenerateMock<IAccountService>();
            _franchiseAddressService = MockRepository.GenerateMock<IFranciseAddressService>();
            _logger = MockRepository.GenerateMock<ILogger>();
            jo.Id = 12344;
            jo.CompanyId = 99;
            jo.CompanyLocationId = 1;
            jo.JobTitle = "Test Job Order";
            jo.JobOrderType = new JobOrderType() { JobOrderTypeName = "Permanent" };
            jo.JobOrderTypeId = 1;//1,2,3,20,75
            jo.SalaryMin = 100000.00m;
            jo.SalaryMax = 150000.00m;
            jo.Company = new Company() { CompanyName = "Test GC Company" };
            jo.JobDescription = @"<p>This is a test job order description.
            we show this one to you only for test. It does not have any useful meaning!</p>";
            jo.JobOrderCategoryId = 11;//11
            jo.FranchiseId = 1;

            _workContext.Expect(x => x.CurrentAccount).Return(new Account() { FirstName = "Nikki", LastName = "Duan", Email = "Nikki@gc-employment.com", WorkPhone = "465-789-7815" });
            _workContext.Expect(x => x.CurrentFranchise).Return(new Franchise() { Id = jo.FranchiseId, FranchiseGuid = Guid.NewGuid(), WebSite = "www.gc-employment.com" });
            _franchiseAddressService.Expect(x => x.GetAllFranchiseAddressByFranchiseGuid(_workContext.CurrentFranchise.FranchiseGuid)).Return(new List<FranchiseAddress>(){new FranchiseAddress()
            {
                AddressLine1 = "5050 Dufferin",
                AddressLine2 = "Unit 109"
                ,
                City = new Core.Domain.Common.City() { CityName = "North York" },
                StateProvince = new Core.Domain.Common.StateProvince() { StateProvinceName = "STName", Abbreviation = "ON" },
                Country = new Core.Domain.Common.Country() { CountryName = "Canada2", TwoLetterIsoCode = "CA" },
                PostalCode = "M3J0A5"
            }});
            _companyDivisionService.Expect(x => x.GetCompanyLocationById(jo.CompanyLocationId)).Return(new CompanyLocation()
            {
                AddressLine1 = "230 Tempus",
                AddressLine2 = "Suite 1023"
                ,
                City = new Core.Domain.Common.City() { CityName = "Halifax" },
                StateProvince = new Core.Domain.Common.StateProvince() { StateProvinceName = "STName3", Abbreviation = "NS" },
                Country = new Core.Domain.Common.Country() { CountryName = "Canada3", TwoLetterIsoCode = "CA" },
                PostalCode = "N2B0H2"
            });

            _jobOrderService.Expect(x=>x.UpdateJobOrder(jo));
            _webService = new WebService(_companyDivisionService,_accountService,_workContext,_franchiseAddressService,_logger,_jobBoardService,_jobOrderService);
        }

        [Test]
        public void IsValidJobXML()
        {
            string outMessage = string.Empty;
            var xml = _webService.CreateSoapEnvelope(jo, "xrtpjobsx01", "rtp987654", 178, out outMessage);
            bool result = _webService.ValidateJobXML(xml,out outMessage);
            result.ShouldBeTrue();
            outMessage.ShouldEqual(String.Empty);
        }

        [Test]
        public void CallWebService()
        {
            string outMessage = string.Empty;
            _webService.CallMonsterWebService("https://gateway.monster.com:8443/bgwBroker", jo, "xrtpjobsx01", "rtp987654",178,out outMessage);
            outMessage.ShouldEqual(string.Empty);
            jo.MonsterPostingId.ShouldNotBeNull();
        }
    }
}
