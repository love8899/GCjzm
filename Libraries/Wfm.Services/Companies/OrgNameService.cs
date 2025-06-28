using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Services.Accounts;
using Wfm.Services.Common;
using Wfm.Services.Franchises;


namespace Wfm.Services.Companies
{
    public partial class OrgNameService : IOrgNameService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IAccountService _accountService;
        private readonly ICompanyDepartmentService _departmentService;
        private readonly ICompanyDivisionService _locationService;
        private readonly IPositionService _positionService;
        private readonly IShiftService _shiftService;
        private readonly IFranchiseService _vendorService;

        #endregion


        #region Ctor

        public OrgNameService(
            IWorkContext workContextService,
            IAccountService accountService,
            ICompanyDepartmentService departmentService,
            ICompanyDivisionService locationService,
            IPositionService positionService,
            IShiftService shiftService,
            IFranchiseService vendorService
            )
        {
            _workContext = workContextService;

            _accountService = accountService;
            _departmentService = departmentService;
            _locationService = locationService;
            _positionService = positionService;
            _shiftService = shiftService;
            _vendorService = vendorService;
        }

        #endregion


        #region Methods

        public string GetOrgNameById(string colName, int id, out string org)
        {
            var name = string.Empty;
            org = string.Empty;

            switch (colName)
            {
                case "FranchiseId":
                case "VendorId":
                    org = "VendorName";
                    var vendor = _vendorService.GetFranchiseById(id);
                    if (vendor != null)
                        name = vendor.FranchiseName;
                    break;

                case "CompanyLocationId":
                case "LocationId":
                    org = "Location";
                    var location = _locationService.GetCompanyLocationById(id);
                    if (location != null)
                        name = location.LocationName;
                    break;

                case "CompanyDepartmentId":
                case "DepartmentId":
                    org = "Department";
                    var department = _departmentService.GetCompanyDepartmentById(id);
                    if (department != null)
                        name = department.DepartmentName;
                    break;

                case "PositionId":
                    org = "Position";
                    var position = _positionService.GetPositionById(id);
                    if (position != null)
                        name = position.Name;
                    break;

                case "ShiftId":
                    org = "Shift";
                    var shift = _shiftService.GetShiftById(id);
                    if (shift != null)
                        name = shift.ShiftName;
                    break;
            }

            return name;
        }


        public IList<SelectListItem> GetVendorsAsDropDownList(bool idVal = true)
        {
            return _vendorService.GetAllFranchisesAsSelectList(_workContext.CurrentAccount, idVal: idVal);
        }


        public IList<SelectListItem> GetCompanyLocationsAsDropDownList(bool idVal = true)
        {
            return _locationService.GetAllCompanyLocationsByAccount(_workContext.CurrentAccount)
                .Select(x => new SelectListItem()
                {
                    Text = x.LocationName,
                    Value = idVal ? x.Id.ToString() : x.LocationName
                }).ToList();
        }


        public IList<SelectListItem> GetCompanyDepartmentsAsDropDownList(bool idVal = true)
        {
            return _departmentService.GetAllCompanyDepartmentsByAccount(_workContext.CurrentAccount)
                .Select(x => new SelectListItem()
                {
                    Text = x.DepartmentName,
                    Value = idVal ? x.Id.ToString() : x.DepartmentName
                }).ToList();
        }


        public IList<SelectListItem> GetClientAccountsAsDropDownList(bool idVal = true)
        {
            return _accountService.GetAllClientAccountForTask().Where(x => x.CompanyId == _workContext.CurrentAccount.CompanyId)
                .AsEnumerable().Select(x => new SelectListItem()
                {
                    Text = x.FullName,
                    Value = idVal ? x.Id.ToString() : x.FullName
                }).ToList();
        }


        public IList<SelectListItem> GetPositionsAsDropDownList(bool idVal = true)
        {
            return _positionService.GetAllPositionByCompanyId(_workContext.CurrentAccount.CompanyId)
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = idVal ? x.Id.ToString() : x.Name
                }).ToList();
        }


        public IList<SelectListItem> GetShiftsAsDropDownList(bool idVal = true)
        {
            return _shiftService.GetAllShiftsForDropDownList(companyId: _workContext.CurrentAccount.CompanyId, idVal: idVal);
        }

        #endregion
    }

}
