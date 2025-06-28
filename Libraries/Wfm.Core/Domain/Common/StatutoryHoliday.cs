using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Common
{
    public class StatutoryHoliday : BaseEntity
    {
        public int StateProvinceId { get; set; }
        public string StatutoryHolidayName { get; set; }
        public DateTime HolidayDate { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public int EnteredBy { get; set; }
        public virtual StateProvince StateProvince { get; set; }
    }
}
