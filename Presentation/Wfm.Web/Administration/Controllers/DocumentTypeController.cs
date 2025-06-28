using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Media;
using Wfm.Core.Domain.Media;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Media;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.Extensions;

namespace Wfm.Admin.Controllers
{
    public class DocumentTypeController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor
        public DocumentTypeController(IActivityLogService activityLogService,
            IDocumentTypeService documentTypeService,
            IPermissionService permissionService,
            ILogger logger,
            ILocalizationService localizationService)
        {
            _activityLogService = activityLogService;
            _documentTypeService = documentTypeService;
            _permissionService = permissionService;
            _logger = logger;
            _localizationService = localizationService;
        }
        #endregion

        #region GET :/DocumentType/Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDocumentTypes))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/DocumentType/Index
        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDocumentTypes))
                return AccessDeniedView();

            var documentTypes = _documentTypeService.GetAllDocumentTypes(true);
            List<DocumentTypeModel> DocumentTypeModelList = new List<DocumentTypeModel>();
            foreach (var item in documentTypes)
            {
                DocumentTypeModel model = MappingExtensions.ToModel(item);
                DocumentTypeModelList.Add(model);
            }
            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = DocumentTypeModelList, // Process data (paging and sorting applied)
                Total = documentTypes.Count // Total number of records
            };
            return Json(result);
        }

        #endregion

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<DocumentTypeModel> models)
        {
            var results = new List<DocumentTypeModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    model.UpdatedOnUtc = System.DateTime.UtcNow;
                    model.CreatedOnUtc = System.DateTime.UtcNow;
                    DocumentType documentType = model.ToEntity();
                    _documentTypeService.InsertDocumentType(documentType);
                    //activity log
                    _activityLogService.InsertActivityLog("AddNewDocumentType", _localizationService.GetResource("ActivityLog.AddNewDocumentType"), documentType.TypeName);
                    model.Id = documentType.Id;
                    results.Add(model);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Edit([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<DocumentTypeModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShifts))
                return AccessDeniedView();

            if (models != null && ModelState.IsValid)
            {
                foreach (DocumentTypeModel model in models)
                {
                    try
                    {
                        model.UpdatedOnUtc = System.DateTime.UtcNow;
                        DocumentType documentType = _documentTypeService.GetDocumentTypeById(model.Id);
                        if (documentType == null)
                            return RedirectToAction("Index");
                        documentType = model.ToEntity(documentType);
                        _documentTypeService.UpdateDocumentType(documentType);
                        //activity log
                        _activityLogService.InsertActivityLog("UpdateDocumentType", _localizationService.GetResource("ActivityLog.UpdateDocumentType"), documentType.TypeName);
                    }
                    catch (Exception ex)
                    {
                      ModelState.AddModelError("Id", _localizationService.GetLocaleStringResourceByName("Common.UnexpectedError").ResourceValue);
                     _logger.Error("EditDocumentType()", ex, userAgent: Request.UserAgent);
                    }
                }
            }
            return Json(models.ToDataSourceResult(request, ModelState));
        }


        #region POST:/DocumentType/Delete/
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDocumentTypes))
                return AccessDeniedView();

            var documentType = _documentTypeService.GetDocumentTypeById(id);
            if (documentType == null)
                //No document type found with the specified id
                return RedirectToAction("Index");
            try
            {
                _documentTypeService.DeleteDocumentType(documentType);

                //activity log
                _activityLogService.InsertActivityLog("DeleteDocumentType", _localizationService.GetResource("ActivityLog.DeleteDocumentType"), documentType.TypeName);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.DocumentType.Deleted"));
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = documentType.Id });
            }
        }
        #endregion
    }
}