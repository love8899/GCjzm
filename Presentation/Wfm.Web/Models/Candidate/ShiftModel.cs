using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Candidate
{
    public partial class ShiftModel : BaseWfmEntityModel
    {
        public string ShiftName { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public bool IsActive { get; set; }

        public int EnteredBy { get; set; }

        public bool EnableInRegistration { get; set; }

        public bool EnableInSchedule { get; set; }
    }
}