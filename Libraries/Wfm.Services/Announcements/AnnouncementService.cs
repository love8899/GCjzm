using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Announcements;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Companies;

namespace Wfm.Services.Announcements 
{
    public partial class AnnouncementService : IAnnouncementService 
    {
       

        #region Fields

        private readonly IRepository<Announcement> _announcementRepository;
        private readonly IRepository<AnnouncementTarget> _announcementTargetRepository;
        private readonly IRepository<AnnouncementIsRead> _announcementIsReadRepository;  
        private readonly ICompanyVendorService _companyVendorService;

        #endregion

        #region Ctor

      
        public AnnouncementService(
            IRepository<Announcement> announcementRepository,
             IRepository<AnnouncementTarget> announcementTargetRepository,
             IRepository<AnnouncementIsRead> announcementIsReadRepository,
             ICompanyVendorService companyVendorService 
            )
        {
            _announcementRepository = announcementRepository;
            _announcementTargetRepository = announcementTargetRepository;
            _announcementIsReadRepository = announcementIsReadRepository;
            _companyVendorService = companyVendorService;          
        }

        #endregion 

        #region Methods
      

        /// <summary>
        /// Inserts a announcement
        /// </summary>
        /// <param name="announcement">announcement</param>
        public virtual void InsertAnnouncement(Announcement announcement)
        {
            if (announcement == null)
                throw new ArgumentNullException("announcement");

            _announcementRepository.Insert(announcement);
           
        }
        public virtual void InsertAnnouncement(Announcement announcement, int[] franchiseIds, int[] companyIds)
        {
            if (announcement == null)
                throw new ArgumentNullException("announcement");

            this.InsertAnnouncement(announcement);

            if (announcement.ForFranchise || announcement.ForClient)
            {
                var AnnouncementTarget = new List<AnnouncementTarget>();
                if (franchiseIds.Count() > 0)
                {
                    for (int i = 0; i < franchiseIds.Length; i++)
                    {
                        AnnouncementTarget.Add(new AnnouncementTarget
                        {
                            FranchiseId = franchiseIds[i],
                            AnnouncementId = announcement.Id,
                            CompanyId = null,
                        });
                    }
                }
                else if (companyIds.Count() > 0)
                {
                    for (int i = 0; i < companyIds.Length; i++)
                    {
                        AnnouncementTarget.Add(new AnnouncementTarget
                        {
                            FranchiseId = null,
                            AnnouncementId = announcement.Id,
                            CompanyId = companyIds[i],
                        });
                    }
                }
                else
                {
                    AnnouncementTarget.Add(new AnnouncementTarget
                    {
                        FranchiseId = null,
                        AnnouncementId = announcement.Id,
                        CompanyId = null,
                    });
                }
                _announcementTargetRepository.Insert(AnnouncementTarget);

            }

        }
      
        public virtual void UpdateAnnouncement(Announcement announcement, int[] franchiseIds, int[] companyIds)
        {
            if (announcement == null)
                throw new ArgumentNullException("announcement");

            var toDeleteAnnouncementTarget = announcement.AnnouncementTarget.Where(x => x.AnnouncementId == announcement.Id).ToList();
            toDeleteAnnouncementTarget.ForEach(x =>
            {
                _announcementTargetRepository.Delete(x);
            });
            if (announcement.ForFranchise || announcement.ForClient)
            {
               
                if (franchiseIds.Count() > 0)
                {
                    for (int i = 0; i < franchiseIds.Length; i++)
                    {
                        announcement.AnnouncementTarget.Add(new AnnouncementTarget
                        {
                            FranchiseId = franchiseIds[i],
                            AnnouncementId = announcement.Id,
                            CompanyId = null,
                        });
                    }
                }
                else if (companyIds.Count() > 0)
                {
                    for (int i = 0; i < companyIds.Length; i++)
                    {
                        announcement.AnnouncementTarget.Add(new AnnouncementTarget
                        {
                            FranchiseId = null,
                            AnnouncementId = announcement.Id,
                            CompanyId = companyIds[i],
                        });
                    }
                }
                else
                {
                    announcement.AnnouncementTarget.Add(new AnnouncementTarget
                    {
                        FranchiseId = null,
                        AnnouncementId = announcement.Id,
                        CompanyId = null,
                    });
                }              
            }

            _announcementRepository.Update(announcement);
         
        }

        public void MarkAnnouncementAsDeleted(Announcement announcement) { 
            if (announcement == null)
                throw new ArgumentNullException("announcement");

            announcement.IsDeleted = true;
            announcement.UpdatedOnUtc = System.DateTime.UtcNow;
            _announcementRepository.Update(announcement);
        }

        public virtual Announcement GetAnnouncementById(int announcementId)
        {
            if (announcementId == 0)
                return null;

            return _announcementRepository.GetById(announcementId);
        }
        public virtual Announcement GetAnnouncementByGuid(Guid announcementGuid )
        {
            return _announcementRepository.Table.Where(a => a.AnnouncementGuid == announcementGuid).FirstOrDefault();
        } 
        public IQueryable<Announcement> GetAllAnnouncementsAsQueryable(Account account, bool showHidden = false)
        {
            var query = _announcementRepository.TableNoTracking;
           
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            if (!account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(c => c.EnteredByAccount.FranchiseId == account.FranchiseId);

            query = query.OrderByDescending(x => x.UpdatedOnUtc);               

            return query.AsQueryable();
        }

        public IList<Announcement> GetAnnouncementsForClient(Account account)
        {
            var query = _announcementRepository.Table.Where(c => c.IsDeleted == false);

            query = query.Where(x => x.ForClient && x.StartDate <= DateTime.Today.Date && x.EndDate >= DateTime.Today.Date);

            //filter already read records
            var readAnnouncements = _announcementIsReadRepository.Table.Where(a => a.AccountId == account.Id);           
            query = query.Where(a => !readAnnouncements.Any(x => x.AnnouncementId == a.Id));

         
            
            query = from a in query
                    from target in _announcementTargetRepository.Table.
                               Where(at => at.AnnouncementId == a.Id && (at.CompanyId == null || at.CompanyId == account.CompanyId))
                    select a;

            //getting all vendors for current account
            var companyVendors = _companyVendorService.GetAllCompanyVendorsByCompanyId(account.CompanyId);

           
            // getting all announcements which are created by vendoraccount which are not linked to current company account
            var lstLimitedToFranchise = query.Where(x => x.EnteredByAccount.IsLimitedToFranchises 
                                        &&  !companyVendors.Any(cv=>cv.VendorId==x.EnteredByAccount.FranchiseId));
            
            // filtering records which are creatd by vendors not linked to this company.
            query = query.Where(a => !lstLimitedToFranchise.Any(l => l.Id == a.Id));

            return query.OrderBy(a=>a.DisplayOrder).ToList();
        }

        public bool MarkAnnouncementAsRead(Account account, Guid announcementGuid) 
        { 
            var announcement=this.GetAnnouncementByGuid(announcementGuid);
            var result = false;
            if(announcement!=null)
            {
                var query = _announcementIsReadRepository.Table.Where(a => a.AccountId == account.Id && a.AnnouncementId == announcement.Id);
                if (query.ToList().Count == 0)
                {
                    try
                    {
                        var announcementIsRead = new AnnouncementIsRead();
                        announcementIsRead.AccountId = account.Id;
                        announcementIsRead.AnnouncementId = announcement.Id;
                        announcementIsRead.IsRead = true;
                        announcementIsRead.MarkAsReadDate = DateTime.Now;
                        _announcementIsReadRepository.Insert(announcementIsRead);
                        result = true;
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
            return result;
        }

        public IList<Announcement> GetAnnouncementsForVendor(Account account) 
        {
            var query = _announcementRepository.Table.Where(c => c.IsDeleted == false);

            query = query.Where(x => x.ForFranchise && x.StartDate <= DateTime.Today.Date && x.EndDate >= DateTime.Today.Date);

            // getting only records which are created by MSP.
            query = query.Where(x => !x.EnteredByAccount.IsLimitedToFranchises);
            
            //filter already read records
            var readAnnouncements = _announcementIsReadRepository.Table.Where(a => a.AccountId == account.Id);
            query = query.Where(a => !readAnnouncements.Any(x => x.AnnouncementId == a.Id));

            query = from a in query
                    from target in _announcementTargetRepository.Table.
                               Where(at => at.AnnouncementId == a.Id && (at.FranchiseId == null || at.FranchiseId == account.FranchiseId))
                    select a;

            return query.OrderBy(a => a.DisplayOrder).ToList();
        }

        public IList<Announcement> GetAnnouncementForCandidate(Candidate candidate)
        {
            var query = _announcementRepository.Table.Where(c => c.IsDeleted == false);

            query = query.Where(x => x.ForCandidate && x.StartDate <= DateTime.Today.Date && x.EndDate >= DateTime.Today.Date);

            // getting only records which are created by MSP or by the Candidate vendor.
            query = query.Where(x => !x.EnteredByAccount.IsLimitedToFranchises|| x.EnteredByAccount.FranchiseId==candidate.FranchiseId);

            return query.OrderBy(a => a.DisplayOrder).ToList();
        }
        #endregion
    }
}
