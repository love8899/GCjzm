using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Wfm.Core.Domain.Employees;
using Wfm.Shared.Models.Employees;
using Wfm.Core.Domain.Candidates;
using Wfm.Shared.Models.Incident;
using Wfm.Core.Domain.Incident;
using Wfm.Shared.Models.Companies;
using Wfm.Core.Domain.Companies;
using Wfm.Shared.Models.Scheduling;
using Wfm.Core.Domain.Scheduling;
using Wfm.Core.Domain.Accounts;
using Wfm.Shared.Models.Common;
using Wfm.Core.Domain.Common;

namespace Wfm.Shared.Mapping
{
    public static class DefaultMappingRule
    {
        public static void Create()
        {
            Mapper.CreateMap<DateTime?, TimeSpan>()
                .ConstructUsing(dt => { return dt.GetValueOrDefault().TimeOfDay; });
            Mapper.CreateMap<DateTime, TimeSpan>()
                .ConstructUsing(dt => { return dt.TimeOfDay; });
            Mapper.CreateMap<TimeSpan, DateTime>()
                .ConstructUsing(tsp => { return DateTime.Today.Add(tsp); });
            Mapper.CreateMap<TimeSpan, DateTime?>()
                .ConstructUsing(tsp => { return DateTime.Today.Add(tsp); });

            // TODO
            //Mapper.CreateMap<CandidateJobOrder, CandidateJobOrderModel>();
            //Mapper.CreateMap<CandidateJobOrderModel, CandidateJobOrder>();

            //Mapper.CreateMap<CandidateKeySkill, CandidateKeySkillModel>();
            //Mapper.CreateMap<CandidateKeySkillModel, CandidateKeySkill>();

            //Mapper.CreateMap<CandidateWorkHistory, CandidateWorkHistoryModel>();
            //Mapper.CreateMap<CandidateWorkHistoryModel, CandidateWorkHistory>();

            //Mapper.CreateMap<CandidateJobOrderStatus, CandidateJobOrderStatusModel>();
            //Mapper.CreateMap<CandidateJobOrderStatusModel, CandidateJobOrderStatus>();

            //Mapper.CreateMap<CandidateAddress, CandidateAddressModel>();
            //Mapper.CreateMap<CandidateAddressModel, CandidateAddress>();

            //Mapper.CreateMap<CandidateJobOrderStatusHistory, CandidateJobOrderStatusHistoryModel>();
            //Mapper.CreateMap<CandidateJobOrderStatusHistoryModel, CandidateJobOrderStatusHistory>();

            Mapper.CreateMap<Salutation, SalutationModel>();
            Mapper.CreateMap<SalutationModel, Salutation>();

            Mapper.CreateMap<VetranType, VetranTypeModel>();
            Mapper.CreateMap<VetranTypeModel, VetranType>();

            //Mapper.CreateMap<EthnicType, EthnicTypeModel>();
            //Mapper.CreateMap<EthnicTypeModel, EthnicType>();

            Mapper.CreateMap<Gender, GenderModel>();
            Mapper.CreateMap<GenderModel, Gender>();


            //Mapper.CreateMap<Language, LanguageModel>();
            //Mapper.CreateMap<LanguageModel, Language>();

            //Mapper.CreateMap<LocaleStringResource, LocaleStringResourceModel>();
            //Mapper.CreateMap<LocaleStringResourceModel, LocaleStringResource>();

            Mapper.CreateMap<Transportation, TransportationModel>();
            Mapper.CreateMap<TransportationModel, Transportation>();

            Mapper.CreateMap<Source, SourceModel>();
            Mapper.CreateMap<SourceModel, Source>();

            //
            Mapper.CreateMap<IncidentReport, IncidentReportModel>()
                .ForMember(m => m.CompanyName, opt => opt.MapFrom(e => e.Company.CompanyName))
                .ForMember(m => m.CandidateFirstName, opt => opt.MapFrom(e => e.Candidate.FirstName))
                .ForMember(m => m.CandidateLastName, opt => opt.MapFrom(e => e.Candidate.LastName))
                .ForMember(m => m.Position, opt => opt.MapFrom(e => e.JobOrder != null ? e.JobOrder.JobTitle : string.Empty))
                .ForMember(m => m.IncidentCategoryCode, opt => opt.MapFrom(e => e.IncidentCategory.IncidentCategoryCode))
                .ForMember(m => m.IncidentCategoryName, opt => opt.MapFrom(e => e.IncidentCategory.Description))
                .ForMember(m => m.LocationName, opt => opt.MapFrom(e => e.Location != null ? e.Location.LocationName : string.Empty))
                .ForMember(m => m.ReportedByUserName, opt => opt.MapFrom(e => e.ReportedByAccount.FirstName + ' ' + e.ReportedByAccount.LastName));
            Mapper.CreateMap<IncidentReportModel, IncidentReport>();
            Mapper.CreateMap<IncidentReportFile, IncidentReportFileModel>();
            Mapper.CreateMap<IncidentReportFileModel, IncidentReportFile>();

            Mapper.CreateMap<Employee, EmployeeGridModel>()
                .ForMember(m => m.PreferredWorkLocation, opt => opt.MapFrom(e => e.CompanyLocation.LocationName))
                .ForMember(m => m.JobTitle, opt => opt.MapFrom(x => x.EmployeeJobRoles != null && x.EmployeeJobRoles.Any(y => y.IsPrimary) ? x.EmployeeJobRoles.First(y => y.IsPrimary).CompanyJobRole.Name : string.Empty));

            Mapper.CreateMap<Employee, EmployeeModel>()
                .ForMember(m => m.PreferredWorkLocation, opt => opt.MapFrom(e => e.CompanyLocation.LocationName))
                .ForMember(m => m.SalutationName, opt => opt.MapFrom(e => e.Salutation.SalutationName))
                .ForMember(m => m.GenderName, opt => opt.MapFrom(e => e.Gender.GenderName))
                .ForMember(m => m.EthnicTypeName, opt => opt.MapFrom(e => e.EthnicType.EthnicTypeName))
                .ForMember(m => m.EnteredByUserName, opt => opt.MapFrom(e => e.EnteredByAccount.Username))
                .ForMember(m => m.VetranTypeName, opt => opt.MapFrom(e => e.VetranType.VetranTypeName))
                .ForMember(m => m.HireDate, opt => opt.MapFrom(e => e.EmployeePayrollSettings.OrderByDescending(x => x.LastHireDate)
                    .Select(x => x.LastHireDate).FirstOrDefault()))
                .ForMember(m => m.TerminationDate, opt => opt.MapFrom(e => e.EmployeePayrollSettings.OrderByDescending(x => x.LastHireDate)
                    .Select(x => x.TerminationDate).FirstOrDefault()))
                .ForMember(m => m.JobTitle, opt => opt.MapFrom(x => x.EmployeeJobRoles != null && x.EmployeeJobRoles.Any(y => y.IsPrimary) ? x.EmployeeJobRoles.First(y => y.IsPrimary).CompanyJobRole.Name : string.Empty))
                .ForMember(m => m.PrimaryJobRoleId, opt => opt.MapFrom(x => x.EmployeeJobRoles != null && x.EmployeeJobRoles.Any(y => y.IsPrimary) ? new int?(x.EmployeeJobRoles.First(y => y.IsPrimary).CompanyJobRole.Id) : null));
            Mapper.CreateMap<EmployeeTimeoffBalance, EmployeeTimeoffBalanceModel>()
                .ForMember(m => m.EmployeeTimeoffTypeName, opt => opt.MapFrom(e => e.EmployeeTimeoffType.Name))
                .ForMember(m => m.AllowNegative, opt => opt.MapFrom(e => e.EmployeeTimeoffType.AllowNegative))
                .ForMember(m => m.EmployeeName, opt => opt.MapFrom(e => e.Employee.FirstName + ' ' + e.Employee.LastName));
            Mapper.CreateMap<EmployeeTimeoffBalanceModel, EmployeeTimeoffBalance>();
            Mapper.CreateMap<EmployeeTimeoffBooking, EmployeeTimeoffBookingModel>()
                .ForMember(m => m.EmployeeName, opt => opt.MapFrom(e => e.Employee.FirstName + ' ' + e.Employee.LastName))
                .ForMember(m => m.EmployeeIntId, opt => opt.MapFrom(e => e.Employee.Id))
                .ForMember(m => m.StartTime, opt => opt.MapFrom(e => e.TimeOffStartDateTime.TimeOfDay))
                .ForMember(m => m.EndTime, opt => opt.MapFrom(e => e.TimeOffEndDateTime.TimeOfDay));
            Mapper.CreateMap<EmployeeTimeoffBookingModel, EmployeeTimeoffBooking>()
                .ForMember(e => e.Year, opt => opt.MapFrom(m => m.TimeOffStartDateTime.Year))
                .ForMember(e => e.EmployeeId, opt => opt.MapFrom(m => m.EmployeeIntId))
                .ForMember(e => e.BookedTimeoffInHours, opt => opt.MapFrom(m =>
                    m.TimeOffStartDateTime.Date == m.TimeOffEndDateTime.Date ?
                        Math.Min((m.TimeOffEndDateTime - m.TimeOffStartDateTime).Hours, 8) :
                        (m.TimeOffEndDateTime - m.TimeOffStartDateTime).Days * 8))
                .ForMember(e => e.TimeOffStartDateTime, opt => opt.MapFrom(m =>
                    m.TimeOffStartDateTime.Date == m.TimeOffEndDateTime.Date && m.StartTime.HasValue ?
                        m.TimeOffStartDateTime.Date + m.StartTime.Value.TimeOfDay :
                        m.TimeOffStartDateTime))
                .ForMember(e => e.TimeOffEndDateTime, opt => opt.MapFrom(m =>
                    m.TimeOffStartDateTime.Date == m.TimeOffEndDateTime.Date && m.EndTime.HasValue ?
                        m.TimeOffEndDateTime.Date + m.EndTime.Value.TimeOfDay :
                        m.TimeOffEndDateTime));
            Mapper.CreateMap<EmployeeTimeoffBooking, EmployeeTimeoffBookingHistoryModel>()
                .ForMember(m => m.EmployeeName, opt => opt.MapFrom(e => e.Employee.FirstName + " " + e.Employee.LastName))
                .ForMember(m => m.EmployeeTimeoffTypeName, opt => opt.MapFrom(e => e.EmployeeTimeoffType.Name))
                .ForMember(m => m.BookedByAccountName, opt => opt.MapFrom(e => e.BookedByAccount.FirstName + " " + e.BookedByAccount.LastName))
                .ForMember(m => m.LatestBalanceInHours, opt => opt.MapFrom(e => e.EmployeeTimeoffBalance != null ? 
                    e.EmployeeTimeoffBalance.LatestBalanceInHours : e.EmployeeTimeoffType.DefaultAnnualEntitlementInHours));
            //
            Mapper.CreateMap<EmployeeModel, Candidate>();
            //
            Mapper.CreateMap<CompanyJobRole, CompanyJobRoleModel>()
                .ForMember(m => m.CompanyName, opt => opt.MapFrom(e => e.Company.CompanyName))
                .ForMember(m => m.LocationId, opt => opt.MapFrom(e => e.CompanyLocationId))
                .ForMember(m => m.LocationName, opt => opt.MapFrom(e => string.Format("{0}", e.CompanyLocation.LocationName)))
                .ForMember(m => m.RequiredSkillIds, opt => opt.MapFrom(e => e.RequiredSkills.Select(x => x.Skill.Id.ToString()).ToArray()))
                .ForMember(m => m.RequiredSkillNames, opt => opt.MapFrom(e => string.Join(", ", e.RequiredSkills.Select(x => x.Skill.SkillName).ToArray())));
            Mapper.CreateMap<CompanyJobRoleModel, CompanyJobRole>()
                .ForMember(e => e.CompanyLocationId, opt => opt.MapFrom(m => m.LocationId));
            Mapper.CreateMap<CompanyJobRoleSkill, CompanyJobRoleSkillModel>()
                .ForMember(m => m.CompanyJobRoleName, opt => opt.MapFrom(e => e.CompanyJobRole.Name))
                .ForMember(m => m.SkillName, opt => opt.MapFrom(e => e.Skill.SkillName));
            Mapper.CreateMap<CompanyJobRoleSkillModel, CompanyJobRoleSkill>();
            //
            Mapper.CreateMap<EmployeeJobRole, EmployeeJobRoleModel>()
                .ForMember(m => m.EmployeeIntId, opt => opt.MapFrom(e => e.EmployeeId));
            Mapper.CreateMap<EmployeeJobRoleModel, EmployeeJobRole>()
                .ForMember(e => e.EmployeeId, opt => opt.MapFrom(m => m.EmployeeIntId));
            //
            Mapper.CreateMap<CompanyShift, CompanyShiftModel>()
                .ForMember(m => m.CompanyLocationId, opt => opt.MapFrom(e => e.CompanyDepartment.CompanyLocationId));
            Mapper.CreateMap<CompanyShiftModel, CompanyShift>();
            //
            Mapper.CreateMap<CompanyShiftJobRole, CompanyShiftJobRoleModel>();
            Mapper.CreateMap<CompanyShiftJobRole, CompanyShiftJobRoleGridModel>();
            Mapper.CreateMap<CompanyShiftJobRoleModel, CompanyShiftJobRole>();
            Mapper.CreateMap<CompanyJobRole, CompanyJobRoleDropdownModel>();
            Mapper.CreateMap<Account, AccountDropdownModel>()
                .ForMember(m => m.Name, opt => opt.MapFrom(e => e.FullName));
            //
            Mapper.CreateMap<SchedulePeriod, SchedulePeriodModel>()
                .ForMember(m => m.PeriodStartDate, opt => opt.MapFrom(e => TimeZone.CurrentTimeZone.ToLocalTime(e.PeriodStartUtc)))
                .ForMember(m => m.PeriodEndDate, opt => opt.MapFrom(e => TimeZone.CurrentTimeZone.ToLocalTime(e.PeriodEndUtc)))
                .ForMember(m => m.CompanyLocationId, opt => opt.MapFrom(e => e.CompanyDepartment.CompanyLocationId))
                .ForMember(m => m.LocationText, opt => opt.MapFrom(e => e.CompanyDepartmentId.HasValue ? string.Format("@[{0} - {1}]", 
                    e.CompanyDepartment.CompanyLocation.LocationName, e.CompanyDepartment.DepartmentName) : string.Empty));
            Mapper.CreateMap<SchedulePeriodModel, SchedulePeriod>()
                .ForMember(m => m.PeriodStartUtc, opt => opt.MapFrom(e => TimeZone.CurrentTimeZone.ToUniversalTime(e.PeriodStartDate)))
                .ForMember(m => m.PeriodEndUtc, opt => opt.MapFrom(e => TimeZone.CurrentTimeZone.ToUniversalTime(e.PeriodEndDate.GetValueOrDefault())));
            Mapper.CreateMap<ShiftSchedule, ShiftScheduleModel>()
                .ForMember(m => m.StartTimeOfDay, opt => opt.MapFrom(e => DateTime.Today.AddTicks(e.StartTimeOfDayTicks)));
            Mapper.CreateMap<ShiftScheduleModel, ShiftSchedule>()
                .ForMember(e => e.CompanyShiftId, opt => opt.MapFrom(m => m.CompanyShiftId > 0 ? m.CompanyShiftId : m.CompanyShift.Id))
                .ForMember(e => e.StartTimeOfDayTicks, opt => opt.MapFrom(m => m.StartTimeOfDay.GetValueOrDefault().TimeOfDay.Ticks));
            Mapper.CreateMap<CompanyShift, CompanyShiftDropdownModel>()
                .ForMember(m => m.Name, opt => opt.MapFrom(e => string.Format("{0} - ({1}/{2})",
                        e.Shift.ShiftName,
                        e.CompanyShiftJobRoles.Sum(y => y.MandantoryRequiredCount),
                        e.CompanyShiftJobRoles.Sum(y => y.ContingencyRequiredCount))));
            Mapper.CreateMap<CompanyShiftDropdownModel, CompanyShift>();
            //
            Mapper.CreateMap<EmployeeSchedule, EmployeeScheduleModel>()
                .ForMember(m => m.CandidateGuid, opt => opt.MapFrom(e => e.Employee.CandidateGuid))
                //.ForMember(m => m.EmployeeId, opt => opt.MapFrom(e => e.Employee.EmployeeId))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(e => e.Employee.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(e => e.Employee.LastName))
                .ForMember(m => m.EmployeeType, opt => opt.MapFrom(e => e.Employee.EmployeeType))
                .ForMember(m => m.PreferredWorkLocation, opt => opt.MapFrom(e => e.Employee.CompanyLocation.LocationName))
                .ForMember(m => m.HomePhone, opt => opt.MapFrom(e => e.Employee.HomePhone))
                .ForMember(m => m.MobilePhone, opt => opt.MapFrom(e => e.Employee.MobilePhone))
                .ForMember(m => m.JobTitle, opt => opt.MapFrom(x => x.Employee.EmployeeJobRoles != null 
                    && x.Employee.EmployeeJobRoles.Any(y => y.IsPrimary) ? x.Employee.EmployeeJobRoles.First(y => y.IsPrimary).CompanyJobRole.Name : string.Empty))
                .ForMember(m => m.ScheduledShiftName, opt => opt.MapFrom(x => x.CompanyShift != null ? x.CompanyShift.Shift.ShiftName : string.Empty))
                .ForMember(m => m.ScheduledJobRoleName, opt => opt.MapFrom(x => x.JobRole != null ? x.JobRole.Name : string.Empty))
                .ForMember(m => m.StartDate, opt => opt.MapFrom(x => x.SchedulePeriod.PeriodStartUtc.ToLocalTime()));
            Mapper.CreateMap<EmployeeScheduleModel, EmployeeSchedule>();
            Mapper.CreateMap<EmployeeScheduleDailyModel, EmployeeScheduleDaily>();
            Mapper.CreateMap<EmployeeScheduleDailyBreakModel, EmployeeScheduleDailyBreak>();

            Mapper.CreateMap<EmployeeScheduleDaily, EmployeeScheduleDailyModel>()
                .ForMember(m => m.ReplacementEmployeeId, opt => opt.MapFrom(e => e.ReplacementEmployeeId?? e.EmployeeSchedule.EmployeeId))
                .ForMember(m => m.Description, opt => opt.MapFrom(e => e.ReplacementEmployee != null ?
                    e.ReplacementEmployee.ToString() : e.EmployeeSchedule.Employee.ToString()))
                .ForMember(m => m.IsAdhoc, opt => opt.MapFrom(e => e.EmployeeSchedule.ForDailyAdhoc))
                .ForMember(m => m.Title, opt => opt.MapFrom(e => e.ReplacementCompanyJobRole != null ? 
                    e.ReplacementCompanyJobRole.Name : e.EmployeeSchedule.JobRole.Name))
                .ForMember(m => m.ReplacementCompanyJobRoleId, opt => opt.MapFrom(e => e.ReplacementCompanyJobRoleId?? e.EmployeeSchedule.JobRoleId));
            Mapper.CreateMap<EmployeeScheduleDailyBreak, EmployeeScheduleDailyBreakModel>();

            Mapper.CreateMap<EmployeeSchedulePreviewModel, EmployeeScheduleDailyModel>()
                .ForMember(e => e.ReplacementEmployeeId, opt => opt.MapFrom(m => m.EmployeeId))
                .ForMember(e => e.ReplacementCompanyJobRoleId, opt => opt.MapFrom(m => m.CompanyJobRoleId))
                .AfterMap((p, o) =>
                {
                    var isUpdate = p.EmployeeScheduleId != 0;
                    o.StartTimeOfDay = p.Start - p.ScheduelDate;
                    o.EndTimeOfDay = p.End - p.ScheduelDate;
                    o.Breaks = p.BreakModels
                        .Select(x =>
                            (x.BreakTimeOfDay > o.EndTimeOfDay || x.BreakTimeOfDay < o.StartTimeOfDay) ?
                            new EmployeeScheduleDailyBreakModel
                            {
                                BreakTimeOfDay = TimeSpan.FromTicks(o.Start.Ticks + (o.End - o.Start).Ticks / 2 - o.ScheduelDate.Ticks),
                                BreakLengthInMinutes = x.BreakLengthInMinutes,
                            } : x).ToList(); 
                    if (!isUpdate)
                    {
                        o.CreatedOnUtc = DateTime.UtcNow;
                    }
                    o.UpdatedOnUtc = DateTime.UtcNow;
                });
            //
            Mapper.CreateMap<ScheduleJobOrder, ScheduleJobOrderModel>()
                .ForMember(m => m.JobOrderId, opt => opt.MapFrom(e => e.JobOrder.Id))
                .ForMember(m => m.JobTitle, opt => opt.MapFrom(e => e.JobOrder.JobTitle))
                .ForMember(m => m.StartDate, opt => opt.MapFrom(e => e.JobOrder.StartDate))
                .ForMember(m => m.StartTime, opt => opt.MapFrom(e => e.JobOrder.StartTime))
                .ForMember(m => m.EndDate, opt => opt.MapFrom(e => e.JobOrder.EndDate))
                .ForMember(m => m.EndTime, opt => opt.MapFrom(e => e.JobOrder.EndTime));
            Mapper.CreateMap<ScheduleJobOrderModel, ScheduleJobOrder>();
            //
            Mapper.CreateMap<CandidateJobOrder, CandidatePipelineSimpleModel>()
                .ForMember(m => m.CandidateGuid, opt => opt.MapFrom(e => e.Candidate.CandidateGuid))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(e => e.Candidate.FirstName))
                .ForMember(m => m.LastName, opt => opt.MapFrom(e => e.Candidate.LastName))
                .ForMember(m => m.HomePhone, opt => opt.MapFrom(e => e.Candidate.HomePhone))
                .ForMember(m => m.MobilePhone, opt => opt.MapFrom(e => e.Candidate.MobilePhone))
                .ForMember(m => m.Email, opt => opt.MapFrom(e => e.Candidate.Email))
                .ForMember(m => m.StatusName, opt => opt.MapFrom(e => e.CandidateJobOrderStatus.StatusName));
            //
            Mapper.CreateMap<ScheduleStatusHistory, ScheduleStatusHistoryModel>();
            Mapper.CreateMap<ScheduleStatusHistoryModel, ScheduleStatusHistory>();
        }
        public static EmployeeGridModel ToGridModel(this Employee entity)
        {
            return Mapper.Map<Employee, EmployeeGridModel>(entity);
        }
        public static EmployeeModel ToModel(this Employee entity)
        {
            return Mapper.Map<Employee, EmployeeModel>(entity);
        }
        public static EmployeeTimeoffBalanceModel ToModel(this EmployeeTimeoffBalance entity)
        {
            var model = Mapper.Map<EmployeeTimeoffBalance, EmployeeTimeoffBalanceModel>(entity);
            model.AllowNegative = entity.EmployeeTimeoffType.AllowNegative;
            model.EmployeeTimeoffTypeName = entity.EmployeeTimeoffType.Name;
            if(entity.Employee!=null)
                model.EmployeeName = entity.Employee.FirstName + " " + entity.Employee.LastName;
            return model;
        }
        public static EmployeeTimeoffBalance ToEntity(this EmployeeTimeoffBalanceModel model)
        {
            return Mapper.Map<EmployeeTimeoffBalanceModel, EmployeeTimeoffBalance>(model);
        }
        public static EmployeeTimeoffBookingModel ToEditModel(this EmployeeTimeoffBooking entity)
        {
            return Mapper.Map<EmployeeTimeoffBooking, EmployeeTimeoffBookingModel>(entity);
        }
        public static EmployeeTimeoffBooking ToEntity(this EmployeeTimeoffBookingModel model)
        {
            return Mapper.Map<EmployeeTimeoffBookingModel, EmployeeTimeoffBooking>(model);
        }
        public static EmployeeTimeoffBooking ToEntity(this EmployeeTimeoffBookingModel model,EmployeeTimeoffBooking entity)
        {
            return Mapper.Map<EmployeeTimeoffBookingModel, EmployeeTimeoffBooking>(model,entity);
        }
        public static EmployeeTimeoffBookingHistoryModel ToModel(this EmployeeTimeoffBooking entity)
        {
            return Mapper.Map<EmployeeTimeoffBooking, EmployeeTimeoffBookingHistoryModel>(entity);
        }
        public static Candidate ToEntity(this EmployeeModel model, Candidate entity)
        {
            if (entity != null)
            {
                return Mapper.Map<EmployeeModel, Candidate>(model, entity);
            }
            else
            {
                return Mapper.Map<EmployeeModel, Candidate>(model);
            }
        }
        public static IncidentReportModel ToModel(this IncidentReport entity)
        {
            var model = Mapper.Map<IncidentReport, IncidentReportModel>(entity);
            model.CandidateGuid = entity.Candidate.CandidateGuid;
            model.EmployeeId = entity.Candidate.EmployeeId;
            return model;
        }
        public static IncidentReport ToEntity(this IncidentReportModel model)
        {
            return Mapper.Map<IncidentReportModel, IncidentReport>(model);
        }
        public static IncidentReportFileModel ToModel(this IncidentReportFile entity)
        {
            return Mapper.Map<IncidentReportFile, IncidentReportFileModel>(entity);
        }
        public static IncidentReportFile ToEntity(this IncidentReportFileModel model)
        {
            return Mapper.Map<IncidentReportFileModel, IncidentReportFile>(model);
        }
        public static CompanyJobRoleDropdownModel ToDropdownModel(this CompanyJobRole entity)
        {
            return Mapper.Map<CompanyJobRole, CompanyJobRoleDropdownModel>(entity);
        }
        public static CompanyJobRoleModel ToModel(this CompanyJobRole entity)
        {
            return Mapper.Map<CompanyJobRole, CompanyJobRoleModel>(entity);
        }
        public static CompanyJobRole ToEntity(this CompanyJobRoleModel model)
        {
            return Mapper.Map<CompanyJobRoleModel, CompanyJobRole>(model);
        }
        public static CompanyJobRoleSkillModel ToModel(this CompanyJobRoleSkill entity)
        {
            return Mapper.Map<CompanyJobRoleSkill, CompanyJobRoleSkillModel>(entity);
        }
        public static CompanyJobRoleSkill ToEntity(this CompanyJobRoleSkillModel model)
        {
            return Mapper.Map<CompanyJobRoleSkillModel, CompanyJobRoleSkill>(model);
        }
        public static EmployeeJobRoleModel ToModel(this EmployeeJobRole entity)
        {
            return Mapper.Map<EmployeeJobRole, EmployeeJobRoleModel>(entity);
        }
        public static EmployeeJobRole ToEntity(this EmployeeJobRoleModel model)
        {
            return Mapper.Map<EmployeeJobRoleModel, EmployeeJobRole>(model);
        }
        public static CompanyShiftModel ToModel(this CompanyShift entity)
        {
            return Mapper.Map<CompanyShift, CompanyShiftModel>(entity);
        }
        public static CompanyShift ToEntity(this CompanyShiftModel model)
        {
            return Mapper.Map<CompanyShiftModel, CompanyShift>(model);
        }
        public static CompanyShiftJobRoleModel ToModel(this CompanyShiftJobRole entity)
        {
            return Mapper.Map<CompanyShiftJobRole, CompanyShiftJobRoleModel>(entity);
        }
        public static CompanyShiftJobRole ToEntity(this CompanyShiftJobRoleModel model)
        {
            return Mapper.Map<CompanyShiftJobRoleModel, CompanyShiftJobRole>(model);
        }
        public static SchedulePeriodModel ToModel(this SchedulePeriod entity)
        {
            var model = Mapper.Map<SchedulePeriod, SchedulePeriodModel>(entity);
            model.CompanyLocationId = entity.CompanyLocationId;
            return model;
        }
        public static IEnumerable<SchedulePeriodModel> ToModels(this IEnumerable<SchedulePeriod> entities)
        {
            foreach(var e in entities)
            {
                yield return e.ToModel();
            }
        }
        public static SchedulePeriod ToEntity(this SchedulePeriodModel model)
        {
            return Mapper.Map<SchedulePeriodModel, SchedulePeriod>(model);
        }
        public static ShiftScheduleModel ToModel(this ShiftSchedule entity)
        {
            return Mapper.Map<ShiftSchedule, ShiftScheduleModel>(entity);
        }
        public static ShiftSchedule ToEntity(this ShiftScheduleModel model)
        {
            return Mapper.Map<ShiftScheduleModel, ShiftSchedule>(model);
        }
        public static EmployeeScheduleModel ToModel(this EmployeeSchedule entity)
        {
            return Mapper.Map<EmployeeSchedule, EmployeeScheduleModel>(entity);
        }
        public static EmployeeSchedule ToEntity(this EmployeeScheduleModel model)
        {
            return Mapper.Map<EmployeeScheduleModel, EmployeeSchedule>(model);
        }
        public static EmployeeScheduleDailyModel ToModel(this EmployeeScheduleDaily entity)
        {
            return Mapper.Map<EmployeeScheduleDaily, EmployeeScheduleDailyModel>(entity);
        }
        public static EmployeeScheduleDaily ToEntity(this EmployeeScheduleDailyModel model)
        {
            return Mapper.Map<EmployeeScheduleDailyModel, EmployeeScheduleDaily>(model);
        }
        public static EmployeeScheduleDailyModel ToOverride(this EmployeeSchedulePreviewModel model)
        {
            return Mapper.Map<EmployeeSchedulePreviewModel, EmployeeScheduleDailyModel>(model);
        }
        public static ScheduleJobOrderModel ToModel(this ScheduleJobOrder entity)
        {
            return Mapper.Map<ScheduleJobOrder, ScheduleJobOrderModel>(entity);
        }
        public static ScheduleJobOrder ToEntity(this ScheduleJobOrderModel model)
        {
            return Mapper.Map<ScheduleJobOrderModel, ScheduleJobOrder>(model);
        }
        public static CandidatePipelineSimpleModel ToSimpleModel(this CandidateJobOrder entity)
        {
            return Mapper.Map<CandidateJobOrder, CandidatePipelineSimpleModel>(entity);
        }
        public static ScheduleStatusHistory ToEntity(this ScheduleStatusHistoryModel model)
        {
            return Mapper.Map<ScheduleStatusHistoryModel, ScheduleStatusHistory>(model);
        }
        public static ScheduleStatusHistoryModel ToModel(this ScheduleStatusHistory entity)
        {
            return Mapper.Map<ScheduleStatusHistory, ScheduleStatusHistoryModel>(entity);
        }
    }
}
