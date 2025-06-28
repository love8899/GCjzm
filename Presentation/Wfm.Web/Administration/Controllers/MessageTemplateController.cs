using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Messages;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using Kendo.Mvc.UI;
using System.Linq;
using Wfm.Services.Accounts;
using System.Text.RegularExpressions;
using Kendo.Mvc.Extensions;

namespace Wfm.Admin.Controllers
{
    public class MessageTemplateController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IAccountService _accountService;
        private readonly IEmailAccountService _emailAccountService;
        #endregion

        #region Ctor

        public MessageTemplateController(
            IActivityLogService activityLogService,
            IMessageTemplateService messageTemplateService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IAccountService accountService, IEmailAccountService emailAccountService
            )
        {
            _activityLogService = activityLogService;
            _messageTemplateService = messageTemplateService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _accountService = accountService;
            _emailAccountService = emailAccountService;
        }

        #endregion

        #region GET/POST: /MessageNotification/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            var messageTemplates = _messageTemplateService.GetAllMessageTemplates(0);

            List<MessageTemplateModel> messageTemplateModelList = new List<MessageTemplateModel>();

            foreach (var item in messageTemplates)
            {
                MessageTemplateModel i = item.ToModel();
                var account = _emailAccountService.GetEmailAccountById(item.EmailAccountId);
                if (account != null)
                    i.EmailAccountName = account.Email;
                messageTemplateModelList.Add(i);
            }

            return Json(messageTemplateModelList.ToDataSourceResult(request));
        }

        #endregion

        #region Get:MessageTemplate/Create

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            MessageTemplateModel messageTemplateModel = new MessageTemplateModel();

            messageTemplateModel.IsActive = true;
            return View(messageTemplateModel);
        }

        #endregion

        #region POST:/MessageTemplate/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(MessageTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();
            if(ModelState.IsValid)
                ValidateEmailAddresses(model.BccEmailAddresses, model.CCEmailAddresses);
            if (ModelState.IsValid)
            {
                var roles = model.AccountRolesIds != null ? model.AccountRolesIds.ToArray() : new int[] { };
                MessageTemplate messageTemplate = model.ToEntity();
                _messageTemplateService.InsertMessageTemplate(messageTemplate, roles);

                //activity log
                _activityLogService.InsertActivityLog("AddNewMessageTemplate", _localizationService.GetResource("ActivityLog.AddNewMessageTemplate"), messageTemplate.Subject);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.MessageTemplate.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = messageTemplate.Id }) : RedirectToAction("Index");

            }
            return View(model);
        } //end create post

        #endregion

        #region GET :/MessageNotification/Edit/5

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            MessageTemplate messageTemplate = _messageTemplateService.GetMessageTemplateById(id);
            if (messageTemplate == null)
                return RedirectToAction("Index");

            MessageTemplateModel messageTemplateModel = messageTemplate.ToModel();
            return View(messageTemplateModel);
        }
        #endregion

        #region POST:/MessageNotification/Edit/5

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(MessageTemplateModel messageTemplateModel, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return AccessDeniedView();

            MessageTemplate messageTemplate = _messageTemplateService.GetMessageTemplateById(messageTemplateModel.Id);
            if (messageTemplate == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
                ValidateEmailAddresses(messageTemplateModel.BccEmailAddresses, messageTemplateModel.CCEmailAddresses);
            if (ModelState.IsValid)
            {
                var roles = messageTemplateModel.AccountRolesIds != null ? messageTemplateModel.AccountRolesIds.ToArray() : new int[] { };

                messageTemplate = messageTemplateModel.ToEntity(messageTemplate);
                _messageTemplateService.UpdateMessageTemplate(messageTemplate, roles);

                //activity log
                _activityLogService.InsertActivityLog("UpdateMessageTemplate", _localizationService.GetResource("ActivityLog.UpdateMessageTemplate"), messageTemplate.Subject);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.MessageTemplate.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = messageTemplate.Id }) : RedirectToAction("Index");
            }


            return View(messageTemplateModel);
        }
        #endregion

        #region Validate CC and BCC email
        [NonAction]
        private void ValidateEmailAddresses(string bccEmails, string ccEmails)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var delimiters = new char[] { ';', ',' };
            if (!string.IsNullOrWhiteSpace(bccEmails))
            {
                var emails = bccEmails.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                foreach (var email in emails)
                {
                    
                    Match match = regex.Match(email);
                    if (match.Success)
                        continue;
                    else
                    {
                        ModelState.AddModelError("BccEmailAddresses", _localizationService.GetResource("Admin.Candidate.Candidate.Fields.Email.Invalid"));
                        break;
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(ccEmails))
            {
                var emails = ccEmails.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                foreach (var email in emails)
                {
                    Match match = regex.Match(email);
                    if (match.Success)
                        continue;
                    else
                    {
                        ModelState.AddModelError("CCEmailAddresses", _localizationService.GetResource("Admin.Candidate.Candidate.Fields.Email.Invalid"));
                        break;
                    }
                }
            }
        }
        #endregion
    } //end controll
} //end namespace