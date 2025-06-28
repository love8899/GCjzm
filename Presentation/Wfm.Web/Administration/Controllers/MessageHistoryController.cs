using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Messages;
using Wfm.Core.Domain.Messages;
using Wfm.Core;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Web.Framework;

namespace Wfm.Admin.Controllers
{
    public class MessageHistoryController : BaseAdminController
    {
        #region Fields

        private readonly IMessageHistoryService _messageHistoryService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public MessageHistoryController(IMessageHistoryService messageHistoryService, 
            IPermissionService permissionService,
            IWorkContext workContext)
        {
            _messageHistoryService = messageHistoryService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        #endregion

        #region GET :/MessageHistory/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageHistory))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/MessageHistory/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageHistory))
                return AccessDeniedView();

            var messageHistories = _messageHistoryService.GetAllMessageHistoriesAsQueryable(_workContext.CurrentAccount).PagedForCommand(request);

            List<MessageHistoryModel> messageHistoryModelList = new List<MessageHistoryModel>();

            foreach (var item in messageHistories)
            {
                MessageHistoryModel model = MappingExtensions.ToModel(item);
                messageHistoryModelList.Add(model);
            }

            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = messageHistoryModelList, // Process data (paging and sorting applied)
                Total = messageHistories.TotalCount // Total number of records
            };

            return Json(result);
        }

        #endregion


        #region GET :/MessageHistory/Details

        [HttpGet]
        public ActionResult Details(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageHistory))
                return AccessDeniedView();

            MessageHistory messageHistory = _messageHistoryService.GetMessageHistoryById(id);

            MessageHistoryModel model = messageHistory.ToModel();

            return View(model);
        }

        #endregion

    }
}
