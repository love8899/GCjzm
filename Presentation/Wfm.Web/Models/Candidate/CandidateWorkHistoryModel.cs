using System;
using System.Collections.Generic;
using Wfm.Web.Framework.Mvc;
using Wfm.Web.Models.Companies;

namespace Wfm.Web.Models.Candidate
{

    public partial class CandidateWorkHistoryModel : BaseWfmEntityModel
    {

        public int CandidateId { get; set; }

        public string Title { get; set; }

        public string CompanyName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Note { get; set; }


        public IList<CompanyModel> AllCompanies { get; set; }

       // public virtual Wfm.Core.Domain.Candidates.Candidate Candidate { get; set; }


    }

}