﻿using Wfm.Services.Localization;
using NUnit.Framework;
using Rhino.Mocks;

namespace Wfm.Web.MVC.Tests.Public.Validators
{
    [TestFixture]
    public abstract class BaseValidatorTests
    {
        protected ILocalizationService _localizationService;
        
        [SetUp]
        public void Setup()
        {
            //set up localziation service used by almost all validators
            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _localizationService.Expect(l => l.GetResource("")).Return("Invalid").IgnoreArguments();
        }
    }
}
