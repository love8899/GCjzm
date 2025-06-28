using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Announcements;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Services.Announcements  
{
    /// <summary>
    /// Message template service
    /// </summary>
    public partial interface IAnnouncementService 
    {
        void InsertAnnouncement(Announcement announcement);
        void InsertAnnouncement(Announcement announcement, int[] franchiseIds, int[] companyIds);

        void UpdateAnnouncement(Announcement announcement, int[] franchiseIds, int[] companyIds);
        void MarkAnnouncementAsDeleted(Announcement announcement);
         
        Announcement GetAnnouncementById(int announcementId);
        Announcement GetAnnouncementByGuid(Guid announcementGuid);
        IQueryable<Announcement> GetAllAnnouncementsAsQueryable(Account account, bool showHidden = false);
        IList<Announcement> GetAnnouncementsForClient(Account account);
        IList<Announcement> GetAnnouncementsForVendor(Account account); 

        bool MarkAnnouncementAsRead(Account account, Guid announcementGuid);

        IList<Announcement> GetAnnouncementForCandidate(Candidate candidate);
    }
}
