using System;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Localization;
using Wfm.Services.Messages;
using Wfm.Web.Framework.Security;
using Wfm.Web.Models.Home;
using Wfm.Web.Framework.Filters;


namespace Wfm.Web.Controllers
{
    public partial class HomeController : BaseWfmController
    {
        #region Fields
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly CommonSettings _commonSetting;
        #endregion

        #region Constructors
        public HomeController(
            IQueuedEmailService queuedEmailService,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            ILocalizationService localizationService,
            CommonSettings commonSetting
            )
        {
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailAccountService = emailAccountService;
            this._localizationService = localizationService;
            this._commonSetting = commonSetting;
        }
        #endregion


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult CorporateSocialResponsibility()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }

        public ActionResult ApplicationAgreement()
        {
            return View();
        }

        public ActionResult FranchiseOpportunities()
        {
            return View();
        }

        public ActionResult ClientPrivacyPolicy()
        {
            return View();
        }

        public ActionResult ClientApplicationAgreement()
        {
            return View();
        }

        public ActionResult ClientTermsOfUse()
        {
            return View();
        }


        #region ContactUs

        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ContactUs()
        {
            var model = new ContactUsModel();
            return View(model);
        } 


        [HttpPost, ActionName("ContactUs")]
        [ValidateAntiForgeryToken]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult ContactUsSEND(ContactUsModel model)
        {
            try
            {
                string response = this.Request["g-recaptcha-response"];
                if (String.IsNullOrWhiteSpace(response) || !reCaptchaValidate.Validate(response))
                {
                    ModelState.AddModelError("reCaptcha", _localizationService.GetResource("Common.WrongCaptcha"));
                }

                if (ModelState.IsValid)
                {
                    StringBuilder sbMessage = new StringBuilder();
                    sbMessage.Append("<html>");
                    sbMessage.Append("<title>" + model.ContactName + " - " + model.Subject + "</title>");
                    sbMessage.Append("<body>");
                    sbMessage.Append("<br /> ");
                    sbMessage.Append("<b>Name:</b> " + model.ContactName);
                    sbMessage.Append("<br />");
                    sbMessage.Append("<b>Company:</b> " + model.Company);
                    sbMessage.Append("<br />");
                    sbMessage.Append("<b>Phone:</b> " + model.Phone);
                    sbMessage.Append("<br />");
                    sbMessage.Append("<b>E-mail:</b> " + model.Email);
                    sbMessage.Append("<br />");
                    sbMessage.Append("<b>Question/Comment:</b> " + model.Message);
                    sbMessage.Append("<br />");
                    sbMessage.Append("<b>Respond By:</b> " + model.RespondBy);
                    sbMessage.Append("<br />");
                    sbMessage.Append("<b>Best time to respond:</b> " + model.BestTimeToRespond);
                    sbMessage.Append("<br />");
                    sbMessage.Append("</body>");
                    sbMessage.Append("</html>");

                    //get email account
                    var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                    //if (emailAccount == null)
                    //    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                    //if (emailAccount == null)
                    //    throw new WfmException("Email account can't be loaded");

                    var email = new QueuedEmail()
                    {
                        Priority = 3,
                        EmailAccountId = emailAccount.Id,
                        FromName = emailAccount.DisplayName,
                        From = emailAccount.Email,
                        ToName = _commonSetting.SiteTitle + " Web Contact",
                        To = _emailAccountSettings.DefaultToEmailAddress,
                        CC = "",
                        Bcc = _emailAccountSettings.DefaultBccEmailAddress,
                        Subject = model.Subject + " (" + model.Company + ")",
                        Body = sbMessage.ToString(),
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                    };

                    if (!model.SuccessfullySent)
                    {
                        _queuedEmailService.InsertQueuedEmail(email);

                        model.SuccessfullySent = true;
                        model.Result = "Thank you for the message, we will contact you shortly.";
                    }

                }

            }
            catch (Exception)
            {
                model.SuccessfullySent = false;
                model.Result = "Error on sending message, please try again later or call us.";
            }

            return View(model);
        }

        #endregion


        #region Locations

        public ActionResult Locations()
        {
            return View();
        }

        #endregion

    }

}
