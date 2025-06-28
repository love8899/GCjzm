using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using Wfm.Admin.Models.Common;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Employees;
using Wfm.Services.Common;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.Localization;
using Wfm.Services.Security;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.Payroll;
using Wfm.Services.Policies;
using Wfm.Services.ClockTime;
using Wfm.Services.WSIBS;
using Wfm.Services.Candidates;


namespace Wfm.Admin.Controllers
{
    public partial class CommonController : BaseAdminController
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly IGenderService _genderService;
        private readonly ISalutationService _salutationService;
        private readonly ITransportationService _transportationService;
        private readonly ISourceService _sourceService;
        private readonly IAddressTypeService _addressTypeService;
        private readonly IEthnicTypeService _ethnicTypeService;
        private readonly IVetranTypeService _vetranTypeService;
        private readonly IBankService _bankService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyDivisionService _companyDivisionService;
        private readonly ICompanyDepartmentService _companyDepartmentService;

        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        //private readonly IFranchiseContext _franchiseContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IPositionService _positionService;
        private readonly IShiftService _shiftService;
        private readonly IIntersectionService _intersectionService;
        private readonly IFranchiseService _franchiseService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly IMealPolicyService _mealPolicyService;
        private readonly IBreakPolicyService _breakPolicyService;
        private readonly IRoundingPolicyService _roundingPolicyService;
        private readonly IClockDeviceService _clockDeviceService;
        private readonly IPayGroupService _payGroupService;
        private readonly IRepository<EmployeeType> _employeeTypeRepository;
        private readonly IWSIBService _wSIBService;
        private readonly IPayrollCalendarService _payrollCalendarService;
        private readonly IPaymentHistoryService _paymentHistoryService;
        #endregion

        #region Constructors

        public CommonController(
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            IGenderService genderService,
            ISalutationService salutationService,
            ITransportationService transportationService,
            ISourceService sourceService,
            IAddressTypeService addressTypeService,
            IEthnicTypeService ethnicTypeService,
            IVetranTypeService vetranTypeService,
            IBankService bankService,
            ICompanyService companyService,
            ICompanyDivisionService companyDivisionService,
            ICompanyDepartmentService companyDepartmentService,
            ILanguageService languageService, 
            IWorkContext workContext,
            //IFranchiseContext franchiseContext,
            IPermissionService permissionService, 
            ILocalizationService localizationService,
            IPositionService positionService,
            IShiftService shiftService,
            IIntersectionService intersectionService,
            IFranchiseService franchiseService,
            ICompanyVendorService companyVendorService,
            IMealPolicyService mealPolicyService,
            IBreakPolicyService breakPolicyService,
            IRoundingPolicyService roundingPolicyService,
            IClockDeviceService clockDeviceService,
            IPayGroupService payGroupService,
            IRepository<EmployeeType> employeeTypeRepository,
            IWSIBService wSIBService,
            IPayrollCalendarService payrollCalendarService,
            IPaymentHistoryService paymentHistoryService)
        {
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _genderService = genderService;
            _salutationService = salutationService;
            _transportationService = transportationService;
            _sourceService = sourceService;
            _addressTypeService = addressTypeService;
            _ethnicTypeService = ethnicTypeService;
            _vetranTypeService = vetranTypeService;
            _bankService = bankService;
            _companyService = companyService;
            _companyDepartmentService = companyDepartmentService;
            _companyDivisionService = companyDivisionService;

            _languageService = languageService;
            _workContext = workContext;
            //_franchiseContext = franchiseContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _positionService = positionService;
            _shiftService = shiftService;
            _intersectionService = intersectionService;
            _franchiseService = franchiseService;
            _companyVendorService = companyVendorService;
            _mealPolicyService = mealPolicyService;
            _breakPolicyService = breakPolicyService;
            _roundingPolicyService = roundingPolicyService;
            _clockDeviceService = clockDeviceService;
            _payGroupService = payGroupService;
            _employeeTypeRepository = employeeTypeRepository;
            _wSIBService = wSIBService;
            _payrollCalendarService = payrollCalendarService;
            _paymentHistoryService = paymentHistoryService;
        }

        #endregion

        #region Methods

        //header links
        [ChildActionOnly]
        public ActionResult AdminHeaderLinks()
        {
            var account = _workContext.CurrentAccount;


            var model = new AdminHeaderLinksModel
            {
                IsAuthenticated = account != null,
                AccountEmailUsername = account == null ? string.Empty : account.Username
            };

            return PartialView(model);
        }

               
        ////language
        //[ChildActionOnly]
        //public ActionResult LanguageSelector()
        //{
        //    var model = new LanguageSelectorModel();
        //    model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
        //    model.AvailableLanguages = _languageService
        //        .GetAllLanguages(franchiseId: _franchiseContext.CurrentStore.Id)
        //        .Select(x => x.ToModel())
        //        .ToList();
        //    return PartialView(model);
        //}


        public ActionResult LanguageSelected(int customerlanguage)
        {
            var language = _languageService.GetLanguageById(customerlanguage);
            if (language != null)
            {
                _workContext.WorkingLanguage = language;
            }
            return Content("Changed");
        }

        #endregion


        //JsonResult Text/Id

        #region // JsonResult: GetCascadeCountries

        public JsonResult GetCascadeCountries()
        {
            var countries = _countryService.GetAllCountries();
            var countryDropDownList = new List<SelectListItem>();
            foreach (var c in countries)
            {
                var item = new SelectListItem()
                {
                    Text = c.CountryName,
                    Value = c.Id.ToString()
                };
                countryDropDownList.Add(item);
            }

            return Json(countryDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: GetCascadeStateProvinces

        public JsonResult GetCascadeStateProvinces(string countryId)
        {
            var stateProvinceDropDownList = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(countryId))
            {
                var stateProvinces = _stateProvinceService.GetAllStateProvincesByCountryId(Convert.ToInt32(countryId));

                foreach (var p in stateProvinces)
                {
                    var item = new SelectListItem()
                    {
                        Text = p.StateProvinceName,
                        Value = p.Id.ToString()
                    };
                    stateProvinceDropDownList.Add(item);
                }
            }
            return Json(stateProvinceDropDownList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region // JsonResult: GetCascadeCities

        public JsonResult GetCascadeCities(string stateProvinceId)
        {
            var cityDropDownList = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(stateProvinceId))
            {
                var citites = _cityService.GetAllCitiesByStateProvinceId(Convert.ToInt32(stateProvinceId));

                foreach (var p in citites)
                {
                    var item = new SelectListItem()
                    {
                        Text = p.CityName,
                        Value = p.Id.ToString()
                    };
                    cityDropDownList.Add(item);
                }
            }
            return Json(cityDropDownList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region // JsonResult: GetGenders

        public JsonResult GetGendersAsSelectList()
        {
            var genders = _genderService.GetAllGenders();
            var genderDropDownList = new List<SelectListItem>();
            foreach (var c in genders)
            {
                var item = new SelectListItem()
                {
                    Text = c.GenderName,
                    Value = c.Id.ToString()
                };
                genderDropDownList.Add(item);
            }

            return Json(genderDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: GetCascadeSalutations

        public JsonResult GetSalutations()
        {
            var salutations = _salutationService.GetAllSalutations();
            return Json(salutations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalutationsAsSelectList()
        {
            var salutations = _salutationService.GetAllSalutations();
            var salutationDropDownList = new List<SelectListItem>();
            foreach (var c in salutations)
            {
                var item = new SelectListItem()
                {
                    Text = c.SalutationName,
                    Value = c.Id.ToString()
                };
                salutationDropDownList.Add(item);
            }

            return Json(salutationDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: GetCascadeTransportations

        public JsonResult GetTransportationsAsSelectList()
        {
            var transportations = _transportationService.GetAllTransportations();
            var transportationDropDownList = new List<SelectListItem>();
            foreach (var c in transportations)
            {
                var item = new SelectListItem()
                {
                    Text = c.TransportationName,
                    Value = c.Id.ToString()
                };
                transportationDropDownList.Add(item);
            }

            return Json(transportationDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: GetCascadeSources

        public JsonResult GetCascadeSources()
        {
            var sources = _sourceService.GetAllSources();
            var sourceDropDownList = new List<SelectListItem>();
            foreach (var c in sources)
            {
                var item = new SelectListItem()
                {
                    Text = c.SourceName,
                    Value = c.Id.ToString()
                };
                sourceDropDownList.Add(item);
            }

            return Json(sourceDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: GetCascadeAddressTypes

        public JsonResult GetCascadeAddressTypes()
        {
            var addressTypes = _addressTypeService.GetAllAddressTypes();
            var addressTypeDropDownList = new List<SelectListItem>();
            foreach (var c in addressTypes)
            {
                var item = new SelectListItem()
                {
                    Text = c.AddressTypeName,
                    Value = c.Id.ToString()
                };
                addressTypeDropDownList.Add(item);
            }

            return Json(addressTypeDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: GetCascadeEthnicTypes

        public JsonResult GetCascadeEthnicTypes()
        {
            var ethnicTypes = _ethnicTypeService.GetAllEthnicTypes();
            var ethnicTypeDropDownList = new List<SelectListItem>();
            foreach (var c in ethnicTypes)
            {
                var item = new SelectListItem()
                {
                    Text = c.EthnicTypeName,
                    Value = c.Id.ToString()
                };
                ethnicTypeDropDownList.Add(item);
            }

            return Json(ethnicTypeDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: GetCascadeVetranTypes

        public JsonResult GetCascadeVetranTypes()
        {
            var vetranTypes = _vetranTypeService.GetAllVetranTypes();
            var vetranTypeDropDownList = new List<SelectListItem>();
            foreach (var c in vetranTypes)
            {
                var item = new SelectListItem()
                {
                    Text = c.VetranTypeName,
                    Value = c.Id.ToString()
                };
                vetranTypeDropDownList.Add(item);
            }

            return Json(vetranTypeDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region // JsonResult: GetCascadeBanks

        public JsonResult GetCascadeBanks()
        {
            var banks = _bankService.GetAllBanks();
            var bankDropDownList = new List<SelectListItem>();
            foreach (var c in banks)
            {
                var item = new SelectListItem()
                {
                    Text = c.BankName,
                    Value = c.Id.ToString()
                };
                bankDropDownList.Add(item);
            }

            return Json(bankDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region //JsonResult : GetCascadeCompanies

        public JsonResult GetCascadeCompanies()
        {
            var companies = _companyService.Secure_GetAllCompanies(_workContext.CurrentAccount);
            var companiesDropDownList = new List<SelectListItem>();
            if (companies.Count() > 0)
                companiesDropDownList = companies.Select(x => new SelectListItem() { Text = x.CompanyName, Value = x.Id.ToString() }).ToList();


            return Json(companiesDropDownList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCompaniesWithGuid()
        {
            var companies = _companyService.Secure_GetAllCompanies(_workContext.CurrentAccount);
            var companiesDropDownList = new List<Tuple<int, Guid, string>>();
            if (companies.Count() > 0)
                companiesDropDownList = companies.Select(x => new Tuple<int,Guid,string>(x.Id,x.CompanyGuid,x.CompanyName)).ToList();


            return Json(companiesDropDownList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCascadeCompaniesWithPunchClocks(bool activeOnly = true, bool excludeEnrolment = true)
        {
            var companies = _clockDeviceService.GetAllClockDevicesAsQueryable(activeOnly, excludeEnrolment).Select(x => x.CompanyLocation.Company).Distinct();
            var result = companies.Select(x => new SelectListItem()
            {
                Text = x.CompanyName,
                Value = x.Id.ToString()
            }).OrderBy(x => x.Text);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCascadeCompaniesWithHandPunchClocks(bool activeOnly = true, bool excludeEnrolment = true)
        {
            var companies = _clockDeviceService.GetAllClockDevicesWithIPAddress(activeOnly, excludeEnrolment).Select(x => x.CompanyLocation.Company).Distinct();
            var result = companies.Select(x => new SelectListItem()
            {
                Text = x.CompanyName,
                Value = x.Id.ToString()
            }).OrderBy(x => x.Text);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region JsonResult: CompanyLocations

        public JsonResult GetCascadeCompanyLocationsWithDefaultRow(string companyId)
        {
            var locationDropDownList = new List<SelectListItem>();
            // Add default zero value
            locationDropDownList.Add(new SelectListItem() { Text = "None", Value = "0" });

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                var locations = _companyDivisionService.GetAllCompanyLocationsByCompanyId(Convert.ToInt32(companyId)).OrderBy(x => x.LocationName);

                foreach (var c in locations)
                {
                    locationDropDownList.Add(new SelectListItem() { Text = c.LocationName, Value = c.Id.ToString() });
                }
            }
            return Json(locationDropDownList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeLocations(string companyId)
        {
            var locationDropDownList = new List<SelectListItem>();

            if (!string.IsNullOrWhiteSpace(companyId)&&companyId!="0")
            {
                var locations = _companyDivisionService.GetAllCompanyLocationsByCompanyId(Convert.ToInt32(companyId)).OrderBy(x => x.LocationName);

                foreach (var c in locations)
                {
                    locationDropDownList.Add(new SelectListItem() { Text = c.LocationName, Value = c.Id.ToString() });
                }
            }
            return Json(locationDropDownList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCascadeLocationsWithHandPunchClocks(int companyId, bool activeOnly = true, bool excludeEnrolment = true)
        {
            var result = _clockDeviceService.GetAllClockDevicesWithIPAddress(activeOnly, excludeEnrolment)
                .Where(x => x.CompanyLocation.CompanyId == companyId).Select(x => x.CompanyLocation).Distinct()
                .Select(x => new SelectListItem()
            {
                Text = x.LocationName,
                Value = x.Id.ToString()
            }).OrderBy(x => x.Text);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region GetCascadePositions
        public JsonResult GetCascadePositions(int companyId)
        {
            var positionList = new List<SelectListItem>();
            if (companyId > 0)
            {
                positionList = _positionService.GetAllPositionByCompanyId(companyId).Select(x => new SelectListItem() { Text = x.Code, Value = x.Id.ToString() }).ToList();
            }
            return Json(positionList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region JsonResult: Departments
        public JsonResult GetCascadeCompanyDepartments(string companyId)
        {
            var departmentsDropDownList = new List<SelectListItem>();
            // Add default zero value
            departmentsDropDownList.Add(new SelectListItem() { Text = "None", Value = "0" });

            if (!string.IsNullOrWhiteSpace(companyId))
            {
                var departments = _companyDepartmentService.GetAllCompanyDepartmentsByCompanyId(Convert.ToInt32(companyId));

                foreach (var c in departments)
                {
                    departmentsDropDownList.Add(new SelectListItem() { Text = c.DepartmentName, Value = c.Id.ToString() });
                }
            }
            return Json(departmentsDropDownList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeDepartments(string locationId)
        {
            var locId = String.IsNullOrEmpty(locationId) ? 0 : Convert.ToInt32(locationId);

            var departmentDropDownList = new List<SelectListItem>();
            // Add default zero value
            departmentDropDownList.Add(new SelectListItem() { Text = "None", Value = "0" });

            if (locId > 0)
            {
                var departments = _companyDepartmentService.GetAllCompanyDepartmentByLocationId(locId).OrderBy(x => x.DepartmentName);

                foreach (var c in departments)
                {
                    var item = new SelectListItem()
                    {
                        Text = c.DepartmentName,
                        Value = c.Id.ToString()
                    };
                    departmentDropDownList.Add(item);
                }
            }

            return Json(departmentDropDownList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Shift
        public JsonResult GetShiftsAsSelectList()
        {

            var resultDropDownList = _shiftService.GetAllShiftsForDropDownList();


            return Json(resultDropDownList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Major Intersection
        public JsonResult GetIntersectionsAsSelectList()
        {
            var result = _intersectionService.GetAllIntersections();
            var resultDropDownList = new List<SelectListItem>();
            foreach (var c in result)
            {
                var item = new SelectListItem()
                {
                    Text = c.IntersectionName,
                    Value = c.Id.ToString()
                };
                resultDropDownList.Add(item);
            }

            return Json(resultDropDownList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetAllLanguages

        public JsonResult GetAllLanguages()
        {
            var result = _languageService.GetAllLanguages().Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetAllFranchises

        public JsonResult GetAllVendors()
        {
            var result = _franchiseService.GetAllFranchises().Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.FranchiseName });
            if (_workContext.CurrentAccount.IsLimitedToFranchises)
                result = result.Where(x => x.Value == _workContext.CurrentAccount.FranchiseId.ToString());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetCascadeFranchises
        public JsonResult GetCascadeVendors(Guid? companyGuid)
        {
            var result =  _companyVendorService.GetAllCompanyVendorsByCompanyGuid(companyGuid).Select(x => new SelectListItem() { Value = x.Vendor.Id.ToString(), Text = x.Vendor.FranchiseName });
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCascadeVendorsWithGuid(Guid? companyGuid)
        {
            var result = _companyVendorService.GetAllCompanyVendorsByCompanyGuid(companyGuid).ToList()
                    .Select(x => new Tuple<int, Guid, string>(x.VendorId, x.Vendor.FranchiseGuid, x.Vendor.FranchiseName));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeAllVendors()
        {
            var franchises = _franchiseService.GetAllFranchises()
                                    .Where(x => !x.IsDefaultManagedServiceProvider)
                                    .Select(x => new Tuple<int, Guid, string>(x.Id, x.FranchiseGuid, x.FranchiseName));

            return Json(franchises, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetCascadePolicies
        public JsonResult GetCascadeMealPolicies(int companyId)
        {
            var result = _mealPolicyService.GetMealPoliciesByCompanyId(companyId).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeBreakPolicies(int companyId)
        {
            var result = _breakPolicyService.GetBreakPoliciesByCompanyId(companyId).Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeRoundingPolicies(int companyId)
        {
            var result = _roundingPolicyService.GetRoundingPoliciesByCompanyId(companyId).Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Cascade Clock Devices

        public JsonResult GetCascadeClockDevices(int companyLocationId, bool onlyWithIPAddress = false, bool excludeEnrolment = false)
        {
            var devices = _clockDeviceService.GetClockDevicesByCompanyLocationId(companyLocationId, excludeEnrolment)
                .Where(x => !onlyWithIPAddress || !(x.IPAddress == null || x.IPAddress == string.Empty));
            var result = devices.Select(x => new SelectListItem() { Text = x.ClockDeviceUid, Value = x.Id.ToString() });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        #endregion


        #region GetCascadeWSIBCode
        public JsonResult GetCascadeWSIBCode(int companyLocationId)
        {
            var codeList = new List<SelectListItem>();
            var locationProvince = _companyDivisionService.GetCompanyLocationById(companyLocationId);
            if (locationProvince != null)
            {
                codeList.AddRange( _wSIBService.GetAllWSIBsByProvinceId(locationProvince.StateProvinceId).Select(x => new SelectListItem() { Text=String.Concat(x.Code," - ",x.Description),Value=x.Code}).ToList());
            }

            return Json(codeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeWSIBCodeByStateProvinceId(int stateProvinceId)
        {
            var result = _wSIBService.GetAllWSIBsByProvinceId(stateProvinceId).Select(x => new SelectListItem() { Text = String.Concat(x.Code, " - ", x.Description), Value = x.Code });
            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetAllPayGroups

        public JsonResult GetAllPayGroups(int year=0, int vendorId=0)
        {
            if (vendorId == 0) vendorId = _workContext.CurrentFranchise.Id;
            if (year == 0) year = DateTime.Today.Year;

            var result = _payGroupService.GetAllPayGroups()
                                         .Where(x => x.FranchiseId == vendorId && x.Payroll_Calendar.Any(y => y.Year == year))
                                         .OrderBy(x=>x.Name);

            return Json(result.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllPayGroupsByVendor(int vendorId = 0)
        {
            if (vendorId == 0) vendorId = _workContext.CurrentFranchise.Id;
            var result = _payGroupService.GetAllPayGroups()
                                         .Where(x => x.FranchiseId == vendorId)
                                         .OrderBy(x => x.Name);

            return Json(result.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetAllEmployeeTypes

        public JsonResult GetAllEmployeeTypes()
        {
            var result = _employeeTypeRepository.TableNoTracking.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetAllPayPeriod
        public JsonResult GetAllPayPeriod(string paygroupIds,int year)
        {
            var query = _payrollCalendarService.GetPayrollCalendarByYearAndPayGroupIds(year, paygroupIds, true).ToList()
                .Select(x => new SelectListItem() { Text=String.Concat(x.PayPeriodStartDate.ToString("dd MMM yyyy")," --> ",x.PayPeriodEndDate.ToString("dd MMM yyyy")),Value=x.Id.ToString()});
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetAllPayrollBatches

        public ActionResult GetAllPayrollBatches(int payrollCalendarId, string companies)
        {
            int[] companyds = companies.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();

            var query = _paymentHistoryService.GetPayrollBatchByPayCalendarId(payrollCalendarId)
                                              .Where(x => companyds.Contains(x.CompanyId.Value))
                                              .Select(x => new SelectListItem() { Text=x.Id.ToString(),Value=x.Id.ToString()});
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }

}
