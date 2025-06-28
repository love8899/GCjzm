using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Test
{
    public class TestChoiceModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.TestQuestion")]
        public int TestQuestionId { get; set; }

        [WfmResourceDisplayName("Web.Tests.TestChoice.Fields.TestChoiceText")]
        public string TestChoiceText { get; set; }

        [WfmResourceDisplayName("Web.Tests.TestChoice.Fields.TestChoiceValue")]
        public string TestChoiceValue { get; set; }

        [WfmResourceDisplayName("Web.Tests.TestChoice.Fields.ImageFileLocation")]
        public string ImageFileLocation { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public virtual TestQuestionModel TestQuestionModel { get; set; }


        public string ImageFileUrl { get; set; }

    }
}