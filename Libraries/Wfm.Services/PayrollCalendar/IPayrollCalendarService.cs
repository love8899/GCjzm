using System;
using System.Linq;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Services.Payroll
{
    public partial interface IPayrollCalendarService
    {
        void Insert(Payroll_Calendar entity);
        void Update(Payroll_Calendar entity);

        IQueryable<Payroll_Calendar> GetPayrollCalendarAsQueryable(int year, int payGroupId);
        Payroll_Calendar GetNextPayrollCalendar(DateTime refDate, int franchiseId);
        IQueryable<PayFrequencyType> GetAllPayFrequencyTypesAsQueryable();

        Payroll_Calendar GetPayrollCalendarById(int Id);
        Payroll_Calendar GetPayrollCalendarByIdNoTracking(int Id);
        IQueryable<Payroll_Calendar> GetPayrollCalendarByYearAndPayGroupIds(int year, string payGroupIds, bool committedOnly=false);

        void CreateCalendarEntries(int year, int payGroupId, int payFrequency, DateTime startDate, DateTime payDate);
    }
}
