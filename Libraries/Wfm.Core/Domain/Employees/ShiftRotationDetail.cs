using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Common;

namespace Wfm.Core.Domain.Employees
{
    public class ShiftRotationDetail : BaseEntity
    {
        public int ShiftRotationHeaderId { get; set; }
        public virtual ShiftRotationHeader ShiftRotationHeader { get; set; }
        public int NumberOfDays { get; set; }
        public int? ShiftId { get; set; }
        public Shift Shift { get; set; }
    }
}
