using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Web.Framework;


namespace Wfm.Shared.Models.Search
{
    public class SearchTimeSheetModel : SearchPlacementModel
    {
        public SearchTimeSheetModel()
        {
            sf_To = _maxDate = DateTime.Today;

            AvailableCandidateWorkTimeStatus = new List<SelectListItem>();
        }

        public SearchTimeSheetModel(DateTime? from, DateTime? to) : this()
        {
            if (from.HasValue && from > _minDate && from <= _maxDate)
                sf_From = from.Value;
            if (to.HasValue && to >= _minDate)  // && to < _maxDate )
                sf_To = to.Value;
        }

        [WfmResourceDisplayName("Common.Status")]
        public int sf_CandidateWorkTimeStatusId { get; set; }

        public IList<SelectListItem> AvailableCandidateWorkTimeStatus { get; set; }
    }
}
