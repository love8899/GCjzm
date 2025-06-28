using Wfm.Core.Domain.Payroll;


namespace Wfm.Data.Mapping.Payroll
{
    public class T4_2020_Map : T4_Base_Map<T4_2020>
    {
        public T4_2020_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("T4_2020"));

            //this.Property(x => x.FirstName).HasMaxLength(20);
        }
    }
}
