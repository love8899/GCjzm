using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Companies;
using Wfm.Web.Framework;


namespace Wfm.Shared.Models.Search
{
    public class SearchBusinessLogic
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IOrgNameService _orgNameService;

        #endregion


        #region Ctor

        public SearchBusinessLogic(IWorkContext workContextService, IOrgNameService orgNameService )
        {
            _workContext = workContextService;
            _orgNameService = orgNameService;
        }

        #endregion


        #region Company

        private void _PopulateSearchCompanyModel(SearchCompanyModel model, bool idVal = true)
        {
            model.AvailableLocations = _orgNameService.GetCompanyLocationsAsDropDownList(idVal);

            model.AvailableDepartments = _orgNameService.GetCompanyDepartmentsAsDropDownList(idVal);

            model.AvailableContacts = _orgNameService.GetClientAccountsAsDropDownList(idVal);
        }


        public SearchCompanyModel GetSearchCompanyModel(bool idVal = true)
        {
            var model = new SearchCompanyModel();
            if (_workContext.CurrentAccount.IsClientAccount)
                model.sf_CompanyId = _workContext.CurrentAccount.CompanyId;

            this._PopulateSearchCompanyModel(model, idVal);

            return model;
        }

        #endregion


        #region Job Order

        private void _PopulateSearchJobOrderModel(SearchJobOrderModel model, bool idVal = true)
        {
            this._PopulateSearchCompanyModel(model, idVal);

            model.AvailablePositions = _orgNameService.GetPositionsAsDropDownList(idVal);

            model.AvailableShifts = _orgNameService.GetShiftsAsDropDownList(idVal);

            model.AvailableStatus = JobOrderStatusEnum.Active.ToSelectList(false).ToList();
        }


        public SearchJobOrderModel GetSearchJobOrderModel(DateTime? start = null, DateTime? end = null, bool idVal = true)
        {
            var model = new SearchJobOrderModel(start, end);
            if (_workContext.CurrentAccount.IsClientAccount)
                model.sf_CompanyId = _workContext.CurrentAccount.CompanyId;

            this._PopulateSearchJobOrderModel(model, idVal);

            return model;
        }

        #endregion


        #region Placement

        public void PopulateSearchPlacementModel(SearchPlacementModel model, bool idVal = true)
        {
            this._PopulateSearchJobOrderModel(model, idVal);
            model.AvaliableVendors = _orgNameService.GetVendorsAsDropDownList(idVal);
        }


        public SearchPlacementModel GetSearchPlacementModel(DateTime? from = null, DateTime? to = null, bool idVal = true)
        {
            var model = new SearchPlacementModel(from, to);
            if (_workContext.CurrentAccount.IsClientAccount)
                model.sf_CompanyId = _workContext.CurrentAccount.CompanyId;

            this.PopulateSearchPlacementModel(model, idVal);

            return model;
        }

        #endregion


        #region Attendance

        private void _PopulateSearchAttendanceModel(SearchAttendanceModel model)
        {
            this.PopulateSearchPlacementModel(model);
            model.AvailableStatus = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Scheduled", Value = "Scheduled" },
                new SelectListItem() { Text = "Punched in", Value = "Punched in" },
                new SelectListItem() { Text = "No Show", Value = "No Show" }
            };
        }


        public SearchAttendanceModel GetSearchAttendanceModel(DateTime? from = null, DateTime? to = null)
        {
            var model = new SearchAttendanceModel(from, to);
            if (_workContext.CurrentAccount.IsClientAccount)
                model.sf_CompanyId = _workContext.CurrentAccount.CompanyId;

            _PopulateSearchAttendanceModel(model);

            return model;
        }

        #endregion


        #region Time Sheet

        private void _PopulateSearchTimeSheetModel(SearchTimeSheetModel model)
        {
            this.PopulateSearchPlacementModel(model);
            model.AvailableCandidateWorkTimeStatus = CandidateWorkTimeStatus.PendingApproval
                .ToSelectList(false, new int[] { (int)CandidateWorkTimeStatus.Matched, })
                .ToList<SelectListItem>();
        }


        public SearchTimeSheetModel GetSearchTimeSheetModel(DateTime? from = null, DateTime? to = null)
        {
            var model = new SearchTimeSheetModel(from, to);
            if (_workContext.CurrentAccount.IsClientAccount)
                model.sf_CompanyId = _workContext.CurrentAccount.CompanyId;

            this._PopulateSearchTimeSheetModel(model);

            return model;
        }

        #endregion


        #region Job Posting

        public SearchJobPostingModel GetSearchJobPostingModel(DateTime? start = null, DateTime? end = null, bool idVal = true)
        {
            var model = new SearchJobPostingModel(start, end);
            if (_workContext.CurrentAccount.IsClientAccount)
                model.sf_CompanyId = _workContext.CurrentAccount.CompanyId;

            _PopulateSearchJobOrderModel(model, idVal);

            return model;
        }

        #endregion
    }
}
