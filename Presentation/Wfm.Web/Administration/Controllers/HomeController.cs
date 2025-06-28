using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Messages;
using Wfm.Core;
using Wfm.Services.Franchises;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Services.Announcements;


namespace Wfm.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IFranchiseService _franchiseService;
        private readonly IMessageCategoryService _messageCategoryService;
        private readonly IMessageService _messageService;
        private readonly IWorkContext _workContext;

        private readonly Message_BL _message_BL;
        private readonly IAnnouncementService _announcementService;

        #endregion

        #region Ctor

        public HomeController(IPermissionService permissionService,
                              IFranchiseService franchiseService,
                              IMessageCategoryService messageCategoryServcie,
                              IMessageService messageService,
                              IWorkContext workContext,
                              IAnnouncementService announcementService
               )
        {
            _permissionService = permissionService;
            _franchiseService = franchiseService;
            _messageCategoryService = messageCategoryServcie;
            _messageService = messageService;
            _workContext = workContext;

            _announcementService = announcementService;
            _message_BL = new Message_BL(_workContext, _messageService);
        }

        #endregion


        public ActionResult News()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            return View();
        }
     
        public ActionResult _TabAnnouncements()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            return PartialView();
        }


        public ActionResult Index()
        {
           // return View();
            return RedirectToAction("SignIn", "Account");
        }


        #region Messages

        public ActionResult GetNumOfUnReadMsg()
        {
            var UnreadNum = _messageService.GetTheNumberOfUnreadMessages(_workContext.CurrentAccount.Id);

            return Content(UnreadNum > 0 ? UnreadNum.ToString() : String.Empty); 
        }


        public ActionResult _TabMessages(string viewName = "_TabMessages")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            ViewBag.MessageCategories = _messageCategoryService.GetAllMessageCategories(activeOnly:true).OrderBy(x => x.Id).Select(x => x.CategoryName).ToList();

            return PartialView(viewName);
        }


        [HttpPost]
        public ActionResult _MessageGrid([DataSourceRequest] DataSourceRequest request, MessageCriteria criteria)
        {
            var messages = _message_BL.GetAllMessagesByCriteria(criteria);
            var result = messages.ProjectTo<MessageModel>();

            return Json(result.ToDataSourceResult(request));
        }

        #endregion


        #region Message Details

        public ActionResult _MessageDetails(int id, MessageCriteria criteria)
        {
            var message = _messageService.GetMessageById(id);
            if (message.IsCandidate || message.AccountId != _workContext.CurrentAccount.Id)
                return AccessDeniedView();

            int prevMsgId, nextMsgId, currIndex, total;
            _message_BL.GetPreviousAndNextMessageIds(id, criteria, out prevMsgId, out nextMsgId, out currIndex, out total);
            ViewBag.PrevMsgId = prevMsgId;
            ViewBag.CurrMsgId = id;
            ViewBag.NextmsgId = nextMsgId;
            ViewBag.CurrIndex = currIndex;
            ViewBag.Total = total;
            ViewBag.IsRead = message.IsRead ? "true" : "false";

            // set IsRead
            _message_BL.MarkMessageReadStatus(message, isRead: true);

            var model = message.ToModel();

            return PartialView(model);
        }

        #endregion


        #region Download Message Template

        public ActionResult DownloadMessageAttachment(int id)
        {
            var messsage = _messageService.GetMessageById(id);

            if (messsage != null && !String.IsNullOrEmpty(messsage.MessageHistory.AttachmentFileName))
            {
                var attachmentFilePath = messsage.MessageHistory.AttachmentFilePath;
                var attachmentFileName = messsage.MessageHistory.AttachmentFileName;
                var attachmentFile = messsage.MessageHistory.AttachmentFile;
                var contentType = "application/octet-stream";

                if (!String.IsNullOrEmpty(attachmentFilePath))
                {
                    string filePath = Path.Combine(attachmentFilePath, attachmentFileName);

                    if (System.IO.File.Exists(filePath))
                        return File(filePath, contentType, attachmentFileName);
                }

                else if (attachmentFile.Length > 0)
                    return File(attachmentFile, contentType, attachmentFileName);
            }

            ErrorNotification("The message or attachment can not be found!");
            return RedirectToAction("News");
        }

        #endregion


        #region Mark Selected as Read

        [HttpPost]
        public ActionResult MarkSelectedAsRead(List<int> selectedIds)
        {
            var readNum = 0;

            if (selectedIds != null && selectedIds.Count > 0)
            {
                foreach(var id in selectedIds)
                {
                    var message = _messageService.GetMessageById(id);
                    if (!message.IsRead)
                    {
                        _message_BL.MarkMessageReadStatus(message, isRead: true);
                        readNum++;
                    }
                }
            }

            return Json(new { Result = true, ReadNum = readNum });
        }

        #endregion


        #region Mark Selected as Unread

        [HttpPost]
        public ActionResult MarkSelectedAsUnread(List<int> selectedIds)
        {
            var unreadNum = 0;

            if (selectedIds != null && selectedIds.Count > 0)
            {
                foreach (var id in selectedIds)
                {
                    var message = _messageService.GetMessageById(id);
                    if (message.IsRead)
                    {
                        _message_BL.MarkMessageReadStatus(message, isRead: false);
                        unreadNum++;
                    }
                }
            }

            return Json(new { Result = true, UnreadNum = unreadNum });
        }

        #endregion


        #region Announcements
        public ActionResult GetAnnouncementView()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var model = _announcementService.GetAnnouncementsForVendor(_workContext.CurrentAccount);
            return PartialView("_Announcements", model);
        }
        public ActionResult MarkAnnouncementAsRead(Guid announcementGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            bool result = _announcementService.MarkAnnouncementAsRead(_workContext.CurrentAccount, announcementGuid);
            return Json(new { success = result });
        }

        #endregion
    }
}
