using System;
using System.Collections.Generic;

namespace Wfm.Core.Domain.Payroll
{
    public partial class Payroll_Item_SubType
    {
        public Payroll_Item_SubType()
        {
            this.Payroll_Item = new List<Payroll_Item>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string UnitName { get; set; }
        public string Code { get; set; }
        public bool IsInternal { get; set; }
        public virtual ICollection<Payroll_Item> Payroll_Item { get; set; }
    }
}
