using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.ClockTime;
using Wfm.Core;
using Wfm.Core.Domain.ClockTime;
using Wfm.Services.Companies;
using Wfm.Services.Logging;
using Wfm.Services.Candidates;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Services.ClockTime;
using Wfm.Web.Framework.Controllers;
using Wfm.Web.Framework;


namespace Wfm.Admin.Controllers
{

    //[AdminAuthorize(Roles = "Admin, System")]
    public class SmartCardController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly ISmartCardService _smartCardService;
        private readonly IPermissionService _permissionService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICompanyDivisionService _companyLocationService;
        private readonly IClockTimeService _clockTimeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger; 
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SmartCardController(IActivityLogService activityLogService,
            ISmartCardService smartCardService,
            IPermissionService permissionService,
            ICandidateService candidateService,
            ICandidateJobOrderService candidateJobOrderService,
            ICompanyDivisionService companyLocationService,
            IClockTimeService clockTimeService,
            ILocalizationService localizationService,
            ILogger logger,
            IWorkContext workContext)
        {
            _activityLogService = activityLogService;
            _smartCardService = smartCardService;
            _permissionService = permissionService;
            _candidateService = candidateService;
            _candidateJobOrderService = candidateJobOrderService;
            _companyLocationService = companyLocationService;
            _clockTimeService = clockTimeService;
            _localizationService = localizationService;
            _logger = logger;
            _workContext = workContext;
        }

        #endregion


        #region GET :/SmartCard/Index

        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCandidateSmartCards) && !_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/SmartCard/Index

        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCandidateSmartCards) && !_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();

            var smartCards = _smartCardService.GetAllSmartCardsAsQueryable();
            if (_workContext.CurrentAccount.IsLimitedToFranchises)
                smartCards = smartCards.Where(x => x.Candidate != null && x.Candidate.FranchiseId == _workContext.CurrentAccount.FranchiseId);

            var result = smartCards.ProjectTo<CandidateSmartCardModel>();

            return Json(result.ToDataSourceResult(request));
        }
        #endregion

        #region GET :/SmartCard/AddSmartCard

        [HttpGet]
        public ActionResult AddSmartCard()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/SmartCard/AddSmartCard

        [HttpPost] //point out this is for httppost
        public ActionResult AddSmartCard([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();

            var candidates = _candidateService.GetAllCandidatesAsQueryable(_workContext.CurrentAccount, true).PagedForCommand(request);

            List<CandidateModel> candidateModelList = new List<CandidateModel>();

            foreach (var c in candidates)
            {
                var candidateModel = c.ToModel();

                candidateModel.HavingSmartCard = false;

                // Get all smart cards for each candidate
                var smartCardList = _smartCardService.GetAllSmartCardsByCandidateId(candidateModel.Id, true);
                if (smartCardList != null && smartCardList.Count > 0)
                {
                    candidateModel.CandidateSmartCardId = string.Join(" || ", smartCardList.Select(x => x.SmartCardUid)); ;
                    candidateModel.HavingSmartCard = true;
                }

                candidateModelList.Add(candidateModel);
            }

            var result = new DataSourceResult()
            {
                Data = candidateModelList,
                Total = candidates.TotalCount
            };

            return Json(result);
        }

        #endregion

        #region GET :/SmartCard/Create

        [HttpGet]
        public ActionResult Create(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();
            CandidateSmartCardModel_BL bl = new CandidateSmartCardModel_BL(_candidateService);
            string error = string.Empty;
            var model = bl.GetNewSmardCardModel(guid, out error);
            if (error.Length > 0)
            {
                ErrorNotification(error);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        #endregion

        #region POST:/SmartCard/Create

        [HttpPost]
        public ActionResult Create(CandidateSmartCardModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                // check if the SmartCard already exists
                if (_smartCardService.GetSmartCardBySmartCardUid(model.SmartCardUid) != null)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.AddNew.Fail.CardExists"));
                    return View(model);
                }
                CandidateSmartCard candidateSmartCard = model.ToEntity();
                _smartCardService.Insert(candidateSmartCard);

                //activity log
                _activityLogService.InsertActivityLog("AddNewSmartCard", _localizationService.GetResource("ActivityLog.AddNewSmartCard"), model.SmartCardUid);

                SuccessNotification(_localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.Added"));
                return RedirectToAction("Details", "Candidate", new { guid = model.CandidateGuid, tabId = "tab-smart-cards" });
            }
            return View(model);


            
        }

        #endregion


        #region GET :/SmartCard/Details

        [HttpGet]
        public ActionResult Details(int id)
        {
            if (id == 0)
                return RedirectToAction("AddSmartCard");

            var candidateSmartCards = _smartCardService.GetAllSmartCardsByCandidateId(id, true);
            if (candidateSmartCards != null)
            {
                List<CandidateSmartCardModel> candidateSmartCardModelList = new List<CandidateSmartCardModel>(
                    candidateSmartCards.Select(x =>
                    {
                        var model = x.ToModel();
                        return model;
                    })
                );

                ViewBag.CandidateId = id;

                return View(candidateSmartCardModelList);
            }
            return RedirectToAction("AddSmartCard");
        }

        #endregion


        #region GET :/SmartCard/Edit
        [HttpGet]
        public ActionResult Edit(Guid id) 
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();

            CandidateSmartCard candidateSmartCard = _smartCardService.GetSmartCardByGuid(id); 

            CandidateSmartCardModel candidateSmartCardModel = candidateSmartCard.ToModel();
            return View(candidateSmartCardModel);
        }

        #endregion

        #region POST:/SmartCard/Edit
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(CandidateSmartCardModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                // check if the Smart Card already exists
                CandidateSmartCard smartCard = _smartCardService.GetSmartCardBySmartCardUid(model.SmartCardUid);

                if (smartCard == null)
                    return RedirectToAction("Index");  //Not found with the specified id

                if (model.Id != smartCard.Id)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.AddNew.Fail.CardExists"));
                    return View(model);
                }

                this._updateSmartCard(model);

                SuccessNotification(_localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = smartCard.CandidateSmartCardGuid }) : RedirectToAction("Index", "SmartCard");
            }

            return View(model);
        }
        #endregion


        #region POST:/SmartCard/Delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();

            CandidateSmartCard smartCard = _smartCardService.GetSmartCardById(id);
            if (smartCard == null)
                //Not found with the specified id
                return RedirectToAction("Index");

            try
            {
                _smartCardService.Delete(smartCard);

                //activity log
                _activityLogService.InsertActivityLog("DeleteSmartCard", _localizationService.GetResource("ActivityLog.DeleteSmartCard"), smartCard.SmartCardUid);

                SuccessNotification(_localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.Deleted"));
                return RedirectToAction("Index", "SmartCard");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", "SmartCard", new { id = smartCard.CandidateSmartCardGuid });
            }
        }
        #endregion


        #region Batch edit

        [HttpPost]
        public ActionResult _AddSmartCard([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateSmartCardModel> models)
        {
            var results = new List<CandidateSmartCardModel>();

            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    //model.SmartCardUid = model.SmartCardUid.ExtractAlphanumericText();

                    // check if the SmartCard already exists
                    if (_smartCardService.GetSmartCardBySmartCardUid(model.SmartCardUid) != null)
                    {
                        ModelState.AddModelError("SmartCardUid", _localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.AddNew.Fail.CardExists"));
                        continue;
                    }

                    var entity = model.ToEntity();
                    entity.CreatedOnUtc = DateTime.UtcNow;
                    entity.UpdatedOnUtc = entity.CreatedOnUtc;
                    _smartCardService.Insert(entity);

                    //activity log
                    _activityLogService.InsertActivityLog("AddNewSmartCard", _localizationService.GetResource("ActivityLog.AddNewSmartCard"), model.SmartCardUid);

                    results.Add(model);
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public ActionResult _EditSmartCard([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<CandidateSmartCardModel> models)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    // check if the Smart Card already exists
                    if (_smartCardService.IsDuplicate(model.SmartCardUid, model.Id))
                    {
                        ModelState.AddModelError("SmartCardUid", _localizationService.GetResource("Admin.TimeClocks.CandidateSmartCard.AddNew.Fail.CardExists"));
                        continue;
                    }

                    this._updateSmartCard(model);
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }

        #endregion

        private void _updateSmartCard(CandidateSmartCardModel model)
        {
            var entity = _smartCardService.GetSmartCardById(model.Id);

            if (!model.IsActive && !model.DeactivatedDate.HasValue && entity.IsActive)
            {
                // the record in DB is active but the model is changed to inactive...
                model.DeactivatedDate = DateTime.UtcNow;
            }
            else if (model.IsActive && model.DeactivatedDate.HasValue && !entity.IsActive)
            {
                // the record in DB is inactive but the model is changed to active...
                model.DeactivatedDate = null;
                if (model.ActivatedDate == entity.ActivatedDate)
                    model.ActivatedDate = DateTime.UtcNow;
            }

            model.EnteredBy = _workContext.CurrentAccount.Id;

            model.ToEntity(entity);
            _smartCardService.Update(entity);

            //activity log
            _activityLogService.InsertActivityLog("UpdateSmartCard", _localizationService.GetResource("ActivityLog.UpdateSmartCard"), model.SmartCardUid);
        }


        #region Candidate Match

        public ActionResult CandidateMatch(int companyLocationId, string smartCardUid, DateTime clockInOut)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SelectClockTimeCandidate))
                return AccessDeniedView();

            var location = _companyLocationService.GetCompanyLocationById(companyLocationId);

            ViewBag.CompanyLocationId = companyLocationId;
            ViewBag.SmartCardUid = smartCardUid;
            ViewBag.ClockInOut = clockInOut;

            return View();
        }


        [HttpPost]
        public ActionResult CandidateMatch([DataSourceRequest] DataSourceRequest request, int companyLocationId, string smartCardUid, DateTime clockInOut)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.SelectClockTimeCandidate))
                return AccessDeniedView();

            var match_BL = new CandidateSmartCardMatch_BL(_candidateService, _smartCardService, _candidateJobOrderService, _clockTimeService);
            var result = match_BL.GetAllCandidatesMatched(companyLocationId, smartCardUid, clockInOut);

            return Json(result.ToDataSourceResult(request));
        }


        [HttpPost]
        public JsonResult ConfirmCandidate(string smartCardUid, int candidateId, string selectedUid)
        {
            try
            {
                var match_BL = new CandidateSmartCardMatch_BL(_candidateService, _smartCardService, _candidateJobOrderService, _clockTimeService);
                match_BL.ConfirmCandidateForClockTimes(smartCardUid, candidateId, selectedUid, _workContext.CurrentAccount.Id);
            }
            catch (WfmException ex)
            {
                return Json(new { Succeed = false, Error = ex.Message });
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
                return Json(new { Succeed = false, Error = _localizationService.GetResource("Common.UnexpectedError") });
            }

            return Json(new { Succeed = true });
        }

        #endregion
    }
}
