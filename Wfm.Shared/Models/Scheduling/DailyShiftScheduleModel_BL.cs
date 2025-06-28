using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Services.Scheduling;
using Kendo.Mvc.Extensions;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Scheduling;


namespace Wfm.Shared.Models.Scheduling
{
    public class DailyShiftScheduleModel_BL
    {
        #region Field
        private readonly ISchedulingDemandService _scheduleDemandService;
        private readonly IRepository<CompanyShiftJobRole> _companyShiftJobRoleRepository;
        private readonly IRepository<ShiftScheduleDailyDemandAdjustment> _shiftScheduleDailyDemandAdjustmentRepository;
        #endregion

        #region Ctor
        public DailyShiftScheduleModel_BL(ISchedulingDemandService scheduleDemandService,
                                          IRepository<CompanyShiftJobRole> companyShiftJobRoleRepository,
                                          IRepository<ShiftScheduleDailyDemandAdjustment> shiftScheduleDailyDemandAdjustmentRepository)
        {
            _scheduleDemandService = scheduleDemandService;
            _companyShiftJobRoleRepository = companyShiftJobRoleRepository;
            _shiftScheduleDailyDemandAdjustmentRepository = shiftScheduleDailyDemandAdjustmentRepository;
        }
        #endregion
        public DataSourceResult GetDailyShiftScheduleModel(DataSourceRequest request, int schedulePeriodId)
        {
            var dailyShiftSchedule = _scheduleDemandService.GetDailyShiftSchedule(schedulePeriodId);
            var dailyShiftScheduleModels = new List<DailyShiftScheduleModel>();
            foreach (var d in dailyShiftSchedule)
            {
                var model = new DailyShiftScheduleModel
                {
                    Id = d.Id,
                    CompanyShiftId = d.CompanyShiftId,
                    Description = d.CompanyShift.Shift.ShiftName,
                    End = d.ScheduelDate.AddTicks(d.StartTimeOfDayTicks).AddHours(Convert.ToDouble(d.LengthInHours)),
                    IsAllDay = false,
                    SchedulePeriodId = d.SchedulePeriodId,
                    Start = d.ScheduelDate.AddTicks(d.StartTimeOfDayTicks),
                    Title = d.CompanyShift.Shift.ShiftName,
                    CreatedOnUtc = d.CreatedOnUtc,
                    UpdatedOnUtc = d.UpdatedOnUtc,
                    OpeningDictionary = d.CompanyShift.CompanyShiftJobRoles
                        .Select(x => new { CompanyJobRoleId = x.CompanyJobRoleId, MandantoryRequiredCount = x.MandantoryRequiredCount, ContingencyRequiredCount = x.ContingencyRequiredCount })
                        .ToDictionary(y => y.CompanyJobRoleId.ToString(), y => new DailyCount
                        {
                            MandantoryRequiredCount = y.MandantoryRequiredCount, 
                            ContingencyRequiredCount = y.ContingencyRequiredCount
                        }),
                };
                d.Adjustments.Where(adj => model.OpeningDictionary.ContainsKey(adj.CompanyJobRoleId.ToString())).ToList().ForEach(adj =>
                {
                    model.OpeningDictionary[adj.CompanyJobRoleId.ToString()] = new DailyCount
                    {
                        MandantoryRequiredCount = adj.AdjustedMandantoryRequiredCount,
                        ContingencyRequiredCount = adj.AdjustedContingencyRequiredCount
                    };
                });
                dailyShiftScheduleModels.Add(model);
            }
            return dailyShiftScheduleModels.ToDataSourceResult(request);
        }

        public void CreateOrUpdateDailyShiftSchedule(DailyShiftScheduleModel model, out string errors)
        {
            errors = string.Empty;
            try
            {
                var entity = model.ToEntity();

                if (entity.Id == 0)
                    _scheduleDemandService.InsertDailyShiftSchedule(entity);
                else
                    _scheduleDemandService.UpdateDailyShiftSchedule(entity);

                // insert or update adjustment
                foreach (var jobRoleId in model.OpeningDictionary.Keys)
                    _InsertOrUpdateDailyDemandAdjustment(entity, Convert.ToInt32(jobRoleId), model.OpeningDictionary[jobRoleId]);
            }
            catch (Exception ex)
            {
                errors = "CreateOrUpdateDailyShiftSchedule():" + ex.ToString();
            }
        }


        private void _InsertOrUpdateDailyDemandAdjustment(ShiftScheduleDaily shiftScheduleDaily, int jobRoleId, DailyCount dailyCount)
        {
            var shiftJobRole = _companyShiftJobRoleRepository.Table.Where(x => x.CompanyShiftId == shiftScheduleDaily.CompanyShiftId)
                               .Where(x => x.CompanyJobRoleId == jobRoleId).FirstOrDefault();
            if (shiftScheduleDaily != null && shiftJobRole != null)
            {
                if (dailyCount.MandantoryRequiredCount != shiftJobRole.MandantoryRequiredCount || 
                     dailyCount.ContingencyRequiredCount != shiftJobRole.ContingencyRequiredCount)
                {
                    var adjustment = _shiftScheduleDailyDemandAdjustmentRepository.Table.Where(x => x.ShiftScheduleDailyId == shiftScheduleDaily.Id)
                                     .Where(x => x.CompanyJobRoleId == jobRoleId).FirstOrDefault();
                    if (adjustment != null)
                    {
                        adjustment.AdjustedMandantoryRequiredCount = dailyCount.MandantoryRequiredCount;
                        adjustment.AdjustedContingencyRequiredCount = dailyCount.ContingencyRequiredCount;
                        adjustment.UpdatedOnUtc = DateTime.UtcNow;
                        _shiftScheduleDailyDemandAdjustmentRepository.Update(adjustment);
                    }
                    // new adjustment
                    else
                    {
                        _AddDailyDemandAdjustment(shiftScheduleDaily.Id, shiftJobRole.CompanyJobRoleId, dailyCount);
                    }
                }
            }
        }


        private void _AddDailyDemandAdjustment(int shiftScheduleDailyId, int jobRoleId, DailyCount dailyCount)
        {
            var adjustment = new ShiftScheduleDailyDemandAdjustment()
            {
                ShiftScheduleDailyId = shiftScheduleDailyId,
                CompanyJobRoleId = jobRoleId,
                AdjustedMandantoryRequiredCount = dailyCount.MandantoryRequiredCount,
                AdjustedContingencyRequiredCount = dailyCount.ContingencyRequiredCount,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            _shiftScheduleDailyDemandAdjustmentRepository.Insert(adjustment);
        }
    }
}
