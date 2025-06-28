using System;

namespace Wfm.Client.Models.Companies
{
    public class CompanyVendorModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public Guid FranchiseGuid { get; set; }
        public string VendorWebsite { get; set; }
    } 
}