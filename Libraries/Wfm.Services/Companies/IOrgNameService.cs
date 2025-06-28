using System.Collections.Generic;
using System.Web.Mvc;


namespace Wfm.Services.Companies
{
    public partial interface IOrgNameService
    {
        string GetOrgNameById(string colName, int id, out string org);

        // drop down list, tested for client only
        IList<SelectListItem> GetVendorsAsDropDownList(bool idVal = true);

        IList<SelectListItem> GetCompanyLocationsAsDropDownList(bool idVal = true);

        IList<SelectListItem> GetCompanyDepartmentsAsDropDownList(bool idVal = true);

        IList<SelectListItem> GetClientAccountsAsDropDownList(bool idVal = true);

        IList<SelectListItem> GetPositionsAsDropDownList(bool idVal = true);

        IList<SelectListItem> GetShiftsAsDropDownList(bool idVal = true);
    }
}
