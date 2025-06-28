using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.TimeSheet;

namespace Wfm.Core.Domain.JobOrders
{
 
    public class JobOrderWithCompanyAddress
    {
        private ICollection<CandidateJobOrder> _candidateJobOrders;
        private ICollection<JobOrderOpening> _jobOrderOpenings;

        public int Id { get; set; }

        public DateTime? CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }
        public Guid JobOrderGuid { get; set; }
        public int CompanyId { get; set; }
        public int CompanyLocationId { get; set; }
        public int CompanyDepartmentId { get; set; }
        public int CompanyContactId { get; set; }
        public string CompanyJobNumber { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public DateTime? HiringDurationExpiredDate { get; set; }
        public DateTime? EstimatedFinishingDate { get; set; }
        public string EstimatedMargin { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int SchedulePolicyId { get; set; }
        public int JobOrderTypeId { get; set; }
        public string Salary { get; set; }
        public int JobOrderStatusId { get; set; }
        public int JobOrderCategoryId { get; set; }

        //public int CompanyBillingRateId { get; set; }
        public string BillingRateCode { get; set; }

        public int ShiftId { get; set; }
        public string ShiftSchedule { get; set; }
        //public string Supervisor { get; set; }
        public decimal? HoursPerWeek { get; set; }
        public string Note { get; set; }
        public bool RequireSafeEquipment { get; set; }
        public bool RequireSafetyShoe { get; set; }
        public bool IsInternalPosting { get; set; }
        public bool IsPublished { get; set; }
        public bool IsHot { get; set; }
        public bool AllowSuperVisorModifyWorkTime { get; set; }
        //public bool AllowDailyApproval { get; set; }
        public int RecruiterId { get; set; }
        public int OwnerId { get; set; }
        public int EnteredBy { get; set; }
        public int FranchiseId { get; set; }
        public JobOrderCloseReasonCode? JobOrderCloseReason { get; set; }

        public bool IsDeleted { get; set; }



        public string AddressLine1 { get; set; }
        public string CityName { get; set; } 

        public string CompanyDepartmentName { get; set; }



        public virtual JobOrderCategory JobOrderCategory { get; set; }
        public virtual JobOrderType JobOrderType { get; set; }
        public virtual JobOrderStatus JobOrderStatus { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual Company Company { get; set; }

        public virtual List<JobOrderOvertimeRule> JobOrderOvertimeRules { get; set; }

        public virtual ICollection<ClientTimeSheetDocument> ClientTimeSheetDocuments { get; set; }

        public virtual ICollection<CandidateJobOrder> CandidateJobOrders
        {
            get { return _candidateJobOrders ?? (_candidateJobOrders = new List<CandidateJobOrder>()); }
            protected set { _candidateJobOrders = value; }
        }

        public virtual ICollection<JobOrderOpening> JobOrderOpenings
        {
            get { return _jobOrderOpenings ?? (_jobOrderOpenings = new List<JobOrderOpening>()); }
            protected set { _jobOrderOpenings = value; }
        }

        public virtual int OpeningNumber
        {
            get
            {
                var setting = JobOrderOpenings.Where(x => x.StartDate <= DateTime.Today && (!x.EndDate.HasValue || x.EndDate.Value > DateTime.Today)).FirstOrDefault();
                return setting != null ? setting.OpeningNumber : 0;
            }
        }
    }

}
