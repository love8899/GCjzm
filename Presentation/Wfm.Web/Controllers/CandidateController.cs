using Autofac.Features.Indexed;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Media;
using Wfm.Core.Domain.Tests;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Announcements;
using Wfm.Services.Candidates;
using Wfm.Services.ClockTime;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.JobOrders;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Media;
using Wfm.Services.Messages;
using Wfm.Services.Payroll;
using Wfm.Services.Test;
using Wfm.Services.TimeSheet;
using Wfm.Shared.Models.Accounts;
using Wfm.Shared.Extensions;
using Wfm.Web.Extensions;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Filters;
using Wfm.Web.Framework.Security;
using Wfm.Web.Models.Candidate;
using Wfm.Web.Models.Test;
using Wfm.Web.Models.TimeSheet;
using Wfm.Web.Models.Accounts;


namespace Wfm.Web.Controllers
{
    public class CandidateController : BaseWfmController
    {
        #region Fields

        private readonly ICandidateService _candidateService;
        private readonly ICandidateAddressService _candidateAddressService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IActivityLogService _activityLogService;
        private readonly ICandidateKeySkillService _candidateKeySkillsService;
        private readonly ICandidateTestResultService _candidateTestResultService;
        private readonly ICompanyService _companyService;
        private readonly IAttachmentService _attachmentService;
        private readonly IAttachmentTypeService _attachmentTypeService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IJobOrderTestCategoryService _jobOrderTestCategoryService;
        private readonly ITestService _testService;
        private readonly ISkillService _skillService;
        private readonly IClockDeviceService _clockDeviceService;
        private readonly IWorkTimeService _workTimeService;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWebHelper _webHelper;
        private readonly CandidateSettings _candidateSettings;
        private readonly CandidateWorkTimeSettings _candidateWorkTimeSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly ILogger _logger;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly ISecurityQuestionService _securityQuestionService;
        private readonly IFranchiseService _franchiseService;
        private readonly Wfm.Services.Accounts.IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly IAnnouncementService _announcementService;
        private readonly RegisterCandidate_BL _registerCandidate_BL;
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly IConfirmationEmailLinkService _confirmationEmailLinkService;
        private readonly IPaymentHistoryService _paymentHistoryService;
        private readonly ICandidateTestLinkService _candidateTestLinkService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly ICandidateAppliedJobsService _candidateAppliedJobsService;
        private readonly ICandidateAvailabilityService _availabilityService;
        private readonly IIndex<int, IT4BaseService> _t4Services;
        private readonly IIndex<int, IRL1BaseService> _rl1Services;
        private readonly ITaxFormService _taxFormService;

        #endregion


        #region Ctor

        public CandidateController(
            ICandidateService candidatesService,
            ICandidateAddressService candidateAddressService,
            ICandidateJobOrderService candidateJobOrderService,
            IActivityLogService activityLogService,
            ICandidateKeySkillService candidateKeySkillsService,
            ICandidateTestResultService candidateTestResultService,
            ICompanyService companyService,
            ISkillService skillService,
            IAttachmentService attachmentService,
            IAttachmentTypeService attachmentTypeService,
            IJobOrderService jobOrderService,
            IJobOrderTestCategoryService jobOrderTestCategoryService,
            ITestService testService,
            ILocalizationService localizationService,
            IGenericAttributeService genericAttributeService,
            IWorkflowMessageService workflowMessageService,
            IClockDeviceService clockDeviceService,
            IWorkTimeService workTimeService,
            IWebHelper webHelper,
            CandidateSettings candidateSettings,
            CandidateWorkTimeSettings candidateWorkTimeSettings,
            MediaSettings mediaSettings,
            ILogger logger,
            IDocumentTypeService documentTypeService,
            ISecurityQuestionService securityQuestionService,
            Wfm.Services.Accounts.IAccountPasswordPolicyService accountPasswordPolicyService,
            IFranchiseService franchiseService,
            IAnnouncementService announcementService,
           ICandidatePictureService candidatePictureService,
            IConfirmationEmailLinkService confirmationEmailService,
            IPaymentHistoryService paymentHistoryService,
            ICandidateTestLinkService candidateTestLinkService,
            ICompanyCandidateService companyCandidateService,
            ICandidateAppliedJobsService candidateAppliedJobsService,
            ICandidateAvailabilityService availabilityService,
            IIndex<int, IT4BaseService> t4Services,
            IIndex<int, IRL1BaseService> rl1Services,
            ITaxFormService taxFormService
            )
        {
            _candidateService = candidatesService;
            _candidateAddressService = candidateAddressService;
            _candidateJobOrderService = candidateJobOrderService;
            _activityLogService = activityLogService;
            _candidateKeySkillsService = candidateKeySkillsService;
            _candidateTestResultService = candidateTestResultService;
            _companyService = companyService;
            _skillService = skillService;
            _attachmentService = attachmentService;
            _attachmentTypeService = attachmentTypeService;
            _jobOrderService = jobOrderService;
            _jobOrderTestCategoryService = jobOrderTestCategoryService;
            _testService = testService;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
            _workflowMessageService = workflowMessageService;
            _clockDeviceService = clockDeviceService;
            _workTimeService = workTimeService;
            _webHelper = webHelper;
            _candidateSettings = candidateSettings;
            _candidateWorkTimeSettings = candidateWorkTimeSettings;
            _mediaSettings = mediaSettings;
            _logger = logger;
            _documentTypeService = documentTypeService;
            _securityQuestionService = securityQuestionService;
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _franchiseService = franchiseService;
            _announcementService = announcementService;
            _candidatePictureService = candidatePictureService;
            _paymentHistoryService = paymentHistoryService;
            _candidateTestLinkService = candidateTestLinkService;
            _registerCandidate_BL = new RegisterCandidate_BL(_candidateService, _candidateAddressService, _activityLogService, _candidateKeySkillsService, _attachmentService, _attachmentTypeService
                                                         , _localizationService, _genericAttributeService, _workflowMessageService, _candidateSettings, _logger, _documentTypeService, _accountPasswordPolicyService
                                                         , _candidatePictureService,_franchiseService); 
            _confirmationEmailLinkService = confirmationEmailService;
            _companyCandidateService = companyCandidateService;
            _candidateAppliedJobsService = candidateAppliedJobsService;

            _availabilityService = availabilityService;

            _t4Services = t4Services;
            _rl1Services = rl1Services;
            _taxFormService = taxFormService;
        }

        #endregion


        #region Utilities

        private string GetCandidateSearchKeyWords(IList<CandidateKeySkill> skills, CandidateAddress address)
        {
            IList<String> keyWords = new List<String>();
            foreach (var item in skills)
            {
                keyWords.Add(item.KeySkill);
            }

            return string.Join(", ", keyWords);
        }

        #endregion


        #region Index

        public ActionResult Index()
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var model = candidate.ToModel();
            var franchise = _franchiseService.GetFranchiseById(model.FranchiseId);
            model.FranchiseName = franchise?.FranchiseName;
            ViewBag.QrCodeStr = _candidateService.GetCandidateQrCodeStr(candidate);

            return View(model);
        }

        #endregion


        #region SignIn

        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult SignIn(string returnUrl, bool? alert = false)
        {
            if (String.IsNullOrWhiteSpace(returnUrl))
                returnUrl = Request.QueryString["returnUrl"];

            ViewBag.ReturnUrl = returnUrl;
            var model = new CandidateLogOnModel();
            if (alert.HasValue && alert.Value)
                ModelState.AddModelError("", "The session has expired or security tokens do not match");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult SignIn(CandidateLogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName != null) 
                    model.UserName = model.UserName.Trim();

                TimeSpan time = new TimeSpan();
                bool passwordIsExpired;
                bool showPasswordExpiryWarning;

                var loginResult = _candidateService.AuthenticateCandidate(model.UserName, model.Password, out time, out passwordIsExpired, out showPasswordExpiryWarning);
                switch (loginResult)
                {
                    case CandidateLoginResults.Successful:
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, false);
                            Candidate candidate = _candidateService.GetCandidateByUsername(model.UserName);

                            candidate.LastIpAddress = _webHelper.GetCurrentIpAddress();
                            candidate.LastLoginDateUtc = DateTime.UtcNow;
                            _candidateService.UpdateCandidate(candidate);

                            // Build Session
                            Session["CandidateId"] = (int)candidate.Id;

                            //activity log
                            _activityLogService.InsertActivityLog("Candidate.Login",
                                _localizationService.GetResource("ActivityLog.Candidate.Login"), candidate, model.UserName);

                            if (passwordIsExpired)
                            {
                                WarningNotification(_localizationService.GetResource("Web.Candidate.Candidate.Login.Fail.PasswordExpired"));
                                return RedirectToAction("ChangePasswordWhenPasswordExpired", "Candidate");
                            }

                            if (showPasswordExpiryWarning)
                                WarningNotification(String.Format(_localizationService.GetResource("Web.Candidate.Candidate.Login.Fail.PasswordWillExpire"), time.Days, time.Hours, time.Minutes));

                            if (!String.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);
                            else
                                return RedirectToAction("Index", "Candidate");
                        }

                    case CandidateLoginResults.CandidateNotExist:
                        {
                            Candidate candidate = null;
                            //activity log
                            _activityLogService.InsertActivityLog("Candidate.LoginFail",
                                _localizationService.GetResource("ActivityLog.Candidate.LoginFail"), candidate,
                                String.Concat(_localizationService.GetResource("Web.Candidate.Candidate.Login.Fail.CandidateNotExist") , " / " , model.UserName ) );

                            ErrorNotification(_localizationService.GetResource("Web.Candidate.Candidate.Login.Fail"));
                            break;
                        }

                    case CandidateLoginResults.NotActive:
                        {
                            Candidate candidate = null;
                            //activity log
                            _activityLogService.InsertActivityLog("Candidate.LoginFail",
                                _localizationService.GetResource("ActivityLog.Candidate.LoginFail"), candidate,
                                _localizationService.GetResource("Web.Candidate.Candidate.Login.Fail.NotActive") + " / " + model.UserName );

                            ErrorNotification(_localizationService.GetResource("Web.Candidate.Candidate.Login.Fail"));
                            break;
                        }

                    case CandidateLoginResults.Deleted:
                        {
                            Candidate candidate = null;
                            //activity log
                            _activityLogService.InsertActivityLog("Candidate.LoginFail",
                                _localizationService.GetResource("ActivityLog.Candidate.LoginFail"), candidate,
                                _localizationService.GetResource("Web.Candidate.Candidate.Login.Fail.Deleted") + " / " + model.UserName );

                            ErrorNotification(_localizationService.GetResource("Web.Candidate.Candidate.Login.Fail"));
                            break;
                        }

                    case CandidateLoginResults.NotRegistered:
                        {
                            //Candidate candidate = null;
                            ////activity log
                            //_activityLogService.InsertActivityLog("Candidate.LoginFail",
                            //    _localizationService.GetResource("ActivityLog.Candidate.LoginFail"), candidate,
                            //    _localizationService.GetResource("Web.Candidate.Candidate.Login.Fail.NotRegistered") + " / " + model.Email );

                            ErrorNotification(_localizationService.GetResource("Web.Candidate.Candidate.Login.Fail"));
                            break;
                        }

                    case CandidateLoginResults.WrongPassword:
                    default:
                        {
                            Candidate candidate = null;
                            //activity log
                            _activityLogService.InsertActivityLog("Candidate.LoginFail",
                                _localizationService.GetResource("ActivityLog.Candidate.LoginFail"), candidate,
                                _localizationService.GetResource("Web.Candidate.Candidate.Login.Fail.WrongPassword") + " / " + model.UserName);

                            ErrorNotification(_localizationService.GetResource("Web.Candidate.Candidate.Login.Fail"));
                            break;
                        }
                }
            }

            return View(model);
        }
    
        #endregion


        #region SignOut

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();

            Session["CandidateId"] = null;

            return RedirectToRoute("HomePage");
        }

        #endregion


        #region NewRegister

        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult NewRegister()
        {
            var model = new CreateEditCandidateModel()
            {
                CandidateModel = new CandidateModel()
                {
                    GenderId = (int)GenderEnum.Male,
                    SalutationId = (int)SalutationEnum.Mr,
                    TransportationId = (int)TransportationEnum.Other,
                    ShiftId = (int)ShiftEnum.Any,
                    EthnicTypeId = 1,
                    VetranTypeId = 1,
                    SourceId = 1
                },
                CountryId=2,
                LastUsedDate1 = DateTime.Today.AddDays(-1)
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult NewRegister(CreateEditCandidateModel model, HttpPostedFileBase attachments)
        {
            // step 1 - validate data

            // Check Agreement
            if (model.CandidateModel.Entitled == false)
            {
                ErrorNotification(_localizationService.GetResource("Web.Candidate.Candidate.Added.Fail.NotAccpectedTerm"));
                return PartialView(model);
            }

            // Validate password typo
            if (model.CandidateModel.Password != model.CandidateModel.RePassword)
            {
                ErrorNotification(_localizationService.GetResource("Common.EnteredPasswordsDoNotMatch"));
                return PartialView(model);
            }
            else 
            {
                StringBuilder errors = new StringBuilder();
                bool valid = _accountPasswordPolicyService.ApplyPasswordPolicy(0, "Candidate", model.CandidateModel.Password, String.Empty, Core.Domain.Accounts.PasswordFormat.Clear, String.Empty, out errors);
                if (!valid)
                {
                    ModelState.AddModelError("CandidateModel.Password", errors.ToString());
                    return PartialView(model);
                }
            }

            // Validate last used date
            if (model.LastUsedDate1 == null || model.LastUsedDate1 < System.DateTime.Now.AddYears(-50))
            {
                ErrorNotification(_localizationService.GetResource("Web.Candidate.Candidate.Added.Fail.InvalidLastUsedDate"));
                return PartialView(model);
            }

            string response = this.Request["g-recaptcha-response"];
            if (String.IsNullOrWhiteSpace(response) || !reCaptchaValidate.Validate(response))
            {
                ModelState.AddModelError("reCaptcha", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            // skip security questions and answers
            ModelState.Remove("CandidateModel.SecurityQuestion1Id");
            ModelState.Remove("CandidateModel.SecurityQuestion1Answer");
            ModelState.Remove("CandidateModel.SecurityQuestion2Id");
            ModelState.Remove("CandidateModel.SecurityQuestion2Answer");

            if (ModelState.IsValid)
            {
                int candidateId = 0;
                string message = "";
                bool result = _registerCandidate_BL.RegisterCandidate(model, attachments, out candidateId, out  message);
                if (result)
                {
                    return RedirectToAction("RegisterResult", new { message = message });
                }
                else
                {
                    ErrorNotification(message);
                    ViewBag.NewPage = false;
                    return PartialView(model);
                }
            }

            return PartialView(model);
        }


        public ActionResult RegisterResult(string message)
        {
            ViewBag.Message = message;
            return PartialView();
        }


        public ActionResult CandidateActivation(string token, string email)
        {
            var candidate = _candidateService.GetCandidateByEmail(email);
            if (candidate == null)
                return RedirectToRoute("HomePage");

            var cToken = candidate.GetAttribute<string>(SystemCandidateAttributeNames.CandidateActivationToken);
            if (String.IsNullOrEmpty(cToken))
                return RedirectToRoute("HomePage");

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            //activate user account
            candidate.IsActive = true;
            _candidateService.UpdateCandidate(candidate);
            _genericAttributeService.SaveAttribute(candidate, SystemCandidateAttributeNames.CandidateActivationToken, "");
            //send welcome message
            _workflowMessageService.SendCandidateWelcomeMessage(candidate, 1);
            //send required test link
            _workflowMessageService.SendCandidateTestsMessage(candidate, 1);
            var model = new CandidateActivationModel();
            model.Result = _localizationService.GetResource("Web.Candidate.Candidate.Activated");
            return View(model);
        }

        #endregion


        #region CandidateRecovery

        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult CandidateRecovery()
        {
            var model = new PasswordRecoveryModel();
            return View(model);
        }


        [HttpPost, ActionName("CandidateRecovery")]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult CandidateRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var candidate = _candidateService.GetCandidateByEmail(model.Email);

                if (candidate != null && candidate.IsActive && !candidate.IsBanned && !candidate.IsDeleted)
                {

                    // check if security questions are present for the candidate
                    if (candidate.SecurityQuestion1Id == null || candidate.SecurityQuestion2Id == null)
                    {
                        WarningNotification(_localizationService.GetResource("Common.PasswordRecovery.SecurityQuestionsNotPresent"));
                        return View(model);
                    }

                    // Prevent message attack
                    // --------------------------------
                    var cPrt = candidate.GetAttribute<string>(SystemCandidateAttributeNames.PasswordRecoveryToken);
                    if (!String.IsNullOrEmpty(cPrt))
                    {
                        var ga = _genericAttributeService.GetAttributesForEntity(candidate.Id, "Candidate").OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefault();
                        // check if the request interval is less than one minute
                        if (ga.UpdatedOnUtc >= (DateTime?)DateTime.UtcNow.AddMinutes(-1))
                        {
                            WarningNotification(_localizationService.GetResource("Web.Candidate.CandidateRecovery.EmailAlreadySent"));
                            return View(model);
                        }
                    }


                    // Pasword recovery process
                    // --------------------------------

                    var passwordRecoveryToken = "TwoGuid".ToToken(2);
                    var tokenExpiryDate = DateTime.UtcNow.AddDays(3).ToString("u"); // Token will expire after three days

                    //password recovery
                    _genericAttributeService.SaveAttribute(candidate, SystemCandidateAttributeNames.PasswordRecoveryToken, passwordRecoveryToken);
                    _genericAttributeService.SaveAttribute(candidate, SystemCandidateAttributeNames.TokenExpiryDate, tokenExpiryDate.ToString());
                    //send notification
                    _workflowMessageService.SendCandidatePasswordRecoveryMessage(candidate, 1);

                    //activity log
                    _activityLogService.InsertActivityLog("Candidate.RetrievePassword", _localizationService.GetResource("ActivityLog.Candidate.RetrievePassword"), candidate, candidate.GetFullName());

                    //Update candidate FailedSecurityQuestionAttempts
                    candidate.FailedSecurityQuestionAttempts = 0;
                    _candidateService.UpdateCandidate(candidate);

                    model.Email = string.Empty;
                }
                else if (candidate != null && !candidate.IsActive && !candidate.IsBanned && !candidate.IsDeleted)
                {
                    // Prevent message attack
                    // --------------------------------
                    var cPrt = candidate.GetAttribute<string>(SystemCandidateAttributeNames.CandidateActivationToken);
                    if (!String.IsNullOrEmpty(cPrt))
                    {
                        var ga = _genericAttributeService.GetAttributesForEntity(candidate.Id, "Candidate").OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefault();
                        // check if the request interval is less than one minute
                        if (ga.UpdatedOnUtc >= (DateTime?)DateTime.UtcNow.AddMinutes(-1))
                        {
                            WarningNotification(_localizationService.GetResource("Web.Candidate.CandidateRecovery.EmailAlreadySent"));
                            return View(model);
                        }
                    }


                    // Candidate validation process
                    // --------------------------------

                    //email validation
                    _genericAttributeService.SaveAttribute(candidate, SystemCandidateAttributeNames.CandidateActivationToken, "TwoGuid".ToToken(2));
                    //send notification
                    _workflowMessageService.SendCandidateEmailValidationMessage(candidate, 1);

                    //activity log
                    _activityLogService.InsertActivityLog("Candidate.ActivateCandidate", _localizationService.GetResource("ActivityLog.Candidate.ActivateCandidate"), candidate, candidate.GetFullName());

                    model.Email = string.Empty;
                }

                SuccessNotification(_localizationService.GetResource("Common.ResetPassword.EmailSent"));
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion


        #region PasswordRecoveryConfirm

        private bool ValidateParametersForPasswordRecovery(string token, string email)
        {
            var candidate = _candidateService.GetCandidateByEmail(email);
            if (candidate == null)
                return false;

            var eDate = candidate.GetAttribute<string>(SystemCandidateAttributeNames.TokenExpiryDate);
            if (String.IsNullOrEmpty(eDate))
                return false;

            if (DateTime.Parse(eDate) < DateTime.UtcNow)
                return false;

            var cPrt = candidate.GetAttribute<string>(SystemCandidateAttributeNames.PasswordRecoveryToken);
            if (String.IsNullOrEmpty(cPrt))
                return false;

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }


        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult PasswordRecoveryConfirm(string token, string email)
        {
            if (!ValidateParametersForPasswordRecovery(token, email))
                return RedirectToRoute("HomePage");

            var candidate = _candidateService.GetCandidateByEmail(email);

            var model = new PasswordRecoveryConfirmModel();
            model.PasswordPolicyModel =_accountPasswordPolicyService.Retrieve("Candidate").PasswordPolicy.ToModel();
            model.SecurityQuestion1 = _securityQuestionService.GetSecurityQuestionById(Convert.ToInt32(candidate.SecurityQuestion1Id)).Question;
            model.SecurityQuestion2 = _securityQuestionService.GetSecurityQuestionById(Convert.ToInt32(candidate.SecurityQuestion2Id)).Question;

            return View(model);
        }


        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        //[FormValueRequired("set-password")]
        public ActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            if (!ValidateParametersForPasswordRecovery(token, email))
                return RedirectToRoute("HomePage");

            if (ModelState.IsValid)
            {
                var candidate = _candidateService.GetCandidateByEmail(email);

                if (_candidateSettings.MaxFailedSecurityQuestionAttempts >= candidate.FailedSecurityQuestionAttempts)
                {
                    if (_candidateService.ValidateSecurityQuestions(candidate, model.SecurityQuestion1Answer, model.SecurityQuestion2Answer))
                    {
                        var response = _candidateService.ResetPassword(model.NewPassword, model.ConfirmNewPassword, null, candidate.Username); 
                        if (String.IsNullOrWhiteSpace(response))
                        {
                            _genericAttributeService.SaveAttribute(candidate, SystemCandidateAttributeNames.PasswordRecoveryToken, "");
                            _genericAttributeService.SaveAttribute(candidate, SystemCandidateAttributeNames.TokenExpiryDate, "");

                            model.SuccessfullyChanged = true;
                            model.Result = _localizationService.GetResource("Common.PasswordHasBeenChanged");

                            //activity log
                            _activityLogService.InsertActivityLog("Candidate.ResetPassword", _localizationService.GetResource("ActivityLog.ResetCandidatePassword"), candidate, candidate.GetFullName());
                        }
                        else
                        {
                            model.Result = response;
                        }
                    }
                    else
                    {
                        model.Result = _localizationService.GetResource("Common.PasswordRecovery.InvalidSecurityAnswer");
                    }
                }
                else
                {

                    model.Result = _localizationService.GetResource("Common.PasswordRecovery.ReachedMaxSecurityQuestionLimit");
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion


        #region ViewProfile

        public ActionResult ViewProfile()
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var model = candidate.ToCandidateUpdateProfileModel();
            model.TransportationModel = candidate.Transportation.ToModel();
            model.SalutationModel = candidate.Salutation.ToModel();

            return View(model);
        }

        #endregion


        #region UpdateProfile

        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult UpdateProfile()
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var model = candidate.ToCandidateUpdateProfileModel();
            if (candidate.SecurityQuestion1 == null)
                model.ShowSecurityQuestions = true;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult UpdateProfile(CandidateUpdateProfileModel model)
        {
            var candidate = _candidateService.GetCandidateById(model.Id);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var securityAnswer1 = string.Empty;
            var securityAnswer2 = string.Empty;
            var isSecurityValid = true;

            if (candidate.SecurityQuestion1 != null)
                securityAnswer1 = candidate.SecurityQuestion1Answer;
            else
            {
                if (string.IsNullOrEmpty(model.SecurityQuestion1Answer))
                {
                    ModelState.AddModelError("", _localizationService.GetResource("SecurityQuestion1Answer.Required"));
                    isSecurityValid = false;
                    model.SecurityQuestion1Id = null;
                    model.SecurityQuestion2Id = null;
                }
            }

            if (candidate.SecurityQuestion2 != null)
                securityAnswer2 = candidate.SecurityQuestion2Answer;
            else
            {
                if (string.IsNullOrEmpty(model.SecurityQuestion2Answer))
                {
                    ModelState.AddModelError("", _localizationService.GetResource("SecurityQuestion2Answer.Required"));
                    isSecurityValid = false;
                    model.SecurityQuestion1Id = null;
                    model.SecurityQuestion2Id = null;
                }
            }

            if (ModelState.IsValid && isSecurityValid)
            {
                candidate = model.ToCandidateUpdateProfileEntity(candidate);
                if (candidate.SecurityQuestion1 != null)
                {
                    candidate.SecurityQuestion1Answer = securityAnswer1;
                    candidate.SecurityQuestion2Answer = securityAnswer2;
                }
                else
                {
                    _candidateService.SetSecurityQuestionInformation(candidate);
                }
                    _candidateService.UpdateCandidate(candidate);
                //activity log
                _activityLogService.InsertActivityLog("Candidate.UpdateProfile", _localizationService.GetResource("ActivityLog.Candidate.UpdateProfile"), candidate, candidate.GetFullName());

                SuccessNotification(_localizationService.GetResource("Web.Candidate.Candidate.UpdateProfile.Updated"));
            }
            else
                return View(model);

            return RedirectToAction("ViewProfile");
        }

        #endregion


        #region ChangePassword

        [AuthorizeUser]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordModel()
            {
                PasswordPolicyModel = _accountPasswordPolicyService.Retrieve("Candidate").PasswordPolicy.ToModel()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            ViewBag.Message = String.Empty;

            if (String.IsNullOrWhiteSpace(model.OldPassword))
                ModelState.AddModelError("OldPassword", _localizationService.GetResource("Common.OldPassword.Required"));

            if (ModelState.IsValid)
            {
                string errorMessage = _candidateService.ResetPassword(model.NewPassword, model.ConfirmNewPassword, model.OldPassword, User.Identity.Name);

                if (!String.IsNullOrWhiteSpace(errorMessage))
                {
                    ErrorNotification(errorMessage);
                    return View(model);
                }
                else
                {
                    // Password was successfully updated
                    //activity log
                    _activityLogService.InsertActivityLog("Candidate.UpdatePassword", _localizationService.GetResource("ActivityLog.UpdateCandidatePassword"), User.Identity.Name);

                    SuccessNotification(_localizationService.GetResource("Common.PasswordHasBeenChanged"));

                    return RedirectToAction("Index", "Candidate");
                }
            }

            return View(model);
        }


        public ActionResult ChangePasswordWhenPasswordExpired(string userName)
        {
            var changePasswordmodel = new ChangePasswordModel();
            changePasswordmodel.PasswordPolicyModel = _accountPasswordPolicyService.Retrieve("Candidate").PasswordPolicy.ToModel();
            return View(changePasswordmodel);
        }


        [HttpPost]
        public ActionResult ChangePasswordWhenPasswordExpired(ChangePasswordModel model)
        {
            if (String.IsNullOrWhiteSpace(model.OldPassword))
                ModelState.AddModelError("OldPassword", _localizationService.GetResource("Common.OldPassword.Required"));

            if (ModelState.IsValid)
            {
                string errorMessage = _candidateService.ResetPassword(model.NewPassword, model.ConfirmNewPassword, model.OldPassword, User.Identity.Name);
                if (String.IsNullOrWhiteSpace(errorMessage))
                {
                    // Password is successfully updated
                    //activity log
                    _activityLogService.InsertActivityLog("Candidate.UpdatePassword", _localizationService.GetResource("ActivityLog.UpdateCandidatePassword"), User.Identity.Name);

                    SuccessNotification(_localizationService.GetResource("Common.PasswordHasBeenChanged"));

                    return RedirectToAction("SignIn", "Candidate");
                }
                else 
                {
                    ErrorNotification(errorMessage);
                    return View(model);
                }
            }
            return View(model);
        }

        #endregion


        #region ChangeSecurityQuestions

        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ChangeSecurityQuestions()
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var model = new CandidateChangeSecurityQuestionsModel();
            model.SecurityQuestion1Id = candidate.SecurityQuestion1Id;
            model.SecurityQuestion2Id = candidate.SecurityQuestion2Id;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult ChangeSecurityQuestions(CandidateChangeSecurityQuestionsModel model)
        {
            ViewBag.Message = String.Empty;

            if (ModelState.IsValid)
            {
                string errorMessage = _candidateService.ChangeSecurityQuestions(Convert.ToInt32(model.SecurityQuestion1Id), model.SecurityQuestion1Answer, Convert.ToInt32(model.SecurityQuestion2Id), model.SecurityQuestion2Answer, User.Identity.Name);
                if (errorMessage == null)
                    return RedirectToRoute("CandidateSignIn");
                else if (errorMessage.Length > 0)
                {
                    ErrorNotification(errorMessage);
                    return View(model);
                }
                else
                {
                    //activity log
                    _activityLogService.InsertActivityLog("Candidate.ChangeSecurityQuestions", _localizationService.GetResource("ActivityLog.Candidate.UpdateSecurityQuestions"), User.Identity.Name);
                    SuccessNotification(_localizationService.GetResource("Candidate.SecurityQuestionsUpdated"));
                    return RedirectToAction("Index", "Candidate");
                }
            }

            return View(model);
        }

        #endregion


        #region AttachmentList

        public ActionResult AddCandidateAttachment()
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var model = new CandidateAttachmentModel()
            {
                CandidateId = candidate.Id
            };

            return View(model);
        }


        [ChildActionOnly]
        [HandleChildActionAccessError]
        public ActionResult _CandidateAttachmentList(int candidateId)
        {
            return PartialView();
        }


        [HttpPost]
        [AuthorizeUser]
        public ActionResult _CandidateAttachmentList([DataSourceRequest] DataSourceRequest request, int candidateId)
        {
            var attachments = _attachmentService.GetAttachmentsByCandidateId(candidateId, true);

            return Json(attachments.ToDataSourceResult(request, x => x.ToModel()));
        }


        public ActionResult GetFileStartNameByDocumentId(int documentTypeId)
        {
            string fileName = string.Empty;

            var documentType = _documentTypeService.GetDocumentTypeById(documentTypeId);
            if (documentType != null && !string.IsNullOrEmpty(documentType.FileName))
            {
                fileName = documentType.FileName.Replace("*.*", "").ToUpper();
            }
            return Json(new { FileName = fileName }, JsonRequestBehavior.AllowGet);
        }


        [AuthorizeUser]
        public ActionResult UploadAttachmentFiles(IEnumerable<HttpPostedFileBase> attachments, int candidateId, int documentTypeId, DateTime? expiryDate)
        {
            // The Name of the Upload component is "attachments"
            foreach (var file in attachments)
            {
                // prepare
                var fileName = Path.GetFileName(file.FileName);
                var contentType = file.ContentType;

                // not supported file format
                AttachmentType attachmentType = _attachmentTypeService.GetAttachmentTypeByFileExtension(Path.GetExtension(fileName));
                if (attachmentType == null)
                    return Content("File format is not supported.");

                try
                {
                    using (Stream stream = file.InputStream)
                    {
                        var fileBinary = new byte[stream.Length];
                        stream.Read(fileBinary, 0, fileBinary.Length);

                        // upload attachment
                        string result = _attachmentService.UploadCandidateAttachment(candidateId, fileBinary, fileName, contentType, documentTypeId, expiryDate);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return Content(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Content(String.Format("Failed to upload attachment '{0}'. Reason: {1}", fileName, ex.Message));
                }
            }

            // Return an empty string to signify success
            return Content("");
        }


        //[AuthorizeUser]
        //public ActionResult RemoveAttachmentFile(string[] fileNames)
        //{
        //    //foreach (var fullName in fileNames)
        //    //{
        //    //    var fileName = Path.GetFileName(fullName);
        //    //    var physicalPath = Path.Combine(Server.MapPath("~/"), fileName);

        //    //    if (System.IO.File.Exists(physicalPath))
        //    //    {
        //    //        System.IO.File.Delete(physicalPath);
        //    //    }
        //    //}


        //    // Return an empty string to signify success
        //    return Content("");
        //}


        [AuthorizeUser]
        public ActionResult DownloadCandidateAttachment(Guid? guid)
        {
            var attachment = _attachmentService.GetAttachmentByGuid(guid);
            if (attachment == null)
                return RedirectToAction("Index");

            if (!String.IsNullOrEmpty(attachment.StoredPath))
            {
                var contentType = attachment.ContentType;               
                var basePath = _webHelper.GetRootDirectory();
                var folderPath = Path.Combine(basePath, attachment.StoredPath);
                var filePath = Path.Combine(folderPath, attachment.StoredFileName);
                if (System.IO.File.Exists(filePath))
                {
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = attachment.OriginalFileName,
                        // always prompt the user for downloading, set to true if you want 
                        // the browser to try to show the file inline
                        Inline = false,
                    };
                    Response.AppendHeader("Content-Disposition", cd.ToString());
                    return File(filePath, contentType);
                }

                //return new EmptyResult();
                //return RedirectToAction("Details", new { id = attachment.CandidateId });
                _logger.Error("DownloadCandidateAttachment: File not found" + " - " + attachment.StoredPath + @"\" + attachment.StoredFileName, 
                              userAgent: Request.UserAgent);

                return Content(_localizationService.GetResource("Common.FileNotFound"));
            }
            else
            {
                var cd = new System.Net.Mime.ContentDisposition
                {                   
                    FileName = attachment.OriginalFileName,
                    // always prompt the user for downloading, set to true if you want 
                    // the browser to try to show the file inline
                    Inline = false,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(attachment.AttachmentFile, attachment.ContentType);              
            }
        }

        #endregion


        #region Availability

        [AuthorizeUser]
        public ActionResult Availability()
        {
            return View();
        }


        [ChildActionOnly]
        [HandleChildActionAccessError]
        public ActionResult _AvailabilityScheduler(DateTime? refDate)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn");

            ViewBag.CandidateId = candidate.Id;
            ViewBag.RefDate = refDate ?? DateTime.Today;
            ViewBag.ReadOnly = false;   // TODO

            return PartialView();
        }


        [HttpPost]
        public ActionResult _CandidateAvailabilityByShift([DataSourceRequest] DataSourceRequest request, int candidateId, DateTime startDate, DateTime endDate)
        {
            var availability = _availabilityService.GetAllCandidateAvailabilityByCandidate(candidateId, startDate, endDate, byShift: true, excludePast: true);

            var result = availability.Select(x => x.ToModel()).OrderBy(x => x.Start).OrderBy(x => x.DisplayOrder)
                .ToDataSourceResult(request);

            return Json(result);
        }


        [HttpPost]
        public ActionResult _SaveAvailability(CandidateAvailabilityModel model)
        {
            var error = string.Empty;
            if (ModelState.IsValid && !model.ReadOnly && model.Changed)
            {
                try
                {
                    // existing changed, to be removed
                    if (model.Id > 0)
                    {
                        var entity = _availabilityService.GetCandidateAvailabilityById(model.Id);
                        if (entity != null)
                        {
                            _availabilityService.Delete(entity);
                            model.Id = 0;
                        }
                    }
                    // new availability
                    else if (model.Id == 0 && model.TypeId > 0)
                    {
                        var entity = model.ToEntity();
                        _availabilityService.Insert(entity);
                        model.Id = entity.Id;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("_SaveAvailability(): Failed", ex);
                    error += _localizationService.GetResource("Common.UnexpectedError");
                }
            }
            else
            {
                var errs = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                error += String.Join(" | ", errs.Select(o => o.ToString()).ToArray());
            }

            return Json(new { Succeed = String.IsNullOrWhiteSpace(error), Error = error, Id = model.Id });
        }

        #endregion


        #region AppliedJobs

        [AuthorizeUser]
        public ActionResult AppliedJobs()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AppliedJobs([DataSourceRequest] DataSourceRequest request)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var candidateJobOrders = candidate.AppliedJobs;
            var modelList = new List<CandidateAppliedJobModel>();

            foreach (var item in candidateJobOrders)
            {
                CompanyLocation location = null;
                if (item.JobOrder.Company.CompanyLocations != null && item.JobOrder.Company.CompanyLocations.Any())
                    location = item.JobOrder.Company.CompanyLocations.Where(x => x.Id == item.JobOrder.CompanyLocationId).FirstOrDefault();

                var model = new CandidateAppliedJobModel()
                {
                    CandidateId = item.CandidateId,
                    JobOrderId = item.JobOrderId,
                    JobTitle = item.JobOrder.JobTitle,
                    Address = location == null ? String.Empty : location.AddressLine1,
                    City = location == null || location.City == null ? String.Empty : location.City.CityName,
                    PostalCode = location == null || location.PostalCode == null ? String.Empty : location.PostalCode,
                    CreatedOnUtc = item.CreatedOnUtc,
                    UpdatedOnUtc = item.UpdatedOnUtc
                };

                modelList.Add(model);
            }
            var result = modelList.ToDataSourceResult(request);

            return Json(result);
        }

        #endregion


        #region KeySkill

        public ActionResult CandidateKeySkillIndex()
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var model = _candidateKeySkillsService.GetCandidateKeySkillsByCandidateId(candidate.Id).Select(x => x.ToModel());

            return View(model);
        }


        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult CandidateKeySkillCreate()
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var model = new CandidateKeySkillModel() { CandidateId = candidate.Id };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult CandidateKeySkillCreate(CandidateKeySkillModel model)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            if (ModelState.IsValid)
            {
                var keySkill = model.ToEntity();
                keySkill.CreatedOnUtc = System.DateTime.UtcNow;
                keySkill.UpdatedOnUtc = System.DateTime.UtcNow;
                _candidateKeySkillsService.InsertCandidateKeySkill(keySkill);

                // Update search keys
                var candidateKeySkills = _candidateKeySkillsService.GetCandidateKeySkillsByCandidateId(candidate.Id);
                var candidateAddress = _candidateAddressService.GetAllCandidateAddressesByCandidateId(model.CandidateId).FirstOrDefault();
                candidate.SearchKeys = GetCandidateSearchKeyWords(candidateKeySkills, candidateAddress);
                _candidateService.UpdateCandidate(candidate);

                //activity log
                _activityLogService.InsertActivityLog("Candidate.AddNewKeySkill", _localizationService.GetResource("ActivityLog.Candidate.AddNewKeySkill"), candidate, candidate.GetFullName() + " / " + model.KeySkill);

                return RedirectToAction("CandidateKeySkillIndex");
            }

            return View(model);
        }


        [AuthorizeUser]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult CandidateKeySkillEdit(Guid? guid)
        {
            if (!guid.HasValue)
                return RedirectToAction("CandidateKeySkillIndex");

            var keySkill = _candidateKeySkillsService.GetCandidateKeySkillByGuid(guid);
            if (keySkill == null)
                return RedirectToAction("CandidateKeySkillIndex");

            return View(keySkill.ToModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult CandidateKeySkillEdit(CandidateKeySkillModel model)
        {
            Candidate candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn");

            CandidateKeySkill keySkill = _candidateKeySkillsService.GetCandidateKeySkillById(model.Id);
            if (keySkill == null)
                return RedirectToAction("CandidateKeySkillIndex");


            if (ModelState.IsValid)
            {
                keySkill = model.ToEntity(keySkill);

                keySkill.UpdatedOnUtc = System.DateTime.UtcNow;

                _candidateKeySkillsService.UpdateCandidateKeySkill(keySkill);


                // Update search keys
                IList<CandidateKeySkill> candidateKeySkills = _candidateKeySkillsService.GetCandidateKeySkillsByCandidateId(candidate.Id);
                CandidateAddress candidateAddress = _candidateAddressService.GetAllCandidateAddressesByCandidateId(model.CandidateId).FirstOrDefault();
                candidate.SearchKeys = GetCandidateSearchKeyWords(candidateKeySkills, candidateAddress);
                _candidateService.UpdateCandidate(candidate);


                //activity log
                _activityLogService.InsertActivityLog("Candidate.UpdateKeySkill", _localizationService.GetResource("ActivityLog.Candidate.UpdateKeySkill"), candidate, candidate.GetFullName() + " / " + model.KeySkill);


                return RedirectToAction("CandidateKeySkillIndex");
            }

            return View(model);
        }


        //[MvcSiteMapNode(Title = "Delete", ParentKey = "KeySkill")]
        //public ActionResult CandidateKeySkillDelete(Guid? guid)
        //{
        //    Candidate candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
        //    if (candidate == null)
        //        return RedirectToRoute("CandidateSignIn");

        //    CandidateKeySkill keySkill = _candidateKeySkillsService.GetCandidateKeySkillByGuid(guid);

        //    if (keySkill == null)
        //        return RedirectToAction("Index");

        //    int _Counter = _candidateKeySkillsService.GetCandidateKeySkillsByCandidateId(candidate.Id).Count();

        //    if (_Counter <= 3)
        //        return RedirectToAction("Index");

        //    _candidateKeySkillsService.DeleteCandidateKeySkill(keySkill);


        //    //activity log
        //    _activityLogService.InsertActivityLog("Candidate.DeleteKeySkill", _localizationService.GetResource("ActivityLog.Candidate.DeleteKeySkill"), candidate, candidate.GetFullName());


        //    return RedirectToAction("CandidateKeySkillIndex");
        //}

        #endregion


        #region Test

        public ActionResult TestIndex()
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            ViewBag.CandidateId = candidate.Id;

            var appliedCompanyIds = candidate.AppliedJobs.Select(x => x.JobOrder.CompanyId).Distinct().ToList();

            var industryIds = _companyService.GetAllCompanies(0, int.MaxValue, false).Where(x => appliedCompanyIds.Contains(x.Id) ).Select(x => x.IndustryId).Distinct().ToList();

            var models = _testService.GetAllCategories()
                .Where(x => (
                        !x.CompanyId.HasValue && !x.IndustryID.HasValue) || 
                        (x.CompanyId.HasValue && appliedCompanyIds.Contains(x.CompanyId.Value)) ||
                        (x.IndustryID.HasValue && (industryIds.Contains(null) || industryIds.Contains(x.IndustryID.Value)))
                 )
                .Select(x => x.ToModel());

            return View(models);
        }


        [AuthorizeUser]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult BeginTest(int categoryId = 0)
        {
            if (categoryId <= 0)
                return RedirectToAction("TestIndex");

            var testCategory = _testService.GetTestCategoryById(categoryId);
            if (testCategory == null)
                return RedirectToAction("TestIndex");

            // retry
            var lastTestModel = TempData["LastTestModel"] != null ? TempData["LastTestModel"] as TestModel : null;
            if (TempData["LastModelState"] != null)
                ModelState.Merge((ModelStateDictionary)TempData["LastModelState"]);

            var testModel = GetTestModelByCategory(testCategory, lastTestModel);
            ViewBag.TestCategory = testCategory.TestCategoryName;

            return View(testModel);
        }


        [HttpPost, ActionName("BeginTest")]
        [ValidateAntiForgeryToken()]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult BeginTestConfirm(TestModel testModel)
        {
            var candidate = new Candidate();
            if (testModel.CandidateGuid == null || testModel.CandidateGuid == Guid.Empty)
            {
                candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
                if (candidate == null)
                    return RedirectToRoute("CandidateSignIn");
            }
            else
            {
                candidate = _candidateService.GetCandidateByGuid(testModel.CandidateGuid);
                if (candidate == null)
                    return RedirectToRoute("CandidateSignIn");
            }

            var isPassed = AfterSubmitTest(testModel, candidate, out List<int> failedQuestions);
            if (isPassed)
            {
                SuccessNotification("You passed the test successfully.");
                return RedirectToAction("TestIndex");
            }

            // allow retry
            else
            {
                WarningNotification("Please correct the wrong answer(s) and submit again.");

                foreach (var q in failedQuestions)
                    ModelState.AddModelError(String.Format("TestQuestions[{0}]", q), String.Format("Q.{0}: empty or wrong answer", q + 1));
                TempData["LastTestModel"] = testModel;
                TempData["LastModelState"] = ModelState;

                return RedirectToAction("BeginTest", new { categoryId = testModel.CategoryId});
            }
        }


        [ChildActionOnly]
        [HandleChildActionAccessError]
        public ActionResult _CandidateTestResult(int CandidateId)
        {
            var model = _candidateTestResultService.GetCandidateTestResultsByCandidateId(CandidateId).Select(x => x.ToModel());
            return PartialView(model);
        }

        #endregion


        #region TestVideo

        [AuthorizeUser]
        public ActionResult TestVideo()
        {
            string[] supportedExtensions = { ".mp4", ".m4v", ".f4v", ".mov", ".flv", ".webm" };
            string videoFileDirectory = @"~/" + _mediaSettings.VideoFileLocation.Replace(@"\", "/");

            var models = new List<MediaFileModel>();
            try
            {
                string videoFilePath = _webHelper.MapPath(videoFileDirectory);
                int i = 0;
                var videoFiles = new DirectoryInfo(videoFilePath).GetFiles("*.*", SearchOption.TopDirectoryOnly).Where(fi => supportedExtensions.Contains(Path.GetExtension(fi.Name).ToLower()));
                foreach (var videoFile in videoFiles)
                {
                    i++;
                    var model = new MediaFileModel()
                    {
                        FileNo = i,
                        MediaFilePath = videoFileDirectory + "/" + videoFile.Name,
                        ImageFilePath = videoFileDirectory + "/" + Path.GetFileNameWithoutExtension(videoFile.Name) + ".jpg",
                        Title = Regex.Replace(Path.GetFileNameWithoutExtension(videoFile.Name), @"([A-Z])(?=[a-z])|(?<=[a-z])([A-Z])", " $1").Trim()
                    };

                    models.Add(model);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                ErrorNotification("Could not find a part of the path '" + videoFileDirectory + "'");
                _logger.Error("TestVideo()", ex, userAgent: Request.UserAgent);
            }
            catch (Exception ex)
            {
                ErrorNotification("Unexpected error");
                _logger.Error("TestVideo()", ex, userAgent: Request.UserAgent);
            }

            return View(models);
        }

        #endregion


        #region TestMaterial

        [AuthorizeUser]
        public ActionResult TestMaterial()
        {
            return View();
        }


        [HttpPost]
        [AuthorizeUser]
        public ActionResult _TestMaterials([DataSourceRequest]DataSourceRequest request)
        {
            var materials = _testService.GetAllTestMaterials();
            var result = materials.ToDataSourceResult(request, m => m.ToModel());

            return Json(result);
        }


        public ActionResult DownloadTestMaterial(Guid? guid)
        {
            var material = _testService.RetrieveByGuid(guid);
            if (material == null)
                return RedirectToAction("List");

            Response.AppendHeader("Content-Disposition", String.Concat("inline; filename=\"", material.AttachmentFileName, "\""));
            return File(material.AttachmentFile, material.ContentType);
        }

        #endregion


        #region H&S Orientation

        [AuthorizeUser]
        public ActionResult HSOrientation()
        {
            return View();
        }

        #endregion


        #region ApplyJob

        public ActionResult ApplyJob(int jobOrderId = 0)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            if (jobOrderId == 0)
                return RedirectToRoute("JobPost");

            var isApplied = _candidateAppliedJobsService.HasCandidateAppliedTheJob(candidate.Id, jobOrderId);
            if (isApplied == true)
            {
                ViewBag.Message = _localizationService.GetResource("Web.Candidate.Candidate.ApplyJob.AlreadyApplied");
                return View();
            }

            var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
            if (jobOrder == null)
                return RedirectToRoute("JobPost");

            if (jobOrder.JobOrderType.IsDirectHire)
            {
                int candidateJobOrderStatusId = (int)CandidateJobOrderStatusEnum.DirectHireSubmitted;

                var candidatejoborder = new CandidateJobOrder
                {
                    CandidateId = candidate.Id,
                    JobOrderId = jobOrder.Id,
                    StartDate = DateTime.Now.Date,
                    CandidateJobOrderStatusId = candidateJobOrderStatusId,
                    EnteredBy = candidate.Id,
                    CreatedOnUtc = System.DateTime.UtcNow,
                    UpdatedOnUtc = System.DateTime.UtcNow
                };

                _candidateJobOrderService.InsertCandidateJobOrder(candidatejoborder);
            }
            else
            {
                int[] candidateIds = new int[1];
                candidateIds[0] = candidate.Id;
                _companyCandidateService.AddCandidatesToCompanyIfNotYet(jobOrder.CompanyId, candidateIds, DateTime.Today);
            }
            _candidateAppliedJobsService.CandidateAppliedJob(candidate.Id, jobOrder.Id);

            
            //activity log
            _activityLogService.InsertActivityLog("Candidate.ApplyJob",
                _localizationService.GetResource("ActivityLog.Candidate.ApplyJob"), candidate, candidate.Id + " / " + jobOrder.Id);

            //notify recruitor
            _workflowMessageService.SendJobOrderAppliedRecruiterNotification(jobOrder, candidate, 1);

            // Add Email notification
            var JobTests = _jobOrderTestCategoryService.GetJobOrderTestCategoryByJobOrderId(jobOrderId);

            if (JobTests.Count > 0)
            {
                // TO DO: notify candidate for tests.................
            }

            ViewBag.Message = _localizationService.GetResource("Web.Candidate.Candidate.ApplyJob.Applied");

            return View();
        }

        #endregion


        #region Time Entry

        public ActionResult TimeEntry(DateTime? refDate)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            ViewBag.CandidateId = candidate.Id;
            ViewBag.RefDate = refDate ?? DateTime.Today;

            return View();
        }


        [HttpPost]
        public ActionResult _CandidateWorkTimeByWeek([DataSourceRequest] DataSourceRequest request, DateTime? refDate)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn");

            if (!refDate.HasValue)
                return Json(new DataSourceResult());
            
            var workTime_BL = new CandidateWorkTime_BL(_candidateJobOrderService, _jobOrderService, _workTimeService);
            var times = workTime_BL.CandidateWorkTimeByWeek(candidate.Id, refDate.Value);

            return Json(times.ToDataSourceResult(request, x => x.ToModel()));
        }


        [HttpPost]
        public ActionResult _SaveWorkTime([DataSourceRequest] DataSourceRequest request, CandidateWorkTimeModel model)
        {
            if (model != null)
            {
                var minStartTime = model.JobStartDateTime.AddMinutes(-_candidateWorkTimeSettings.StartScanWindowSpanInMinutes);
                var maxEndTime = model.JobEndDateTime.AddMinutes(_candidateWorkTimeSettings.EndScanWindowSpanInMinutes);
                if (model.ClockIn.HasValue && model.ClockOut.HasValue)
                {
                    if (model.ClockIn < minStartTime || model.ClockIn > maxEndTime)
                        ModelState.AddModelError("ClockIn", "'Sign In' time is invalid");
                    if (model.ClockOut < minStartTime || model.ClockOut > maxEndTime)
                        ModelState.AddModelError("ClockIn", "'Sign Out' time is invalid");

                    if (ModelState.IsValid)
                    {
                        var workTime_BL = new CandidateWorkTime_BL(_candidateJobOrderService, _jobOrderService, _workTimeService);
                        workTime_BL.SaveWorkTime(new[] { model });
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        #endregion


        #region WorkTime

        [HttpGet]
        public ActionResult WorkTimeByCandidateInWeb()
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn");

            ViewBag.CandidateId = candidate.Id;
            return View();
        }


        [HttpPost]
        public ActionResult WorkTimeByCandidateInWeb([DataSourceRequest] DataSourceRequest request,DateTime refDate)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var startDate = refDate.Date.GetDayOfThisWeek(DayOfWeek.Sunday);
            var endDate = refDate.Date.GetDayOfThisWeek(DayOfWeek.Saturday).AddDays(1);
            var candidateWorkTimes = _workTimeService.GetWorkTimeByCandidateIdAsQueryable(candidate.Id)
                .Where(x => x.JobStartDateTime >= startDate && x.JobStartDateTime < endDate)
                .Where(x => x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Approved);

            return Json(candidateWorkTimes.ToDataSourceResult(request,x=>x.ToModel()));
        }

        #endregion
      

        #region All Key Skills

        public ActionResult GetAllKeySkills()
        {
            var keyskills = _skillService.GetAllSkills();
            return Json(keyskills.Select(s => s.SkillName), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Announcements

        public ActionResult Announcements() 
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn", new { returnUrl = Request.Url.PathAndQuery });

            var model = _announcementService.GetAnnouncementForCandidate(candidate);
            return View(model);
        }
       

        #endregion


        #region CandidateConfirm

        public ActionResult CandidateConfirm(Guid? guid,bool accept)
        {
            // Do not allow an admin user to click on any of the buttons from the admin site
            if (Request.UrlReferrer != null)
            {
                var _ref = Request.UrlReferrer.ToString();
                if (_ref.IndexOf("Candidate/Detail", StringComparison.OrdinalIgnoreCase) > 0)
                    return RedirectToAction("AccessDenied", "Security", new { area = "Admin", pageUrl = this.Request.RawUrl });
            }

            var link = _confirmationEmailLinkService.GetByGuid(guid);
            if (link==null)
                return InvokeHttp404();
            if (link.IsUsed || link.ValidBefore < DateTime.Now)
                ViewBag.Message = "The link is not valid anymore!";
            else
            {
                link.IsUsed = true;
                link.AcceptOrDecline = accept;
                
                if (accept)
                {
                    //place candidate into the job order 
                    CandidateJobOrder newcjo = new CandidateJobOrder();
                    newcjo.CandidateId = link.CandidateId;
                    newcjo.JobOrderId= link.JobOrderId;
                    newcjo.StartDate = link.StartDate;
                    newcjo.EndDate = link.EndDate;
                    newcjo.CandidateJobOrderStatusId = (int)CandidateJobOrderStatusEnum.Placed;
                    newcjo.EnteredBy = link.EnteredBy;

                    link.Note = _candidateJobOrderService.CreateNewPlacement(newcjo, 0, true);
                    
                    //
                    if (string.IsNullOrEmpty(link.Note))
                        ViewBag.Message = "You sucessfully accepted the job offer! Good Luck!";
                    else
                        ViewBag.Message = "System could not process your response. Please contact our recruiter to accept this job offer!";
                    //send result message to recruiter
                    _workflowMessageService.SendConfirmationFeedBackMessage(link.JobOrderId, link.CandidateId, true, link.EnteredBy, link.Note);
                }
                else
                {
                    link.Note = _candidateJobOrderService.SetCandidateJobOrderToStandbyStatus(link.CandidateId, link.JobOrderId,
                        (int)CandidateJobOrderStatusEnum.Contacted,     // whatever other than Placed
                        link.StartDate, link.EndDate, (int)CandidateJobOrderStatusEnum.Refused);
                    ViewBag.Message = "We are sorry that you declined our job offer!";
                    //send alert to recruiter 
                    _workflowMessageService.SendConfirmationFeedBackMessage(link.JobOrderId, link.CandidateId, false, link.EnteredBy,null);
                }
                _confirmationEmailLinkService.Update(link);
            }

            return View("ConfirmResult");
        }

        #endregion


        #region Candidate Payment History

        [AuthorizeUser]
        public ActionResult PaymentHistory()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CandidatePaymentHistory([DataSourceRequest]DataSourceRequest request)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn");

            var result = _paymentHistoryService.GetAllPaymentHistoryWithPayStubByCandidateId(candidate.Id).ToList();
            return Json(result.ToDataSourceResult(request,m=>m.ToModel()));
        }


        public ActionResult DownloadPayStub(Guid? guid)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn");

            var paystub = _paymentHistoryService.GetPayStubByPaymentGuid(guid, candidate.Id);
            if (paystub == null || paystub.Paystub == null || paystub.Paystub.Length <= 0)
            {
                ErrorNotification("Paystub does not exist! Please contact our payroll department!");
                return RedirectToAction("PaymentHistory");
            }

            var fileName = _paymentHistoryService.SecurePayStubPDFFile(paystub.CandidateId, paystub.Paystub);
            if (System.IO.File.Exists(fileName))
                return File(fileName, "application/pdf",Path.GetFileName(fileName));
            else
            {
                ErrorNotification("Paystub does not exist! Please contact our payroll department!");
                return RedirectToAction("PaymentHistory");
            }
        }

        #endregion


        #region Tax Forms

        [AuthorizeUser]
        public ActionResult TaxForms()
        {
            return View();
        }


        [HttpPost]
        public ActionResult _TaxForms([DataSourceRequest]DataSourceRequest request, int year)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn");

            // only latest
            var forms = _taxFormService.GetAllTaxFormsByCandidateIdAndYear(candidate.Id, year);
            var result = forms.GroupBy(x => new { x.FormType, x.Year })
                .Select(g => g.OrderByDescending(y => y.IssueDate).FirstOrDefault());

            return Json(result.ToDataSourceResult(request));

        }


        public ActionResult DownloadOwnTaxForm(int id, int taxYear, string type)
        {
            var candidate = _candidateService.GetCandidateByUsername(User.Identity.Name);
            if (candidate == null)
                return RedirectToRoute("CandidateSignIn");

            if (type == "T4")
            {
                var t4Class = _t4Services[taxYear];
                var form = t4Class.GetT4DataById(id);
                if (form.CandidateId != candidate.Id)
                    return Content("Access denied");

                return _DownloadT4(id, taxYear);
            }

            else if (type == "RL1")
            {
                var rl1Class = _rl1Services[taxYear];
                var form = rl1Class.GetRL1DataById(id);
                if (form.CandidateId != candidate.Id)
                    return Content("Access denied");

                return _DownloadRL1(id, taxYear);
            }

            else
                return Content("Not implemented yet");
        }


        private ActionResult _DownloadT4(int id, int taxYear)
        {
            var t4Service = _t4Services[taxYear];
            var t4 = t4Service.GetT4DataById(id);
            var pdfBytes = t4Service.GetEmployeeT4PDFBytes(t4.FranchiseId, t4.Id, true, out int candidateId);
            if (pdfBytes.Length > 0)
                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, String.Format("T4_{0}.pdf", taxYear));
            else
                return new HttpNotFoundResult();
        }


        private ActionResult _DownloadRL1(int id, int taxYear)
        {
            var rl1Service = _rl1Services[taxYear];
            var rl1 = rl1Service.GetRL1DataById(id);
            var pdfBytes = rl1Service.GetEmployeeRL1PDFBytes(rl1.FranchiseId, rl1.Id, true, out int candidateId);
            if (pdfBytes.Length > 0)
                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, String.Format("RL1_{0}.pdf", taxYear));
            else
                return new HttpNotFoundResult();
        }

        #endregion


        #region Candidate Click the test link in email

        public ActionResult CandidateTest(Guid? guid)
        {
            var link = _candidateTestLinkService.Retrieve(guid);
            if (link == null)
                return InvokeHttp404();
            if (link.IsUsed || link.ValidBefore < DateTime.Now)
            {
                ErrorNotification("The link is not valid anymore!");
                return RedirectToAction("SignIn");
            }
            else
            {
                link.IsUsed = true;
                _candidateTestLinkService.Update(link);
                var model = GetTestModelByCategory(link.TestCategory);
                model.CandidateGuid = link.Candidate.CandidateGuid;
                ViewBag.TestCategory = link.TestCategory.TestCategoryName;

                return View("BeginTest", model);
            }
        }


        private TestModel GetTestModelByCategory(TestCategory testCategory, TestModel lastTestModel = null)
        {
            var testQuestionModel = new List<TestQuestionModel>();
            var testQuestion = testCategory.TestQuestions.Where(x => x.IsActive && !x.IsDeleted).ToList();

            for (var i = 0; i < testQuestion.Count; i++)
            {
                var item = testQuestion[i];
                var model = item.ToModel();
                model.ImageFileUrl = _testService.GetTestImageUrl(item.ImageFileLocation);
                model.TestQuestionNum = i + 1;

                var testChoiceModelList = new List<TestChoiceModel>();
                foreach (TestChoice choice in item.TestChoices)
                {
                    var choiceModel = choice.ToModel();
                    choiceModel.ImageFileUrl = _testService.GetTestImageUrl(choice.ImageFileLocation);
                    testChoiceModelList.Add(choiceModel);
                }
                model.TestChoiceModelList = testChoiceModelList;

                if (lastTestModel != null)  // preserve choices, for retry
                    model.CandidateChoice = lastTestModel.TestQuestions[i].CandidateChoice;

                testQuestionModel.Add(model);
            }

            return new TestModel()
            {
                CategoryId = testCategory.Id,
                TestQuestions = testQuestionModel
            };
        }


        private bool AfterSubmitTest(TestModel testModel, Candidate candidate, out List<int> failedQuestions)
        {
            int totalMark = 0;
            int correctMark = 0;
            bool isPassed = false;
            int testCategoryId = 0;
            string testCategoryName = "";
            var sbTestResultDetails = new StringBuilder();
            failedQuestions = Enumerable.Empty<int>().ToList();

            var testCategry = _testService.GetTestCategoryById(testModel.CategoryId);

            var questions = testModel.TestQuestions;
            for (var questionSequenceNum = 0; questionSequenceNum < questions.Count; questionSequenceNum++)
            {
                var item = questions[questionSequenceNum];

                totalMark += item.Score;
                if (item.CandidateChoice == item.Answers)
                    correctMark += item.Score;
                else
                    failedQuestions.Add(questionSequenceNum);

                testCategoryId = testModel.CategoryId;
                testCategoryName = testCategry.TestCategoryName;

                string questionContext = _testService.GetTestQuestionById(item.Id).Question;

                sbTestResultDetails.AppendLine("Q." + (questionSequenceNum + 1) + ", " + questionContext);
                sbTestResultDetails.AppendLine("Answer : " + item.CandidateChoice);
                sbTestResultDetails.AppendLine();
            }

            // keep only passed test result
            if (correctMark >= testCategry.PassScore)
            {
                isPassed = true;

                // save test result file
                string testResultFile = _candidateTestResultService.SaveCandiateTestResultToFile(candidate.Id, testModel.CategoryId, sbTestResultDetails);

                // record the test
                var result = new CandidateTestResult()
                {
                    CandidateId = candidate.Id,
                    TestCategoryId = testModel.CategoryId,
                    TestScore = correctMark,
                    TotalScore = totalMark,
                    PassScore = testCategry.PassScore,
                    IsPassed = isPassed,
                    ScoreFilePath = testResultFile,
                    TestedOnUtc = System.DateTime.UtcNow,
                    IsDeleted = false,
                    CreatedOnUtc = System.DateTime.UtcNow,
                    UpdatedOnUtc = System.DateTime.UtcNow
                };

                _candidateTestResultService.InsertCandidateTestResult(result);

                //activity log
                _activityLogService.InsertActivityLog("Candidate.CompleteTest", _localizationService.GetResource("ActivityLog.Candidate.CompleteTest"), candidate, candidate.GetFullName() + "/" + testCategry.TestCategoryName + "/" + (isPassed == true ? "Passed" : "Failed"));
            }

            return isPassed;
        }

        #endregion
    }
}
