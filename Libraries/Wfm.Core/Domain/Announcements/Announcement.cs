using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Core.Domain.Announcements  
{
    public class Announcement : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid AnnouncementGuid { get; set; }

        public string AnnouncementText { get; set; }
        public string Subject { get; set; } 

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DisplayOrder { get; set; }
        public bool ForFranchise { get; set; }
        public bool ForClient { get; set; }
        public bool ForCandidate { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }

        public virtual ICollection<AnnouncementTarget> AnnouncementTarget { get; set; }

        public virtual Account EnteredByAccount { get; set; }
    }
}
