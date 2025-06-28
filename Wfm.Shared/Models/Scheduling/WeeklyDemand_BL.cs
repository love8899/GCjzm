using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.JobOrders;
using Wfm.Core.Domain.Scheduling;
using Wfm.Services.Companies;
using Wfm.Services.Scheduling;


namespace Wfm.Shared.Models.Scheduling
{
    public class WeeklyDemand_BL
    {
        #region Fields

        private readonly ICompanyService _companyService;
        private readonly IRepository<ShiftSchedule> _shiftScheduleRepository;
        private readonly IRepository<ShiftScheduleDailyDemandAdjustment> _shiftScheduleDailyDemandAdjustmentRepository;
        private readonly IRepository<CompanyShiftJobRole> _companyShiftJobRoleRepository;
        private readonly IRepository<ScheduleJobOrder> _scheduleJobOrderRepository;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly ISchedulingDemandService _schedulingDemandService;

        #endregion


        #region Ctor

        public WeeklyDemand_BL(
            ICompanyService companyService,
            IRepository<ShiftSchedule> shiftScheduleRepository,
            IRepository<ShiftScheduleDailyDemandAdjustment> shiftScheduleDailyDemandAdjustmentRepository,
            IRepository<CompanyShiftJobRole> companyShiftJobRoleRepository,
            IRepository<ScheduleJobOrder> scheduleJobOrderRepository,
            IRepository<JobOrder> jobOrderRepository,
            ISchedulingDemandService schedulingDemandService)
        {
            _companyService = companyService;
            _shiftScheduleRepository = shiftScheduleRepository;
            _shiftScheduleDailyDemandAdjustmentRepository = shiftScheduleDailyDemandAdjustmentRepository;
            _companyShiftJobRoleRepository = companyShiftJobRoleRepository;
            _scheduleJobOrderRepository = scheduleJobOrderRepository;
            _jobOrderRepository = jobOrderRepository;
            _schedulingDemandService = schedulingDemandService;
        }

        #endregion


        #region Weekly Deamnd List

        public List<WeeklyDemandModel> GetWeeklyDemandBySchedulePperiod(int locationId, DateTime refDate)
        {
            var demands = new List<WeeklyDemandModel>();
            var period = _schedulingDemandService.GetSchedulePeriodByLocationAndDate(locationId, refDate);
            if (period != null)
            {
                var weekStart = refDate.AddDays(DayOfWeek.Sunday - refDate.DayOfWeek);
                foreach (var schedule in period.ShiftSchedules)
                {
                    foreach (var shiftJobRole in schedule.CompanyShift.CompanyShiftJobRoles)
                    {
                        var dailyAdjustments = _GetDailyCounts(period.Id, shiftJobRole, weekStart);
                        var demand = new WeeklyDemandModel()
                        {
                            // CompanyShiftJobRole is unique
                            Id = shiftJobRole.Id,
                            SchedulePeriodId = period.Id,
                            CompanyDepartmentId = shiftJobRole.CompanyShift.CompanyDepartmentId,
                            Department = shiftJobRole.CompanyShift.CompanyDepartment.DepartmentName,
                            CompanyJobRoleId = shiftJobRole.CompanyJobRoleId,
                            JobRole = shiftJobRole.CompanyJobRole.Name,
                            ShiftId = shiftJobRole.CompanyShift.ShiftId,
                            Shift = shiftJobRole.CompanyShift.Shift.ShiftName,
                            StartTime = DateTime.MinValue + schedule.StartTimeOfDay,
                            LengthInHours = schedule.LengthInHours,
                            SupervisorId = shiftJobRole.SupervisorId.GetValueOrDefault(),
                            Sunday = schedule.SundaySwitch ? dailyAdjustments[0].MandatoryCount : -1,
                            Monday = schedule.MondaySwitch ? dailyAdjustments[1].MandatoryCount : -1,
                            Tuesday = schedule.TuesdaySwitch ? dailyAdjustments[2].MandatoryCount : -1,
                            Wednesday = schedule.WednesdaySwitch ? dailyAdjustments[3].MandatoryCount : -1,
                            Thursday = schedule.ThursdaySwitch ? dailyAdjustments[4].MandatoryCount : -1,
                            Friday = schedule.FridaySwitch ? dailyAdjustments[5].MandatoryCount : -1,
                            Saturday = schedule.SaturdaySwitch ? dailyAdjustments[6].MandatoryCount : -1,
                        };
                        demands.Add(demand);
                    }
                }
            }

            return demands;
        }


        private DailyDemandCounts[] _GetDailyCounts(int schedulePeriodId, CompanyShiftJobRole shiftJobRole, DateTime weekStart)
        {
            var dates = Enumerable.Range(0, 7).Select(x => weekStart.AddDays(x));
            var schedules = _schedulingDemandService.GetDailyShiftSchedule(schedulePeriodId).Where(x => x.CompanyShiftId == shiftJobRole.CompanyShiftId);
            var daily = from d in dates
                        from s in schedules.Where(s => s.ScheduelDate == d).DefaultIfEmpty()
                        select new
                        {
                            ScheduleDate = d,
                            ScheduleId = s != null ? s.Id : 0,
                            MandatoryCount = s != null ? shiftJobRole.MandantoryRequiredCount : -1,
                            ContingencyCount = s != null ? shiftJobRole.ContingencyRequiredCount : -1
                        };
            
            var adjustments = _shiftScheduleDailyDemandAdjustmentRepository.Table;
            var result = from d in daily
                         from a in adjustments.Where(a => a.ShiftScheduleDailyId == d.ScheduleId && a.CompanyJobRoleId == shiftJobRole.CompanyJobRoleId).DefaultIfEmpty()
                         select new DailyDemandCounts()
                         {
                             ScheduleDate = d.ScheduleDate,
                             MandatoryCount = a != null ? a.AdjustedMandantoryRequiredCount : d.MandatoryCount,
                             ContingencyCount = a != null ? a.AdjustedContingencyRequiredCount : d.ContingencyCount
                         };

            return result.ToArray();
        }


        public class DailyDemandCounts
        {
            public DateTime ScheduleDate { get; set; }
            public int MandatoryCount { get; set; }
            public int ContingencyCount { get; set; }
        }

        #endregion


        #region Edit Weekly Demand

        public void UpdateWeeklyDemand(WeeklyDemandModel model, DateTime refDate)
        {
            var weekStart = refDate.AddDays(DayOfWeek.Sunday - refDate.DayOfWeek);
            var shiftJobRole = _companyShiftJobRoleRepository.Table.Where(x => x.Id == model.Id).FirstOrDefault();
            if (shiftJobRole != null)
            {
                _UpdateShiftSchedule(shiftJobRole.CompanyShiftId, model, weekStart);
                _UpdateShiftJobRoleSupervisor(shiftJobRole, model.SupervisorId);
            }
        }


        private void _UpdateShiftJobRoleSupervisor(CompanyShiftJobRole shiftJobRole, int supervisorId)
        {
            shiftJobRole.SupervisorId = supervisorId;
            shiftJobRole.UpdatedOnUtc = DateTime.UtcNow;
            _companyShiftJobRoleRepository.Update(shiftJobRole);
        }


        private void _UpdateShiftSchedule(int companyShiftId, WeeklyDemandModel model, DateTime weekStart)
        {
            var shiftSchedule = _shiftScheduleRepository.Table.Where(x => x.CompanyShiftId == companyShiftId).FirstOrDefault();
            if (shiftSchedule != null)
            {
                var schedulePeriod = _schedulingDemandService.GetSchedulePeriodById(model.SchedulePeriodId);
                _UpdateScheduleDailyForWholeWeek(model, shiftSchedule, weekStart);
            }
        }


        private void _UpdateScheduleDailyForWholeWeek(WeeklyDemandModel model, ShiftSchedule shiftSchedule, DateTime weekStart)
        {
            var dailyList = _schedulingDemandService.GetDailyShiftSchedule(model.SchedulePeriodId)
                            .Where(x => x.CompanyShiftId == shiftSchedule.CompanyShiftId).ToList();

            var startDate = weekStart;
            if (model.Sunday >= 0)
                _InsertOrUpdateDailyDemandAdjustment(shiftSchedule, dailyList, startDate, model.CompanyJobRoleId, model.Sunday, 0);

            startDate = startDate.AddDays(1);
            if (model.Monday >= 0)
                _InsertOrUpdateDailyDemandAdjustment(shiftSchedule, dailyList, startDate, model.CompanyJobRoleId, model.Monday, 0);

            startDate = startDate.AddDays(1);
            if (model.Tuesday >= 0)
                _InsertOrUpdateDailyDemandAdjustment(shiftSchedule, dailyList, startDate, model.CompanyJobRoleId, model.Tuesday, 0);

            startDate = startDate.AddDays(1);
            if (model.Wednesday >= 0)
                _InsertOrUpdateDailyDemandAdjustment(shiftSchedule, dailyList, startDate, model.CompanyJobRoleId, model.Wednesday, 0);

            startDate = startDate.AddDays(1);
            if (model.Thursday >= 0)
                _InsertOrUpdateDailyDemandAdjustment(shiftSchedule, dailyList, startDate, model.CompanyJobRoleId, model.Thursday, 0);

            startDate = startDate.AddDays(1);
            if (model.Friday >= 0)
                _InsertOrUpdateDailyDemandAdjustment(shiftSchedule, dailyList, startDate, model.CompanyJobRoleId, model.Friday, 0);

            startDate = startDate.AddDays(1);
            if (model.Saturday >= 0)
                _InsertOrUpdateDailyDemandAdjustment(shiftSchedule, dailyList, startDate, model.CompanyJobRoleId, model.Saturday, 0);
        }


        private void _InsertOrUpdateDailyDemandAdjustment(ShiftSchedule shiftSchedule, List<ShiftScheduleDaily> dailyList, DateTime scheduleDate, int jobRoleId, int count1, int count2)
        {
            var shiftJobRole = _companyShiftJobRoleRepository.Table
                               .Where(x => x.CompanyShiftId == shiftSchedule.CompanyShiftId && x.CompanyJobRoleId == jobRoleId).FirstOrDefault();
            var shiftScheduleDaily = dailyList.Where(x => x.ScheduelDate == scheduleDate).FirstOrDefault();
            if (shiftJobRole != null && shiftScheduleDaily != null)
            {
                var adjustment = _shiftScheduleDailyDemandAdjustmentRepository.Table
                                 .Where(x => x.ShiftScheduleDailyId == shiftScheduleDaily.Id && x.CompanyJobRoleId == jobRoleId).FirstOrDefault();
                if (adjustment != null && (count1 != shiftJobRole.MandantoryRequiredCount))
                {
                    adjustment.AdjustedMandantoryRequiredCount = count1;
                    adjustment.AdjustedContingencyRequiredCount = count2;
                    adjustment.UpdatedOnUtc = DateTime.UtcNow;
                    _shiftScheduleDailyDemandAdjustmentRepository.Update(adjustment);
                }
                // new adjustment
                else
                {
                    if (count1 != shiftJobRole.MandantoryRequiredCount)
                        _AddDailyDemandAdjustment(shiftScheduleDaily.Id, shiftJobRole.CompanyJobRoleId, count1, count2);
                }
            }
        }


        private void _AddDailyDemandAdjustment(int shiftScheduleDailyId, int jobRoleId, int count1, int count2)
        {
            var adjustment = new ShiftScheduleDailyDemandAdjustment()
            {
                ShiftScheduleDailyId = shiftScheduleDailyId,
                CompanyJobRoleId = jobRoleId,
                AdjustedMandantoryRequiredCount = count1,
                AdjustedContingencyRequiredCount = count2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            _shiftScheduleDailyDemandAdjustmentRepository.Insert(adjustment);
        }

        #endregion
    }
}
