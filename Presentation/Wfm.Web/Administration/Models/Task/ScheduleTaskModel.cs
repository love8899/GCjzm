using FluentValidation.Attributes;
using System;
using Wfm.Admin.Validators.Task;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.ScheduleTask
{
    [Validator(typeof(ScheduleTaskValidator))]
    public class ScheduleTaskModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.ScheduleTask.Fields.Seconds")]
        public int Seconds { get; set; }

        [WfmResourceDisplayName("Common.Type")]
        public string Type { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.ScheduleTask.Fields.StopOnError")]
        public bool StopOnError { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.ScheduleTask.Fields.LastStartUtc")]
        public DateTime? LastStartUtc { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.ScheduleTask.Fields.LastEndUtc")]
        public DateTime? LastEndUtc { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.ScheduleTask.Fields.LastSuccessUtc")]
        public DateTime? LastSuccessUtc { get; set; }
    }
}