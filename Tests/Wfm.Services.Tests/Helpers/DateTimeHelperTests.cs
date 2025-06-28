using System;
using System.Collections.Generic;
using Wfm.Core;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Franchises;
using Wfm.Services.Common;
using Wfm.Services.Configuration;
using Wfm.Services.Helpers;
using Wfm.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Wfm.Services.Tests.Helpers
{
    [TestFixture]
    public class DateTimeHelperTests 
    {
        private IWorkContext _workContext;
        private IGenericAttributeService _genericAttributeService;
        private ISettingService _settingService;
        private DateTimeSettings _dateTimeSettings;
        private IDateTimeHelper _dateTimeHelper;
        private Franchise _franchise;

        [SetUp]
        public new void SetUp()
        {
            _genericAttributeService = MockRepository.GenerateMock<IGenericAttributeService>();
            _settingService = MockRepository.GenerateMock<ISettingService>();

            _workContext = MockRepository.GenerateMock<IWorkContext>();

            _franchise = new Franchise() { Id = 1 };

            _dateTimeSettings = new DateTimeSettings()
            {
                AllowCustomersToSetTimeZone = false,
                DefaultStoreTimeZoneId = ""
            };

            _dateTimeHelper = new DateTimeHelper(_workContext, _settingService, _dateTimeSettings);
        }

        [Test]
        public void Can_find_systemTimeZone_by_id()
        {
            var timeZones = _dateTimeHelper.FindTimeZoneById("E. Europe Standard Time");
            timeZones.ShouldNotBeNull();
            timeZones.Id.ShouldEqual("E. Europe Standard Time");
        }

        [Test]
        public void Can_get_all_systemTimeZones()
        {
            var systemTimeZones = _dateTimeHelper.GetSystemTimeZones();
            systemTimeZones.ShouldNotBeNull();
            (systemTimeZones.Count > 0).ShouldBeTrue();
        }

        [Test]
        public void Can_get_customer_timeZone_with_customTimeZones_enabled()
        {
            _dateTimeSettings.AllowCustomersToSetTimeZone = true;
            _dateTimeSettings.DefaultStoreTimeZoneId = "E. Europe Standard Time"; //(GMT+02:00) Minsk;

            var account = new Account()
            {
                Id = 10,
            };

            _genericAttributeService.Expect(x => x.GetAttributesForEntity(account.Id, "Account"))
                .Return(new List<GenericAttribute>()
                            {
                                new GenericAttribute()
                                    {
                                        FranchiseId = 0,
                                        EntityId = account.Id,
                                        Key = SystemAccountAttributeNames.TimeZoneId,
                                        KeyGroup = "Account",
                                        Value = "Russian Standard Time" //(GMT+03:00) Moscow, St. Petersburg, Volgograd
                                    }
                            });
            var timeZone = _dateTimeHelper.GetCustomerTimeZone(account);
            timeZone.ShouldNotBeNull();
            timeZone.Id.ShouldEqual("Russian Standard Time");
        }

        [Test]
        public void Can_get_customer_timeZone_with_customTimeZones_disabled()
        {
            _dateTimeSettings.AllowCustomersToSetTimeZone = false;
            _dateTimeSettings.DefaultStoreTimeZoneId = "E. Europe Standard Time"; //(GMT+02:00) Minsk;

            var account = new Account()
            {
                Id = 10,
            };

            _genericAttributeService.Expect(x => x.GetAttributesForEntity(account.Id, "Customer"))
                .Return(new List<GenericAttribute>()
                            {
                                new GenericAttribute()
                                    {
                                        FranchiseId = 0,
                                        EntityId = account.Id,
                                        Key = SystemAccountAttributeNames.TimeZoneId,
                                        KeyGroup = "Account",
                                        Value = "Russian Standard Time" //(GMT+03:00) Moscow, St. Petersburg, Volgograd
                                    }
                            });

            var timeZone = _dateTimeHelper.GetCustomerTimeZone(account);
            timeZone.ShouldNotBeNull();
            timeZone.Id.ShouldEqual("E. Europe Standard Time");
        }

        [Test]
        public void Can_convert_dateTime_to_userTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"); //(GMT+02:00) Minsk;
            sourceDateTime.ShouldNotBeNull();

            var destinationDateTime = TimeZoneInfo.FindSystemTimeZoneById("North Asia Standard Time"); //(GMT+08:00) Krasnoyarsk;
            destinationDateTime.ShouldNotBeNull();

            //summer time
            _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 06, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
                .ShouldEqual(new DateTime(2010, 06, 01, 6, 0, 0));

            //winter time
            _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 01, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
                .ShouldEqual(new DateTime(2010, 01, 01, 6, 0, 0));
        }

        [Test]
        public void Can_convert_dateTime_to_utc_dateTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"); //(GMT+02:00) Minsk;
            sourceDateTime.ShouldNotBeNull();

            //summer time
            var dateTime1 = new DateTime(2010, 06, 01, 0, 0, 0);
            var convertedDateTime1 = _dateTimeHelper.ConvertToUtcTime(dateTime1, sourceDateTime);
            convertedDateTime1.ShouldEqual(new DateTime(2010, 05, 31, 21, 0, 0));

            //winter time
            var dateTime2 = new DateTime(2010, 01, 01, 0, 0, 0);
            var convertedDateTime2 = _dateTimeHelper.ConvertToUtcTime(dateTime2, sourceDateTime);
            convertedDateTime2.ShouldEqual(new DateTime(2009, 12, 31, 22, 0, 0));
        }
    }
}
