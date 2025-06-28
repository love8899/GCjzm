using Wfm.Core.Domain.Payroll;


namespace Wfm.Data.Mapping.Payroll
{
    public class T4_2018_Map : T4_Base_Map<T4_2018>
    {
        public T4_2018_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("T4_2018"));

            //this.Property(x => x.FirstName).HasMaxLength(20);
        }
    }
}
