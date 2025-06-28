using System.Collections.Generic;

namespace Wfm.Core.Domain.Common
{
    public class StateProvince : BaseEntity
    {
        public int CountryId { get; set; }
        public string StateProvinceName { get; set; }
        public string Abbreviation { get; set; }
      
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Country Country { get; set; }
        public virtual List<City> Cities { get; set; }
        public virtual List<StatutoryHoliday> StatutoryHolidays { get; set; }
    }
}