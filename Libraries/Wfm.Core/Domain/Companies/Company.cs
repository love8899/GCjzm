using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Policies;


namespace Wfm.Core.Domain.Companies
{
    /// <summary>
    /// Company Information
    /// </summary>
    public class Company : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CompanyGuid { get; set; }
        public string CompanyName { get; set; }
        public string WebSite { get; set; }
        public string KeyTechnology { get; set; }
        public string Note { get; set; }
        public bool IsHot { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAdminCompany { get; set; }
        public string AdminName { get; set; }
        public int OwnerId { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public string CompanyCode { get; set; }
        public int CompanyStatusId { get; set; }

        public int InvoiceIntervalId { get; set; }
        public int? IndustryId { get; set; }

        public virtual List<CompanyLocation> CompanyLocations { get; set; }
        public virtual List<CompanyDepartment> CompanyDepartments { get; set; }
        public virtual List<CompanyBillingRate> CompanyBillingRates { get; set; }
        public virtual List<SchedulePolicy> SchedulePolicies { get; set; }
        public virtual List<BreakPolicy> BreakPolicies { get; set; }
        public virtual List<MealPolicy> MealPolicies { get; set; }
        public virtual List<RoundingPolicy> RoundingPolicies { get; set; }
        public virtual List<CompanyCandidate> CompanyCandidatePool { get; set; }
        public virtual List<CompanyOvertimeRule> CompanyOvertimeRules { get; set; }
        public virtual List<CompanyJobRole> CompanyJobRoles { get; set; }
        public virtual List<CompanyShift> CompanyShifts { get; set; }

        public virtual List<CompanyVendor> CompanyVendors { get; set; }
        public virtual List<CompanyActivity> CompanyActivities { get; set; }
        public virtual CompanyStatus CompanyStatus { get; set; }
        public virtual List<Position> Positions { get; set; }
        public virtual List<CompanyAttachment> CompanyAttachments { get; set; }
    }
}
