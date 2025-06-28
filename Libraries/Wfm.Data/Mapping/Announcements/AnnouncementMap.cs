using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Announcements;


namespace Wfm.Data.Mapping.Announcements 
{
    public class AnnouncementMap : EntityTypeConfiguration<Announcement>
    {
        public AnnouncementMap()
        {
            this.ToTable("Announcement");
            this.HasKey(x => x.Id);
            this.HasRequired(x => x.EnteredByAccount)
                .WithMany()
                .HasForeignKey(x=>x.EnteredBy); 
        }
    }
}
