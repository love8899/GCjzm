using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Payroll;
using System;
using System.Collections.Generic;

namespace Wfm.Services.Payroll
{
    public partial class PayrollCalendarService : IPayrollCalendarService
    {
        #region Fields

        private readonly IRepository<Payroll_Calendar> _payrollCalendarRepository;
        private readonly IRepository<PayGroup> _payGroupRepository;
        private readonly IRepository<PayFrequencyType> _payFrequencyTypeRepository;
        #endregion

        #region Ctor

        public PayrollCalendarService(
                               IRepository<Payroll_Calendar> payrollCalendarRepository,
                               IRepository<PayGroup> payGroupRepository,
                               IRepository<PayFrequencyType> payFrequencyTypeRepository)
        {
            _payrollCalendarRepository = payrollCalendarRepository;
            _payGroupRepository = payGroupRepository;
            _payFrequencyTypeRepository = payFrequencyTypeRepository;
        }

        #endregion 

        #region CRUD

        public void Insert(Payroll_Calendar entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Payroll_Calendar");

            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            
            //insert
            _payrollCalendarRepository.Insert(entity);
        }

        public void Update(Payroll_Calendar entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Payroll_Calendar");

            entity.UpdatedOnUtc = DateTime.UtcNow;
            _payrollCalendarRepository.Update(entity);
        }

        #endregion

        #region Methods

        public IQueryable<Payroll_Calendar> GetPayrollCalendarAsQueryable(int year, int payGroupId)
        {
            return _payrollCalendarRepository.Table.Where(x => x.PayGroupId == payGroupId && x.Year == year).OrderBy(x => x.PayPeriodNumber);
        }

        public Payroll_Calendar GetNextPayrollCalendar(DateTime refDate, int franchiseId)
        {
            //default Biweekly pay group
            var result = from p in _payrollCalendarRepository.TableNoTracking
                         join g in _payGroupRepository.TableNoTracking on p.PayGroupId equals g.Id
                         join f in _payFrequencyTypeRepository.TableNoTracking on g.PayFrequencyTypeId equals f.Id
                         where (f.Frequency == 26 || f.Frequency == 27) && g.FranchiseId == franchiseId && p.PayPeriodStartDate <= refDate && p.PayPeriodEndDate >= refDate
                         select p;
            return result.FirstOrDefault();
        }

        public IQueryable<PayFrequencyType> GetAllPayFrequencyTypesAsQueryable()
        {
            return _payFrequencyTypeRepository.TableNoTracking;
        }

        public Payroll_Calendar GetPayrollCalendarById(int Id)
        {
            return _payrollCalendarRepository.GetById(Id);
        }

        public Payroll_Calendar GetPayrollCalendarByIdNoTracking(int Id)
        {
            return _payrollCalendarRepository.TableNoTracking.Where(x => x.Id == Id).FirstOrDefault();
        }

        public IQueryable<Payroll_Calendar> GetPayrollCalendarByYearAndPayGroupIds(int year, string payGroupIds, bool committedOnly=false)
        {
            var query = _payrollCalendarRepository.Table.Where(x => x.Year == year);
            if (!String.IsNullOrWhiteSpace(payGroupIds))
            {
                int[] groupIds = payGroupIds.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries).Select(x=>int.Parse(x)).ToArray();
                query = query.Where(x => groupIds.Contains(x.PayGroupId));
            }
            if (committedOnly)
                query = query.Where(x => x.PayPeriodCommitDate.HasValue);
            return query;
        }

        public void CreateCalendarEntries(int year, int payGroupId, int payFrequency, DateTime startDate, DateTime payDate)
        {
            TimeSpan diff = payDate - startDate;
            int payDateInc = diff.Days;

            var newCalendar = new List<Payroll_Calendar>();

            for (int i = 1; i <= payFrequency; i++)
            {
                DateTime _endDate = this.CalculatePayPeriodEndDate(startDate, payFrequency);

                var item = new Payroll_Calendar()
                {
                    PayGroupId = payGroupId,
                    PayPeriodNumber = i,
                    PayPeriodStartDate = startDate,
                    PayPeriodPayDate = payDate,
                    PayPeriodEndDate = _endDate,
                    Year = year
                };

                newCalendar.Add(item);

                // Shift the startDate and payDate forward for next calendar item
                startDate = _endDate.AddDays(1);
                payDate = startDate.AddDays(payDateInc);
            }
           
            // Now remove existing calendar rows for the paygroup and year
            var existingCalendar = this.GetPayrollCalendarByYearAndPayGroupIds(year, payGroupId.ToString(), false).ToList();
            foreach (var row in existingCalendar)
                _payrollCalendarRepository.Delete(row);

            // Insert the new calendar rows
            foreach (var row in newCalendar)
                this.Insert(row);
        }

        private DateTime CalculatePayPeriodEndDate(DateTime startDate, int PayFrequency)
        {
            DateTime result = DateTime.MinValue;
            switch (PayFrequency)
            {
                case 12: //Monthly - End Date is always end of the month
                    result = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                    break;
                case 24: //Semi-Monthly - End ddate is either 15th or end of the month
                    if (startDate.Day < 15)
                        result = new DateTime(startDate.Year, startDate.Month, 15);
                    else
                        result = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                    break;
                case 52: // Weekly - End date is one week after the start date
                case 53:  // Same as 52
                    result = startDate.AddDays(6);
                    break;
                case 26: // Biweekly - End date is two weeks after the start date
                case 27: // Same as 26
                    result = startDate.AddDays(13);
                    break;
            }
            return result;
        }
        #endregion

    }
}
