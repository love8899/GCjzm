using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Client.Extensions;
using Wfm.Client.Models.Rescheduling;
using Wfm.Core;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Media;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.Companies;
using Wfm.Services.JobOrders;
using Wfm.Services.Logging;
using Wfm.Services.TimeSheet;
using Wfm.Shared.Models.Search;
using Wfm.Web.Framework.Feature;


namespace Wfm.Client.Controllers
{
    [FeatureAuthorize(featureName: "Rescheduling")]
    public class ReschedulingController : BaseClientController
    {
        #region Fields

        private readonly IWorkTimeService _workTimeService;
        private readonly CandidateWorkTimeSettings _worktimeSettings;
        private readonly IWorkContext _workContext;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly IJobOrderService _jobOrderService;
        private readonly ILogger _logger;
        private readonly EmployeeRescheduling_BL _employeeRescheduling_BL;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly IShiftService _shiftService;
        private readonly IOrgNameService _orgNameService;

        #endregion


        #region Ctor

        public ReschedulingController(IWorkTimeService workTimeService,
                                      CandidateWorkTimeSettings worktimeSettings,
                                      IWorkContext workContext,
                                      ICandidateJobOrderService candidateJobOrderService,
                                      ICandidatePictureService candidatePictureService,
                                      MediaSettings mediaSettings,
                                      IJobOrderService jobOrderService,
                                      ILogger logger,
                                      EmployeeRescheduling_BL employeeRescheduling_BL,
                                      ICompanyBillingService companyBillingService,
                                      IShiftService shiftService,
                                      IOrgNameService orgNameService)
        {
            _workTimeService = workTimeService;
            _worktimeSettings = worktimeSettings;
            _workContext = workContext;
            _candidateJobOrderService = candidateJobOrderService;
            _candidatePictureService = candidatePictureService;
            _mediaSettings = mediaSettings;
            _jobOrderService = jobOrderService;
            _logger = logger;
            _employeeRescheduling_BL = employeeRescheduling_BL;
            _companyBillingService = companyBillingService;
            _shiftService = shiftService;
            _orgNameService = orgNameService;
        }

        #endregion


        #region List

        public ActionResult Index()
        {
            var refDate = DateTime.Today;
            var searchBL = new SearchBusinessLogic(_workContext, _orgNameService);
            var model = searchBL.GetSearchTimeSheetModel(refDate, refDate);

            return View(model);
        }


        [HttpPost]
        public ActionResult Index([DataSourceRequest]DataSourceRequest request, SearchTimeSheetModel model)
        {
            var result = _employeeRescheduling_BL.GetAllPlacementForToday(_workContext.CurrentAccount).Distinct();

            KendoHelper.CustomizePlacementBasedFilters(request, model);

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult _MoveForToday(DateTime refDate)
        {
            var searchBL = new SearchBusinessLogic(_workContext, _orgNameService);
            var model = searchBL.GetSearchTimeSheetModel(refDate, refDate);

            return PartialView(model);
        }


        public ActionResult _MoveForFuture(DateTime refDate)
        {
            var searchBL = new SearchBusinessLogic(_workContext, _orgNameService);
            var model = searchBL.GetSearchAttendanceModel(refDate, refDate);

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult _AllPlacementByDate([DataSourceRequest]DataSourceRequest request, SearchAttendanceModel model)
        {
            var result = _employeeRescheduling_BL.GetAllPlacementByDate(_workContext.CurrentAccount, model.sf_From)
                .ToList()   // earlier load, to facilitate sort/filter by job order start time
                .Select(x => x.ToPlacementModel(model.sf_From));

            if (model.sf_ShiftStartTime.HasValue)   // add date for time only filter
                model.sf_ShiftStartTime = model.sf_From.Date + model.sf_ShiftStartTime.Value.TimeOfDay;

            KendoHelper.CustomizePlacementBasedFilters(request, model, 
                skip: new List<string>() { "ClientTime" },
                mapping: new Dictionary<string, string>() { { "ShiftStartTime", "StartTime" } } );

            return Json(result.ToDataSourceResult(request));
        }

        #endregion


        #region scheduling for today

        /// <summary>
        /// This is used when we have a WorkTime record for the selected employee (employee has punched-in but not punched out)
        /// </summary>
        /// <param name="workTimeId"></param>
        /// <param name="refDate"></param>
        /// <returns></returns>
        public ActionResult MoveToOtherShift(int workTimeId, DateTime refDate)
        {
            var workTime = _workTimeService.GetWorkTimeById(workTimeId);
            if (workTimeId <= 0 || refDate == DateTime.MinValue)
            {
                _logger.Warning(String.Format("MoveToOtherShift(): Invalid input values. workTimeId={0}  refDate={1}", workTimeId, refDate.ToShortDateString()));
                return new EmptyResult();
            }

            int placementId = 0;
            var placement = _candidateJobOrderService.GetAllCandidateJobOrdersByDateRangeAsQueryable(refDate, refDate).Where(x => x.JobOrderId == workTime.JobOrderId).FirstOrDefault();
            if (placement != null)
                placementId = placement.Id;

            var defaultCandidatePicture = _candidatePictureService.GetCandidatePicturesByCandidateId(placement.CandidateId, 1).FirstOrDefault();
            var pictureUrl = _candidatePictureService.GetCandidatePictureUrl(defaultCandidatePicture, _mediaSettings.CandidateDetailsPictureSize * 2, true);

            var model = new ReschedulingModel()
            {
                OrigId = placementId,
                FranchiseId = workTime.FranchiseId,
                CandidateId = workTime.CandidateId,
                EmployeeId = workTime.Candidate.EmployeeId,
                EmployeeName = workTime.Candidate.GetFullName(),
                PictureThumbnailUrl = pictureUrl,
                JobOrderId = workTime.JobOrderId,
                JobOrderGuid = workTime.JobOrder.JobOrderGuid,
                CompanyId = workTime.CompanyId,
                LocationId = workTime.CompanyLocationId,
                DepartmentId = workTime.CompanyDepartmentId,
                PositionId = workTime.JobOrder.PositionId.Value,
                ShiftId = workTime.JobOrder.ShiftId,
                CompanyContactId = workTime.CompanyContactId,
                Supervisor = workTime.JobOrder.CompanyContact.FullName,
                StartTime = workTime.JobOrder.StartTime,
                EndTime = workTime.JobOrder.EndTime,
                StartDate = refDate,
                EndDate = refDate,
                WorkTimeId = workTimeId,
                PunchIn = workTime.ClockIn.Value
            };

            var cmpBilling = _companyBillingService.GetCompanyBillingRateByCompanyIdAndCompanyLocationIdAndRateCode(placement.JobOrder.CompanyId,
                                                    placement.JobOrder.CompanyLocationId, placement.JobOrder.BillingRateCode, refDate);
            if (cmpBilling != null)
                model.PayRate = cmpBilling.RegularPayRate.ToString("F2");

            return PartialView("_JobReschedule", model);
        }


        #endregion

        #region scheduling for future

        [HttpPost]
        public ActionResult _GetJobOrderInfo(int jobOrderId, DateTime refDate)
        {
            bool result = false;
            string jobOrdrGuid = null;
            decimal payRate = 0;

            var jobOrder = _jobOrderService.GetJobOrderById(jobOrderId);
            if (jobOrder != null)
            {
                result = true;
                jobOrdrGuid = jobOrder.JobOrderGuid.ToString();

                var cmpBilling = _companyBillingService.GetCompanyBillingRateByCompanyIdAndCompanyLocationIdAndRateCode(jobOrder.CompanyId, jobOrder.CompanyLocationId,
                                                                                                                        jobOrder.BillingRateCode, refDate);
                if (cmpBilling != null)
                    payRate = cmpBilling.RegularPayRate;
            }

            return Json(new { Succeed = result, jobOrder.PositionId, jobOrder.CompanyContactId, JobOrdrGuid = jobOrdrGuid, PayRate = payRate.ToString("F2") });
        }


        public ActionResult GetShiftsForScheduling()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            var shifts = _shiftService.GetAllShifts(false, false, _workContext.CurrentAccount.CompanyId);
            result.AddRange(shifts.Select(x => new SelectListItem()
            {
                Text = String.Concat(x.ShiftName, " (", x.MinStartTime.Value.ToShortTimeString(), " - ", x.MaxEndTime.Value.ToShortTimeString(), ")") ,
                Value = x.Id.ToString()
            }));

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult _GetPayRateByShift(int shiftId, int positionId, int locationId, int franchiseId, DateTime refDate)
        {
            decimal payRate = _employeeRescheduling_BL.GetPayRateByShift(shiftId, positionId, locationId, franchiseId, refDate);

            return Json(new { Succeed = true, PayRate = payRate.ToString("F2") });
        }


        public ActionResult _JobReschedule(int placementId, DateTime refDate)
        {
            var placement = _candidateJobOrderService.GetCandidateJobOrderById(placementId);
            var defaultCandidatePicture = _candidatePictureService.GetCandidatePicturesByCandidateId(placement.CandidateId, 1).FirstOrDefault();
            var pictureUrl = _candidatePictureService.GetCandidatePictureUrl(defaultCandidatePicture, _mediaSettings.CandidateDetailsPictureSize * 2, true);

            var model = new ReschedulingModel()
            {
                OrigId = placementId,
                FranchiseId = placement.JobOrder.FranchiseId,
                CandidateId = placement.CandidateId,
                EmployeeId = placement.Candidate.EmployeeId,
                EmployeeName = placement.Candidate.GetFullName(),
                PictureThumbnailUrl = pictureUrl,
                JobOrderId = placement.JobOrderId,
                JobOrderGuid = placement.JobOrder.JobOrderGuid,
                CompanyId = placement.JobOrder.CompanyId,
                LocationId = placement.JobOrder.CompanyLocationId,
                DepartmentId = placement.JobOrder.CompanyDepartmentId,
                PositionId = placement.JobOrder.PositionId.Value,
                ShiftId = placement.JobOrder.ShiftId,
                CompanyContactId = placement.JobOrder.CompanyContactId,
                Supervisor = placement.JobOrder.CompanyContact.FullName,
                StartTime = placement.JobOrder.StartTime,
                EndTime = placement.JobOrder.EndTime,
                StartDate = refDate,
                EndDate = refDate
            };

            var cmpBilling = _companyBillingService.GetCompanyBillingRateByCompanyIdAndCompanyLocationIdAndRateCode(placement.JobOrder.CompanyId,
                                                    placement.JobOrder.CompanyLocationId, placement.JobOrder.BillingRateCode, refDate);
            if (cmpBilling != null)
                model.PayRate = cmpBilling.RegularPayRate.ToString("F2");

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _Recalculate(ReschedulingModel model)
        {
            if (model.CompanyContactId == 0) ModelState.Remove("CompanyContactId");
            if (model.DepartmentId == 0) ModelState.Remove("DepartmentId");
            if (model.JobOrderId == 0) ModelState.Remove("JobOrderId");
            if (model.EndDate < model.StartDate) model.EndDate = model.StartDate;

            if (ModelState.IsValid)
            {
                JobOrder newJobOrder;
                var errMessage = _employeeRescheduling_BL.FindMatchingJobOrder(model, out newJobOrder);

                if (!String.IsNullOrWhiteSpace(errMessage))
                {
                    return Json(new { Succeed = false, Error = errMessage });
                }
                else if (newJobOrder != null)
                {
                    if (newJobOrder.JobOrderGuid == model.JobOrderGuid)
                    {
                        // Nothing has changed. The original job order and the new job order are the same
                        return Json(new { Succeed = true, MatchingResult = "NoChange" });
                    }
                    else if (newJobOrder.JobOrderGuid != Guid.Empty)
                    {
                        var cmpBilling = _companyBillingService.GetCompanyBillingRateByCompanyIdAndCompanyLocationIdAndRateCode(newJobOrder.CompanyId,
                            newJobOrder.CompanyLocationId, newJobOrder.BillingRateCode, model.StartDate);
                        var payRate = cmpBilling != null ? cmpBilling.RegularPayRate.ToString("F2") : "0.00";

                        // we've found at least one matching job order
                        return Json(new
                        {
                            Succeed = true,
                            MatchingResult = "ExistingJO",
                            JobOrderId = newJobOrder.Id,
                            JobOrderGuid = newJobOrder.JobOrderGuid,
                            PositionId = newJobOrder.PositionId,  // do we need this one?
                            ShiftId = newJobOrder.ShiftId,
                            CompanyContactId = newJobOrder.CompanyContactId,
                            PayRate = payRate
                        });
                    }
                    else
                    {
                        _logger.Error("_Recalculate(): Invalid matched job order. Matched job order's GUID is empty.", null, _workContext.CurrentAccount);
                        return Json(new { Succeed = false, Error = "Unexpected Error!" }); 
                    }
                }
                else
                {
                    //// We didn't find any existing job order that mathces the new parameters. 

                    // Do we have enough information to create a new job order?
                    int _shiftId = 0; 
                    decimal payRate = 0;
                    var _newShift = _employeeRescheduling_BL.FindMatchingShift(_workContext.CurrentAccount.CompanyId, model.StartTime, model.EndTime);
                    if (_newShift != null)
                    {
                        _shiftId = _newShift.Id;
                        payRate = _employeeRescheduling_BL.GetPayRateByShift(_shiftId, model.PositionId, model.LocationId, model.FranchiseId, model.StartDate);
                    }

                    return Json(new
                    {
                        Succeed = true,
                        MatchingResult = "NewJO",
                        JobOrderId = 0,
                        JobOrderGuid = Guid.Empty,
                        ShiftId = _shiftId,
                        CompanyContactId = 0,
                        PayRate = payRate.ToString("F2")
                    });
                }

            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage) );

                string errMsg = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                return Json(new { Succeed = false, Error = errMsg });
            }

        }


        [HttpPost]
        public ActionResult _SaveSchedule(ReschedulingModel model)
        {
            if (model.CompanyContactId == 0) ModelState.Remove("CompanyContactId");
            if (model.JobOrderId == 0) ModelState.Remove("JobOrderId");
            if (model.DepartmentId == 0) ModelState.Remove("DepartmentId");
            if (model.EndDate < model.StartDate) model.EndDate = model.StartDate;

            var shiftLength = (model.EndTime - model.StartTime).TotalHours;
            if (shiftLength < 0)
                shiftLength += 24;
            if (shiftLength > 12 )
                ModelState.AddModelError("StartTime", "The shift is too long. Please conatct our recruiter for setting up this schedule.");

            // if employee has punched in already, automatically punch him out and create the worktime record for the work
            if (model.WorkTimeId > 0)
            {
                var result = _employeeRescheduling_BL.ManageExistingTimeSheet(model, _worktimeSettings.StartScanWindowSpanInMinutes);
                if (result.Any())
                {
                    foreach (var err in result)
                        ModelState.AddModelError(err.Key, err.Value);
                }
            }

            bool _moveJobOrder = false;

            if (ModelState.IsValid)
            {
                // load original placement record and check if job order has changed
                var placement = _candidateJobOrderService.GetCandidateJobOrderById(model.OrigId);
                if (placement == null)
                    return AccessDeniedView();

                if (model.JobOrderId == 0)
                {
                    //Create new job order
                    var newJo = _employeeRescheduling_BL.CreateMatchingJobOrder(placement.JobOrderId, model);
                    if (newJo != null)
                    {
                        _jobOrderService.InsertJobOrder(newJo);

                        model.JobOrderId = newJo.Id;
                        model.JobOrderGuid = newJo.JobOrderGuid;

                        _moveJobOrder = true;
                    }
                }
                else
                {
                    if (model.JobOrderId != placement.JobOrderId)
                    {
                        //Job order is changed
                        _moveJobOrder = true;
                    }
                }

            }

            if (_moveJobOrder)
            {
                var result = _employeeRescheduling_BL.MoveToOtherJobOrder(model, _workContext.CurrentAccount.Id);
                foreach (var entry in result)
                    ModelState.AddModelError(entry.Key, entry.Value);

                if (model.WorkTimeId > 0)
                {
                    // create the punch-in entry for the new placement
                    var err = _employeeRescheduling_BL.createPunchInEntryForNewPlacement(model);
                }
            }

            var errors = ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage));
            if (errors.Any())
            {
                string errMsg = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                return Json(new { Succeed = false, Error = errMsg });
            }

            if (_moveJobOrder)
                return Json(new { Succeed = true, Message = String.Format("Employee {0}'s work schedule is changed.", model.EmployeeId) });
            else
                return Json(new { Succeed = true, Message = String.Format("Employee {0}'s work schedule is NOT changed.", model.EmployeeId) });
        }

        #endregion

    }
}