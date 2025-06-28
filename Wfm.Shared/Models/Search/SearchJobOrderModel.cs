using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Web.Framework;


namespace Wfm.Shared.Models.Search
{
    public class SearchJobOrderModel : SearchCompanyModel
    {
        public SearchJobOrderModel()
        {
            sf_Start = DateTime.MinValue;
            sf_End = DateTime.MaxValue;

            AvailablePositions = new List<SelectListItem>();
            AvailableShifts = new List<SelectListItem>();
            AvailableStatus = new List<SelectListItem>();
        }

        public SearchJobOrderModel(DateTime? start, DateTime? end) : this()
        {
            if (start.HasValue)
                sf_Start = start.Value;
            if (end.HasValue)
                sf_End = end.Value;
        }

        [WfmResourceDisplayName("Common.JobOrderId")]
        public int sf_JobOrderId { get; set; }

        [WfmResourceDisplayName("Common.JobTitle")]
        public string sf_JobTitle { get; set; }

        [WfmResourceDisplayName("Common.Position")]
        public int sf_PositionId { get; set; }              // for search by Id
        [WfmResourceDisplayName("Common.Position")]
        public string sf_Position { get; set; }             // for search by name

        [WfmResourceDisplayName("Common.Shift")]
        public int sf_ShiftId { get; set; }                 // for search by Id
        [WfmResourceDisplayName("Common.Shift")]
        public string sf_Shift { get; set; }                // for search by name

        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime sf_Start { get; set; }              // start date

        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime sf_End { get; set; }                // end date

        public IList<SelectListItem> AvailablePositions { get; set; }
        public IList<SelectListItem> AvailableShifts { get; set; }
        public IList<SelectListItem> AvailableStatus { get; set; }
    }
}
