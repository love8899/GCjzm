using System;
using Wfm.Web.Framework;

namespace Wfm.Admin.Models.JobOrder
{
    public class JobOrderDateModel
    {
        public int JobOrderId { get; set; }
        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime StartDate { get; set; }
        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime? EndDate { get; set; }
    }
}