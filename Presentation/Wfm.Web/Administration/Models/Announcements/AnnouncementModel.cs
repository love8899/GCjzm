using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Wfm.Admin.Validators;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Announcements  
{
    [Validator(typeof(AnnouncementValidator))]
    public class AnnouncementModel : BaseWfmEntityModel
    {
        public Guid AnnouncementGuid { get; set; }

        [AllowHtml]
        public string AnnouncementText { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [WfmResourceDisplayName("Common.EndDate")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        [WfmResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.Announcement.ForFranchise")]
        public bool ForFranchise { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.Announcement.ForClient")]
        public bool ForClient { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.Announcement.ForCandidate")]
        public bool ForCandidate { get; set; }

        [WfmResourceDisplayName("Common.Subject")]
        public string Subject { get; set; } 

        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }

        [WfmResourceDisplayName("Admin.Vendors")]
        public string[] FranchiseIds { get; set; }
        public IEnumerable<SelectListItem> FranchiseList
        {
            get;
            set;
        }
        public IEnumerable<SelectListItem> SelectedFranchiseList
        {
            get;
            set;
        }
        [WfmResourceDisplayName("Common.Companies")]
        public string[] CompanyIds { get; set; }

        public IEnumerable<SelectListItem> CompanyList
        {
            get;
            set;
        }
        public IEnumerable<SelectListItem> SelectedCompanyList  
        {
            get;
            set;
        }
    }
}