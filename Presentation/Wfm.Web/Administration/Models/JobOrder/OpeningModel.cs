using Wfm.Admin.Models.Commmon;
using Wfm.Web.Framework.CustomAttribute;


namespace Wfm.Admin.Models.JobOrder
{
    public class OpeningModel : SchedulerEvent
    {
        public int OpeningId { get; set; }

        [WfmRequired(ErrorMessage = "Common.JobOrderId.Required")]
        public int JobOrderId { get; set; }

        public int OpeningNumber { get; set; }

        public bool NoWork { get; set; }
        public string NoWorkNote { get; set; }
    }
}
