using Wfm.Core.Domain.TaxForm.RL1;


namespace Wfm.Data.Mapping.Payroll
{
    public class RL1_2015_Map : RL1_Base_Map<RL1_2015>
    {
        public RL1_2015_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("RL1_2015"));
        }
    }
}
