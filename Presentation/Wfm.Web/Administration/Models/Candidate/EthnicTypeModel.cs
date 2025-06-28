using FluentValidation.Attributes;
using Wfm.Admin.Validators.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Candidate
{
    [Validator(typeof(EthnicTypeValidator))]
    public partial class EthnicTypeModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.EthnicType.Fields.EthnicTypeName")]
        public string EthnicTypeName { get; set; }

        //[WfmResourceDisplayName("Common.Description")]
        //public string Description { get; set; }

        //[WfmResourceDisplayName("Common.Note")]
        //public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

    }
}