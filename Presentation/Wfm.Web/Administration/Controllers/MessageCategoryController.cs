using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Models.Messages;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Extensions;
using Wfm.Services.Localization;

namespace Wfm.Admin.Controllers
{

    public class MessageCategoryController : BaseAdminController
    {

        #region Field
        private readonly IPermissionService _permissionService;
        private readonly IMessageCategoryService _messageCategoryService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public MessageCategoryController(IMessageCategoryService messageCategoryService,
                                           IPermissionService permissionService,
                                              ILocalizationService localizationService)
        {
            _messageCategoryService = messageCategoryService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }
        #endregion
        // GET: MessageCategory
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageCategory))
                return AccessDeniedView();

            return View();
        }

        //Post: Position
        [HttpPost]
        public ActionResult Index(DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageCategory))
                return AccessDeniedView();
            var messageCategories = _messageCategoryService.GetAllMessageCategories();
            DataSourceResult result = new DataSourceResult { Data = messageCategories, Total = messageCategories.Count };
            return Json(result);
        }

        [HttpPost]
        public ActionResult EditMessageCategory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MessageCategoryModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageCategory))
                return AccessDeniedView();

            if (models != null && ModelState.IsValid)
            {
                foreach (MessageCategoryModel model in models)
                {
                    try
                    {
                        var entity = _messageCategoryService.GetMessageCategoryById(model.Id);
                        model.UpdatedOnUtc = DateTime.UtcNow;
                        model.ToEntity(entity);
                        _messageCategoryService.UpdateMessageCategory(entity);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("Id", ex.ToString());
                    }
                }
            }
            return Json(models.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateMessageCategory([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<MessageCategoryModel> models)
        {
            var results = new List<MessageCategoryModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    model.IsActive = true;
                    model.UpdatedOnUtc = DateTime.UtcNow;
                    model.CreatedOnUtc = DateTime.UtcNow;
                    var entity = model.ToEntity();
                    _messageCategoryService.InsertMessageCategory(entity);
                    model.Id = entity.Id;
                    results.Add(model);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }
    }
}