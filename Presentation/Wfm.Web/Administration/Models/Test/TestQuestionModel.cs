using System.Web.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Test;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Test
{
    [Validator(typeof(TestQuestionValidator))]
    public class TestQuestionModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.TestCategory")]
        public int TestCategoryId { get; set; }

        [WfmResourceDisplayName("Common.TestQuestion")]
        public string Question { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.TestQuestion.Fields.IsSingleChoice")]
        public bool IsSingleChoice { get; set; }

        [WfmResourceDisplayName("Common.IsMultipleChoice")]
        public bool IsMultipleChoice { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.TestQuestion.Fields.ImageFileLocation")]
        [AllowHtml]
        public string ImageFileLocation { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.TestQuestion.Fields.Answers")]
        [AllowHtml]
        public string Answers { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.TestQuestion.Fields.Score")]
        public int Score { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }


        // helper field
        public string TestCategoryName { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.TestQuestion.Fields.ImageFileUrl")]
        public string ImageFileUrl { get; set; }

    }
}