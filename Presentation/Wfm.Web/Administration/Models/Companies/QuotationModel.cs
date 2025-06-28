using FluentValidation.Attributes;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Companies
{
    public class QuotationModel : BaseWfmEntityModel
    {
        public int CompanyBillingRateId { get; set; }
        public string FileName { get; set; }
    }
}
