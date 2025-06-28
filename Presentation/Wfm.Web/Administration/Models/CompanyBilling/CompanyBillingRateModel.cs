using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Admin.Models.Companies;
using Wfm.Admin.Validators.Company;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.CompanyBilling
{
    [Validator(typeof(CompanyBillingRateValidator))]
    public partial class CompanyBillingRateModel : BaseWfmEntityModel
    {
        public CompanyBillingRateModel()
        {
            if (Quotations == null)
                Quotations = new List<QuotationModel>();
        }

        public Guid CompanyGuid { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.RateCode")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RateCode { get; set; }



        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.ShiftCode")]
        public string ShiftCode { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.RegularBillingRate")]
        public decimal RegularBillingRate { get; set; }

        [WfmResourceDisplayName("Common.RegularPayRate")]
        public decimal RegularPayRate { get; set; }

        [WfmResourceDisplayName("Common.OT.BillingRate")]
        public decimal OvertimeBillingRate { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.OvertimePayRate")]
        public decimal OvertimePayRate { get; set; }


        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.BillingTaxRate")]
        public decimal BillingTaxRate { get; set; }



        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.MaxRate")]
        public decimal? MaxRate { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.WeeklyWorkHours")]
        public decimal WeeklyWorkHours { get; set; }


        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.EffectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.DeactivatedDate")]
        public DateTime? DeactivatedDate { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.ReasonForDeactivation")]
        public string ReasonForDeactivation { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [WfmResourceDisplayName("Common.Franchise")]
        public int FranchiseId { get; set; }

        public string CompanyName { get; set; }

        public string CompanyLocationName { get; set; }

        public bool CompanyIsFiltered { get; set; }
        public bool FranchiseIsFiltered { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.PositionCode")]
        public int PositionId { get; set; }
        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.WSIBCode")]
        public string WSIBCode { get; set; }

        public IList<QuotationModel> Quotations { get; set; }
        public int? DefaultQuotationId { get { return Quotations.Count > 0 ? Quotations[0].Id : (int?)null; } }
    }

}