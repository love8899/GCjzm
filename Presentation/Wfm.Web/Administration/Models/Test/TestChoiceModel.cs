using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Test;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Test
{
    [Validator(typeof(TestChoiceValidator))]
    public class TestChoiceModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.TestQuestion")]
        public int TestQuestionId { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.TestChoice.Fields.TestChoiceText")]
        [AllowHtml]
        public string TestChoiceText { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.TestChoice.Fields.TestChoiceValue")]
        [AllowHtml]
        public string TestChoiceValue { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.TestChoice.Fields.ImageFileLocation")]
        [AllowHtml]
        public string ImageFileLocation { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }


        // helper fields
        public string TestQuestionQuestion { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.TestChoice.Fields.ImageFileUrl")]
        public string ImageFileUrl { get; set; }

    }
}