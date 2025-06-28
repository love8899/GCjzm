using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Employees;

namespace Wfm.Services.Employees
{
    public interface ITimeoffService
    {
        #region CRUD - Timeoff Type
        void InsertNewTimeoffType(EmployeeTimeoffType model);
        void UpdateTimeoffType(EmployeeTimeoffType model);
        IQueryable<EmployeeTimeoffType> GetAllTimeoffTypes(bool activeOnly = false);
        #endregion

        #region Entitlement
        IEnumerable<EmployeeTimeoffBalance> GetEntitlement(int employeeId, int year);
        IEnumerable<EmployeeTimeoffBalance> UpdateEntitlements(IEnumerable<EmployeeTimeoffBalance> entitlements);
        #endregion

        #region Book Timeoff
        void DeleteTimeOff(EmployeeTimeoffBooking timeoff);
        IEnumerable<EmployeeTimeoffBooking> GetBookHistoryByEmployee(int employeeId, int? year = null);
        int GetHoursBetweenDates(int employeeId, DateTime start, DateTime end, int thisBookingId);
        void BookNewtTimeoff(EmployeeTimeoffBooking model);
        EmployeeTimeoffBooking GetTimeoffBookingById(int bookingId);
        void UpdateTimeoffBooking(EmployeeTimeoffBooking model);
        IEnumerable<EmployeeTimeoffBooking> GetBookHistoryByManager(int accountId, int? year = null);
        #endregion

        #region Timeoff Schedule
        IEnumerable<EmployeeTimeoffBooking> GetEmployeeTimeoffBookings(Guid employeeGuid);
        IEnumerable<EmployeeTimeoffBooking> GetTeamTimeoffBookings(IEnumerable<Guid> employeeGuids);
        #endregion
        bool IsTimeoffTypeDuplicate(EmployeeTimeoffType entity);
    }
}
