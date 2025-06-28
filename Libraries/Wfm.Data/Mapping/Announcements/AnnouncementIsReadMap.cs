using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Announcements;



namespace Wfm.Data.Mapping.Announcements 
{
    public class AnnouncementIsReadMap : EntityTypeConfiguration<AnnouncementIsRead>
    {
        public AnnouncementIsReadMap()
        {
            this.ToTable("AnnouncementIsRead");
            this.HasKey(x => x.Id);          
        }
    }
}
