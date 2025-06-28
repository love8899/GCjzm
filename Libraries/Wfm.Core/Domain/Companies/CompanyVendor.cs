using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Franchises;


namespace Wfm.Core.Domain.Companies
{
    public class CompanyVendor : BaseEntity
    {
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public int VendorId { get; set; }
        public virtual Franchise Vendor { get; set; }

        public bool IsActive { get; set; }
    }

}
