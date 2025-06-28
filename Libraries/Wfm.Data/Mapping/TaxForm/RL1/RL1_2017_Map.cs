using Wfm.Core.Domain.TaxForm.RL1;


namespace Wfm.Data.Mapping.Payroll
{
    public class RL1_2017_Map : RL1_Base_Map<RL1_2017>
    {
        public RL1_2017_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("RL1_2017"));

            this.Property(c => c.slipSeq).IsRequired();
        }
    }
}
