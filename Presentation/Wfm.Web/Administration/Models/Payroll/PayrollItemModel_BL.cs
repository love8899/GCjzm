using Wfm.Services.Payroll;

namespace Wfm.Admin.Models.Payroll
{
    public class PayrollItemModel_BL
    {
        private readonly IPayrollItemService _payrollItemService;
        public PayrollItemModel_BL(IPayrollItemService payrollItemService)
        {
            _payrollItemService = payrollItemService;
        }

        public void SetPayrollItemDisplayProperty(PayrollItemDetailModel model)
        {
            model.EnableRate = false;
            model.EnableDebitAccount = false;
            model.EnableCreditAccount = false;
            model.EnableHasYtdMaximum = false;
            model.EnableYTD_Maximum = false;

            model.EnablePrintOnPayStub = !model.IsReadOnly;

            //options
            model.EnableOptions = !model.IsReadOnly && !(model.Type == "ER_TAX" || model.Type == "DEDUCTION");
            switch (model.Type)
            {
                case "EARNING":
                    model.EnableDebitAccount = true;
                    break;
                case "DEDUCTION":
                case "TAX":
                    model.EnableCreditAccount = true;
                    break;
                case "ER_TAX":
                case "VAC_ACC":
                    model.EnableCreditAccount = true;
                    model.EnableDebitAccount = true;
                    break;
                default:
                    break;
            }

            switch (model.SubType)
            {
                case "NET_PAY":
                    model.EnableCreditAccount = true;
                    break;
                case "GROSS_PAY":
                case "TOTAL_DED":
                    break;
                case "CNT":
                    model.EnableHasYtdMaximum = true;
                    model.EnableYTD_Maximum = true;
                    model.EnableRate = true;
                    break;
                case "QHSF":
                case "PERCENT":
                    model.EnableRate = true;
                    break;
            }
        }
    }
}