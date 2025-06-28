using Wfm.Core.Domain.Payroll;


namespace Wfm.Data.Mapping.Payroll
{
    public class T4_2019_Map : T4_Base_Map<T4_2019>
    {
        public T4_2019_Map() : base()
        {
            this.Map(x => x.MapInheritedProperties().ToTable("T4_2019"));

            //this.Property(x => x.FirstName).HasMaxLength(20);
        }
    }
}
