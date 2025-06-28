using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Incident;
using Wfm.Core;
using Wfm.Core.Domain.Incident;
using Wfm.Services.Franchises;
using Wfm.Services.Incident;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;

namespace Wfm.Admin.Controllers
{
    public class IncidentCategoryController : BaseAdminController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IActivityLogService _activityLogService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IIncidentService _incidentService;
        private readonly IFranchiseService _franchiseService;

        #endregion

        #region Ctor
        public IncidentCategoryController(
            IWorkContext workContext,
            IActivityLogService activityLogService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IIncidentService incidentService,
            IFranchiseService franchiseService)
        {
            _workContext = workContext;
            _activityLogService = activityLogService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _incidentService = incidentService;
            _franchiseService = franchiseService;
        }
        #endregion

        #region GET :/IncidentCategory/Index
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIncidentCategory))
                return AccessDeniedView();

            var incidentCategoryList = _incidentService.GetAllIncidentCategories(_workContext.CurrentAccount, true);

            List<IncidentCategoryModel> modelList = new List<IncidentCategoryModel>();
            foreach (var item in incidentCategoryList)
            {
                var model = item.ToModel();
                modelList.Add(model);
            }

            return View(modelList);
        }
        #endregion

        #region Partial Edit/Create Incident Category
        public ActionResult _EditIncidentCategory(int incidentCategoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIncidentCategory))
                return AccessDeniedView();

            var incident = _incidentService.GetAllIncidentCategories(_workContext.CurrentAccount)
                .Where(x => x.Id == incidentCategoryId).FirstOrDefault();
            var model = incident.ToModel();

            model.VendorList = new SelectList(_franchiseService.GetAllFranchisesAsQueryable(_workContext.CurrentAccount).ToArray(),
                "Id", "FranchiseName", model.FranchiseId);
            return PartialView("_CreateOrUpdate", model);
        }

        public ActionResult _NewIncidentCategory()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIncidentCategory))
                return AccessDeniedView();

            var incident = new IncidentCategory();
            var model = incident.ToModel();

            model.VendorList = new SelectList(_franchiseService.GetAllFranchisesAsQueryable(_workContext.CurrentAccount).ToArray(),
                "Id", "FranchiseName", model.FranchiseId);
            return PartialView("_CreateOrUpdate", model);
        }
        [HttpPost]
        public ActionResult _EditIncidentCategory(IncidentCategoryModel model)
        {
            try
            {
                var entity = model.ToEntity();
                if (entity.Id > 0)
                {
                    _incidentService.UpdateCategory(entity);
                }
                else
                {
                    entity.CreatedOnUtc = DateTime.UtcNow;
                    _incidentService.InsertNewCategory(entity);
                }
                return Content("done");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult _EditIncidentTemplate(int incidentCategoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIncidentCategory))
                return AccessDeniedView();

            var model = _incidentService.GetIncidentTemplates(incidentCategoryId).ToArray().Select(x => x.ToModel()).ToArray();
            ViewBag.IncidentCategoryId = incidentCategoryId;
            return PartialView("_Templates", model);
        }
        public ActionResult SaveTemplates(IEnumerable<HttpPostedFileBase> templates, int incidentCategoryId, string note)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIncidentCategory))
                return AccessDeniedView();

            // The Name of the Upload component is "attachments"
            foreach (var file in templates)
            {
                // prepare
                var fileName = Path.GetFileName(file.FileName);
                var contentType = file.ContentType;

                using (Stream stream = file.InputStream)
                {
                    var fileBinary = new byte[stream.Length];
                    stream.Read(fileBinary, 0, fileBinary.Length);

                    // upload attachment
                    var template = new IncidentReportTemplate()
                    {
                        IncidentCategoryId = incidentCategoryId,
                        FileName = fileName,
                        TemplateStream = fileBinary,
                        Note = note,
                        IsActive=true,
                    };
                    _incidentService.InsertIncidentReportTemplate(template);
                }
                //activity log
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult DeleteTemplate(int templateId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIncidentCategory))
                return AccessDeniedView();

            _incidentService.RemoveIncidentReportTemplate(templateId);

            // Return an empty string to signify success
            return Content("");
        }
        public ActionResult DownloadTemplate(int templateId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageIncidentCategory))
                return AccessDeniedView();
            var template = _incidentService.GetIncidentReportTemplate(templateId);
            Response.ClearContent();
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            //  NOTE: If you get an "HttpCacheability does not exist" error on the following line, make sure you have
            //  manually added System.Web to this project's References.
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.AddHeader("content-disposition", "attachment; filename=" + template.FileName);
            Response.ContentType = MimeMapping.GetMimeMapping(template.FileName);

            Response.BinaryWrite(template.TemplateStream);
            Response.Flush();
            Response.End();
            return new EmptyResult();
        }
        #endregion
    }
}