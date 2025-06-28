using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Scheduling;
using Wfm.Services.Messages;
using Wfm.Services.TimeSheet;


namespace Wfm.Services.Employees
{
    public class TimeoffService : ITimeoffService
    {
        #region Fields

        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Account> _accounts;
        private readonly IRepository<EmployeeTimeoffType> _employeeTimeoffTypeRepository;
        private readonly IRepository<EmployeeTimeoffBalance> _employeeTimeoffBalanceRepository;
        private readonly IRepository<EmployeeTimeoffBooking> _employeeTimeoffBookingRepository;
        private readonly IRepository<EmployeeSchedule> _employeeScheduleRepository;
        private readonly IRepository<EmployeeScheduleDaily> _employeeScheduleDailyRepository;
        private readonly IRepository<StatutoryHoliday> _statutoryHolidayRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IWorkTimeService _worktimeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly CommonSettings _commonSettings;
        #endregion

        #region Ctor
        public TimeoffService(
            IRepository<Employee> employeeRepository,
            IRepository<Account> accounts,
            IRepository<EmployeeTimeoffType> employeeTimeoffTypeRepository,
            IRepository<EmployeeTimeoffBalance> employeeTimeoffBalanceRepository,
            IRepository<EmployeeTimeoffBooking> employeeTimeoffBookingRepository,
            IRepository<EmployeeSchedule> employeeScheduleRepository,
            IRepository<EmployeeScheduleDaily> employeeScheduleDailyRepository,
            IRepository<StatutoryHoliday> statutoryHolidayRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IWorkTimeService worktimeService,
            IWorkflowMessageService workflowMessageService,
            IWorkContext workContext,
            CommonSettings commonSettings)
        {
            _employeeRepository = employeeRepository;
            _accounts = accounts;
            _employeeTimeoffTypeRepository = employeeTimeoffTypeRepository;
            _employeeTimeoffBalanceRepository = employeeTimeoffBalanceRepository;
            _employeeTimeoffBookingRepository = employeeTimeoffBookingRepository;
            _employeeScheduleRepository = employeeScheduleRepository;
            _employeeScheduleDailyRepository = employeeScheduleDailyRepository;
            _statutoryHolidayRepository = statutoryHolidayRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _worktimeService = worktimeService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _commonSettings = commonSettings;
        }
        #endregion

        #region CRUD TimeoffType
        public IQueryable<EmployeeTimeoffType> GetAllTimeoffTypes(bool activeOnly = false)
        {
            return _employeeTimeoffTypeRepository.Table
                .Where(x => !activeOnly || x.IsActive);
        }

        public void InsertNewTimeoffType(EmployeeTimeoffType model)
        {
            model.CreatedOnUtc = model.UpdatedOnUtc = DateTime.UtcNow;
            _employeeTimeoffTypeRepository.Insert(model);
        }

        public void UpdateTimeoffType(EmployeeTimeoffType model)
        {
            var _model = _employeeTimeoffTypeRepository.Table.Where(x => x.Id == model.Id)
                .Include(x => x.EmployeeTimeoffBalances).FirstOrDefault();

            if (_model != null)
            {
                var currentYear = DateTime.Today.Year;
                _model.UpdatedOnUtc = DateTime.UtcNow;
                _model.AllowNegative = model.AllowNegative;
                _model.DefaultAnnualEntitlementInHours = model.DefaultAnnualEntitlementInHours;
                _model.Description = model.Description;
                _model.IsActive = model.IsActive;
                _model.Name = model.Name;
                _model.Paid = model.Paid;
                _model.EmployeeTypeId = model.EmployeeTypeId;
                var toUpdate = _model.EmployeeTimeoffBalances.Where(x => x.Year >= currentYear).ToList();
                foreach (var b in toUpdate)
                {
                    b.EntitledTimeoffInHours = model.DefaultAnnualEntitlementInHours.GetValueOrDefault();
                }
                _employeeTimeoffTypeRepository.Update(_model);
            }
        }
        #endregion

        #region Entitlement
        public IEnumerable<EmployeeTimeoffBalance> GetEntitlement(int employeeId, int year)
        {
            int workDayHours = _commonSettings.WorkDayHours;
            var employee = _employeeRepository.TableNoTracking.Where(x => x.Id == employeeId).FirstOrDefault();
            if (employee == null)
                return Enumerable.Empty<EmployeeTimeoffBalance>();
            var result = _employeeTimeoffBalanceRepository.Table
                .Include(x => x.EmployeeTimeoffType)
                .Include(x => x.Employee)
                .Where(x => x.Year == year && x.EmployeeId == employeeId).ToList();
            var typeId = result.Select(x => x.EmployeeTimeoffTypeId).ToArray();
            result.AddRange(_employeeTimeoffTypeRepository.Table
                .Where(x => !typeId.Contains(x.Id) && x.IsActive&&x.EmployeeTypeId==employee.EmployeeTypeId)
                .ToArray()
                .Select(x => new EmployeeTimeoffBalance()
                {
                    EmployeeTimeoffTypeId = x.Id,
                    EmployeeTimeoffType = x,
                    EmployeeId = employeeId,
                    Employee = employee,
                    Year = year,
                    EntitledTimeoffInHours = x.DefaultAnnualEntitlementInHours.GetValueOrDefault(),
                    LatestBalanceInHours = x.DefaultAnnualEntitlementInHours.GetValueOrDefault(),
                }));
            var holidayEntry = result.FirstOrDefault(x => x.EmployeeTimeoffType.Name.Equals("Holiday", StringComparison.OrdinalIgnoreCase));
            if (holidayEntry != null && holidayEntry.Id == 0)
            {
                var totalStatutoryHolidayHours = _statutoryHolidayRepository.Table.Where(x => x.HolidayDate.Year == year).Count() * workDayHours;
                holidayEntry.EntitledTimeoffInHours += totalStatutoryHolidayHours;
                holidayEntry.LatestBalanceInHours += totalStatutoryHolidayHours;
            }
            return result;
        }
        public IEnumerable<EmployeeTimeoffBalance> UpdateEntitlements(IEnumerable<EmployeeTimeoffBalance> entitlements)
        {
            foreach (var entry in entitlements)
            {
                var toUpdate = _employeeTimeoffBalanceRepository.Table.Where(x => x.EmployeeId == entry.EmployeeId &&
                    x.EmployeeTimeoffTypeId == entry.EmployeeTimeoffTypeId &&
                    x.Year == entry.Year).FirstOrDefault();
                if (toUpdate != null)
                {
                    var delta = entry.EntitledTimeoffInHours - toUpdate.EntitledTimeoffInHours;
                    toUpdate.EntitledTimeoffInHours = entry.EntitledTimeoffInHours;
                    toUpdate.LatestBalanceInHours += delta;
                    toUpdate.UpdatedOnUtc = DateTime.UtcNow;
                    _employeeTimeoffBalanceRepository.Update(toUpdate);
                }
                else
                {
                    toUpdate = new EmployeeTimeoffBalance
                    {
                        EmployeeTimeoffTypeId = entry.EmployeeTimeoffTypeId,
                        EmployeeId = entry.EmployeeId,
                        Year = entry.Year,
                        EntitledTimeoffInHours = entry.EntitledTimeoffInHours,
                        LatestBalanceInHours = entry.EntitledTimeoffInHours,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                    };
                    _employeeTimeoffBalanceRepository.Insert(toUpdate);
                }
                yield return toUpdate;
            }
        }
        #endregion


        #region Timeoff Booking

        public void DeleteTimeOff(EmployeeTimeoffBooking timeoff)
        {
            // recover balance first
            _RecoverTimeOffBalance(timeoff);

            _employeeTimeoffBookingRepository.Delete(timeoff);
        }


        private void _RecoverTimeOffBalance(EmployeeTimeoffBooking timeoff)
        {
            // mock rejection
            timeoff.IsRejected = true;
            int totalBookedHours;
            var balanceEntries = PrepareBalanceEntries(timeoff, out totalBookedHours);
            if (balanceEntries != null)
                PersistBalanceEntries(balanceEntries);
        }


        public IEnumerable<EmployeeTimeoffBooking> GetBookHistoryByEmployee(int employeeId, int? year = null)
        {
            return _employeeTimeoffBookingRepository.Table
                .Where(x => x.EmployeeId == employeeId && (!year.HasValue || x.Year == year.Value))
                .Include(x => x.Employee)
                .Include(x => x.EmployeeTimeoffType)
                .Include(x => x.BookedByAccount);
        }


        /// <summary>
        /// Check this to prevent overlap of timeoff booking for different or even same type
        /// </summary>
        /// <returns></returns>
        private int GetAlreadybookedOffHours(int employeeId, DateTime refDate, int thisBookingId)
        {
            int workDayHours = _commonSettings.WorkDayHours;
            var allBookings = _employeeTimeoffBookingRepository.Table
                .Where(x => (!x.IsRejected.HasValue || !x.IsRejected.Value) && x.Id != thisBookingId && x.EmployeeId == employeeId)
                .Where(x => DbFunctions.TruncateTime(x.TimeOffStartDateTime) <= refDate && DbFunctions.TruncateTime(x.TimeOffEndDateTime) >= refDate);
            var result = allBookings.Max(x => (decimal?)x.BookedTimeoffInHours) ?? 0;

            return (int)Math.Min(result, workDayHours);
        }

        
        public int GetHoursBetweenDates(int employeeId, DateTime start, DateTime end, int thisBookingId)
        {
            int days = 0;
            int hours = 0;
            var refDate = start;
            int workDayHours = _commonSettings.WorkDayHours;
            //
            var location = _employeeRepository.Table
                .Where(x => x.Id == employeeId)
                .Include(x => x.CompanyLocation)
                .First().CompanyLocation;
            var stateProvinceId = location != null ? location.StateProvinceId : 
                    _stateProvinceRepository.Table.Where(x => x.Abbreviation == "ON").First().Id;
            var statutoryHolidays = _statutoryHolidayRepository.Table
                .Where(x => x.StateProvinceId == stateProvinceId && x.IsActive)
                .Select(x => x.HolidayDate)
                .ToArray();
            //
            while (refDate.Date <= end.Date)
            {
                var isWeekend = refDate.DayOfWeek == DayOfWeek.Saturday || refDate.DayOfWeek == DayOfWeek.Sunday;
                var isHoliday = statutoryHolidays.Contains(refDate.Date);
                if (!isWeekend && !isHoliday)
                {
                    hours += start.Date == end.Date ? Math.Min((end - start).Hours, workDayHours) : workDayHours;
                    hours -= GetAlreadybookedOffHours(employeeId, refDate.Date, thisBookingId);
                    hours = hours >= 0 ? hours : 0;
                }
                days++;
                refDate = start.AddDays(days).Date;
            }
            return hours;
        }
        /// <summary>
        /// One booking can be a timespan across multiple years
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IEnumerable<Tuple<int, int>> GetTimeoffHoursByYears(EmployeeTimeoffBooking model)
        {
            for (int i = model.TimeOffStartDateTime.Year; i <= model.TimeOffEndDateTime.Year; i++)
            {
                var yearStart = new DateTime(i, 1, 1);
                var yearEnd = new DateTime(i, 12, 31, 23, 59, 59);
                var timeoffThisYearStart = yearStart < model.TimeOffStartDateTime ? model.TimeOffStartDateTime : yearStart;
                var timeOffThisYearEnd = yearEnd < model.TimeOffEndDateTime ? yearEnd : model.TimeOffEndDateTime;
                var hourInThisYear = GetHoursBetweenDates(model.EmployeeId, timeoffThisYearStart, timeOffThisYearEnd, model.Id);
                yield return Tuple.Create(i, hourInThisYear);
            }
        }
        private int GetTotalHoursByOtherBookings(int employeeId, int timeoffTypeId, int year, int[] excludeBookingIds)
        {
            var bookings = _employeeTimeoffBookingRepository.Table
                .Where(x => (!x.IsRejected.HasValue || !x.IsRejected.Value) && x.EmployeeId == employeeId && x.EmployeeTimeoffTypeId == timeoffTypeId)
                .Where(x => !excludeBookingIds.Contains(x.Id)).ToArray()
                .SelectMany(x => GetTimeoffHoursByYears(x));
            return bookings.Where(x => x.Item1 == year).Sum(x => x.Item2);
        }
        private EmployeeTimeoffBalance[] PrepareBalanceEntries(EmployeeTimeoffBooking model, out int totalBookedHours)
        {
            var timeofffTypeEntry = _employeeTimeoffTypeRepository.GetById(model.EmployeeTimeoffTypeId);
            totalBookedHours = 0;
            if (timeofffTypeEntry == null) return null;
            //
            var result = new List<EmployeeTimeoffBalance>();
            foreach (var entry in GetTimeoffHoursByYears(model))
            {
                var balanceEntry = _employeeTimeoffBalanceRepository.Table
                    .Where(x => x.EmployeeId == model.EmployeeId)
                    .Where(x => x.EmployeeTimeoffTypeId == model.EmployeeTimeoffTypeId)
                    .Where(x => x.Year == entry.Item1)
                    .FirstOrDefault();
                if (balanceEntry == null)
                {
                    balanceEntry = new EmployeeTimeoffBalance()
                    {
                        EmployeeId = model.EmployeeId,
                        EmployeeTimeoffTypeId = model.EmployeeTimeoffTypeId.GetValueOrDefault(),
                        Year = entry.Item1,
                        EntitledTimeoffInHours = timeofffTypeEntry.DefaultAnnualEntitlementInHours.GetValueOrDefault(),
                        LatestBalanceInHours = timeofffTypeEntry.DefaultAnnualEntitlementInHours.GetValueOrDefault(),
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                }
                var hoursThisYearByOtherBookings = GetTotalHoursByOtherBookings(model.EmployeeId, model.EmployeeTimeoffTypeId.GetValueOrDefault(), 
                    entry.Item1, new int[] { model.Id });
                totalBookedHours += entry.Item2;
                // reduction only for non-rejected 
                var reduction = !model.IsRejected.HasValue || !model.IsRejected.Value ? entry.Item2 : 0;
                balanceEntry.LatestBalanceInHours = balanceEntry.EntitledTimeoffInHours - hoursThisYearByOtherBookings - reduction;
                balanceEntry.UpdatedOnUtc = DateTime.UtcNow;
                result.Add(balanceEntry);
            }
            return result.ToArray();
        }

        private void PersistBalanceEntries(IEnumerable<EmployeeTimeoffBalance> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.Id > 0)
                {
                    _employeeTimeoffBalanceRepository.Update(entry);
                }
                else
                {
                    _employeeTimeoffBalanceRepository.Insert(entry);
                }
            }
        }

        private void UpdateEmployeeSchedule(EmployeeTimeoffBooking model)
        {
            var timeOffBookStartUtc = model.TimeOffStartDateTime.ToUniversalTime();
            var timeOffBookEndUtc = model.TimeOffEndDateTime.ToUniversalTime();
            var schedules = _employeeScheduleRepository.Table
                .Where(x => x.EmployeeId == model.EmployeeId)
                .Where(x => (x.SchedulePeriod.PeriodStartUtc <= timeOffBookStartUtc && x.SchedulePeriod.PeriodEndUtc >= timeOffBookStartUtc) ||
                (x.SchedulePeriod.PeriodStartUtc <= timeOffBookEndUtc && x.SchedulePeriod.PeriodEndUtc >= timeOffBookEndUtc))
                .Include(x => x.SchedulePeriod)
                .Include(x => x.CompanyShift.CompanyShiftJobRoles).ToArray();
            foreach (var schedule in schedules)
            {
                var startDate = new[] { timeOffBookStartUtc.ToLocalTime(), schedule.SchedulePeriod.PeriodStartUtc.ToLocalTime() }.Max();
                var endDate = new[] { timeOffBookEndUtc.ToLocalTime(), schedule.SchedulePeriod.PeriodEndUtc.ToLocalTime() }.Min();
                int days = 0;
                var date = startDate.AddDays(days).Date;
                while (date <= endDate.Date)
                {
                    var entry = _employeeScheduleDailyRepository.Table
                        .Where(x => x.EmployeeScheduleId == schedule.Id)
                        .Where(x => x.ScheduelDate == date).FirstOrDefault();
                    if (entry != null)
                    {
                        entry.IsDeleted = true;
                        entry.UpdatedOnUtc = DateTime.UtcNow;
                        _employeeScheduleDailyRepository.Update(entry);
                    }
                    else
                    {
                        _employeeScheduleDailyRepository.Insert(new EmployeeScheduleDaily
                        {
                            EmployeeScheduleId = schedule.Id,
                            ScheduelDate = date,
                            IsDeleted = true,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow,
                        });
                    }
                    days++;
                    date = startDate.AddDays(days).Date;
                }
                _workflowMessageService.SendRescheduleTimeoffBookingNotification(schedule, model);
            }
            //
            _worktimeService.RemoveMatchedWorktimeForAfterFactTimeoffBooking(model.Id, model.TimeOffStartDateTime, model.TimeOffEndDateTime);
        }

        public void BookNewtTimeoff(EmployeeTimeoffBooking model)
        {
            int totalBookedHours;
            var balanceEntries = PrepareBalanceEntries(model, out totalBookedHours);
            if (balanceEntries != null)
            {
                // add/update balances first
                PersistBalanceEntries(balanceEntries);
                if (balanceEntries.Length == 1 && balanceEntries[0].Id > 0) model.EmployeeTimeoffBalanceId = balanceEntries[0].Id;
                model.BookedByAccountId = _workContext.CurrentAccount.Id;
                model.BookedTimeoffInHours = totalBookedHours;
                model.CreatedOnUtc = DateTime.UtcNow;
                model.UpdatedOnUtc = DateTime.UtcNow;
                _employeeTimeoffBookingRepository.Insert(model);
                UpdateEmployeeSchedule(model);
            }
        }
        public EmployeeTimeoffBooking GetTimeoffBookingById(int bookingId)
        {
            return _employeeTimeoffBookingRepository.Table.Where(x => x.Id == bookingId).Include(x => x.Employee).FirstOrDefault();
        }
        public void UpdateTimeoffBooking(EmployeeTimeoffBooking model)
        {
            int totalBookedHours;
            var balanceEntries = PrepareBalanceEntries(model, out totalBookedHours);
            if (balanceEntries != null)
            {
                // add/update balances first
                PersistBalanceEntries(balanceEntries);
                if (balanceEntries.Length == 1 && balanceEntries[0].Id > 0) model.EmployeeTimeoffBalanceId = balanceEntries[0].Id;
                model.BookedTimeoffInHours = totalBookedHours;
                model.UpdatedOnUtc = DateTime.UtcNow;
                _employeeTimeoffBookingRepository.Update(model);
                UpdateEmployeeSchedule(model);
            }
        }


        public IEnumerable<EmployeeTimeoffBooking> GetBookHistoryByManager(int accountId, int? year = null)
        {
            var employeeIds = _accounts.TableNoTracking.Where(x => x.ManagerId == accountId).Select(x => x.EmployeeId);

            return _employeeTimeoffBookingRepository.Table
                .Where(x => !year.HasValue || x.Year == year.Value)
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .Include(x => x.Employee)
                .Include(x => x.EmployeeTimeoffType)
                .Include(x => x.BookedByAccount);
        }
        
        #endregion

        #region Timeoff Schedule
        public IEnumerable<EmployeeTimeoffBooking> GetEmployeeTimeoffBookings(Guid employeeGuid)
        {
            return _employeeTimeoffBookingRepository.Table
                .Where(x => x.Employee.CandidateGuid == employeeGuid)
                .Include(x => x.Employee)
                .Include(x => x.EmployeeTimeoffType)
                .ToArray();
        }
        public IEnumerable<EmployeeTimeoffBooking> GetTeamTimeoffBookings(IEnumerable<Guid> employeeGuids)
        {
            return _employeeTimeoffBookingRepository.Table
                .Where(x => employeeGuids.Contains(x.Employee.CandidateGuid))
                .Include(x => x.Employee)
                .Include(x => x.EmployeeTimeoffType)
                .ToArray();
        }
        #endregion

        #region Time Off Duplication
        public bool IsTimeoffTypeDuplicate(EmployeeTimeoffType entity)
        {
            var duplicateType = _employeeTimeoffTypeRepository.TableNoTracking
                                   .Where(x => x.Name == entity.Name && x.EmployeeTypeId == entity.EmployeeTypeId&&x.IsActive);
            if (entity.Id != 0)
                duplicateType = duplicateType.Where(x => x.Id != entity.Id);
            if (duplicateType.Count()>0)
                return true;
            else
                return false;

        }
        #endregion
    }
}
