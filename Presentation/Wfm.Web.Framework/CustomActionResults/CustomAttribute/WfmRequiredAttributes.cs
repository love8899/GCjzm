using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Core.Infrastructure;
using Wfm.Services.Localization;


namespace Wfm.Web.Framework.CustomAttribute
{
    public class WfmRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        private static readonly ILocalizationService _localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = _localizationService.GetResource(this.ErrorMessage),
                ValidationType = "required"
            };
        }
    }
}
