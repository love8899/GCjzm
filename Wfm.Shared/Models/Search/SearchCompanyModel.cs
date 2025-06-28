using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Web.Framework;


namespace Wfm.Shared.Models.Search
{
    public class SearchCompanyModel
    {
        public SearchCompanyModel()
        {
            AvailableCompanies = new List<SelectListItem>();
            AvailableLocations = new List<SelectListItem>();
            AvailableDepartments = new List<SelectListItem>();
            AvailableContacts = new List<SelectListItem>();
        }

        [WfmResourceDisplayName("Common.Company")]
        public int sf_CompanyId { get; set; }               // for search by Id
        [WfmResourceDisplayName("Common.Company")]
        public string sf_Company { get; set; }              // for search by name

        [WfmResourceDisplayName("Common.Location")]
        public int sf_CompanyLocationId { get; set; }       // for search by Id
        [WfmResourceDisplayName("Common.Location")]
        public string sf_Location { get; set; }             // for search by name

        [WfmResourceDisplayName("Common.Department")]
        public int sf_CompanyDepartmentId { get; set; }     // for search by Id
        [WfmResourceDisplayName("Common.Department")]
        public string sf_Department { get; set; }           // for search by name

        [WfmResourceDisplayName("Common.Contact")]
        public int sf_CompanyContactId { get; set; }        // for search by Id
        [WfmResourceDisplayName("Common.Contact")]
        public string sf_Supervisor { get; set; }           // for search by name

        public IList<SelectListItem> AvailableCompanies { get; set; }
        public IList<SelectListItem> AvailableLocations { get; set; }
        public IList<SelectListItem> AvailableDepartments { get; set; }
        public IList<SelectListItem> AvailableContacts { get; set; }
    }
}
