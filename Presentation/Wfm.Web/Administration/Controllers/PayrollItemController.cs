using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Services.Payroll;
using Kendo.Mvc.Extensions;
using Wfm.Admin.Extensions;
using Wfm.Admin.Models.Payroll;

namespace Wfm.Admin.Controllers
{
    public class PayrollItemController : BaseAdminController
    {
        private readonly IPayrollItemService _payrollItemService;
        private readonly IWorkContext _workContext;
        private readonly ITaxFormBoxService _taxFormBoxService;
        public PayrollItemController(IPayrollItemService payrollItemService, IWorkContext workContext, ITaxFormBoxService taxFormBoxService)
        {
            _payrollItemService = payrollItemService;
            _workContext = workContext;
            _taxFormBoxService = taxFormBoxService;
        }

        // GET: PayrollItem
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult _GetPayrollItems([DataSourceRequest]DataSourceRequest request)
        {
            var result = _payrollItemService.GetAllPayrollItems();
            return Json(result.ToDataSourceResult(request, m => m.ToViewModel()));
        }

        public ActionResult _PayrollItemDetails(int id,int tabIndex)
        {
            var payrollItem = _payrollItemService.Retrieve(id);
            PayrollItemModel_BL bl = new PayrollItemModel_BL(_payrollItemService);
            var model = payrollItem.ToModel();
            bl.SetPayrollItemDisplayProperty(model);
            ViewBag.TabIndex = tabIndex;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _UpdatePayrollItem(PayrollItemDetailModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                _payrollItemService.Update(entity);
                return Json(new { Result = true, ErrorMessage = String.Empty });
            }
            else
            {
                var errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));
                var errorMessage = String.Join(" | ", errors.Select(o => o.ToString()).ToArray());
                return Json(new { Result = false, ErrorMessage = errorMessage });
            }
        }

        [HttpPost]
        public ActionResult _GetTaxFormBox([DataSourceRequest]DataSourceRequest request,int payrollItemId, int year)
        {
            var payrollItem = _payrollItemService.Retrieve(payrollItemId);
            var result = _taxFormBoxService.GetAllTaxFormBoxes(year, payrollItem.Payroll_Item_Type.Code);
            if (result == null)
                return Json(new DataSourceResult());
            return Json(result.ToDataSourceResult(request,m=>m.ToSelectedModel(payrollItemId)));
        }

        [HttpPost]
        public ActionResult _UpdateTaxFormBox([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TaxFormBoxModel> models, int payrollItemId)
        {
            if (models != null && ModelState.IsValid)
            {
                foreach (var model in models)
                {
                    var entity = _taxFormBoxService.Retrieve(model.Id);
                    var payrollItem = _payrollItemService.Retrieve(payrollItemId);
                    bool isAlreadySelected = entity.PayrollItems.Where(x => x.ID == payrollItemId).Count() > 0;
                    if (model.IsSelected&&!isAlreadySelected)
                    {  
                        entity.PayrollItems.Add(payrollItem);
                        _taxFormBoxService.Update(entity);                        
                    }
                    if(!model.IsSelected&&isAlreadySelected)
                    {
                        entity.PayrollItems.Remove(payrollItem);
                        _taxFormBoxService.Update(entity);
                    }
                }
            }

            return Json(models.ToDataSourceResult(request, ModelState));
        }
    }
}