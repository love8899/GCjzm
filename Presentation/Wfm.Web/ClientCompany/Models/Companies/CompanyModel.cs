using System;
using System.Collections.Generic;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Client.Models.JobOrder;
using Wfm.Shared.Models.Accounts;

namespace Wfm.Client.Models.Companies
{
    /// <summary>
    /// Company Model
    /// </summary>
    public partial class CompanyModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }

        [WfmResourceDisplayName("Common.WebSite")]
        public string WebSite { get; set; }

        [WfmResourceDisplayName("Common.IsHot")]
        public bool IsHot { get; set; }

        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.IsDeleted")]
        public bool IsDeleted { get; set; }

        [WfmResourceDisplayName("Admin.Companies.Company.Fields.IsAdminCompany")]
        public bool IsAdminCompany { get; set; }

        [WfmResourceDisplayName("Admin.Companies.Company.Fields.AdminName")]
        public string AdminName { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Common.Owner")]
        public int OwnerId { get; set; }

        [WfmResourceDisplayName("Admin.Companies.Company.Fields.KeyTechnology")]
        public string KeyTechnology { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }

        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }


        [WfmResourceDisplayName("Admin.Companies.Company.Fields.OwnerName")]
        public string OwnerName { get; set; }

        [WfmResourceDisplayName("Admin.Companies.Company.Fields.EnteredByName")]
        public string EnteredByName { get; set; }

        public DateTime WorkTimeSelectFrom { get; set; }

        public DateTime WorkTimeSelectTo { get; set; }


        public virtual IList<JobOrderModel> JobOrderModels { get; set; }
        public virtual IList<AccountModel> CompanyContactModels { get; set; }
        public virtual IList<CompanyLocationModel> CompanyLocationModels { get; set; }

        public IList<AccountModel> OwnerList { get; set; }

    }
}