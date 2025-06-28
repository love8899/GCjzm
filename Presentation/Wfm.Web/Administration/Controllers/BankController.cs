using Wfm.Core;
using Wfm.Services.Common;
using Wfm.Services.Logging;
using Wfm.Services.Localization;
using Wfm.Services.Security;

namespace Wfm.Admin.Controllers
{
    public class BankController : BaseAdminController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IActivityLogService _activityLogService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IBankService _bankService;

        #endregion

        #region Ctor
        public BankController(
            IWorkContext workContext,
            IActivityLogService activityLogService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IBankService bankService)
        {
            _workContext = workContext;
            _activityLogService = activityLogService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _bankService = bankService;
        }
        #endregion

        //#region GET :/Bank/Index
        //public ActionResult Index()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanks))
        //        return AccessDeniedView();

        //    var bankList =  _bankService.GetAllBanks(true);

        //    List<BankModel> bankModelList = new List<BankModel>();
        //    foreach (var item in bankList)
        //    {
        //        BankModel model = MappingExtensions.ToModel(item);
        //        bankModelList.Add(model);
        //    }

        //    return View(bankModelList);
        //}
        //#endregion

        //#region POST:/Bank/Index
  
        //[HttpPost]
        //public ActionResult Index([DataSourceRequest] DataSourceRequest request)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanks))
        //        return AccessDeniedView();

        //    var bankList = _bankService.GetAllBanks(true);

        //    List<BankModel> bankModelList = new List<BankModel>();

        //    foreach (var item in bankList)
        //    {
        //        BankModel c = MappingExtensions.ToModel(item);
        //        bankModelList.Add(c);
        //    }

        //    // Initialize the DataSourceResult
        //    var result = new DataSourceResult()
        //    {
        //        Data = bankModelList, // Process data (paging and sorting applied)
        //        Total = bankList.Count // Total number of records
        //    };

        //    return Json(result);
        //}

        //#endregion

        //#region  GET:/Bank/Create
        //public ActionResult Create()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanks))
        //        return AccessDeniedView();

        //    BankModel model = new BankModel();

        //    model.IsActive = true;
        //    model.UpdatedOnUtc = System.DateTime.UtcNow;
        //    model.CreatedOnUtc = System.DateTime.UtcNow;

        //    return View(model);
        //}
        //#endregion

        //#region POST:/Bank/Create
        //[HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        //public ActionResult Create(BankModel model, bool continueEditing)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanks))
        //        return AccessDeniedView();

        //    if (ModelState.IsValid)
        //    {
        //        Bank bank = model.ToEntity();
        //        bank.EnteredBy = _workContext.CurrentAccount.Id;
        //        bank.UpdatedOnUtc = System.DateTime.UtcNow;
        //        bank.CreatedOnUtc = System.DateTime.UtcNow;

        //        _bankService.InsertBank(bank);

        //        //activity log
        //        _activityLogService.InsertActivityLog("AddNewBank", _localizationService.GetResource("ActivityLog.AddNewBank"), model.BankName);

        //        //Notification message
        //        SuccessNotification(_localizationService.GetResource("Admin.Configuration.Bank.Added"));
        //        return continueEditing ? RedirectToAction("Edit", new { id = bank.Id }) : RedirectToAction("Index");
        //    }

        //    //If we got this far, something failed, redisplay form
        //    return View(model);
        //}
        //#endregion

        //#region GET :/Bank/Edit
        //public ActionResult Edit(int id)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanks))
        //        return AccessDeniedView();

        //    Bank bank = _bankService.GetBankById(id);
        //    if (bank == null)
        //        //No bank found with the specified id
        //        return RedirectToAction("Index");

        //    BankModel model = bank.ToModel();
        //    model.UpdatedOnUtc = System.DateTime.UtcNow;

        //    return View(model);
        //}
        //#endregion

        //#region POST:/Bank/Edit
        //[HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        //public ActionResult Edit(BankModel model, bool continueEditing)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageBanks))
        //        return AccessDeniedView();

        //    Bank bank= _bankService.GetBankById(model.Id);
        //    if (bank == null)
        //        //No bank found with the specified id
        //        return RedirectToAction("Index");


        //    if (ModelState.IsValid)
        //    {
        //        // the fellowing step is mapping bankModel contents to a entity name as bank
        //        // and assignment to Bank
        //        bank = model.ToEntity(bank);
        //        bank.UpdatedOnUtc = System.DateTime.UtcNow;


        //        _bankService.UpdateBank(bank);

        //        //activity log
        //        _activityLogService.InsertActivityLog("UpdateBank", _localizationService.GetResource("ActivityLog.UpdateBank"), model.BankName);


        //        SuccessNotification(_localizationService.GetResource("Admin.Configuration.Bank.Updated"));
        //        return continueEditing ? RedirectToAction("Edit", new { id = bank.Id }) : RedirectToAction("Index");
        //    }

        //    //If we got this far, something failed, redisplay form
        //    return View(model);
        //}
        //#endregion

    }
}

