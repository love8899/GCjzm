using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Test
{
    public class TestCategoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Name")]

        public string TestCategoryName { get; set; }

        [WfmResourceDisplayName("Web.Tests.TestCategory.Fields.TestUrl")]
        public string TestUrl { get; set; }

        [WfmResourceDisplayName("Common.TotalScore")]
        public int TotalScore { get; set; }

        [WfmResourceDisplayName("Common.PassingScore")]
        public int PassScore { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public virtual List<TestQuestionModel> TestQuestion { get; set; }
    }
}