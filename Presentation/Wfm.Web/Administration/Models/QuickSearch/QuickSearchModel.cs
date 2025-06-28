using System.Collections.Generic;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Common;
using Wfm.Admin.Models.Candidate;
using Wfm.Admin.Models.JobOrder;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.QuickSearch
{
    [Validator(typeof(QuickSearchValidator))]
    public class QuickSearchModel : BaseWfmEntityModel
    {
        public string KeyWord { get; set; }
        public string Domain { get; set; }

        public virtual IList<CandidateModel> gridCandidateModels { get; set; }
        public virtual IList<JobOrderModel> gridJobOrderModels { get; set; }
    }
}