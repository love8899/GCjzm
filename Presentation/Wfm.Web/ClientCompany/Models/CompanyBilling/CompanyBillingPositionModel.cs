using FluentValidation.Attributes;
using System;
using Wfm.Client.Validators.Company;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.CompanyBilling
{
    [Validator(typeof(CompanyBillingPositionValidator))]
    public class CompanyBillingPositionModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingPosition.Fields.BillingPositionCode")]
        public string BillingPositionCode { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingPosition.Fields.BillingPositionName")]
        public string BillingPositionName { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingPosition.Fields.EffectiveDate")]
        public DateTime? EffectiveDate { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingPosition.Fields.DeactivatedDate")]
        public DateTime? DeactivatedDate { get; set; }

        [WfmResourceDisplayName("Admin.Configuration.CompanyBillingPosition.Fields.ReasonForDeactivation")]
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