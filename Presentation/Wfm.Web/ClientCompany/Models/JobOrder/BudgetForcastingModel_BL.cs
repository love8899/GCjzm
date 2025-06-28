using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wfm.Core;
using Wfm.Data;
using Wfm.Services.Companies;
using Wfm.Services.JobOrders;

namespace Wfm.Client.Models.JobOrder
{
    public class BudgetForcastingModel_BL
    {
        public List<BudgetForcastingModel> GetAllBudgetForcastingModel(IJobOrderService _jobOrderService,
                                                                        IWorkContext _workContext,
                                                                        ICompanyDivisionService _companyDivisionService,
                                                                        ICompanyDepartmentService _companyDepartmentSerice)
        {
            var jobOrders = _jobOrderService.GetJobOrdersByAccountAndCompany(_workContext.CurrentAccount,_workContext.CurrentAccount.CompanyId,DateTime.Today);
            List<BudgetForcastingModel> results = new List<BudgetForcastingModel>();
            foreach (var jobOrder in jobOrders)
            {
                if (!_jobOrderService.AreBillingRatesDefinedForJobOrderByDateRange(jobOrder.Id, DateTime.Today, DateTime.MaxValue))
                    continue;
                BudgetForcastingModel model = new BudgetForcastingModel();
                model.JobOrderId = jobOrder.Id;
                model.JobTitle = jobOrder.JobTitle;
                if (jobOrder.CompanyLocationId != 0)
                    model.Location = _companyDivisionService.GetCompanyLocationById(jobOrder.CompanyLocationId).LocationName;
                if (jobOrder.CompanyDepartmentId != 0)
                    model.Department = _companyDepartmentSerice.GetCompanyDepartmentById(jobOrder.CompanyDepartmentId).DepartmentName;
                model.StartDate = DateTime.Today;
                model.EndDate = DateTime.Today;
                model.TotalCost = 0;
                model.Position = jobOrder.Position == null ? String.Empty : jobOrder.Position.Name;
                model.Shift = jobOrder.Shift.ShiftName;
                model.NumberOfEmployees = 0;
                //model.BillingRateCode = jobOrder.BillingRateCode;
                results.Add(model);
            }
            return results;
        }

        public void CalculateBudget(BudgetForcastingModel model,IDbContext _dbContext)
        {
            SqlParameter[] paras = new SqlParameter[4];
            paras[0] = new SqlParameter("start", model.StartDate);
            paras[1] = new SqlParameter("end", model.EndDate);
            paras[2] = new SqlParameter("required", model.NumberOfEmployees);
            paras[3] = new SqlParameter("jobOrderId", model.JobOrderId);

            string query = @"Exec [JobOrderBudgetForcastingCalculator] @start,@end,@required,@jobOrderId";
            List<BudgetForcastingModel> result = _dbContext.SqlQuery<BudgetForcastingModel>(query,paras).ToList();
            if (result != null&&result.Count>0)
                model.TotalCost = result.FirstOrDefault().TotalCost;
        }
    }
}