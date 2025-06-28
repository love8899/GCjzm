using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Announcements;



namespace Wfm.Data.Mapping.Announcements 
{
    public class AnnouncementTargetMap : EntityTypeConfiguration<AnnouncementTarget>
    {
        public AnnouncementTargetMap()
        {
            this.ToTable("AnnouncementTarget");
            this.HasKey(x => x.Id);          
        }
    }
}
