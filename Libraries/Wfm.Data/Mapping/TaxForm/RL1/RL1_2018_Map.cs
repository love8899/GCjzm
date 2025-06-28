using Wfm.Core.Domain.TaxForm.RL1;


namespace Wfm.Data.Mapping.Payroll
{
    public class RL1_2018_Map : RL1_Base_Map<RL1_2018>
    {
        public RL1_2018_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("RL1_2018"));

            this.Property(c => c.slipSeq).IsRequired();
        }
    }
}
