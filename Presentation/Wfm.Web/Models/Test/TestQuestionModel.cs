using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Test
{
    public class TestQuestionModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.TestCategory")]
        public int TestCategoryId { get; set; }

        public int TestQuestionNum { get; set; }

        [WfmResourceDisplayName("Common.TestQuestion")]
        public string Question { get; set; }

        [WfmResourceDisplayName("Web.Tests.TestQuestion.Fields.IsSingleChoice")]
        public bool IsSingleChoice { get; set; }

        [WfmResourceDisplayName("Common.IsMultipleChoice")]
        public bool IsMultipleChoice { get; set; }

        [WfmResourceDisplayName("Web.Tests.TestQuestion.Fields.ImageFileLocation")]
        [AllowHtml]
        public string ImageFileLocation { get; set; }

        [WfmResourceDisplayName("Web.Tests.TestQuestion.Fields.Answers")]
        [AllowHtml]
        public string Answers { get; set; }

        [WfmResourceDisplayName("Web.Tests.TestQuestion.Fields.Score")]
        public int Score { get; set; }


        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public virtual TestCategoryModel TestCategoryModel { get; set; }
        public virtual List<TestChoiceModel> TestChoiceModelList { get; set; }


        public string ImageFileUrl { get; set; }

        public string CandidateChoice { get; set; }
    }
}