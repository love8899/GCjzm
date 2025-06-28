using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Localization;
using Wfm.Data;
using Wfm.Services.Configuration;
using Wfm.Services.Events;
using Wfm.Services.Localization;
using Wfm.Services.Franchises;
using Wfm.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Wfm.Services.Tests.Localization
{
    [TestFixture]
    public class LanguageServiceTests 
    {
        private IRepository<Language> _languageRepo;
        private ILanguageService _languageService;
        private ISettingService _settingService;
        private IEventPublisher _eventPublisher;
        private LocalizationSettings _localizationSettings;
        private IDataProvider _dataProvider;
        private IDbContext _dbContext;

        [SetUp]
        public new void SetUp()
        {
            _languageRepo = MockRepository.GenerateMock<IRepository<Language>>();
            var lang1 = new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                FlagImageFileName = "us.png",
                IsActive = true,
                DisplayOrder = 1
            };
            var lang2 = new Language
            {
                Name = "Russian",
                LanguageCulture = "ru-Ru",
                FlagImageFileName = "ru.png",
                IsActive = true,
                DisplayOrder = 2
            };

            _languageRepo.Expect(x => x.Table).Return(new List<Language>() { lang1, lang2 }.AsQueryable());

            var cacheManager = new WfmNullCache();

            _settingService = MockRepository.GenerateMock<ISettingService>();
            _dataProvider = MockRepository.GenerateMock<IDataProvider>();
            _dbContext = MockRepository.GenerateMock<IDbContext>();

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _localizationSettings = new LocalizationSettings();
            _languageService = new LanguageService(cacheManager, _dataProvider, _dbContext, _languageRepo);
        }

        [Test]
        public void Can_get_all_languages()
        {
            var languages = _languageService.GetAllLanguages();
            languages.ShouldNotBeNull();
            (languages.Count > 0).ShouldBeTrue();
        }
    }
}
