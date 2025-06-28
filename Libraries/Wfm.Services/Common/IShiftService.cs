using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface IShiftService
    {

        /// <summary>
        /// Inserts the shift.
        /// </summary>
        /// <param name="shift">The shift.</param>
        void InsertShift(Shift shift);

        /// <summary>
        /// Updates the shift.
        /// </summary>
        /// <param name="shift">The shift.</param>
        void UpdateShift(Shift shift);

        /// <summary>
        /// Deletes the shift.
        /// </summary>
        /// <param name="shift">The shift.</param>
        void DeleteShift(Shift shift);

        /// <summary>
        /// Gets the shift by shift id.
        /// </summary>
        /// <param name="shiftId">The shift id.</param>
        /// <returns></returns>
        Shift GetShiftById(int id);

        int GetShiftIdByName(string name, int? companyId = 0, bool includeDeleted = true);

        bool AnyDuplicate(int id, int companyId, string name, bool includeDeleted = false);

        /// <summary>
        /// Gets all shifts.
        /// </summary>
        /// <returns></returns>
        IList<Shift> GetAllShifts(bool showInactive = false, bool showHidden = false, int? companyId = 0);
        IList<SelectListItem> GetAllShiftsForDropDownList(bool showInactive = false, bool showHidden = false, int? companyId = 0, bool idVal = true);
        IQueryable<Shift> GetAllShiftsAsQueryable(bool showInactive = false, bool showHidden = false);

        IQueryable<Shift> GetAllShiftsEnableInSchedule();
    }
}
