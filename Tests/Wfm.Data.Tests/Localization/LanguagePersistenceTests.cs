using System.Linq;
using Wfm.Core.Domain.Localization;
using Wfm.Tests;
using NUnit.Framework;

namespace Wfm.Data.Tests.Localization
{
    [TestFixture]
    public class LanguagePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_language()
        {
            var lang = new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Rtl = true,
                IsActive = true,
                DisplayOrder = 1
            };

            var fromDb = SaveAndLoadEntity(lang);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("English");
            fromDb.LanguageCulture.ShouldEqual("en-Us");
            fromDb.UniqueSeoCode.ShouldEqual("en");
            fromDb.FlagImageFileName.ShouldEqual("us.png");
            fromDb.Rtl.ShouldEqual(true);
            fromDb.IsActive.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
        }

        [Test]
        public void Can_save_and_load_language_with_localeStringResources()
        {
            var lang = new Language
                           {
                               Name = "English",
                               LanguageCulture = "en-Us",
                               FlagImageFileName = "us.png",
                               IsActive = true,
                               DisplayOrder = 1
                           };
            lang.LocaleStringResources.Add
                (
                    new LocaleStringResource()
                    {
                        ResourceName = "ResourceName1",
                        ResourceValue = "ResourceValue2"
                    }
                );
            var fromDb = SaveAndLoadEntity(lang);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("English");

            fromDb.LocaleStringResources.ShouldNotBeNull();
            (fromDb.LocaleStringResources.Count == 1).ShouldBeTrue();
            fromDb.LocaleStringResources.First().ResourceName.ShouldEqual("ResourceName1");
        }
    }
}
