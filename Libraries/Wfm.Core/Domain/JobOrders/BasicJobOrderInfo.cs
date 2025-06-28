using System;

namespace Wfm.Core.Domain.JobOrders
{
    public partial class BasicJobOrderInfo:BaseEntity
    {
        public Guid JobOrderGuid { get; set; }
        public Guid CompanyGuid { get; set; }
        public string JobTitle { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set; }
        public string CityName { get; set; }
        public string DepartmentName { get; set; }
        public string Supervisor { get; set; }
        public int JobOrderTypeId { get; set; }
        public int JobOrderCategoryId { get; set; }
        public int ShiftId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public string SchedulePolicyName { get; set; }
        public string BillingRateCode { get; set; }
        public int JobOrderStatusId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsHot { get; set; }
        public bool IsInternalPosting { get; set; }
        public string RecruiterName { get; set; }

        public int FranchiseId { get; set; }
        public int? JobPostingId { get; set; }

        public DateTime? CreatedOn { get { return (CreatedOnUtc.HasValue ? CreatedOnUtc.Value : DateTime.MinValue).ToLocalTime(); } }

        public DateTime? UpdatedOn { get { return (UpdatedOnUtc.HasValue ? UpdatedOnUtc.Value : DateTime.MinValue).ToLocalTime(); } }
    }
}
