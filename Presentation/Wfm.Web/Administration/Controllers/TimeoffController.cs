using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Timeoff;
using Wfm.Core.Domain.Employees;
using Wfm.Services.Employees;
using Wfm.Services.Security;
using Kendo.Mvc.Extensions;

namespace Wfm.Admin.Controllers
{
    public class TimeoffController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ITimeoffService _timeoffService;

        #endregion

        #region Ctor
        public TimeoffController(
            IPermissionService permissionService,
            ITimeoffService timeoffService)
        {
            _permissionService = permissionService;
            _timeoffService = timeoffService;
        }
        #endregion

        #region GET :/Timeoff Type/Index
        public ActionResult TimeoffTypeIndex()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTimeoffType))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult Index(DataSourceRequest request)
        {
            var timeoffTypeList = _timeoffService.GetAllTimeoffTypes();
            return Json(timeoffTypeList.ToDataSourceResult(request,m=>m.ToModel()));
        }
        #endregion


        #region batch edit/update/delete
        [HttpPost]
        public ActionResult CreateTimeoffType([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<EmployeeTimeoffTypeModel> models)
        {
            var results = new List<EmployeeTimeoffTypeModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    bool duplicate = _timeoffService.IsTimeoffTypeDuplicate(entity);
                    if (!duplicate)
                    {
                        _timeoffService.InsertNewTimeoffType(entity);
                        model.Id = entity.Id;
                        results.Add(model);
                    }
                    else
                        ModelState.AddModelError("Name", "The timeoff type is duplicate!");
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult UpdateTimeoffType([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<EmployeeTimeoffTypeModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    bool duplicate = _timeoffService.IsTimeoffTypeDuplicate(entity);
                    if (!duplicate)
                    {
                        _timeoffService.UpdateTimeoffType(entity);
                    }
                    else
                        ModelState.AddModelError("Name", "The timeoff type is duplicate!");

                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }



        #endregion
    }
}