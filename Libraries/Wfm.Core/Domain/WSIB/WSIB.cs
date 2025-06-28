using System;
using Wfm.Core.Domain.Common;

namespace Wfm.Core.Domain.WSIBS
{
    public class WSIB:BaseEntity
    {
        public string Code { get; set; }
        public int ProvinceId { get; set; }
        public virtual StateProvince Province { get; set; }
        public decimal Rate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int UpdatedBy { get; set; }
        public int EnteredBy { get; set; }
        public string Description { get; set; }
    }
}
