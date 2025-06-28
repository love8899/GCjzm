using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.JobOrders;


namespace Wfm.Client.Controllers
{
    public class JobOrderController : BaseClientController
    {
        #region Fields

        private readonly IJobOrderService _jobOrderService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public JobOrderController(IJobOrderService jobOrderService,
            IWorkContext workContext
        )
        {
            _jobOrderService = jobOrderService;
            _workContext = workContext;
        }

        #endregion   

        #region helper methods
        private List<SelectListItem> convertJobOredrListToSelectList(List<JobOrder> jobOrders)
        {
            var jobOrderList = new List<SelectListItem>();

            foreach (var j in jobOrders)
            {
                var item = new SelectListItem()
                {
                    Text = j.Id.ToString() + " --- " + j.JobTitle,
                    Value = j.Id.ToString()
                };
                jobOrderList.Add(item);
            }

            return jobOrderList;
        }

        private IQueryable<JobOrder> getFilteredJobOrdersAsQueryable(int locationId, int departmentId, DateTime startDate)
        {
            var cmpId = _workContext.CurrentAccount.CompanyId;

            var jobOrders = _jobOrderService.GetAllJobOrdersByCompanyIdAsQueryable(cmpId)
                                            .Where(x => (x.JobOrderStatusId == (int)JobOrderStatusEnum.Active || x.JobOrderStatusId == (int)JobOrderStatusEnum.Closed) &&
                                                        x.StartDate <= startDate && (!x.EndDate.HasValue || x.EndDate >= startDate) &&
                                                        x.CompanyLocationId == locationId);
            if (departmentId > 0)
                jobOrders = jobOrders.Where(x => x.CompanyDepartmentId == departmentId);

            jobOrders = jobOrders.OrderByDescending(x => x.Id);

            return jobOrders;
        }

        private List<SelectListItem> getFilteredJobOrdersAsList(int locationId, int departmentId, DateTime startDate)
        {
            var jobOrders = this.getFilteredJobOrdersAsQueryable(locationId, departmentId, startDate);

            return this.convertJobOredrListToSelectList(jobOrders.ToList());
        }

        #endregion


        #region //JsonResult : GetCascadeJobOrders

        public JsonResult GetCascadeJobOrders(string locationId, string dateString)
        {
            var locId = String.IsNullOrEmpty(locationId) ? 0 : Convert.ToInt32(locationId);

            var startDate = DateTime.Today;
            if (!String.IsNullOrEmpty(dateString))
                DateTime.TryParse(dateString, out startDate);

            var jobOrderList = this.getFilteredJobOrdersAsList(locId, 0, startDate);

            return Json(jobOrderList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeJobOrdersForScheduling(int locationId, int departmentId, int positionId, string dateString, DateTime startTime, DateTime endTime)
        {
            var startDate = DateTime.Today;
            if (!String.IsNullOrEmpty(dateString))
                DateTime.TryParse(dateString, out startDate);

            var jobOrders = this.getFilteredJobOrdersAsQueryable(locationId, departmentId, startDate).Where(x => x.PositionId == positionId).ToList();

            if (jobOrders.Count > 0)
               jobOrders = jobOrders.Where(x => x.StartTime.TimeOfDay == startTime.TimeOfDay && x.EndTime.TimeOfDay == endTime.TimeOfDay).ToList();


            var jobOrderList = this.convertJobOredrListToSelectList(jobOrders);

            return Json(jobOrderList, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}
