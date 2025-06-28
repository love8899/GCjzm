using System;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.CompanyBilling
{

    public partial class CompanyBillingRateModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.RateCode")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RateCode { get; set; }

    
        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.PositionCode")]
        public int PositionId { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.PositionCode")]
        public string PositionCode { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.ShiftCode")]
        public string ShiftCode { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }
        [WfmResourceDisplayName("Common.RegularBillingRate")]
        public decimal RegularBillingRate { get; set; }


        [WfmResourceDisplayName("Common.OT.BillingRate")]
        public decimal OvertimeBillingRate { get; set; }


        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.BillingTaxRate")]
        public decimal BillingTaxRate { get; set; }


        [WfmResourceDisplayName("Admin.Companies.CompanyBillingRate.Fields.MaxRate")]
        public decimal MaxRate { get; set; }

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

        [WfmResourceDisplayName("Common.FranchiseId")]
        public int FranchiseId { get; set; }
        public string FranchiseName { get; set; }

        [WfmResourceDisplayName("Common.Company")]
        public string CompanyName { get; set; }

        public string CompanyLocationName { get; set; }

        public string EffectiveDateString { get { return this.EffectiveDate == null ? "" : this.EffectiveDate.Value.ToString("MM/dd/yyyy"); } }
        public string DeactivatedDateString { get { return this.DeactivatedDate == null ? "" : this.DeactivatedDate.Value.ToString("MM/dd/yyyy"); } }

    }

}