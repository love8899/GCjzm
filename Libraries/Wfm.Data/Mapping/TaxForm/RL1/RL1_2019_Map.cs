using Wfm.Core.Domain.TaxForm.RL1;


namespace Wfm.Data.Mapping.Payroll
{
    public class RL1_2019_Map : RL1_Base_Map<RL1_2019>
    {
        public RL1_2019_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("RL1_2019"));

            this.Property(c => c.slipSeq).IsRequired();
        }
    }
}
