using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Web.Framework;


namespace Wfm.Shared.Models.Search
{
    public class SearchAttendanceModel : SearchPlacementModel
    {
        public SearchAttendanceModel()
        {
            //sf_To = _maxDate = DateTime.Today;

            AvailableStatus = new List<SelectListItem>();
        }

        public SearchAttendanceModel(DateTime? from, DateTime? to) : this()
        {
            if (from.HasValue && from > _minDate && from <= _maxDate)
                sf_From = from.Value;
            if (to.HasValue && to >= _minDate)  // && to < _maxDate )
                sf_To = to.Value;
        }

        [WfmResourceDisplayName("Common.ShiftStartTime")]
        public DateTime? sf_ShiftStartTime { get; set; }

        [WfmResourceDisplayName("Common.Status")]
        public string sf_Status { get; set; }

        // client local time
        public DateTime sf_ClientTime { get; set; }

        public new IList<SelectListItem> AvailableStatus { get; set; }
    }
}
