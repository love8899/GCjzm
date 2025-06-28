using System;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Franchises;


namespace Wfm.Core.Domain.Announcements
{
   public class AnnouncementTarget
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; }
        public int? CompanyId { get; set; }
        public int? FranchiseId { get; set; }
        public virtual Announcement Announcement { get; set; }
        public virtual Company Company { get; set; }
        public virtual Franchise Franchise { get; set; } 
    } 
}
