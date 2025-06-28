using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Web.Framework;


namespace Wfm.Shared.Models.Search
{
    public class SearchIncidentModel : SearchPlacementModel
    {
        public SearchIncidentModel()
        {
            sf_To = _maxDate = DateTime.Today;

            AvailableIncidentCategories = new List<SelectListItem>();
        }

        public SearchIncidentModel(DateTime? from, DateTime? to) : this()
        {
            if (from.HasValue && from > _minDate && from <= _maxDate)
                sf_From = from.Value;
            if (to.HasValue && to >= _minDate)  // && to < _maxDate )
                sf_To = to.Value;
        }

        [WfmResourceDisplayName("Admin.Configuration.IncidentCategory")]
        public int sf_IncidentCategoryId { get; set; }

        public IList<SelectListItem> AvailableIncidentCategories { get; set; }
    }
}
