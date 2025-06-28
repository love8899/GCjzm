using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Client.Models.CompanyBilling;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Companies;
using Wfm.Web.Framework;
using Wfm.Client.Extensions;
using System.Text.RegularExpressions;

namespace Wfm.Client.Models.Companies
{
    public class CompanyBillingRate_BL
    {
       
        private readonly ICompanyBillingService _companyBillingService;
        
        private readonly ICompanyDivisionService _companyDivisionService;

        public CompanyBillingRate_BL(
            ICompanyBillingService companyBillingService,         
            ICompanyDivisionService companyDivisionService
            )
        {
            _companyBillingService = companyBillingService;
            _companyDivisionService = companyDivisionService;
        }
        public DataSourceResult GetAllCompanyBillingRates(DataSourceRequest request, int? companyId)
        {
            List<CompanyBillingRate> billingRates = new List<CompanyBillingRate>();
            List<CompanyBillingRateModel> modelList = new List<CompanyBillingRateModel>();

            if (companyId.HasValue)
            {
                billingRates = _companyBillingService.GetAllCompanyBillingRatesByCompanyId(companyId.Value).ToList();
                foreach (var item in billingRates)
                {
                    CompanyBillingRateModel billingRateModel = MappingExtensions.ToModel(item);
                    var location = _companyDivisionService.GetCompanyLocationById(billingRateModel.CompanyLocationId);
                    if (location != null)
                        billingRateModel.CompanyLocationName = location.LocationName;
                    if (billingRateModel.Note != null)
                        billingRateModel.Note = Regex.Replace(HttpUtility.HtmlDecode(billingRateModel.Note), "<.*?>", String.Empty);

                    modelList.Add(billingRateModel);
                }
            }
            // Initialize the DataSourceResult
            var result = new DataSourceResult()
            {
                Data = modelList.PagedForCommand(request), // Process data (paging and sorting applied)
                Total = billingRates.Count // Total number of records
            };
            return result;
        }
     
    }
}