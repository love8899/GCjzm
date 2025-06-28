using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Client.Models.Companies
{
    public class CompanyDepartmentModel :BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public int CompanyLocationId { get; set; }

        [WfmResourceDisplayName("Common.Department")]
        public string DepartmentName { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyDepartment.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

        [WfmResourceDisplayName("Admin.Companies.CompanyDepartment.Fields.FaxNumber")]
        public string FaxNumber { get; set; }

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


        public virtual CompanyModel Company { get; set; }


        [WfmResourceDisplayName("Common.Location")]
        public string CompanyLocationName { get; set; }
    }
}