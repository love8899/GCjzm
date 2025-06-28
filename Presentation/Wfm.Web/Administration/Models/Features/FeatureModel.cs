using FluentValidation.Attributes;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.Feature;


namespace Wfm.Admin.Models.Features
{
    [Validator(typeof(FeatureValidator))]
    public class FeatureModel : BaseWfmEntityModel
    {
        public string Area {get; set; }

        public string Code {get; set; }

        public string Name {get; set; }

        public string Description {get; set; }

        public bool IsActive { get; set; }
    }

}
