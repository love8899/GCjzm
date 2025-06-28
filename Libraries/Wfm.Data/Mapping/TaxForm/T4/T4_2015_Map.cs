using Wfm.Core.Domain.Payroll;


namespace Wfm.Data.Mapping.Payroll
{
    public class T4_2015_Map : T4_Base_Map<T4_2015>
    {
        public T4_2015_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("T4_2015"));

            this.Property(x => x.PensionableEarnings).HasColumnName("PPensionableEarnings");

            //this.Property(x => x.FirstName).HasMaxLength(12);
        }
    }
}
