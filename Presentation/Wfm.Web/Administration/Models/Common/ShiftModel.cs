using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Common;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Common
{
    [Validator(typeof(ShiftValidator))]
    public partial class ShiftModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.Shift.Fields.ShiftName")]
        public string ShiftName { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public bool EnableInRegistration { get; set; }

        public bool EnableInSchedule { get; set; }

        public int CompanyId { get; set; }

        [UIHint("Time")]
        public DateTime? MinStartTime { get; set; }

        [UIHint("Time")]
        public DateTime? MaxEndTime { get; set; }
    }
}