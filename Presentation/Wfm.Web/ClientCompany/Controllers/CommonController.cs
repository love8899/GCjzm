using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Client.Models.Common;
using Wfm.Core;
using Wfm.Services.Localization;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using System;
using Wfm.Services.Logging;
using Wfm.Services.Candidates;
using Wfm.Services.Features;
using Wfm.Client.Extensions;
using Kendo.Mvc.UI;
using Wfm.Services.Franchises;
using Wfm.Core.Domain.Candidates;
using Kendo.Mvc.Extensions;
using Wfm.Services.Media;
using System.IO;
using Wfm.Core.Domain.Media;
using Wfm.Client.Models.Candidate;
using Wfm.Services.JobOrders;
using Wfm.Core.Domain.JobOrders;
using Wfm.Services.Test;


namespace Wfm.Client.Controllers
{
    public partial class CommonController : BaseClientController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        //private readonly IFranchiseContext _franchiseContext;
        private readonly IPositionService _positionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;
        private readonly ILogger _logger;
        private readonly ICandidateService _candidateService;
        private readonly IFranchiseService _franchiseService;
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly ICandidateKeySkillService _candidateKeySkillsService;
        private readonly ITestService _testService;
        private readonly ICandidateTestResultService _candidateTestResultService;
        private readonly IAttachmentService _attachmentService;
        private readonly IWebHelper _webHelper;
        private readonly MediaSettings _mediaSettings;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IJobOrderTestCategoryService _jobOrderTestCategoryService;
        private readonly IUserFeatureService _userFeatureService;
        private readonly IShiftService _shiftService;
        private readonly ICompanyBillingService _companyBillingService;
        #endregion

        #region Constructors

        public CommonController( 
            ILanguageService languageService, 
            IWorkContext workContext,
            //IFranchiseContext franchiseContext,
            IPositionService positionService,
            ICompanyDepartmentService companyDepartmentService,
            ILogger logger,
            ICandidateService candidateService,
            IFranchiseService franchiseService,
            ICandidatePictureService candidatePictureService,
            ICandidateKeySkillService candidateKeySkillsService,
            ITestService testService,
            ICandidateTestResultService candidateTestResultService,
            IAttachmentService attachmentService,
            IWebHelper webHelper,
            MediaSettings mediaSettings,
            ICompanyDivisionService companyDivisionService,
            IJobOrderService jobOrderService,
            IJobOrderTestCategoryService jobOrderTestCategoryService,
            IUserFeatureService userFeatureService,
            IShiftService shiftService,
            ICompanyBillingService companyBillingService)
        {
            this._languageService = languageService;
            this._workContext = workContext;
            //this._franchiseContext = franchiseContext;
            _positionService = positionService;
            _companyDepartmentService = companyDepartmentService;
            _logger = logger;
            _candidateService = candidateService;
            _franchiseService = franchiseService;
            _candidatePictureService = candidatePictureService;
            _candidateKeySkillsService = candidateKeySkillsService;
            _testService = testService;
            _candidateTestResultService = candidateTestResultService;
            _attachmentService = attachmentService;
            _webHelper = webHelper;
            _mediaSettings = mediaSettings;
            _companyDivisionService = companyDivisionService;
            _jobOrderService = jobOrderService;
            _jobOrderTestCategoryService = jobOrderTestCategoryService;
            _userFeatureService = userFeatureService;
            _shiftService = shiftService;
            _companyBillingService = companyBillingService;
        }

        #endregion

        #region Methods
        //header links
        [ChildActionOnly]
        public ActionResult ClientHeaderLinks()
        {
            var account = _workContext.CurrentAccount;


            var model = new ClientHeaderLinksModel
            {
                IsAuthenticated = account != null,
                AccountEmailUsername = account == null ? string.Empty : account.Username
            };

            return PartialView(model);
        }

        ////language
        //[ChildActionOnly]
        //public ActionResult LanguageSelector()
        //{
        //    var model = new LanguageSelectorModel();
        //    model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
        //    model.AvailableLanguages = _languageService
        //        .GetAllLanguages(franchiseId: _franchiseContext.CurrentStore.Id)
        //        .Select(x => x.ToModel())
        //        .ToList();
        //    return PartialView(model);
        //}
        public ActionResult LanguageSelected(int customerlanguage)
        {
            var language = _languageService.GetLanguageById(customerlanguage);
            if (language != null)
            {
                _workContext.WorkingLanguage = language;
            }
            return Content("Changed");
        }

        #region GetCascadePositions
        public JsonResult GetCascadePositions()
        {
            var positionList = new List<SelectListItem>();
            int companyId = _workContext.CurrentAccount.CompanyId;
            if (companyId > 0)
            {
                positionList = _positionService.GetAllPositionByCompanyId(companyId).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            }
            return Json(positionList, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region GetCascadeShift

        public JsonResult GetCascadeShift(bool withRate = true)
        {
            var cmpId = _workContext.CurrentAccount.CompanyId; ;
            var companyShift = _userFeatureService.CheckFeatureByName(cmpId, "Rescheduling");
            var shifts = _shiftService.GetAllShifts(companyId: companyShift ? cmpId : 0);
            if (withRate)
            {
                var companyBillingRateShift = _companyBillingService.GetAllCompanyBillingRatesByCompanyIdAndRefDate(cmpId, null).Select(x => x.ShiftCode).ToList();
                shifts = shifts.Where(s => companyBillingRateShift.Contains(s.ShiftName)).ToList();
            }

            return Json(shifts.Select(x => new SelectListItem() { Text = x.ShiftName, Value = x.Id.ToString() }), JsonRequestBehavior.AllowGet);
        }

        #endregion


        public JsonResult GetCascadeLocations()
        {
            var locationDropDownList = new List<SelectListItem>();
            int companyId = _workContext.CurrentAccount.CompanyId;
            if (companyId != 0)
            {
                var locations = _companyDivisionService.GetAllCompanyLocationsByCompanyId(Convert.ToInt32(companyId))
                                .OrderBy(x => x.LocationName)
                                .Select(x => new SelectListItem() { Text = x.LocationName, Value = x.Id.ToString() });

                locationDropDownList.AddRange(locations);
            }
            return Json(locationDropDownList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeDepartments(string locationId)
        {
            var locId = string.IsNullOrEmpty(locationId) ? 0 : System.Convert.ToInt32(locationId);

            var departmentDropDownList = new List<SelectListItem>();

            if (locId > 0)
            {
                var departments = _companyDepartmentService.GetAllCompanyDepartmentByLocationId(locId).OrderBy(x => x.DepartmentName);

                foreach (var c in departments)
                {
                    var item = new SelectListItem()
                    {
                        Text = c.DepartmentName,
                        Value = c.Id.ToString()
                    };
                    departmentDropDownList.Add(item);
                }
            }

            return Json(departmentDropDownList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeJobOrders(string locationId, string departmentId,DateTime? refDate)
        {
            if (!refDate.HasValue)
                refDate = DateTime.Today;

            int companyId = _workContext.CurrentAccount.CompanyId;
            
            var locId = String.IsNullOrEmpty(locationId) ? 0 : Convert.ToInt32(locationId);
            var deptId = String.IsNullOrEmpty(departmentId) ? 0 : Convert.ToInt32(departmentId);


            var jobOrders = _jobOrderService.GetAllJobOrdersByCompanyIdAsQueryable(companyId)
                            .Where(x => x.JobOrderStatusId == (int)JobOrderStatusEnum.Active &&
                                        x.StartDate <= refDate.Value && (!x.EndDate.HasValue || x.EndDate >= refDate.Value) &&
                                        x.CompanyLocationId == locId)
                            .OrderByDescending(x => x.Id).AsQueryable();

            if (deptId > 0)
                jobOrders = jobOrders.Where(x => x.CompanyDepartmentId == deptId);


            var jobOrderList = new List<SelectListItem>();
            //jobOrderList.Add(new SelectListItem() { Text = "None", Value = "0" });

            foreach (var j in jobOrders.ToList())
            {
                var item = new SelectListItem()
                {
                    Text = j.CompanyContact.FullName + " -- " + j.JobTitle,
                    Value = j.Id.ToString()
                };
                jobOrderList.Add(item);
            }

            return Json(jobOrderList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Basic Information
        public ActionResult _CandidateBasicInfo(Guid? guid, string viewName = null)
        {
            if (guid == null)
            {
                _logger.Error("_CandidateBasicInfo():Guid is null!");
                ErrorNotification("Fail to read data!");
                return View();
            }
            var entity = _candidateService.GetCandidateByGuidForClient(guid);
            if (entity == null)
            {
                _logger.Error("_CandidateBasicInfo():Candidate does not exist!");
                ErrorNotification("Fail to read data!");
                return View();
            }
            var model = entity.ToBasicModel();
            var franchise= _franchiseService.GetFranchiseById(model.FranchiseId);
            model.FranchiseName = franchise==null?"":franchise.FranchiseName;
            var defaultCandidatePicture = _candidatePictureService.GetCandidatePicturesByCandidateId(model.Id, 1).FirstOrDefault();
            model.PictureThumbnailUrl = _candidatePictureService.GetCandidatePictureUrl(defaultCandidatePicture,_mediaSettings.CandidateDetailsPictureSize);

            if (String.IsNullOrWhiteSpace(viewName))
                return PartialView(model);
            else
                return View(viewName, model);
        }


        [HttpPost]
        public JsonResult GetCandidatekeySkills([DataSourceRequest] DataSourceRequest request, Guid? guid)
        {

            if (guid == null)
            {
                ErrorNotification("The candidate does not exist!");
                return Json(new List<CandidateKeySkillModel>());
            }
            var entity = _candidateService.GetCandidateByGuid(guid.Value);
            if (entity == null)
            {
                ErrorNotification("The candidate does not exist!");
                return Json(new List<CandidateKeySkillModel>());
            }
            var candidateKeySkills = _candidateKeySkillsService.GetCandidateKeySkillsByCandidateId(entity.Id);

            return Json(candidateKeySkills.ToDataSourceResult(request,x=>x.ToModel()));
        }


        [HttpPost]
        public JsonResult GetCandidateTestResults([DataSourceRequest] DataSourceRequest request, Guid? guid, int? companyId)
        {
            var results = Enumerable.Empty<CandidateTestResult>();
            if (guid != null)
            {
                var candidate = _candidateService.GetCandidateByGuid(guid.Value);
                if (candidate != null)
                    results = _candidateTestResultService.GetCandidateTestResultsByCandidateId(candidate.Id).Where(x => x.IsPassed);
            }

            if (results.Any() && companyId.HasValue)
            {
                var testCategorieIds = _jobOrderTestCategoryService.GetTestCategoriesByCompany(companyId.Value).Select(x => x.Id);
                results = results.Where(x => testCategorieIds.Contains(x.TestCategoryId));
            }

            return Json(results.ToDataSourceResult(request, x => x.ToModel()));
        }


        public ActionResult DownloadCandidateTestResult(int id)
        {
            var notFound = File(System.Text.Encoding.UTF8.GetBytes("Not found"), "text/plain", "error.txt");
            var testResult = _candidateTestResultService.GetCandidateTestResultById(id);
            if (testResult == null)
                return notFound;

            // test result file
            var basePath = _webHelper.GetRootDirectory();
            var testResultFileWithPath = Path.Combine(basePath, testResult.ScoreFilePath);

            var fileInfo = new FileInfo(testResultFileWithPath);
            var fileName = fileInfo.Name;
            var contentType = _attachmentService.GetMimeType(fileName);

            var testCategory = _testService.GetTestCategoryById(testResult.TestCategoryId);
            var originalFileName = String.Concat(testResult.CandidateId, "_", testCategory.TestCategoryName, ".txt");
            if (System.IO.File.Exists(testResultFileWithPath))
                return File(testResultFileWithPath, contentType, originalFileName);

            return notFound;
        }


        [HttpPost]
        public ActionResult GetCandidateAttachments([DataSourceRequest] DataSourceRequest request, Guid? guid)
        {
            if (guid == null)
            {
                ErrorNotification("The candidate does not exist!");
                return Json(new List<CandidateAttachmentModel>());
            }
            var entity = _candidateService.GetCandidateByGuid(guid.Value);
            if (entity == null)
            {
                ErrorNotification("The candidate does not exist!");
                return Json(new List<CandidateAttachmentModel>());
            }
            var candidateAttachments = _attachmentService.GetAttachmentsByCandidateId(entity.Id)
                                       .Where(x => x.DocumentType.InternalCode == Convert.ToString(CandidateDocumentTypeEnum.CERTIFICATES)
                                             || (x.DocumentType.InternalCode == Convert.ToString(CandidateDocumentTypeEnum.CLIENTDOCUMENTS) && x.CompanyId==_workContext.CurrentAccount.CompanyId));


            return Json(candidateAttachments.ToDataSourceResult(request,x=>x.ToModel()));
        }

        public ActionResult DownloadAttachment(int attachmentId)
        {
            CandidateAttachment attachment = _attachmentService.GetAttachmentById(attachmentId);
            if (attachment == null)
                return RedirectToAction("List");

            if (!String.IsNullOrEmpty(attachment.StoredPath))
            {
                string contentType = attachment.ContentType;
                string originalFileName = attachment.OriginalFileName;
                string basePath = _webHelper.GetRootDirectory();
                string folderPath = Path.Combine(basePath, attachment.StoredPath);

                string filePath = Path.Combine(folderPath, attachment.StoredFileName);

                if (System.IO.File.Exists(filePath))
                    return File(filePath, contentType, originalFileName);

                ErrorNotification("File not found. Please contact the system administrator.");
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            else
            {
                return File(attachment.AttachmentFile, attachment.ContentType, attachment.OriginalFileName);
            }
        }
        #endregion

    }
}
