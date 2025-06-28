using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Test;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Test
{
    [Validator(typeof(TestCategoryValidator))]
    public class TestCategoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Name")]
        public string TestCategoryName { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.TestCategory.Fields.TestUrl")]
        [AllowHtml]
        public string TestUrl { get; set; }

        [WfmResourceDisplayName("Common.TotalScore")]
        public int TotalScore { get; set; }

        [WfmResourceDisplayName("Common.PassingScore")]
        public int PassScore { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        [AllowHtml]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
        [WfmResourceDisplayName("TestCategory.IsRequiredWhenRegistration")]
        public bool IsRequiredWhenRegistration { get; set; }
    }
}