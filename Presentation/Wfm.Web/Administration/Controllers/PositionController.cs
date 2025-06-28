using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;
using Wfm.Services.Common;
using Wfm.Services.Security;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Common;


namespace Wfm.Admin.Controllers
{
    public class PositionController :BaseAdminController
    {
        #region Field
        private readonly IPositionService _positionService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctro
        public PositionController(IPositionService positionService, IPermissionService permissionService, IWorkContext workContext)
        {
            _positionService = positionService;
            _permissionService = permissionService;
            _workContext = workContext;
        }
        #endregion
        // GET: Position
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePositions))
                return AccessDeniedView();
            return View();
        }

        #region Company Position Batch Edit/Create
        [HttpPost]
        public ActionResult _CompanyPositionList([DataSourceRequest]DataSourceRequest request, Guid? companyGuid)
        {
            List<Position> result = new List<Position>();
            if (companyGuid == null)
            {
                var positions = _positionService.GetAllPositions();
                if (_workContext.CurrentAccount.IsClientAdministrator())
                    result = positions.Where(x => x.CompanyId == _workContext.CurrentAccount.CompanyId).ToList();
                else
                    result = positions.ToList();
            }
            else
                result = _positionService.GetAllPositionByCompanyGuid(companyGuid).ToList();
            return Json(result.ToDataSourceResult(request,x=>x.ToModel()));
        }

        [HttpPost]
        public ActionResult _AddCompanyPosition([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PositionModel> models)
        {
            var results = new List<PositionModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    _positionService.InsertPosition(entity);

                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _EditCompanyPosition([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PositionModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _positionService.GetPositionById(model.Id);
                    model.ToEntity(entity);
                    _positionService.UpdatePosition(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _RemoveCompanyPosition([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PositionModel> models)
        {
            if (models.Any())
            {
                foreach (var model in models)
                {
                    var entity = _positionService.GetPositionById(model.Id);
                    _positionService.DeletePosition(entity);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}
