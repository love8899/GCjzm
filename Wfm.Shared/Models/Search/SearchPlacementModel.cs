using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Web.Framework;


namespace Wfm.Shared.Models.Search
{
    public class SearchPlacementModel : SearchJobOrderModel
    {
        protected DateTime _minDate = new DateTime(2014, 9, 1);
        protected DateTime _maxDate = DateTime.MaxValue;

        public SearchPlacementModel()
        {
            sf_From = _minDate;
            sf_To = _maxDate;

            AvaliableVendors = new List<SelectListItem>();
            AvailableJobOrders = new List<SelectListItem>();
        }

        public SearchPlacementModel(DateTime? from, DateTime? to) : this()
        {
            if (from.HasValue && from > _minDate && from <= _maxDate)
                sf_From = from.Value;
            if (to.HasValue && to >= _minDate)  // && to < _maxDate )
                sf_To = to.Value;
        }

        [WfmResourceDisplayName("Common.Vendor")]
        public int sf_FranchiseId { get; set; }

        [WfmResourceDisplayName("Common.BadgeId")]
        public int? sf_CandidateId { get; set; }

        [WfmResourceDisplayName("Common.EmployeeId")]
        public string sf_EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.FirstName")]
        public string sf_EmployeeFirstName { get; set; }
        [WfmResourceDisplayName("Common.LastName")]
        public string sf_EmployeeLastName { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime sf_From { get; set; }

        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime sf_To { get; set; }

        public IList<SelectListItem> AvailableJobOrders { get; set; }
        public IList<SelectListItem> AvaliableVendors { get; set; }

        public DateTime MinDate() { return _minDate; }
        public DateTime MaxDate() { return _maxDate; }
    }
}
