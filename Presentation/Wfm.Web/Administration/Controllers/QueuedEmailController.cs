using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Messages;
using Wfm.Core;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Helpers;
using Wfm.Services.Localization;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.UI;

namespace Wfm.Admin.Controllers
{
    public partial class QueuedEmailController : BaseAdminController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor
        public QueuedEmailController(IWorkContext workContext,
                                     IQueuedEmailService queuedEmailService,
                                     IDateTimeHelper dateTimeHelper, 
                                     ILocalizationService localizationService,
                                     IPermissionService permissionService)
        {
            this._workContext = workContext;
            this._queuedEmailService = queuedEmailService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }
        #endregion

        #region GET :/QueuedEmail/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/QueuedEmail/Index

        [HttpPost]
        public ActionResult List([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var messageHistories = _queuedEmailService.GetAllQueuedEmailsAsQueryable(_workContext.CurrentAccount).PagedForCommand(request);

            var result = new DataSourceResult
            {
                Data = messageHistories.Select(x =>
                {
                    var m = x.ToModel();

                    if (x.SentOnUtc.HasValue)
                        m.SentOn = _dateTimeHelper.ConvertToUserTime(x.SentOnUtc.Value, DateTimeKind.Utc);

                    //little hack here:
                    //ensure that email body is not returned
                    //otherwise, we can get the following error if emails have too long body:
                    //"Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property. "
                    //also it improves performance
                    //m.Body = "";

                    return m;
                }),
                Total = messageHistories.TotalCount
            };

            return Json(result);
        }

        #endregion

        //#region GET :/QueuedEmail/List

        //public ActionResult List()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
        //        return AccessDeniedView();

        //    var model = new QueuedEmailListModel();
        //    return View(model);
        //}

        //#endregion

        //#region POST:/QueuedEmail/List

        //[HttpPost]
        //public ActionResult QueuedEmailList(DataSourceRequest command, QueuedEmailListModel model)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
        //        return AccessDeniedView();

        //    DateTime? startDateValue = (model.SearchStartDate == null) ? null
        //                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchStartDate.Value, _dateTimeHelper.CurrentTimeZone);

        //    DateTime? endDateValue = (model.SearchEndDate == null) ? null
        //                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SearchEndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

        //    var queuedEmails = _queuedEmailService.SearchEmails(model.SearchFromEmail, model.SearchToEmail,
        //        startDateValue, endDateValue,
        //        model.SearchLoadNotSent, model.SearchMaxSentTries, true,
        //        command.Page - 1, command.PageSize);
        //    var gridModel = new DataSourceResult
        //    {
        //        Data = queuedEmails.Select(x =>
        //        {
        //            var m = x.ToModel();

        //            if (x.SentOnUtc.HasValue)
        //                m.SentOn = _dateTimeHelper.ConvertToUserTime(x.SentOnUtc.Value, DateTimeKind.Utc);

        //            //little hack here:
        //            //ensure that email body is not returned
        //            //otherwise, we can get the following error if emails have too long body:
        //            //"Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property. "
        //            //also it improves performance
        //            m.Body = "";

        //            return m;
        //        }),
        //        Total = queuedEmails.TotalCount
        //    };
        //    return new JsonResult
        //    {
        //        Data = gridModel
        //    };
        //}

        //#endregion

        //#region POST:/QueuedEmail/GoToEmailByNumber

        //[HttpPost, ActionName("List")]
        //[FormValueRequired("go-to-email-by-number")]
        //public ActionResult GoToEmailByNumber(QueuedEmailListModel model)
        //{
        //    var queuedEmail = _queuedEmailService.GetQueuedEmailById(model.GoDirectlyToNumber);
        //    if (queuedEmail != null)
        //        return RedirectToAction("Edit", "QueuedEmail", new { id = queuedEmail.Id });
        //    else
        //        return List();
        //}

        //#endregion

        #region GET :/QueuedEmail/Edit

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var email = _queuedEmailService.GetQueuedEmailById(id);
            if (email == null)
                //No email found with the specified id
                return RedirectToAction("Index");

            var model = email.ToModel();
            if (email.SentOnUtc.HasValue)
                model.SentOn = _dateTimeHelper.ConvertToUserTime(email.SentOnUtc.Value, DateTimeKind.Utc);
            return View(model);
        }

        #endregion

        #region POST:/QueuedEmail/Edit

        [HttpPost, ActionName("Edit")]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(QueuedEmailModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var email = _queuedEmailService.GetQueuedEmailById(model.Id);
            if (email == null)
                //No email found with the specified id
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                email = model.ToEntity(email);
                _queuedEmailService.UpdateQueuedEmail(email);

                SuccessNotification(_localizationService.GetResource("Admin.System.QueuedEmails.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = email.Id }) : RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            if (email.SentOnUtc.HasValue)
                model.SentOn = _dateTimeHelper.ConvertToUserTime(email.SentOnUtc.Value, DateTimeKind.Utc);
            return View(model);
        }
        #endregion

        #region POST:/QueuedEmail/Requeue

        [HttpPost, ActionName("Edit"), FormValueRequired("requeue")]
        public ActionResult Requeue(QueuedEmailModel queuedEmailModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var queuedEmail = _queuedEmailService.GetQueuedEmailById(queuedEmailModel.Id);
            if (queuedEmail == null)
                //No email found with the specified id
                return RedirectToAction("Index");

            var requeuedEmail = new QueuedEmail()
            {
                Priority = queuedEmail.Priority,
                From = queuedEmail.From,
                FromName = queuedEmail.FromName,
                To = queuedEmail.To,
                ToName = queuedEmail.ToName,
                ReplyTo = queuedEmail.ReplyTo,
                ReplyToName = queuedEmail.ReplyToName,
                CC = queuedEmail.CC,
                Bcc = queuedEmail.Bcc,
                Subject = queuedEmail.Subject,
                Body = queuedEmail.Body,
                AttachmentFilePath = queuedEmail.AttachmentFilePath,
                AttachmentFileName = queuedEmail.AttachmentFileName,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = queuedEmail.EmailAccountId
            };
            _queuedEmailService.InsertQueuedEmail(requeuedEmail);

            SuccessNotification(_localizationService.GetResource("Admin.System.QueuedEmails.Requeued"));
            return RedirectToAction("Edit", new { id = requeuedEmail.Id });
        }

        #endregion

        #region POST:/QueuedEmail/Delete

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            var email = _queuedEmailService.GetQueuedEmailById(id);
            if (email == null)
                //No email found with the specified id
                return RedirectToAction("Index");

            _queuedEmailService.DeleteQueuedEmail(email);

            SuccessNotification(_localizationService.GetResource("Admin.System.QueuedEmails.Deleted"));
            return RedirectToAction("Index");
        }

        #endregion

        #region POST:/QueuedEmail/DeleteSelected

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var queuedEmails = _queuedEmailService.GetQueuedEmailsByIds(selectedIds.ToArray());
                foreach (var queuedEmail in queuedEmails)
                    _queuedEmailService.DeleteQueuedEmail(queuedEmail);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region POST:/QueuedEmail/DeleteAll

        [HttpPost, ActionName("Index")]
        [FormValueRequired("delete-all")]
        public ActionResult DeleteAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageQueue))
                return AccessDeniedView();

            _queuedEmailService.DeleteAllEmails();

            SuccessNotification(_localizationService.GetResource("Admin.System.QueuedEmails.DeletedAll"));
            return RedirectToAction("Index");
        }

        #endregion
    }
}
