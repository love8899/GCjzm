using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Candidates;
using Wfm.Services.ClockTime;
using Wfm.Services.JobOrders;
using Wfm.Services.Logging;
using Wfm.Services.Messages;
using Wfm.Services.TimeSheet;
using Wfm.Core.Domain.Common;
using Wfm.Services.Common;
using System.Data.SqlClient;
using Wfm.Data;
using Wfm.Services.Companies;
using Wfm.Client.Models.TimeSheet;
using Wfm.Core.Data;


namespace Wfm.Client.Models.Rescheduling
{
    public class EmployeeRescheduling_BL
    {
        private readonly IWorkTimeService _workTimeService;
        private readonly IWorkContext _workContext;
        private readonly IClockTimeService _clockTimeService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IJobOrderService _jobOrderService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ILogger _logger;
     //   private readonly CandidateWorkTimeSettings _candidateWorkTimeSettings;
        private readonly IShiftService _shiftService;
        private readonly ICompanyBillingService _companyBillingService;
        private readonly IDbContext _dbContext;
        private readonly IRepository<CandidateJobOrder> _candidateJobOrderRepository;
        private readonly IRepository<CandidateWorkTime> _candidateWorkTimeRepository;
        private readonly IRepository<CompanyLocation> _companyLocationRepository;
        private readonly IRepository<CompanyDepartment> _companyDepartmentRepository;
        private readonly IClockDeviceService _clockDeviceService;
        private readonly IPositionService _positionService;

        public EmployeeRescheduling_BL(IWorkTimeService workTimeService,
                                      IWorkContext workContext,
                                      IClockTimeService clockTimeService,
                                      ICandidateJobOrderService candidateJobOrderService,
                                      IJobOrderService jobOrderService,
                                      IWorkflowMessageService workflowMessageService,
                                      ILogger logger,
                                   //   CandidateWorkTimeSettings candidateWorkTimeSettings,
                                      IShiftService shiftService,
                                      ICompanyBillingService companyBillingService,
                                      IRepository<CandidateJobOrder> candidateJobOrderRepository,
                                      IRepository<CandidateWorkTime> candidateWorkTimeRepository,
                                      IRepository<CompanyLocation> companyLocationRepository,
                                      IRepository<CompanyDepartment> companyDepartmentRepository,
                                      IDbContext dbContext,
                                      IClockDeviceService clockDeviceService,
                                      IPositionService positionService)
        {
            _workTimeService = workTimeService;
            _workContext = workContext;
            _clockTimeService = clockTimeService;
            _candidateJobOrderService = candidateJobOrderService;
            _jobOrderService = jobOrderService;
            _workflowMessageService = workflowMessageService;
            _logger = logger;
          //  _candidateWorkTimeSettings = candidateWorkTimeSettings;
            _shiftService = shiftService;
            _companyBillingService = companyBillingService;
            _candidateJobOrderRepository = candidateJobOrderRepository;
            _candidateWorkTimeRepository = candidateWorkTimeRepository;
            _companyLocationRepository = companyLocationRepository;
            _companyDepartmentRepository = companyDepartmentRepository;
            _dbContext = dbContext;
            _clockDeviceService = clockDeviceService;
            _positionService = positionService;
        }


        //public Dictionary<string, string> MoveCandidateToNewJobOrder(EmployeeReschedulingModel model)
        //{
        //    Dictionary<string, string> errors = new Dictionary<string, string>();

        //    var originalWorkTime = _workTimeService.GetWorkTimeById(model.WorkTimeId);

        //    if (model.EmployeeId != originalWorkTime.Candidate.EmployeeId || originalWorkTime.CompanyId != _workContext.CurrentAccount.CompanyId)
        //    {
        //        errors.Add("EmployeeId", String.Concat("Input Data is corrupted ", model.WorkTimeId));
        //        return errors;
        //    }

        //    var previousJobOrder = _jobOrderService.GetJobOrderById(originalWorkTime.JobOrderId);
        //    var newJobOrder = _jobOrderService.GetJobOrderById(model.JobOrderId);


        //    // validate the time for the new job order
        //    var validStartTime = new DateTime(originalWorkTime.JobStartDateTime.Year, originalWorkTime.JobStartDateTime.Month, originalWorkTime.JobStartDateTime.Day, newJobOrder.StartTime.Hour, newJobOrder.StartTime.Minute, 0);
        //    var validEndTime = new DateTime(originalWorkTime.JobStartDateTime.Year, originalWorkTime.JobStartDateTime.Month, originalWorkTime.JobStartDateTime.Day, newJobOrder.EndTime.Hour, newJobOrder.EndTime.Minute, 0);

        //    var newStartTime = new DateTime(originalWorkTime.JobStartDateTime.Year, originalWorkTime.JobStartDateTime.Month, originalWorkTime.JobStartDateTime.Day, model.StartTime.Hour, model.StartTime.Minute, 0);

        //    // is the new job order a night shift?
        //    bool isNightShift = (newJobOrder.EndTime < newJobOrder.StartTime);
        //    if (isNightShift)
        //    {
        //        validEndTime = validEndTime.AddDays(1);
        //        if (model.StartTime.TimeOfDay < new TimeSpan(11, 59, 59))
        //            newStartTime = newStartTime.AddDays(1);
        //    }

        //    if (newStartTime < validStartTime || newStartTime >= validEndTime)
        //    {
        //        errors.Add("StartTime", String.Format("Start time for the new job order is invalid. It should be between {0} and {1} ", newJobOrder.StartTime.ToString("hh:mm tt"), newJobOrder.EndTime.ToString("hh:mm tt")));
        //    }

        //    //int min = _candidateWorkTimeSettings.StartScanWindowSpanInMinutes;
        //    //if (TimeSpan.Compare(model.StartTime.TimeOfDay, newJobOrder.StartTime.AddMinutes(-min).TimeOfDay) < 0)
        //    //    ModelState.AddModelError("StartTime", String.Concat("Start time should be later than  ", newJobOrder.StartTime.AddMinutes(-min).ToString("hh:mm tt"),"."));

        //    if (errors.Count > 0)
        //        return errors;

        //    bool calcNewWorkTime = false;

        //    if (model.StartTime == model.PunchIn)
        //    {
        //        // Since the start of the move matches exactlt with the punch in, we will change the assignment for the current day.
        //        // When the start time of the new placement is later than the actual punch, employee will end up placed into two job orders for the day (the else part)

        //        //*** void the existing worktime record and place the employee into another job order
        //        _workTimeService.ChangeCandidateWorkTimeStatus(originalWorkTime, CandidateWorkTimeStatus.Voided, _workContext.CurrentAccount);

        //        var origPlacement = _candidateJobOrderService.GetAllCandidateJobOrdersByJobOrderIdAndCandidateIdAndDate(originalWorkTime.JobOrderId, originalWorkTime.CandidateId, DateTime.Today)
        //                                                   .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed).FirstOrDefault();


        //        var _reschedulingModel = new ReschedulingModel()
        //        {
        //            OrigId = origPlacement != null ? origPlacement.Id : 0,
        //            CandidateId = originalWorkTime.CandidateId,
        //            CompanyId = originalWorkTime.CompanyId,
        //            CompanyContactId = newJobOrder.CompanyContactId,
        //            DepartmentId = newJobOrder.CompanyDepartmentId,
        //            EmployeeId = originalWorkTime.Candidate.EmployeeId,
        //            EndDate = DateTime.Today,
        //            EndTime = newJobOrder.EndTime,
        //            StartDate = DateTime.Today,
        //            FranchiseId = originalWorkTime.FranchiseId,
        //            JobOrderGuid = newJobOrder.JobOrderGuid,
        //            JobOrderId = newJobOrder.Id,
        //            LocationId = newJobOrder.CompanyLocationId,
        //            PositionId = newJobOrder.PositionId.Value,
        //            ShiftId = newJobOrder.ShiftId,
        //            StartTime = model.StartTime
        //        };

        //        errors = this.MoveToOtherJobOrder(_reschedulingModel, _workContext.CurrentAccount.Id);
        //        calcNewWorkTime = true;
        //    }
        //    else
        //    {
        //        //*** employee will end up placed into two job orders for the day (the else part)
        //        //transfer to the same location 
        //        //1. Add punch out for the candidate
        //        CandidateClockTime clockTime = new CandidateClockTime();

        //        //  if (previousJobOrder.CompanyLocationId == newJobOrder.CompanyLocationId)
        //        //  {
        //        clockTime.ClockDeviceUid = originalWorkTime.ClockDeviceUid;
        //        //}
        //        //else
        //        //{
        //        //    var clocks = _clockDeviceService.GetClockDevicesByCompanyLocationId(newJobOrder.CompanyLocationId);
        //        //    if (clocks != null && clocks.Count > 0)
        //        //        clockTime.ClockDeviceUid = clocks[0].ClockDeviceUid;
        //        //    else
        //        //        errors.Add("PunchOut", "There is no active clock device setup for the new location. Operation failed.");
        //        //}

        //        //if (errors.Count > 0)
        //        //    return errors;

        //        clockTime.SmartCardUid = originalWorkTime.SmartCardUid;
        //        clockTime.ClockInOut = newStartTime;
        //        clockTime.IsRescheduleClockTime = false;
        //        clockTime.Source = "Manual";


        //        var error = _clockTimeService.AdvancedInsert(clockTime);
        //        if (!String.IsNullOrWhiteSpace(error))
        //            errors.Add("PunchOut", String.Format("Cannot add the punch-out record for the current job order. Reason: ", error));
        //        else
        //        {
        //            //2. re-calculate work time for previous work time
        //            originalWorkTime.ClockOut = newStartTime;
        //            _workTimeService.CalculateAndSaveWorkTime(originalWorkTime, true);
        //            _workTimeService.SetClockTimeStatusByWorkTime(originalWorkTime, (int)CandidateClockTimeStatus.Processed, true);
        //        }


        //        if (!errors.Any())
        //        {
        //            //3. Place the candidate into the new job order only for today
        //            _candidateJobOrderService.InsertOrUpdateCandidateJobOrder(model.JobOrderId, originalWorkTime.CandidateId, newStartTime, (int)CandidateJobOrderStatusEnum.Placed, newStartTime, _workContext.CurrentAccount.Id);

        //            //4. add punch in for the candidate in new job order
        //            clockTime.IsRescheduleClockTime = false;
        //            clockTime.CandidateClockTimeStatusId = 0;
        //            var clocks = _clockDeviceService.GetClockDevicesByCompanyLocationId(newJobOrder.CompanyLocationId);

        //            if (clocks != null && clocks.Count > 0)
        //                clockTime.ClockDeviceUid = clocks[0].ClockDeviceUid;
        //            else
        //                errors.Add("PunchOut", "There is no active clock device setup for the new location. Operation failed.");

        //            if (errors.Count > 0)
        //                return errors;


        //            error = _clockTimeService.AdvancedInsert(clockTime);

        //            if (!String.IsNullOrWhiteSpace(error))
        //                errors.Add("PunchIn", String.Format("Cannot add the punch-in record for the new job order. Reason: ", error));
        //            else
        //            {
        //                calcNewWorkTime = true;
        //            }

        //        }
        //    }


        //    if (calcNewWorkTime && errors.Count == 0)
        //    {
        //        // base CandidateWorkTime, with common fields determined by job order and date
        //        var cwt = _workTimeService.PrepareCandidateWorkTimeByJobOrderAndDate(null /*account*/, newJobOrder, newStartTime);
        //        cwt.CandidateWorkTimeStatusId = (int)CandidateWorkTimeStatus.PendingApproval;
        //        cwt.EnteredBy = _workContext.CurrentAccount.Id;
        //        var candidateAsList = new List<int>();
        //        candidateAsList.Add(originalWorkTime.CandidateId);

        //        List<Wfm.Services.TimeSheet.WorkTimeService.ClockTimeToBeProcessed> punchRecords = _clockTimeService.GetAllCandidateClockTimesAsQueryable()
        //                                                                                           .Where(x => x.CandidateId == originalWorkTime.CandidateId &&
        //                                                                                                       x.CompanyLocationId == newJobOrder.CompanyLocationId &&
        //                                                                                                       x.ClockInOut == newStartTime &&
        //                                                                                                       x.IsDeleted == false &&
        //                                                                                                       x.CandidateClockTimeStatusId == (int)CandidateClockTimeStatus.NoStatus)
        //                                                                                            .Select(item => new Wfm.Services.TimeSheet.WorkTimeService.ClockTimeToBeProcessed()
        //                                                                                            {
        //                                                                                                SmartCardUid = item.SmartCardUid,
        //                                                                                                ClockInTime = item.ClockInOut,
        //                                                                                                // DateTime ClockOutTime 
        //                                                                                                ClockDeviceUid = item.ClockDeviceUid,
        //                                                                                                Source = item.Source
        //                                                                                            }).ToList();

        //        _workTimeService.CalculateWorkTimeForOneCandidate(originalWorkTime.CandidateId, cwt, punchRecords, newStartTime, newStartTime);
        //        // .CalculateWorkTimeForCandidates(candidateAsList, cwt);
        //    }

        //    return errors;
        //}


        public Dictionary<string, string> ManageExistingTimeSheet(ReschedulingModel model, int earlierStart = 0)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            var originalWorkTime = _workTimeService.GetWorkTimeById(model.WorkTimeId);

            if (model.EmployeeId != originalWorkTime.Candidate.EmployeeId || originalWorkTime.CompanyId != _workContext.CurrentAccount.CompanyId)
            {
                errors.Add("EmployeeId", String.Concat("Input Data is corrupted ", model.WorkTimeId));
                return errors;
            }

            var newStartTime = GetRealStartTime(originalWorkTime.JobStartDateTime, model.StartTime, out int offset);

            // for now, not allow to move to previous or next day
            // because this creates too many complications in worktime calculations and placements
            if (offset != 0)
            {
                errors.Add("StartTime", "The new start time must not cross midnight");
                return errors;
            }

            if (newStartTime < originalWorkTime.JobStartDateTime.AddMinutes(-earlierStart))
            {
                errors.Add("StartTime", String.Format("The new start time cannot be {0} mins earlier than the original", earlierStart));
                return errors;
            }

            if (newStartTime <= originalWorkTime.JobStartDateTime) 
            {
                // Since the start of the move is not later than the original start, we will change the assignment for the current day.
                // Otherwise, employee will end up placed into two job orders for the day (the else part)

                //*** void the existing worktime record and place the employee into another job order
                _workTimeService.ChangeCandidateWorkTimeStatus(originalWorkTime, CandidateWorkTimeStatus.Voided, _workContext.CurrentAccount);

                // Change the status of the punch-in record, so it is used by the task again
                var punchRecords = _clockTimeService.GetAllClockTimesByCandidateIdAndLocationIdAndDateTimeRange(model.CandidateId, originalWorkTime.CompanyLocationId, model.PunchIn, model.PunchIn);
                if (punchRecords != null)
                    _clockTimeService.UpdateClockTimeStatus(punchRecords, (int)CandidateClockTimeStatus.NoStatus);

                _logger.Information(String.Format("ManageExistingTimeSheet() - New scheduled start time matched existing punch record. Existing time sheet is voided. WorkTimeId={0}", model.WorkTimeId), account: _workContext.CurrentAccount);
            }
            else
            {
                //*** employee will end up placed into two job orders for the day (the else part)
             
                //1. Add punch out for the candidate
                // TODO: what if new start time later than the orig shift end???
                var error = this.createNewPunchEntry(newStartTime, originalWorkTime.ClockDeviceUid, originalWorkTime.SmartCardUid);

                if (!String.IsNullOrWhiteSpace(error))
                    errors.Add("PunchOut", String.Format("Cannot add the punch-out record for the current job order. Reason: ", error));
                else
                {
                    //2. re-calculate work time for previous work time
                    originalWorkTime.ClockOut = newStartTime;
                    _workTimeService.CalculateAndSaveWorkTime(originalWorkTime, true);
                    _workTimeService.SetClockTimeStatusByWorkTime(originalWorkTime, (int)CandidateClockTimeStatus.Processed, true);

                    _logger.Information(String.Format("ManageExistingTimeSheet() - Auto created a punch out record and recalculated the worktime for existing time sheet. WorkTimeId={0}", model.WorkTimeId), account: _workContext.CurrentAccount);
                }
              
            }

            return errors;
        }


        public DateTime GetRealStartTime(DateTime origStartTime, DateTime newStartTime, out int offset)
        {
            // TODO: better way to guess or decide???
            var toPrevDate = origStartTime.Hour < 12 && newStartTime.Hour >= 12;
            var toNextDate = origStartTime.Hour >= 12 && newStartTime.Hour < 12;

            // real date and start time
            offset = toPrevDate ? -1 : (toNextDate ? 1 : 0);
            var realDate = origStartTime.Date.AddDays(offset);

            return realDate.Date + newStartTime.TimeOfDay;
        }


        private string createNewPunchEntry(DateTime punchTime, string clockDeviceUid, string smartCardUid)
        {
            CandidateClockTime clockTime = new CandidateClockTime()
            {
                ClockDeviceUid = clockDeviceUid,
                SmartCardUid = smartCardUid,
                ClockInOut = punchTime,
                IsRescheduleClockTime = false,
                Source = "Manual",
                EnteredBy = _workContext.CurrentAccount.Id,
                Note = "Automatic punch entry created by system after supervisor rescheduled the work assignment"
            };

            return _clockTimeService.AdvancedInsert(clockTime);
        }


        public string createPunchInEntryForNewPlacement(ReschedulingModel model)
        {
            var result = string.Empty;

            var originalWorkTime = _workTimeService.GetWorkTimeById(model.WorkTimeId);
            var newStartTime = GetRealStartTime(originalWorkTime.JobStartDateTime, model.StartTime, out int offset);
            if (newStartTime > originalWorkTime.JobStartDateTime)   // only when new start later than the orig
            {
                var clocks = _clockDeviceService.GetClockDevicesByCompanyLocationId(model.LocationId);
                if (!clocks.Any())
                    return "There is no active clock device setup for the new location. Operation failed.";

                result = this.createNewPunchEntry(newStartTime, clocks[0].ClockDeviceUid, originalWorkTime.SmartCardUid);
            }

            return result;
        }


        public IList<CandidateWorkTimeModel> GetAllPlacementForToday(Account account)
        {
            var lstResult = Enumerable.Empty<CandidateWorkTimeModel>().ToList();

            if (account == null)
                return lstResult;

            var refDate = DateTime.Today;

            var placementQuery = _candidateJobOrderRepository.TableNoTracking
                                                    .Where(cjo => cjo.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed &&
                                                                  cjo.JobOrder.CompanyId == account.CompanyId &&
                                                                  cjo.StartDate <= refDate && (!cjo.EndDate.HasValue || cjo.EndDate >= refDate));

            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}

            // Jobs for Location Manager
            else if (account.IsCompanyLocationManager())
                placementQuery = placementQuery.Where(cjo => cjo.JobOrder.CompanyLocationId == account.CompanyLocationId); // search within location

            // Jobs for Department Supervisor
            else if (account.IsCompanyDepartmentSupervisor() || account.IsCompanyDepartmentManager())
                placementQuery = placementQuery.Where(cjo => cjo.JobOrder.CompanyLocationId == account.CompanyLocationId &&
                                                             cjo.JobOrder.CompanyDepartmentId == account.CompanyDepartmentId  // search within department
                                                     );
            else
                return new List<CandidateWorkTimeModel>(); // no role

            var minRange = refDate.Date;
            var maxRange = refDate.Date.AddDays(1);
            var worktimeQuery = _candidateWorkTimeRepository.TableNoTracking
                                    .Where(x => (x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.Processed ||
                                                 x.CandidateWorkTimeStatusId == (int)CandidateWorkTimeStatus.PendingApproval) &&
                                                 x.CompanyId == account.CompanyId &&
                                                 x.JobStartDateTime >= minRange && x.JobStartDateTime < maxRange &&
                                                 x.ClockOut == null);

            var result = from cjo in placementQuery
                         join tl in worktimeQuery on new { cjo.CandidateId, cjo.JobOrderId } equals new { tl.CandidateId, tl.JobOrderId } into group0
                         from g0 in group0.DefaultIfEmpty()
                         orderby cjo.JobOrder.StartTime
                         select new
                         {
                             PlacementId = cjo.Id,
                             WorkTimeId = g0 != null ? g0.Id : 0,

                             cjo.CandidateId,
                             cjo.Candidate.EmployeeId,
                             cjo.Candidate.CandidateGuid,
                             cjo.Candidate.FirstName,
                             cjo.Candidate.LastName,

                             cjo.JobOrderId,
                             cjo.JobOrder.JobTitle,
                             cjo.JobOrder.CompanyContactId,
                             cjo.JobOrder.CompanyLocationId,
                             cjo.JobOrder.CompanyDepartmentId,

                             CandidateWorkTimeStatusId = g0 != null ? g0.CandidateWorkTimeStatusId : 0,
                             JobStartDateTime = cjo.JobOrder.StartTime,
                             JobEndDateTime = cjo.JobOrder.EndTime,
                             ClockIn = g0 != null ? g0.ClockIn : null,
                             ClockOut = g0 != null ? g0.ClockOut : null,
                         };

            lstResult = result.Distinct().ToList().Select(item => new CandidateWorkTimeModel()
                    {
                        Id = item.WorkTimeId > 0 ? item.WorkTimeId : item.PlacementId,
                        CandidateId = item.CandidateId,
                        CandidateGuid = item.CandidateGuid,
                        CandidateWorkTimeStatusId = item.CandidateWorkTimeStatusId,
                        ClockIn = item.ClockIn,
                        ClockOut = item.ClockOut,
                        CompanyContactId = item.CompanyContactId,
                        CompanyDepartmentId = item.CompanyDepartmentId,
                        CompanyLocationId = item.CompanyLocationId,
                        EmployeeFirstName = item.FirstName,
                        EmployeeLastName = item.LastName,
                        EmployeeId = item.EmployeeId,
                        JobEndDateTime = refDate.Date.AddHours(item.JobEndDateTime.Hour).AddMinutes(item.JobEndDateTime.Minute),
                        JobOrderId = item.JobOrderId,
                        JobStartDateTime = refDate.Date.AddHours(item.JobStartDateTime.Hour).AddMinutes(item.JobStartDateTime.Minute),
                        JobTitle = item.JobTitle,
                    }).ToList();

            return lstResult;
        }

        public IQueryable<CandidateJobOrder> GetAllPlacementByDate(Account account, DateTime? refDate)
        {
            if (account == null)
                return null;

            if (refDate == null)
                refDate = DateTime.Today.AddDays(1);

            var query = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(account.CompanyId, refDate.Value);

            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}

            else if (account.IsCompanyLocationManager())
                query = query.Where(x => x.JobOrder.CompanyLocationId > 0 && x.JobOrder.CompanyLocationId == account.CompanyLocationId);

            else if (account.IsCompanyDepartmentManager())
                query = query.Where(x => x.JobOrder.CompanyLocationId > 0 && x.JobOrder.CompanyLocationId == account.CompanyLocationId &&
                                           x.JobOrder.CompanyDepartmentId > 0 && x.JobOrder.CompanyDepartmentId == account.CompanyDepartmentId);

            else if (account.IsCompanyDepartmentSupervisor())
                query = query.Where(x => x.JobOrder.CompanyLocationId > 0 && x.JobOrder.CompanyLocationId == account.CompanyLocationId &&
                                           x.JobOrder.CompanyDepartmentId > 0 && x.JobOrder.CompanyDepartmentId == account.CompanyDepartmentId &&
                                           x.JobOrder.CompanyContactId > 0 && x.JobOrder.CompanyContactId == account.Id);
            // no role?
            else
                query = Enumerable.Empty<CandidateJobOrder>().AsQueryable();

            return query;
        }


        public Dictionary<string, string> MoveToOtherJobOrder(ReschedulingModel model, int enteredBy)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            if (model.OrigId == 0)
            {
                errors.Add("Rescheduling: ", "Invalid placement record");
                return errors;
            }


            // update openings
            var origPlacement = _candidateJobOrderService.GetCandidateJobOrderById(model.OrigId);
            for (var day = model.StartDate; day <= model.EndDate; day = day.AddDays(1))
            {
                _jobOrderService.UpdateJobOrderOpeningForSelectedDate(origPlacement.JobOrderId, day, -1, "Rescheduled by supervisor");
                _jobOrderService.UpdateJobOrderOpeningForSelectedDate(model.JobOrderId, day, 1, "Rescheduled by supervisor");
            }


            var result = _candidateJobOrderService.CreateOrSavePlacements(new CandidateJobOrder()
            {
                Id = model.OrigId,
                CandidateId = model.CandidateId,
                JobOrderId = model.JobOrderId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CandidateJobOrderStatusId = (int)CandidateJobOrderStatusEnum.Placed,
                EnteredBy = enteredBy
            });

            if (!String.IsNullOrWhiteSpace(result))
            {
                // recover openings
                for (var day = model.StartDate; day <= model.EndDate; day = day.AddDays(1))
                {
                    _jobOrderService.UpdateJobOrderOpeningForSelectedDate(origPlacement.JobOrderId, day, 1, "Rescheduling by supervisor is rolledback");
                    _jobOrderService.UpdateJobOrderOpeningForSelectedDate(model.JobOrderId, day, -1, "Rescheduling by supervisor is rolledback");
                }
                errors.Add("Rescheduling: ", result);
            }
            else
            {
                // alert to employee and recruiter
                if (_workflowMessageService.SendCandidateReschedulingMessage(origPlacement.JobOrderId, model.JobOrderId, model.CandidateId, model.StartDate, model.EndDate) <= 0)
                    errors.Add("Rescheduling: ", "Employee's schedule is updated but system failed to send emails to employee and recruiter.");
                else
                    _logger.Information(String.Format("MoveToOtherJobOrder() - Automatically changed employee's placement. CandidateId={0} , Original jobOrderId={1} , New JobOrderId={2} , StartDate={3} , EndDate={4}", model.CandidateId, origPlacement.JobOrderId, model.JobOrderId, model.StartDate.ToShortDateString(), model.EndDate.ToShortDateString()), account: _workContext.CurrentAccount);
            }

            return errors;
        }


        public string FindMatchingJobOrder(ReschedulingModel model, out Core.Domain.JobOrders.JobOrder jobOrder)
        {
            jobOrder = null;

            if (model.EndDate < model.StartDate)
            {
                return "End Date is invalid.";
            }

            if (model.EndDate.Date.CompareTo(model.StartDate.Date) > 0)
            {
                // Employee cannot be placed in multiple job orders within the selected date range
                var test1 = _candidateJobOrderService.AnyOtherPlacementWithinDateRange(model.JobOrderId, model.CandidateId, model.StartDate, model.EndDate, model.JobOrderId);
                if (test1 != null)
                    return "Employee is placed in multiple job orders in the selected date range";
            }

            // Has anything changed?
            var originalJobOrder = _jobOrderService.GetJobOrderByGuid(model.JobOrderGuid);
            if (originalJobOrder == null)
            {
                _logger.Error(String.Format("FindMatchingJobOrder(): JobOrderGuid {0} is invalid. Data might be tampered.", model.JobOrderGuid), null, _workContext.CurrentAccount);
                return "Unexpected Error!";
            }

            if (originalJobOrder.CompanyLocationId == model.LocationId &&
                originalJobOrder.CompanyDepartmentId == model.DepartmentId &&
                originalJobOrder.PositionId == model.PositionId &&
                originalJobOrder.FranchiseId == model.FranchiseId &&
                originalJobOrder.StartTime.TimeOfDay == model.StartTime.TimeOfDay &&
                originalJobOrder.EndTime.TimeOfDay == model.EndTime.TimeOfDay)
            {
                // nothing has changed
                jobOrder = originalJobOrder;
                return null;
            }


            // Something has changed, find a job order that matches with the new schedule
            var matchedJobOrders = _jobOrderService.GetAllJobOrdersByCompanyIdAsQueryable(_workContext.CurrentAccount.CompanyId)
                                                   .Where(x => x.CompanyLocationId == model.LocationId
                                                            && x.CompanyDepartmentId == model.DepartmentId
                                                            && x.PositionId == model.PositionId
                                                            && x.FranchiseId == model.FranchiseId
                                                            && (x.EndDate == null || x.EndDate > model.StartDate)
                                                            && (x.EndDate == null || x.EndDate >= model.EndDate)
                                                         ).ToList();


            if (matchedJobOrders.Count > 0)
            {
                // Now filter by time
                matchedJobOrders = matchedJobOrders.Where(x => x.StartTime.TimeOfDay == model.StartTime.TimeOfDay && x.EndTime.TimeOfDay == model.EndTime.TimeOfDay).ToList();
                if (matchedJobOrders.Count == 0)
                    return null;

                /// From here, all matching parameters are optional

                // First check if any of these job orders have the same pay rate
                var _tmp = matchedJobOrders.Where(x => x.BillingRateCode == originalJobOrder.BillingRateCode).FirstOrDefault();
                if (_tmp != null)
                    jobOrder = _tmp;
                else
                    jobOrder = matchedJobOrders[0]; // a matching job order is found
            }

            // No matching job order is found
            return null;
        }

        public Shift FindMatchingShift(int companyId, DateTime startTime, DateTime endTime)
        {
            var shift = _shiftService.GetAllShiftsAsQueryable().Where(x => x.CompanyId == companyId)
                                                               .ToList().Where(x => x.MinStartTime.Value <= startTime.TimeOfDay &&
                                                                                x.MaxEndTime.Value >= endTime.TimeOfDay);

            return shift.FirstOrDefault();
        }

        public decimal GetPayRateByShift(int shiftId, int positionId, int locationId, int franchiseId, DateTime refDate)
        {
            var _shift = _shiftService.GetShiftById(shiftId);
            if (_shift == null)
            {
                _logger.Error(String.Format("GetPayRateByShift(): ShiftId {0} is invalid. Data might be tampered or configuration is incorrect.", shiftId), null, _workContext.CurrentAccount);
                return 0;
            }

            decimal _payRate = 0;
            string _shiftCode = _shift.ShiftName;

            var cmpBillingRates = _companyBillingService.GetAllCompanyBillingRatesByCompanyIdAndRefDate(_workContext.CurrentAccount.CompanyId, refDate);
            cmpBillingRates = cmpBillingRates.Where(x => x.FranchiseId == franchiseId && x.PositionId == positionId && x.CompanyLocationId == locationId && x.ShiftCode == _shiftCode).ToList();

            if (cmpBillingRates.Count > 0)
                _payRate = cmpBillingRates[0].RegularPayRate;

            return _payRate;
        }

        public string GetNewRateCode(int positionId, int franchiseId, int shiftId)
        {
            var _shift = _shiftService.GetShiftById(shiftId);
            if (_shift == null)
            {
                _logger.Error(String.Format("GetNewRateCode(): ShiftId {0} is invalid. Data might be tampered or configuration is incorrect.", shiftId), null, _workContext.CurrentAccount);
                return null;
            }

            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("PositionId", positionId);
            paras[1] = new SqlParameter("franchiseId", franchiseId);
            paras[2] = new SqlParameter("shiftCode", _shift.ShiftName);

            return _dbContext.SqlQuery<string>("SELECT [dbo].[UDF_GetRateCode](@PositionId, @ShiftCode, @FranchiseId)", paras).FirstOrDefault();
        }

        public Wfm.Core.Domain.JobOrders.JobOrder CreateMatchingJobOrder(int OriginalJobOrderId, ReschedulingModel model)
        {
            var originalJO = _jobOrderService.GetJobOrderById(OriginalJobOrderId);
            if (originalJO == null)
            {
                _logger.Error(String.Format("CreateMatchingJobOrder(): OriginalJobOrderId {0} is invalid. Data might be tampered or configuration is incorrect.", OriginalJobOrderId), null, _workContext.CurrentAccount);
                return null;
            }

            var result = new Wfm.Core.Domain.JobOrders.JobOrder();

            result.CompanyId = _workContext.CurrentAccount.CompanyId;
            result.CompanyLocationId = model.LocationId;
            result.CompanyDepartmentId = model.DepartmentId;
            result.CompanyContactId = model.CompanyContactId;
            result.PositionId = model.PositionId;
            
            var position = _positionService.GetPositionById(model.PositionId);
            string positionName = position == null? "Invalid position": position.Name;

            result.JobTitle = String.Concat("Auto created-", positionName , " (", model.StartTime.ToShortTimeString(), " - ", model.EndTime.ToShortTimeString(), ")");
            result.JobDescription = String.Format("Auto created Based on job order {0} - {1}", originalJO.Id, originalJO.JobTitle);
            result.Note = String.Format("This job order is created by system based on supervisor's changes in the employee's schedule. The original job order, which is used as template for this job order, is {0}", originalJO.Id);

            result.StartDate = model.StartDate;
            result.EndDate = model.EndDate.AddHours(model.EndTime.Hour).AddMinutes(model.EndTime.Minute);

            result.StartTime = model.StartTime;
            result.EndTime = model.EndTime;
            result.ShiftId = model.ShiftId;

            // Properties that are set based on the original job order
            result.SchedulePolicyId = originalJO.SchedulePolicyId;
            result.JobOrderTypeId = originalJO.JobOrderTypeId;
            result.JobOrderStatusId = originalJO.JobOrderStatusId;
            result.JobOrderCategoryId = originalJO.JobOrderCategoryId;
            result.AllowSuperVisorModifyWorkTime = originalJO.AllowSuperVisorModifyWorkTime;
            result.AllowTimeEntry = originalJO.AllowTimeEntry;
            result.IncludeHolidays = originalJO.IncludeHolidays;
            result.OwnerId = originalJO.OwnerId;
            result.RecruiterId = originalJO.RecruiterId;
            result.LabourType = originalJO.LabourType;
            result.HoursPerWeek = originalJO.HoursPerWeek;

            result.EnteredBy = _workContext.CurrentAccount.Id;
            result.FranchiseId = originalJO.FranchiseId;

            result.BillingRateCode = this.GetNewRateCode(model.PositionId, originalJO.FranchiseId, model.ShiftId);
            if (result.BillingRateCode == null)
            {
                _logger.Error("CreateMatchingJobOrder(): Cannot create the billing rate code.", null, _workContext.CurrentAccount);
                return null;
            }

            // If the work is scheduled for less than one week, every scheduled day will be marked as a working day, otherwise, use the original job order's working day flags
            if (model.StartDate.AddDays(7) > model.EndDate)
            {
                for (var day = result.StartDate; day <= result.EndDate.Value; day = day.AddDays(1))
                {
                    var _dayOfWeek = day.DayOfWeek;

                    switch (_dayOfWeek)
                    {
                        case DayOfWeek.Saturday: result.SaturdaySwitch = true; break;
                        case DayOfWeek.Sunday: result.SundaySwitch = true; break;
                        case DayOfWeek.Monday: result.MondaySwitch = true; break;
                        case DayOfWeek.Tuesday: result.TuesdaySwitch = true; break;
                        case DayOfWeek.Wednesday: result.WednesdaySwitch = true; break;
                        case DayOfWeek.Thursday: result.ThursdaySwitch = true; break;
                        case DayOfWeek.Friday: result.FridaySwitch = true; break;
                    }
                }
            }
            else
            {
                result.SaturdaySwitch = originalJO.SaturdaySwitch;
                result.SundaySwitch = originalJO.SundaySwitch;
                result.MondaySwitch = originalJO.MondaySwitch;
                result.TuesdaySwitch = originalJO.TuesdaySwitch;
                result.WednesdaySwitch = originalJO.WednesdaySwitch;
                result.ThursdaySwitch = originalJO.ThursdaySwitch;
                result.FridaySwitch = originalJO.FridaySwitch;
            }

            return result;
        }

    }
}