using Wfm.Core.Domain.Payroll;


namespace Wfm.Data.Mapping.Payroll
{
    public class T4_2016_Map : T4_Base_Map<T4_2016>
    {
        public T4_2016_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("T4_2016"));

            //this.Property(x => x.FirstName).HasMaxLength(20);
        }
    }
}
