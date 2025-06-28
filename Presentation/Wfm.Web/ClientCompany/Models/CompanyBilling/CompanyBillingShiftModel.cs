using FluentValidation.Attributes;
using System;
using Wfm.Client.Validators.Company;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.CompanyBilling
{
    [Validator(typeof(CompanyBillingShiftValidator))]
    public class CompanyBillingShiftModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingShift.Fields.BillingShiftCode")]
        public string BillingShiftCode { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingShift.Fields.BillingShiftName")]
        public string BillingShiftName { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingShift.Fields.EffectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingShift.Fields.DeactivatedDate")]
        public DateTime? DeactivatedDate { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingShift.Fields.ReasonForDeactivation")]
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
    }
}