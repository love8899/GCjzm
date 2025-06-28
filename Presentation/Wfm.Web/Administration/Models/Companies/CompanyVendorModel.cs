using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Companies
{
    public class CompanyVendorModel
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public int VendorId { get; set; }
        public string VendorName { get; set; }

        public bool IsActive { get; set; }
    }

}
