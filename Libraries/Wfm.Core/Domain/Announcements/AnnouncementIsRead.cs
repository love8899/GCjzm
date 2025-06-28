using System;
using Wfm.Core.Domain.Accounts;

namespace Wfm.Core.Domain.Announcements
{
 public  class AnnouncementIsRead
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; }
        public int AccountId { get; set; }
        public bool IsRead { get; set; }
        public DateTime MarkAsReadDate { get; set; }
        public virtual Announcement Announcement { get; set; }
        public virtual Account Account { get; set; }
    }
}
