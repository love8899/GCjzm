using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Employees
{
    public class ShiftRotationHeader : BaseEntity
    {
        public ShiftRotationHeader()
        {
            this.RotationDetails = new List<ShiftRotationDetail>();
        }
        public string ShiftRotationName { get; set; }
        public int RepeatCycleInDays { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ShiftRotationDetail> RotationDetails { get; set; }
    }
}
