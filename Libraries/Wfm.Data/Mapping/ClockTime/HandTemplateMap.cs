using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.ClockTime;


namespace Wfm.Data.Mapping.TimeClocks
{
    public partial class HandTemplateMap : EntityTypeConfiguration<HandTemplate>
    {
        public HandTemplateMap()
        {
            this.ToTable("HandTemplate");
            this.HasKey(x => x.Id);

            this.Property(x => x.HandTemplateGuid).IsRequired();
        }
    }
}
