using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Franchise;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Franchises
{
    [Validator(typeof(FranchiseValidator))]
    public partial class FranchiseModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.FranchiseId")]
        public string FranchiseId { get; set; }

        public Guid FranchiseGuid { get; set; }

        [WfmResourceDisplayName("Common.FranchiseName")]
        public string FranchiseName { get; set; }

        [WfmResourceDisplayName("Admin.Franchises.Franchise.Fields.PrimaryContactName")]
        public string PrimaryContactName { get; set; }

        [WfmResourceDisplayName("Common.Email")]
        public string Email { get; set; }

        [WfmResourceDisplayName("Common.WebSite")]
        public string WebSite { get; set; }

        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }

        [WfmResourceDisplayName("Admin.Franchises.Franchise.Fields.ReasonForDisabled")]
        public string ReasonForDisabled { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Common.Owner")]
        public int OwnerId { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }
        public string EnteredName { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [WfmResourceDisplayName("Admin.Franchises.Franchise.Fields.IsDefaultManagedServiceProvider")]
        public bool IsDefaultManagedServiceProvider { get; set; }
        public bool EnableStandAloneJobOrders { get; set; }
        public bool IsLinkToPublicSite { get; set; }
        [WfmResourceDisplayName("Common.ShortName")]
        public string ShortName { get; set; }

        public byte[] FranchiseLogo { get; set; }
        public string FranchiseLogoFileName { get; set; }
        public bool KeepCurrentLogo { get; set; }
    }
}