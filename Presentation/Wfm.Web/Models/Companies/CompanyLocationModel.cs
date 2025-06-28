using Wfm.Shared.Models.Common;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Companies
{
    public partial class CompanyLocationModel : BaseWfmEntityModel
    {

        [WfmResourceDisplayName("Common.CompanyId")]
        public int CompanyId { get; set; }

        [WfmResourceDisplayName("Common.Location")]
        public string LocationName { get; set; }

        [WfmResourceDisplayName("Web.Companies.CompanyLocation.Fields.PrimaryPhone")]
        public string PrimaryPhone { get; set; }

        [WfmResourceDisplayName("Web.Companies.CompanyLocation.Fields.SecondaryPhone")]
        public string SecondaryPhone { get; set; }

        [WfmResourceDisplayName("Web.Companies.CompanyLocation.Fields.FaxNumber")]
        public string FaxNumber { get; set; }

        [WfmResourceDisplayName("Common.UnitNumber")]
        public string UnitNumber { get; set; }

        [WfmResourceDisplayName("Common.AddressLine1")]
        public string AddressLine1 { get; set; }

        [WfmResourceDisplayName("Common.AddressLine2")]
        public string AddressLine2 { get; set; }

        [WfmResourceDisplayName("Common.AddressLine3")]
        public string AddressLine3 { get; set; }

        [WfmResourceDisplayName("Common.City")]
        public int CityId { get; set; }

        [WfmResourceDisplayName("Common.StateProvinceId")]
        public int StateProvinceId { get; set; }

        [WfmResourceDisplayName("Common.Country")]
        public int CountryId { get; set; }

        [WfmResourceDisplayName("Common.PostalCode")]
        public string PostalCode { get; set; }



        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }

        [WfmResourceDisplayName("Common.EnteredBy")]
        public int? EnteredBy { get; set; }



        public virtual CompanyModel CompanyModel { get; set; }
        public virtual CountryModel CountryModel { get; set; }
        public virtual StateProvinceModel StateProvinceModel { get; set; }
        public virtual CityModel CityModel { get; set; }

    }  

}