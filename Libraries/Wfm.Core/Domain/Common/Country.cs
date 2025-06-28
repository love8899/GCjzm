using System.Collections.Generic;

namespace Wfm.Core.Domain.Common
{
    public class Country : BaseEntity
    {
        public string CountryName { get; set; }
        public string TwoLetterIsoCode { get; set; }
        public string ThreeLetterIsoCode { get; set; }
        public int NumericIsoCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public virtual List<StateProvince> StateProvinces { get; set; }
    }
}