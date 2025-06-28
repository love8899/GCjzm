using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Accounts;
using Wfm.Admin.Models.Franchises;
using Wfm.Core;
using Wfm.Core.Domain.Franchises;
using Wfm.Services.Accounts;
using Wfm.Services.Companies;
using Wfm.Services.Logging;
using Wfm.Services.Franchises;
using Wfm.Services.Localization;
using Wfm.Services.Messages;
using Wfm.Services.Security;
using Wfm.Web.Framework.Controllers;
using System;
using System.Web;
using Wfm.Services.Media;
using Wfm.Web.Framework;
using Wfm.Services.Payroll;
using Wfm.Core.Domain.Payroll;
using Wfm.Admin.Models.Payroll;


namespace Wfm.Admin.Controllers
{
    public class VendorController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly ILogger _logger;
        private readonly IFranchiseService _franchiseService;
        private readonly IFranchiseSettingService _franchiseSettingService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IAccountService _accountService;
        private readonly ICompanyVendorService _companyVendorService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly VendorMassEmailSettings _vendorMassEmailSettings;
        private readonly IVendorCertificateService _vendorCertificateService;
        private readonly CertificateBL _certificateBL;
        private readonly IFranciseAddressService _franchiseAddressService;
        private readonly IPayrollSettingService _payrollSettingService;
        #endregion

        #region Ctor

        public VendorController(IActivityLogService activityLogService, 
                                ILogger logger,
                                IFranchiseService franchiseService,
                                IFranchiseSettingService franchiseSettingService,
                                IWorkContext workContext, 
                                IPermissionService permissionService, 
                                ILocalizationService localizationService, 
                                IAccountService accountService, 
                                ICompanyVendorService companyVendorService, 
                                IRecruiterCompanyService recruiterCompanyService, 
                                IEmailAccountService emailAccountService, 
                                VendorMassEmailSettings vendorMassEmailSettings, 
                                IAttachmentTypeService attachmentTypeService,
                                IVendorCertificateService vendorCertificateService,
                                IFranciseAddressService franchiseAddressService,
                                IPayrollSettingService payrollSettingService)
        {
            _activityLogService = activityLogService;
            _logger = logger;
            _franchiseService = franchiseService;
            _franchiseSettingService = franchiseSettingService;
            _workContext = workContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _accountService = accountService;
            _companyVendorService = companyVendorService;
            _recruiterCompanyService = recruiterCompanyService;
            _emailAccountService = emailAccountService;
           
            _vendorMassEmailSettings = vendorMassEmailSettings;
            _vendorCertificateService = vendorCertificateService;
            _certificateBL = new CertificateBL(vendorCertificateService, attachmentTypeService);
            _franchiseAddressService = franchiseAddressService;
            _payrollSettingService = payrollSettingService;
        }

        #endregion

        #region  GET :/Vendor/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            // Franchise access only
            if (_workContext.CurrentAccount.IsLimitedToFranchises == true)
                return RedirectToAction("Details", new { guid = _workContext.CurrentFranchise.FranchiseGuid });

            return View();
        }
        #endregion

        #region POST:/Vendor/Index
        [HttpPost]
        public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            // Franchise access only
            if (_workContext.CurrentAccount.IsLimitedToFranchises == true)
                return RedirectToAction("Details", new { guid = _workContext.CurrentFranchise.FranchiseGuid });

            var franchises = _franchiseService.GetAllFranchisesAsQueryable(_workContext.CurrentAccount, true);

            return Json(franchises.ProjectTo<FranchiseModel>().ToDataSourceResult(request));
        }

        public ActionResult GetAllVendors(int companyId = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var franchises = _companyVendorService.GetAllCompanyVendorsByCompanyIdAsSelectList(companyId);

            return Json(franchises, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GET :/Vendor/Create

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (_workContext.CurrentAccount.IsLimitedToFranchises)
                return AccessDeniedView();

            var businessLogic = new CreateEditVendor_BL();
            var franchiseModel = businessLogic.GetNewVendorModel(_workContext.CurrentAccount);
            return View(franchiseModel);
        }

        #endregion

        #region POST:/Vendor/Create

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(FranchiseModel franchiseModel, bool continueEditing, IEnumerable<HttpPostedFileBase> files)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (_workContext.CurrentAccount.IsLimitedToFranchises)
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var businessLogic = new CreateEditVendor_BL();
                //franchiseModel = businessLogic.PopulateVendorModel(franchiseModel, _workContext.CurrentAccount.Id);
                var franchise = businessLogic.SaveNewVendor(_franchiseService, franchiseModel,files);

                //activity log
                _activityLogService.InsertActivityLog("AddNewVendor", _localizationService.GetResource("ActivityLog.AddNewVendor"), franchise.FranchiseName);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.Vendor.Added"));

                //Notification message
                return continueEditing ? RedirectToAction("Edit", new { guid = franchise.FranchiseGuid }) : RedirectToAction("Index");
            }
             
            // if error happens
            return View(franchiseModel);
        }
        #endregion

        #region GET :/Vendor/Edit/5

        public ActionResult Edit(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
           
            if (guid == null)
            {
                ErrorNotification("The vendor does not exist!");
                return RedirectToAction("Index");
            }

            if (_workContext.CurrentAccount.IsLimitedToFranchises && _workContext.CurrentFranchise.FranchiseGuid != guid)
                return RedirectToAction("Details", new { guid = _workContext.CurrentFranchise.FranchiseGuid }); 

            string error = string.Empty;
            var businessLogic = new CreateEditVendor_BL();
            FranchiseModel model = businessLogic.GetVendorModel(guid, _franchiseService, _accountService, out error);
            if (error.Length > 0)
            { 
                ErrorNotification(error);
                return RedirectToAction("Index"); 
            }
            return View(model);
        }

        #endregion

        #region POST:/Vendor/Edit/5
        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(FranchiseModel franchiseModel, bool continueEditing, IEnumerable<HttpPostedFileBase> files)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (_workContext.CurrentAccount.IsLimitedToFranchises && _workContext.CurrentFranchise.Id != franchiseModel.Id)
                return RedirectToAction("Details", new { guid = _workContext.CurrentFranchise.FranchiseGuid });

            if (ModelState.IsValid)
            {
                var businessLogic = new CreateEditVendor_BL();
                var franchise = businessLogic.UpdateVendor(_franchiseService, franchiseModel,files);

                //activity log
                _activityLogService.InsertActivityLog("UpdateVendorProfile", _localizationService.GetResource("ActivityLog.UpdateVendorProfile"), franchise.FranchiseName);

                //Notification message
                SuccessNotification(_localizationService.GetResource("Admin.Vendors.Vendor.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { guid = franchise.FranchiseGuid }) : RedirectToAction("Index");
            }

            return View(franchiseModel);
        }

        #endregion

        #region GET :/Vendor/Details/

        public ActionResult Details(Guid? guid, string tabId = "tab-basic-info")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
           
            if (guid == null)
            {
                ErrorNotification("The vendor does not exist!");
                return RedirectToAction("Index");
            }
           
            if (_workContext.CurrentAccount.IsLimitedToFranchises && _workContext.CurrentFranchise.FranchiseGuid != guid)
                return RedirectToAction("Details", new { guid = _workContext.CurrentFranchise.FranchiseGuid });

            Franchise franchise = _franchiseService.GetFranchiseByGuid(guid);
            FranchiseModel mode = franchise.ToModel();

            ViewBag.TabId = tabId;
            ViewBag.FranchiseGuid = franchise.FranchiseGuid;
            ViewBag.FranchiseId = franchise.Id;
            return View(mode);
        }

        #endregion

        #region /Vendor/_CertificateList
        [HttpPost]
        public ActionResult _CertificateList(DataSourceRequest request, Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
            if (guid == null)
            {
                ErrorNotification("The vendor does not exist!");
                return RedirectToAction("Index");
            }
            var vendor = _franchiseService.GetFranchiseByGuid(guid);
            if (vendor == null)
                return Json(null);
            var entities = _vendorCertificateService.GetAllCertificatesByVendorId(vendor.Id).ToList();
            var models = AutoMapper.Mapper.Map<List<VendorCertificate>, List<VendorCertificateModel>>(entities).OrderByDescending(x => x.GeneralLiabilityCertificateExpiryDate);
            return Json(models.ToDataSourceResult(request));
        }
        #endregion

        // Accounts

        #region GET :/VendorDetails/_TabVendorAccountList

        [HttpGet]
        public ActionResult _TabVendorAccountList(Guid vendorGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAccounts))
                return AccessDeniedView();

            ViewBag.VendorGuid = vendorGuid;
            ViewBag.ReturnPath = "~/Admin/" + this.ControllerContext.RouteData.Values["controller"].ToString() + "/Details?guid=" + vendorGuid + "&tabId=tab-accounts";

            return View();
        }

        #endregion

        #region Save Certificate
        [HttpPost]
        public ActionResult _SaveVendorCertificate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<VendorCertificateModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    entity.UpdatedOnUtc = DateTime.UtcNow;
                    _vendorCertificateService.Update(entity, new string[] { "CertificateFileName", "ContentType", "CertificateFile" });
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region Add New Certificate
        [HttpPost]
        public ActionResult _CreateNewCertificate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<VendorCertificateModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
            var results = new List<VendorCertificateModel>();
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = model.ToEntity();
                    _vendorCertificateService.Create(entity);
                    results.Add(entity.ToModel());
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Delete a certificate
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _DeleteCertificate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<VendorCertificateModel> models)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
            if (models.Any())
            {
                foreach (var model in models)
                {
                    _vendorCertificateService.Delete(model.ToEntity());
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
        #endregion
        #region Upload Certificate
        public ActionResult _UploadCertificate(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
            var entity = _vendorCertificateService.Retrive(guid);
            if (entity == null)
            {
                ErrorNotification("The certificate does not exist!");
                return RedirectToAction("Details", new { guid = guid });
            }

            return PartialView(entity.ToModel());
        }
        #endregion

        #region Save Certificate File
        public ActionResult SaveCertificateFile(HttpPostedFileBase attachment, Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
            string error;
            bool result = _certificateBL.SaveCertificateFiles(attachment, guid, out error);
            if (result)
            {
                SuccessNotification("Uploaded sucessfully!");
            }
            else
            {
                ErrorNotification(error);
            }
            return new EmptyResult();
        }
        #endregion

        #region Download Certificate
        public ActionResult _DownloadCertificate(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
            if (guid == null)
            {
                ErrorNotification("The certificate does not exist!");
                return new EmptyResult();
            }
            var certificate = _vendorCertificateService.Retrive(guid);
            if (certificate == null)
            {
                ErrorNotification("The certificate does not exist!");
                return new EmptyResult();
            }
            if (certificate.HasCertificate)
            {
                return File(certificate.CertificateFile, certificate.ContentType, certificate.CertificateFileName);
            }
            else
            {
                ErrorNotification("The certificate file does not exist!");
                return new EmptyResult();
            }
        }
        #endregion

        #region GET :/Vendor/MassEmail

        public ActionResult MassEmail()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MassEmailToVendors))
                return AccessDeniedView();

            return View();
        }

        #endregion

        #region POST:/Vendor/MassEmail

        [HttpPost]
        public ActionResult _GetTargetVendorAccounts([DataSourceRequest] DataSourceRequest request, VendorAccountSelector selector)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var _vendorBL = new CreateEditVendor_BL();
            var result = _vendorBL.GetVendorAccountListBySelector(_accountService, _recruiterCompanyService, _workContext.CurrentAccount, selector)
                         .ProjectTo<AccountsViewModel>();

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult _MassEmail2Selected(bool systemAccount, string subject, string message, string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MassEmailToVendors))
                return AccessDeniedView();

            int done = 0, failed = 0;
            string errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(selectedIds))
            {
                var ids = selectedIds.Split(',').Select(x => int.Parse(x)).ToList();

                var _vendorBL = new CreateEditVendor_BL();
                _vendorBL.SendMassEmailToSelectedVendorAccounts(_emailAccountService, _vendorMassEmailSettings, _workContext.CurrentAccount, ids, subject, message, systemAccount);

                done = ids.Count();
            }
            else
                errorMessage += "\r\n No candidates are selected.";

            return Json(new { Done = done, Failed = failed, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _MassEmail2All(bool systemAccount, string subject, string message, VendorAccountSelector selector)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MassEmailToVendors))
                return AccessDeniedView();

            int done = 0, failed = 0;
            string errorMessage = string.Empty;
            if (selector != null)
            {
                var _vendorBL = new CreateEditVendor_BL();
                var candidates = _vendorBL.GetVendorAccountListBySelector(_accountService, _recruiterCompanyService, _workContext.CurrentAccount, selector);
                var ids = candidates.Select(x => x.Id).ToList();
                done = ids.Count();

                _vendorBL.SendMassEmailToSelectedVendorAccounts(_emailAccountService, _vendorMassEmailSettings, _workContext.CurrentAccount, ids, subject, message, systemAccount);
            }
            else
                errorMessage += "No candidates are selected.\r\n";

            return Json(new { Done = done, Failed = failed, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region TabVendorAddress
        public ActionResult _TabVendorAddress(Guid? vendorGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();
            var franchise = _franchiseService.GetFranchiseByGuid(vendorGuid);
            if (franchise == null)
            {
                ErrorNotification("Unable to load data!");
                return new EmptyResult();
            }
            ViewBag.FranchiseGuid = vendorGuid;           
            return PartialView();
        }

        [HttpPost]
        public ActionResult _AddressList([DataSourceRequest] DataSourceRequest request, Guid? vendorGuid)
        {
            var addresses = _franchiseAddressService.GetAllFranchiseAddressByFranchiseGuid(vendorGuid, true);
            var addressModels = addresses.Select(x => x.ToModel()).ToList();           
            return Json(addressModels.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _CreateNewAddress([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<FranchiseAddressModel> addresses, Guid? vendorGuid)
        {
            var results = new List<FranchiseAddressModel>();
            var franchise = _franchiseService.GetFranchiseByGuid(vendorGuid);
            if (franchise != null && addresses != null && ModelState.IsValid)
            {
                var prevAddress = _franchiseAddressService.GetAllFranchiseAddressByFranchiseGuid(vendorGuid, false).Where(s => s.IsHeadOffice).FirstOrDefault();
                 
                foreach (var addressModel in addresses)
                {
                    if (addressModel.IsHeadOffice && prevAddress != null)
                        ModelState.AddModelError("Id", _localizationService.GetLocaleStringResourceByName("Franchise.FranchiseAddress.HeadOfficeWarning").ResourceValue);
                    else
                    {
                        var address = addressModel.ToEntity();
                        address.EnteredBy = _workContext.CurrentAccount.Id;
                        address.DisplayOrder = 0;
                        address.IsDeleted = false;
                        address.FranchiseId = franchise.Id;
                        address.UpdatedOnUtc = System.DateTime.UtcNow;
                        address.CreatedOnUtc = System.DateTime.UtcNow;
                        CleanUpAddress(address);
                        //create
                        _franchiseAddressService.Create(address);
                        addressModel.Id = address.Id;
                        results.Add(addressModel);
                    }
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _EditAddress([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<FranchiseAddressModel> addresses, Guid? vendorGuid)
        {
            var franchise = _franchiseService.GetFranchiseByGuid(vendorGuid);
            if (franchise!=null && addresses != null && ModelState.IsValid)
            {
                if (addresses.Where(s => s.IsHeadOffice && s.IsActive).Count() > 1)
                    ModelState.AddModelError("Id", _localizationService.GetLocaleStringResourceByName("Franchise.FranchiseAddress.HeadOfficeWarning").ResourceValue);
                
                else
                {
                    var prevAddress = _franchiseAddressService.GetAllFranchiseAddressByFranchiseGuid(vendorGuid, false).Where(s => s.IsHeadOffice).FirstOrDefault();

                  //if previous headoffice address is not null and edit list does not contain that id and there is new head office the return.
                    if (prevAddress != null && addresses.Where(a => a.Id == prevAddress.Id).FirstOrDefault() == null && addresses.Where(s => s.IsHeadOffice && s.IsActive).Select(a => a.Id).Count() > 0)
                        ModelState.AddModelError("Id", _localizationService.GetLocaleStringResourceByName("Franchise.FranchiseAddress.HeadOfficeWarning").ResourceValue);
                    else
                    {
                        foreach (var address in addresses)
                        {
                            address.FranchiseId = franchise.Id;
                            var entity = _franchiseAddressService.Retrieve(address.Id);
                            address.ToEntity(entity);
                            CleanUpAddress(entity);
                            entity.UpdatedOnUtc = System.DateTime.UtcNow;
                            _franchiseAddressService.Update(entity);
                        }
                    }
                }
            }

            return Json(addresses.ToDataSourceResult(request, ModelState));
        }
  
        private void CleanUpAddress(FranchiseAddress address)
        {
            address.PrimaryPhone = address.PrimaryPhone.ExtractNumericText();
            address.SecondaryPhone = address.SecondaryPhone.ExtractNumericText();
            address.FaxNumber = address.FaxNumber.ExtractNumericText();
            address.PostalCode = address.PostalCode.ToUpper().Replace(" ", "");
        }
        #endregion


        #region Settings

        [HttpGet]
        public ActionResult _TabVendorSettings(Guid vendorGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            ViewBag.VendorGuid = vendorGuid;

            return View();
        }


        [HttpPost]
        public ActionResult Settings([DataSourceRequest] DataSourceRequest request, Guid vendorGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var franchiseId = _franchiseService.GetFranchiseByGuid(vendorGuid).Id;
            var settings = _franchiseSettingService.GetAllSettings(franchiseId);

            var result = settings.ProjectTo<FranchiseSettingModel>();

            return Json(result.ToDataSourceResult(request));
        }


        public ActionResult _EditSetting(int settingId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var setting = _franchiseSettingService.GetSettingById(settingId);

            return PartialView(setting.ToModel());
        }


        [HttpPost]
        public ActionResult _SaveSetting(FranchiseSettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var errorMessage = new StringBuilder();
            var result = true;

            if (ModelState.IsValid)
            {
                try
                {
                    var setting = _franchiseSettingService.GetSettingById(model.Id);
                    model.ToEntity(setting);
                    setting.UpdatedOnUtc = DateTime.UtcNow;

                    _franchiseSettingService.UpdateSetting(setting);
                }
                catch (Exception exc)
                {
                    errorMessage.AppendLine(exc.Message);
                    result = false;
                }
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                errorMessage.AppendLine(String.Join(" | ", errors.Select(o => o.ToString()).ToArray()));
                result = false;
            }

            return Json(new { Result = result, ErrorMessage = errorMessage.ToString() }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Smart Card Customization

        [HttpGet]
        public ActionResult _TabSmartCard(Guid vendorGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var model = new SmartCardCustomizationModel();

            var franchise = _franchiseService.GetFranchiseByGuid(vendorGuid);
            var logoBytes = franchise.FranchiseLogo;
            if (logoBytes != null)
                model.LogoStr = Convert.ToBase64String(logoBytes);

            ViewBag.VendorGuid = vendorGuid;
            var style = _franchiseSettingService.GetSettingByKey<string>(franchise.Id, "SmartCardSettings.Style");
            if (!String.IsNullOrWhiteSpace(style))
                ViewBag.Style = style.Replace("\r", string.Empty).Replace("\n", string.Empty);

            var namePrefix = _franchiseSettingService.GetSettingByKey<string>(franchise.Id, "SmartCardSettings.NamePrefix");
            model.NamePrefix = String.IsNullOrWhiteSpace(namePrefix) ? String.Empty : namePrefix;
            var idPrefix = _franchiseSettingService.GetSettingByKey<string>(franchise.Id, "SmartCardSettings.IdPrefix");
            model.IdPrefix = String.IsNullOrWhiteSpace(idPrefix) ? String.Empty : idPrefix;
            var noteText = _franchiseSettingService.GetSettingByKey<string>(franchise.Id, "SmartCardSettings.NoteText");
            model.NoteText = String.IsNullOrWhiteSpace(noteText) ? String.Empty : noteText.Replace("\r\n", "<br/>");

            return View(model);
        }


        [HttpPost]
        public ActionResult _SaveSmartCardSetting(Guid vendorGuid, SmartCardCustomizationModel model)
        {
            try
            {
                var smartcardCustomization_BL = new SmartCardCustomization_BL();
                smartcardCustomization_BL.SaveSmartCardSetting(_franchiseService, _franchiseSettingService, model, vendorGuid);

                SuccessNotification("Smart card setting is saved successfully.");
            }
            catch (Exception ex)
            {
                ErrorNotification("Failed to save smart card setting.");
                _logger.Error(String.Format("Error saving smart card setting by user {0}", _workContext.CurrentAccount.Id), ex, userAgent: Request.UserAgent);
            }

            return RedirectToAction("Details", new { guid = vendorGuid, tabid = "tab-smartcard" });
        }


        public ActionResult _GetSmartCardLayout()
        {
            return PartialView("_SmartCard");
        }


        [HttpPost]
        public ActionResult _GetSmartCardVendorInformation(int franchiseId)
        {
            var style = _franchiseSettingService.GetSettingByKey<string>(franchiseId, "SmartCardSettings.Style");
            if (!String.IsNullOrWhiteSpace(style))
                style = style.Replace("\r", string.Empty).Replace("\n", string.Empty);

            var namePrefix = _franchiseSettingService.GetSettingByKey<string>(franchiseId, "SmartCardSettings.NamePrefix");
            var idPrefix = _franchiseSettingService.GetSettingByKey<string>(franchiseId, "SmartCardSettings.IdPrefix");
            var noteText = _franchiseSettingService.GetSettingByKey<string>(franchiseId, "SmartCardSettings.NoteText");

            var logoStr = String.Empty;
            var franchise = _franchiseService.GetFranchiseById(franchiseId);
            var logoBytes = franchise.FranchiseLogo;
            if (logoBytes != null)
                logoStr = Convert.ToBase64String(logoBytes);

            return Json(new { Success = !String.IsNullOrWhiteSpace(style), Style = style, LogoStr = logoStr, 
                              NamePrefix = namePrefix ?? String.Empty, IdPrefix = idPrefix ?? String.Empty, NoteText = noteText ?? String.Empty });
        }

        #endregion


        #region _TabVendorPayrollSetting
        public ActionResult _TabVendorPayrollSetting(Guid? vendorGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorPayrollSetting))
                return AccessDeniedView();
            var setting = _payrollSettingService.GetPayrollSettingByFranchiseId(vendorGuid);
            return PartialView(setting);
        }

        public ActionResult EditPayrollSetting(Guid? guid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorPayrollSetting))
                return AccessDeniedView();
            var setting = _payrollSettingService.GetPayrollSettingByFranchiseId(guid);
            if (setting == null)
                return View(new PayrollSetting());
            return View(setting);
        }

        [HttpPost]
        public ActionResult EditPayrollSetting(PayrollSetting model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorPayrollSetting))
                return AccessDeniedView();
            //save data
            ValidatePayrollSetting(model);
            if (ModelState.IsValid)
            {
                bool sucess = _payrollSettingService.UpdatePayrollSetting(model);
                if (sucess)
                {
                    SuccessNotification("Payroll Setting has been updated sucessfully!");
                    return RedirectToAction("Details", new { guid = model.FranchiseGuid, tabId = "tab-vendor-payroll-setting" });
                }
                else
                    ErrorNotification("Fail to update payroll setting!");
            }
            return View(model);
        }

        [NonAction]
        private void ValidatePayrollSetting(PayrollSetting setting)
        {
            ModelState.Clear();
            if (String.IsNullOrWhiteSpace(setting.Client_Number))
            {
                ModelState.AddModelError("Client_Number", "Client Number is required!");
            }
            if (!setting.EIRate.HasValue || setting.EIRate <= 0)
            {
                ModelState.AddModelError("EIRate", "EI rate must be positive!");
            }
            if (String.IsNullOrWhiteSpace(setting.DDFileLayout))
            {
                ModelState.AddModelError("DDFileLayout", "File layout is required!");
            }
            if (setting.DDFileLayout == "CIBC_80Byte")
            {
                if (String.IsNullOrWhiteSpace(setting.TransitNumber))
                {
                    ModelState.AddModelError("TransitNumber", "Transit Number is required!");
                }
                if (String.IsNullOrWhiteSpace(setting.AccountNumber))
                {
                    ModelState.AddModelError("AccountNumber", "Account Number is required!");
                }
                if (String.IsNullOrWhiteSpace(setting.InstitutionNumber))
                {
                    ModelState.AddModelError("InstitutionNumber", "Institution Number is required!");
                }
            }
        }
        #endregion

        #region _TabVendorEmailSetting
        public ActionResult _TabVendorEmailSetting(Guid? vendorGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewVendorEmailSetting))
                return AccessDeniedView();
            ViewBag.VendorGuid = vendorGuid;
            return PartialView();
        }

        [HttpPost]
        public ActionResult _TabVendorEmailSetting([DataSourceRequest] DataSourceRequest request,Guid? vendorGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewVendorEmailSetting))
                return AccessDeniedView();
            var result =  _payrollSettingService.GetPayrollEmailSetting(vendorGuid);
            return Json(result.ToDataSourceResult(request,x=>x.ToModel()));
        }

        public ActionResult _CreateOrUpdateEmailSetting(Guid? guid,string code=null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewVendorEmailSetting))
                return AccessDeniedView();
            var franchise = _franchiseService.GetFranchiseByGuid(guid);
            if (franchise == null)
            {
                ErrorNotification("Fail to load data!");
                return RedirectToAction("Index");
            }
            //add
            if (String.IsNullOrWhiteSpace(code))
            {
                if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorEmailSetting))
                    return AccessDeniedView();
                PayrollEmailSettingDetailModel details = new PayrollEmailSettingDetailModel();
                details.FranchiseGuid = franchise.FranchiseGuid;
                details.Simple = false;
                details.FranchiseId = franchise.Id;
                return PartialView(details);
            }
            //edit
            else
            {
                var entity = _payrollSettingService.GetPayrollEmailSetting(guid,code);
                if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorEmailSetting))
                    return PartialView(entity.ToModel());
                else
                    return PartialView(entity.ToDetailedModel());
            }
        }

        [HttpPost]
        public ActionResult _SaveEmailSetting(PayrollEmailSettingDetailModel model)
        {
            ValidateEmailSetting(model);
            if (ModelState.IsValid)
            {
                bool sucess = false;
                if (!model.Simple)
                {
                    sucess = _payrollSettingService.UpdatePayrollEmailSetting(model.ToEntity(),_workContext.CurrentAccount.Id);
                }
                else 
                {
                    var entity = _payrollSettingService.GetPayrollEmailSetting(model.FranchiseGuid, model.Code);
                    entity.Code = model.Code;
                    entity.EmailAddress = model.EmailAddress;
                    entity.EmailSubject = model.EmailSubject;
                    entity.EmailBody = model.EmailBody;
                    sucess = _payrollSettingService.UpdatePayrollEmailSetting(entity,_workContext.CurrentAccount.Id);
                }
                if (sucess)
                    return Json(new { Error = false, Message = "Email Setting has been saved sucessfully!" });
                else
                    return Json(new { Error = true, Message = "Fail to save email setting!" });
            }
            var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
            string msg = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
            return Json(new { Error = true,Message = msg});
        }

        [NonAction]
        private void ValidateEmailSetting(PayrollEmailSettingDetailModel model)
        {
            ModelState.Clear();
            if (String.IsNullOrWhiteSpace(model.EmailAddress) )
            {
                ModelState.AddModelError("EmailAddress", "Email address cannot be empty!");
            }
            else 
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(model.EmailAddress);
                    if(addr.Address != model.EmailAddress)
                        ModelState.AddModelError("EmailAddress", "Email address is not valid!");
                }
                catch
                {
                    ModelState.AddModelError("EmailAddress", "Email address is not valid!");
                }
            }

            if (String.IsNullOrWhiteSpace(model.EmailSubject))
            {
                ModelState.AddModelError("EmailSubject", "Email subject is required!");
            }

            if (String.IsNullOrWhiteSpace(model.EmailBody))
            {
                ModelState.AddModelError("EmailBody", "Email body is required!");
            }
            
            if (!model.Simple)
            {
                if (String.IsNullOrWhiteSpace(model.UserName))
                {
                    ModelState.AddModelError("UserName", "Username is required!");
                }
                if (String.IsNullOrWhiteSpace(model.EmailSmtpClient))
                {
                    ModelState.AddModelError("EmailStmpClient", "Smtp Client is required!");
                }

                if (String.IsNullOrWhiteSpace(model.EmailPassword))
                {
                    ModelState.AddModelError("EmailPassword", "Password is required!");
                }


            }
        }
        #endregion
    }
}
