using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Models.Common;
using Wfm.Core.Domain.Candidates;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Franchises
{
    public partial class VendorAccountSelector
    {
        public int[] CompanyIds { get; set; }

        public int[] VendorIds { get; set; }

        public int[] RoleIds { get; set; }
    }
 
}
