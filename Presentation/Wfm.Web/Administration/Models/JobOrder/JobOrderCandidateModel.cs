using System;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.JobOrder
{
    public class JobOrderCandidateModel : BaseWfmEntityModel
    {
        public Guid CompanyGuid { get; set; }
        public Guid JobOrderGuid { get; set; }
        public int JobOrderId { get; set; }
        public int CompanyId { get; set; }
        public DateTime InquiryDate { get; set; }
        //public JobOrderOpeningModel[] ListOfOpenningChanges { get; set; }
        public int OpeningAvaliable { get; set; }
        public int Placed { get; set; }
        public int Shortage { get { return OpeningAvaliable - Placed; } }
        public int RecruiterId { get; set; }
        public int OwnerId { get; set; }
        //public IList<CandidatePipelineSimpleModel> Pipeline { get; set; }
        //public IList<CandidatePoolModel> MasterPool { get; set; } 
    }
}
