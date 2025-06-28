using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wfm.Admin.Models.Common
{
    public class DNRReasonModel
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}