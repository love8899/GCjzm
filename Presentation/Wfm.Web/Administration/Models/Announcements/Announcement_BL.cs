using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.Announcements;
using Wfm.Services.Accounts;
using Wfm.Services.Announcements;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Admin.Extensions;
using Wfm.Web.Framework;

namespace Wfm.Admin.Models.Announcements
{
    public class Announcement_BL
    {
          #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IAnnouncementService _announcementService;     
        private readonly ILocalizationService _localizationService;
        private readonly IAccountService _accountService;
        private readonly ICompanyService _companyService;
        private readonly IFranchiseService _franchiseService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public Announcement_BL(
            IActivityLogService activityLogService,
            IAnnouncementService announcementService,           
            ILocalizationService localizationService,
            IAccountService accountService,
            IWorkContext workContext,
            ICompanyService companyService,
            IFranchiseService franchiseService
            ) 
        {
            _activityLogService = activityLogService;
            _announcementService = announcementService;         
            _localizationService = localizationService;
            _accountService = accountService;
            _workContext = workContext;
            _companyService = companyService;
            _franchiseService = franchiseService;
        }
        #endregion

        public DataSourceResult GetAllAnnouncementList([DataSourceRequest] DataSourceRequest request)
        {
          var announcements = _announcementService.GetAllAnnouncementsAsQueryable(_workContext.CurrentAccount,true).PagedForCommand(request);

            var announcementModelList = new List<AnnouncementModel>();
            foreach (var x in announcements)
            {
                var announcementModel = x.ToModel();
                announcementModelList.Add(announcementModel);
            }

            var result = new DataSourceResult()
            {
                Data = announcementModelList, // Process data (paging and sorting applied)
                Total = announcements.TotalCount, // Total number of records
            };
            return result;
        }
        public AnnouncementModel GetCreateAnnouncement()
        {
            AnnouncementModel announcementModel = new AnnouncementModel();
            announcementModel.UpdatedOnUtc = System.DateTime.UtcNow;
            announcementModel.CreatedOnUtc = System.DateTime.UtcNow;
            announcementModel.IsDeleted = false;
            announcementModel.ForClient = true;// selecting one value by default
            announcementModel.CompanyList = new SelectList(_companyService.GetAllCompaniesAsQueryable(_workContext.CurrentAccount)
                                                            .OrderBy(x => x.CompanyName).ToArray(),
                                                           "Id", "CompanyName");
            announcementModel.FranchiseList = new SelectList(_franchiseService.GetAllFranchisesAsQueryable(_workContext.CurrentAccount)
                                                            .OrderBy(x => x.FranchiseName).ToArray(),
                                                           "Id", "FranchiseName");
            announcementModel.SelectedCompanyList = new SelectListItem[] { };
            announcementModel.SelectedFranchiseList = new SelectListItem[] { };
            return announcementModel;
        }
        public Guid CreateAnnouncement(AnnouncementModel model)
        {
            var franchiseIds = model.FranchiseIds != null ? model.FranchiseIds.Select(x => int.Parse(x)).ToArray() : new int[] { };
            var companyIds = model.CompanyIds != null ? model.CompanyIds.Select(x => int.Parse(x)).ToArray() : new int[] { };
            model.UpdatedOnUtc = System.DateTime.UtcNow;
            model.CreatedOnUtc = System.DateTime.UtcNow;
            model.EnteredBy = _workContext.CurrentAccount.Id;
            Announcement announcement = model.ToEntity();
            _announcementService.InsertAnnouncement(announcement, franchiseIds, companyIds);

            //activity log
            _activityLogService.InsertActivityLog("AddNewAnnouncement", _localizationService.GetResource("ActivityLog.AddNewAnnouncement"), announcement.Id);

            return announcement.AnnouncementGuid;
        }
        public AnnouncementModel GetEditAnnouncement(Guid guid) 
        {
            Announcement announcement = _announcementService.GetAnnouncementByGuid(guid);
            if (announcement == null)
                return null; 

            AnnouncementModel announcementModel = announcement.ToModel();
            announcementModel.UpdatedOnUtc = System.DateTime.UtcNow;
            announcementModel.CompanyList = new SelectList(_companyService.GetAllCompaniesAsQueryable(_workContext.CurrentAccount)
                                                            .OrderBy(x => x.CompanyName).ToArray(),
                                                           "Id", "CompanyName");

            announcementModel.FranchiseList = new SelectList(_franchiseService.GetAllFranchisesAsQueryable(_workContext.CurrentAccount)
                                                            .OrderBy(x => x.FranchiseName).ToArray(),
                                                           "Id", "FranchiseName");

            announcementModel.SelectedCompanyList = new SelectList(announcement.AnnouncementTarget, "Company.Id", "Company.CompanyName");
            announcementModel.SelectedFranchiseList = new SelectList(announcement.AnnouncementTarget, "Franchise.Id", "Franchise.FranchiseName");

            return announcementModel;
        }
        public Guid UpdateAnnouncement(AnnouncementModel announcementModel, Announcement announcement)
        {
            var franchiseIds = announcementModel.FranchiseIds != null ? announcementModel.FranchiseIds.Select(x => int.Parse(x)).ToArray() : new int[] { };
            var companyIds = announcementModel.CompanyIds != null ? announcementModel.CompanyIds.Select(x => int.Parse(x)).ToArray() : new int[] { };
            announcementModel.EnteredBy = announcement.EnteredBy;
            announcementModel.IsDeleted = announcement.IsDeleted;
            announcementModel.UpdatedOnUtc = System.DateTime.UtcNow;
            announcement = announcementModel.ToEntity(announcement);
            _announcementService.UpdateAnnouncement(announcement, franchiseIds, companyIds);

            //activity log
            _activityLogService.InsertActivityLog("UpdateAnnouncement", _localizationService.GetResource("ActivityLog.UpdateAnnouncement"), announcement.Id);
            return announcementModel.AnnouncementGuid;
        }


    }
}