using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using RecogSys.RdrAccess;
using Wfm.Admin.Models.ClockTime;
using Wfm.Core;
using Wfm.Core.Domain.ClockTime;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Services.ClockTime;


namespace Wfm.Admin.Controllers
{
    public class HandTemplateController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IHandTemplateService _handTemplateService;
        private readonly IPermissionService _permissionService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateJobOrderService _placementService;
        private readonly ICompanyDivisionService _companyLocationService;
        private readonly IClockDeviceService _clockDeviceService;
        private readonly IClockCandidateService _clockCandidateService;
        private readonly ISmartCardService _smartCardService;
        private readonly IClockTimeService _clockTimeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public HandTemplateController(IActivityLogService activityLogService,
            IHandTemplateService handTemplateService,
            IPermissionService permissionService,
            ICandidateService candidateService,
            ICandidateJobOrderService placementService,
            ICompanyDivisionService companyLocationService,
            ISmartCardService smartCardService,
            IClockTimeService clockTimeService,
            IClockDeviceService clockDeviceService,
            IClockCandidateService clockCandidateService,
            ILocalizationService localizationService,
            ILogger logger,
            IWorkContext workContext)
        {
            _activityLogService = activityLogService;
            _handTemplateService = handTemplateService;
            _permissionService = permissionService;
            _candidateService = candidateService;
            _placementService = placementService;
            _companyLocationService = companyLocationService;
            _clockDeviceService = clockDeviceService;
            _clockCandidateService = clockCandidateService;
            _smartCardService = smartCardService;
            _clockTimeService = clockTimeService;
            _localizationService = localizationService;
            _logger = logger;
            _workContext = workContext;
        }

        #endregion


        #region Index

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();

            return View();
        }


        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request, int? candidateId = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCandidateSmartCards))
                return AccessDeniedView();

            //var candidates = _candidateService.GetAllCandidatesAsQueryable(_workContext.CurrentAccount);

            // only candidates with smart card
            var candidates = _smartCardService.GetAllSmartCardsAsQueryable(showInactive: false)
                .Where(x => !_workContext.CurrentAccount.IsLimitedToFranchises || x.Candidate.FranchiseId == _workContext.CurrentAccount.FranchiseId);
            if (candidateId.HasValue)
                candidates = candidates.Where(x => x.Candidate.Id == candidateId);
            // only latest one
            candidates = candidates.GroupBy(x => x.CandidateId).Select(g => g.OrderByDescending(x => x.UpdatedOnUtc).FirstOrDefault());

            var templates = _handTemplateService.GetAllHandTemplatesAsQueryable();
            var result = from c in candidates
                         from t in templates.Where(t => t.CandidateId == c.Candidate.Id).DefaultIfEmpty()
                         select new HandTemplateModel()
                         {
                             HandTemplateGuid = t != null ? t.HandTemplateGuid : Guid.Empty,
                             CandidateGuid = c.Candidate.CandidateGuid,
                             CandidateId = c.Candidate.Id,
                             EmployeeId = c.Candidate.EmployeeId,
                             SmartCardUid = c.SmartCardUid,
                             CandidateFirstName = c.Candidate.FirstName,
                             CandidateLastName = c.Candidate.LastName,
                             FranchiseId = c.Candidate.FranchiseId,
                             IsActive = t != null ? t.IsActive : true,
                             CreatedOnUtc = t != null ? t.CreatedOnUtc : null,
                             UpdatedOnUtc = t != null ? t.UpdatedOnUtc : null,
                         };

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult _Enroll()
        {
            return PartialView("_SelectClock");
        }


        [HttpPost]
        public ActionResult StartEnrolling(int clockDeviceId, int candidateId)
        {
            var errors = new StringBuilder();

            var clockDevice = _clockDeviceService.GetClockDeviceById(clockDeviceId);
            if (clockDevice != null)
            {
                using (var hr = new HandReader(clockDevice.IPAddress))
                {
                    if (hr != null && hr.TryConnect())
                    {
                        hr.Beep(2, 3);
                        var aPrompt = RSI_PROMPT.RSI_RIGHT;
                        var rsp = new RSI_STATUS();
                        if (!hr.StartEnrolling(aPrompt, rsp))
                            errors.AppendLine("Cannot start enrolling process. Try again later.");
                    }
                    else
                        errors.AppendLine("The clock is not ready. Please check.");
                }
            }
            else
                errors.AppendLine("The clock is not found. Please check.");

            return Json(new { Result = errors.Length == 0, ErrorMessage = errors.ToString() }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetLastEnrolled(int clockDeviceId, int candidateId)
        {
            var errors = new StringBuilder();

            var clockDevice = _clockDeviceService.GetClockDeviceById(clockDeviceId);
            if (clockDevice != null)
            {
                using (var hr = new HandReader(clockDevice.IPAddress))
                {
                    if (hr != null && hr.TryConnect())
                    {
                        var rsp = new RSI_STATUS();
                        var userRecord = new RsiUserRecord();
                        var candidateIdStr = candidateId.ToString();
                        var pIDStr = string.Empty;

                        if (clockDevice.ManualID)
                            pIDStr = candidateIdStr;
                        else
                        {
                            var smartCard = _smartCardService.GetActiveSmartCardByCandidateId(candidateId);
                            if (smartCard != null && !String.IsNullOrWhiteSpace(smartCard.SmartCardUid))
                                pIDStr = _smartCardService.GetIdString(smartCard, clockDevice.IDLength);
                        }

                        if (!String.IsNullOrWhiteSpace(pIDStr))
                        {
                            userRecord.pID.SetID(pIDStr);
                            //if (hr.GetLastEnrolledUser(userRecord, addUser: clockDevice.AddOnEnroll))
                            if (hr.GetLastEnrolledUser(userRecord, addUser: true))      // always add user, even for enrolment clock (for verification)
                            {
                                var template = userRecord.ToHandTemplate(candidateIdStr);   // always candidate Id
                                template.EnteredBy = _workContext.CurrentAccount.Id;
                                _handTemplateService.InsertOrUpdate(template);

                                //if (clockDevice.AddOnEnroll)
                                if (true)       // for enrolment clock, users will be eventually removed by the refresh task
                                    _clockCandidateService.InsertOrUpdate(clockDeviceId, candidateId);
                            }
                            else
                                errors.AppendLine("Cannot get the template of enrolled user.");
                        }
                        else
                            errors.AppendLine("Cannot get the enrolled user ID.");
                    }
                    else
                        errors.AppendLine("The clock is not ready. Please check.");
                }
            }
            else
                errors.AppendLine("The clock is not found. Please check.");

            return Json(new { Result = errors.Length == 0, ErrorMessage = errors.ToString() }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AddOrRemoveCandidate(int clockDeviceId, string selectedIds, string action)
        {
            int done = 0, failed = 0;
            var errors = new StringBuilder();

            if (!string.IsNullOrEmpty(selectedIds))
            {
                var clockDevice = _clockDeviceService.GetClockDeviceById(clockDeviceId);
                var ids = selectedIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x));
                if (clockDevice != null && ids.Any())
                {
                    using (var hr = new HandReader(clockDevice.IPAddress))
                    {
                        if (hr != null && hr.TryConnect())
                        {
                            foreach (var id in ids)
                            {
                                try
                                {
                                    if (_clockCandidateService.AddOrRemoveCandidate(clockDevice, hr, action, id))
                                        done++;
                                    else
                                        failed++;
                                }
                                catch (WfmException ex)
                                {
                                    errors.AppendLine(ex.Message);
                                    failed++;
                                }
                                catch (Exception exc)
                                {
                                    _logger.Error(exc.Message, exc, _workContext.CurrentAccount);
                                    errors.AppendLine(_localizationService.GetResource("Common.UnexpectedError"));
                                    failed++;
                                }
                            }
                        }
                        else
                            errors.AppendLine("The clock is not ready. Please check.");
                    }
                }
                else
                    errors.AppendLine("The clock is not found. Please check.");
            }

            var actionResult = action == "add" ? "added to" : "removed from";
            return Json(new { Done = done, Failed = failed, Message = String.Concat(done, " employee(s) ", actionResult, " clock.") }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _SelectClock()
        {
            return PartialView();
        }


        [HttpPost]
        public JsonResult _RemoveAllCandidates(int clockDeviceId)
        {
            var error = string.Empty;

            var clockDevice = _clockDeviceService.GetClockDeviceById(clockDeviceId);
            if (clockDevice != null)
            {
                using (var hr = new HandReader(clockDevice.IPAddress))
                {
                    if (hr != null && hr.TryConnect())
                    {
                        // clear user database inside clock
                        var rsp = new RSI_STATUS();
                        hr.ClearUserDatabase(rsp);

                        var added = _clockCandidateService.GetAllClockCandidatesByClock(clockDeviceId).Select(x => x.CandidateId).ToList();
                        if (added.Any())
                            _clockCandidateService.RemoveClockCandidates(clockDeviceId, added);
                        else
                            error = "No candidates found in the clock.";
                    }
                    else
                        error = "The clock is not ready. Please check.";
                }
            }
            else
                error = "The clock is not found. Please check.";


            return Json(new { Result = !String.IsNullOrWhiteSpace(error) ? error : "All candidates removed from the clock." }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
