using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Logging;
using NUnit.Framework;
using Rhino.Mocks;

namespace Wfm.Services.Tests.Logging
{
    [TestFixture]
    public class AccountActivityServiceTests 
    {
        private ICacheManager _cacheManager;
        private IRepository<ActivityLog> _activityLogRepository;
        private IRepository<ActivityLogType> _activityLogTypeRepository;
        private IWorkContext _workContext;
        //private IActivityLogService _activityLogService;
        private ActivityLogType _activityType1, _activityType2;
        private ActivityLog _activity1, _activity2;
        private Account _account1, _account2;

        [SetUp]
        public new void SetUp()
        {
            _activityType1 = new ActivityLogType
            {
                Id = 1,
                ActivityLogTypeName = "TestKeyword1",
                IsActive = true,
                Description = "Test name1"
            };
            _activityType2 = new ActivityLogType
            {
                Id = 2,
                ActivityLogTypeName = "TestKeyword2",
                IsActive = true,
                Description = "Test name2"
            };
            _account1 = new Account()
            {
                Id = 1,
                Email = "test1@teststore1.com",
                Username = "TestUser1",
                IsDeleted = false,
            };
           _account2 = new Account()
           {
               Id = 2,
               Email = "test2@teststore2.com",
               Username = "TestUser2",
               IsDeleted = false,
           };
            _activity1 = new ActivityLog()
            {
                Id = 1,
                ActivityLogType = _activityType1,
                AccountId = _account1.Id,
            };
            _activity2 = new ActivityLog()
            {
                Id = 2,
                ActivityLogType = _activityType1,
                AccountId = _account2.Id,
            };
            _cacheManager = new WfmNullCache();
            _workContext = MockRepository.GenerateMock<IWorkContext>();
            _activityLogRepository = MockRepository.GenerateMock<IRepository<ActivityLog>>();
            _activityLogTypeRepository = MockRepository.GenerateMock<IRepository<ActivityLogType>>();
            _activityLogTypeRepository.Expect(x => x.Table).Return(new List<ActivityLogType>() { _activityType1, _activityType2 }.AsQueryable());
            _activityLogRepository.Expect(x => x.Table).Return(new List<ActivityLog>() { _activity1, _activity2 }.AsQueryable());
            //_accountActivityService = new AccountActivityService(_cacheManager, _activityLogRepository, _activityLogTypeRepository, _workContext, null, null, null);
        }

    }
}
