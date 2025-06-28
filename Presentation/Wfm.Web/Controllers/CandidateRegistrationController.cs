
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.Franchises;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Media;
using Wfm.Services.Messages;
using Wfm.Web.Models.Candidate;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Filters;


namespace Wfm.Web.Controllers
{
    public class CandidateRegistrationController : BaseWfmController
    {
        #region Fields

        private readonly ICandidateService _candidateService;
        private readonly ICandidateAddressService _candidateAddressService;       
        private readonly IActivityLogService _activityLogService;
        private readonly ICandidateKeySkillService _candidateKeySkillsService;      
        private readonly IAttachmentService _attachmentService;
        private readonly IAttachmentTypeService _attachmentTypeService; 
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;     
        private readonly CandidateSettings _candidateSettings;       
        private readonly ILogger _logger;
        private readonly IDocumentTypeService _documentTypeService; 
        private readonly Wfm.Services.Accounts.IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly RegisterCandidate_BL _registerCandidate_BL;
        private readonly IFranchiseService _franchiseService;
        #endregion


        #region Ctor

        public CandidateRegistrationController(
            ICandidateService candidatesService,
            ICandidateAddressService candidateAddressService,         
            IActivityLogService activityLogService,
            ICandidateKeySkillService candidateKeySkillsService,
            IAttachmentService attachmentService,
            IAttachmentTypeService attachmentTypeService,
            ILocalizationService localizationService,
            IGenericAttributeService genericAttributeService,
            IWorkflowMessageService workflowMessageService,
            CandidateSettings candidateSettings,
            ILogger logger,
            IDocumentTypeService documentTypeService,
            Wfm.Services.Accounts.IAccountPasswordPolicyService accountPasswordPolicyService,
            ICandidatePictureService candidatePictureService,
            IFranchiseService franchiseService
            )
        {
            _candidateService = candidatesService;
            _candidateAddressService = candidateAddressService;          
            _activityLogService = activityLogService;
            _candidateKeySkillsService = candidateKeySkillsService;           
            _attachmentService = attachmentService;
            _attachmentTypeService = attachmentTypeService;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
            _workflowMessageService = workflowMessageService;           
            _candidateSettings = candidateSettings;           
            _logger = logger;
            _documentTypeService = documentTypeService;         
            _accountPasswordPolicyService = accountPasswordPolicyService;         
            _candidatePictureService = candidatePictureService;
            _franchiseService = franchiseService;
            _registerCandidate_BL = new RegisterCandidate_BL(_candidateService, _candidateAddressService, _activityLogService, _candidateKeySkillsService, _attachmentService, _attachmentTypeService
                                                           , _localizationService, _genericAttributeService, _workflowMessageService, _candidateSettings, _logger, _documentTypeService, _accountPasswordPolicyService
                                                           , _candidatePictureService,_franchiseService);
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


        // GET: CandidateRegistration
        public ActionResult Index()
        {
            return View();
        }


        #region CandidateRegistration/InOfficeRegistration

        [AuthorizeIpAddressAttribute(SkipIpCheck = false)]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult InOfficeRegistration()
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
                CountryId = 2,
                LastUsedDate1 = DateTime.Today.AddDays(-1)
            };
            Session["CapturedImage"] = null;
            ViewBag.NewPage = true;
            return View("InOfficeRegistration", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleAntiforgeryTokenErrorAttribute]
        public ActionResult InOfficeRegistration(CreateEditCandidateModel model, HttpPostedFileBase attachments)
        {
            // step 1 - validate data

            // Check Agreement
            if (model.CandidateModel.Entitled == false)
            {
                ErrorNotification(_localizationService.GetResource("Web.Candidate.Candidate.Added.Fail.NotAccpectedTerm"));
                ViewBag.NewPage = false;
                ViewBag.StartIndex = 4;
                return View(model);
            }
             
            // check Profile Pic
            if (Session["CapturedImage"] == null)
            {
                ErrorNotification("Please take picture.");
                ViewBag.NewPage = false;
                ViewBag.StartIndex = 3;
                return View(model);

            }

            // Validate password typo
            if (model.CandidateModel.Password != model.CandidateModel.RePassword)
            {
                ErrorNotification(_localizationService.GetResource("Common.EnteredPasswordsDoNotMatch"));
                ViewBag.NewPage = false;
                ViewBag.StartIndex = 0;
                return View(model);
            }
            else
            {
                StringBuilder errors = new StringBuilder();
                bool valid = _accountPasswordPolicyService.ApplyPasswordPolicy(0, "Candidate", model.CandidateModel.Password, String.Empty, Core.Domain.Accounts.PasswordFormat.Clear, String.Empty, out errors);
                if (!valid)
                {
                    ModelState.AddModelError("CandidateModel.Password", errors.ToString());
                    ViewBag.NewPage = false;
                    ViewBag.StartIndex = 0;
                    return View(model);
                }
            }

            // Validate last used date
            if (model.LastUsedDate1 == null || model.LastUsedDate1 < System.DateTime.Now.AddYears(-50))
            {
                ErrorNotification(_localizationService.GetResource("Web.Candidate.Candidate.Added.Fail.InvalidLastUsedDate"));
                ViewBag.NewPage = false;
                ViewBag.StartIndex = 4;
                return View(model);
            }

            // skip security questions and answers
            ModelState.Remove("CandidateModel.SecurityQuestion1Id");
            ModelState.Remove("CandidateModel.SecurityQuestion1Answer");
            ModelState.Remove("CandidateModel.SecurityQuestion2Id");
            ModelState.Remove("CandidateModel.SecurityQuestion2Answer");

            if (ModelState.IsValid)
            {               
                int candidateId=0;
                string message="";
                bool result = _registerCandidate_BL.RegisterCandidate(model, attachments, out candidateId,out  message);
                if (result)
                {
                    // Insert Candidate Picture
                    var profilePicbytes = Session["CapturedImage"] as byte[];
                    _candidatePictureService.InsertCandidatePicture(profilePicbytes, "image/jpeg", candidateId, true);
                    return RedirectToAction("RegisterResult","Candidate", new { message = message });

                }
                else {
                    ErrorNotification(message);
                    ViewBag.NewPage = false;
                    return View(model);
                }
            }
            ViewBag.NewPage = false;           
            return View(model);
        }

        [AuthorizeIpAddressAttribute(SkipIpCheck = true)]
        public ActionResult RegistrationWizard()
        {
            return InOfficeRegistration();
        }

        #endregion


        [HttpGet]
        public ActionResult CapturePicture(bool fromWizard = false)
        {
            ViewBag.FromWizard = fromWizard;
            return View();
        }

        public ActionResult TakePicture(string imgBase64)
        {

            imgBase64 = imgBase64.Replace("data:image/jpeg;base64,", "");
            byte[] bytes = Convert.FromBase64String(imgBase64);

            Session["CapturedImage"] = bytes;

            return View("CapturePicture");
        }
      
        public JsonResult Rebind()
        {
            var bytes = Session["CapturedImage"] as byte[];
            string path="";
            if(bytes!=null)
                path = "data:image/jpeg;base64," + Convert.ToBase64String(bytes, 0, bytes.Length);
            return Json(path, JsonRequestBehavior.AllowGet); 
        } 
    }

}