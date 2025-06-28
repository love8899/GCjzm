using System;
using System.Collections.Generic;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Models.Companies;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Company;

namespace Wfm.Admin.Models.Candidate
{
    [Validator(typeof(CandidateWorkHistoryValidator))]
    public partial class CandidateWorkHistoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.CandidateId")]
        public int CandidateId { get; set; }

        [WfmResourceDisplayName("Common.Title")]
        public string Title { get; set; }

        [WfmResourceDisplayName("Common.CompanyName")]
        public string CompanyName { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime StartDate { get; set; }

        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime EndDate { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }


        public CandidateModel CandidateModel { get; set; }
        public IList<CompanyModel> AllCompanies { get; set; }

    }

}