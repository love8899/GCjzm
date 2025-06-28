using Wfm.Core.Domain.TaxForm.RL1;


namespace Wfm.Data.Mapping.Payroll
{
    public class RL1_2016_Map : RL1_Base_Map<RL1_2016>
    {
        public RL1_2016_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("RL1_2016"));

            this.Property(c => c.slipSeq).IsRequired();
        }
    }
}
