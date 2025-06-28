using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wfm.Services.Common;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Common;
using Wfm.Services.Security;

namespace Wfm.Admin.Controllers
{
    public class JobBoardController : BaseAdminController
    {
        private readonly IJobBoardService _jobBoardService;
        private readonly IPermissionService _permissionService;
        public JobBoardController(IJobBoardService jobBoardService, IPermissionService permissionService)
        {
            _jobBoardService = jobBoardService;
            _permissionService = permissionService;
        }
        // GET: JobBoard
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobBoards))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public ActionResult Index(DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobBoards))
                return AccessDeniedView();
            var result = _jobBoardService.GetAllJobBoardsAsQuerable();
            return Json(result.ToDataSourceResult(request,m=>m.ToModel()));
        }

        #region Batch Edit/Create
        [HttpPost]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<JobBoardModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobBoards))
                return AccessDeniedView();
            var results = new List<JobBoardModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    if (_jobBoardService.IsDuplicate(entity))
                        ModelState.AddModelError("JobBoardName", "This Job Board already existed!");
                    else
                    {
                        _jobBoardService.Create(entity);
                        model.Id = entity.Id;
                        results.Add(model);
                    }
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<JobBoardModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobBoards))
                return AccessDeniedView();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _jobBoardService.Retrieve(model.Id);
                    model.ToEntity(entity);
                    if (_jobBoardService.IsDuplicate(entity))
                        ModelState.AddModelError("JobBoardName", "This Job Board already existed!");
                    else
                        _jobBoardService.Update(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<JobBoardModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageJobBoards))
                return AccessDeniedView();
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _jobBoardService.Retrieve(model.Id);
                    _jobBoardService.Delete(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}