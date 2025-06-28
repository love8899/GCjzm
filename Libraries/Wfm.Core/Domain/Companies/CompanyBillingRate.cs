using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Franchises;


namespace Wfm.Core.Domain.Companies
{
    /// <summary>
    /// Company Information
    /// </summary>
    public class CompanyBillingRate : BaseEntity
    {
        public CompanyBillingRate()
        {
            Quotations = new List<Quotation>();
        }

        public int CompanyId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RateCode { get; set; }

        public int CompanyLocationId { get; set; }

        public string ShiftCode { get; set; }

        public decimal RegularBillingRate { get; set; }
        public decimal RegularPayRate { get; set; }
        public decimal OvertimeBillingRate { get; set; }
        public decimal OvertimePayRate { get; set; }

        public decimal BillingTaxRate { get; set; }

        public decimal? MaxRate { get; set; }

        public decimal WeeklyWorkHours { get; set; }
        //public decimal AveragingWorkHoursPeriod { get; set; }

        public DateTime EffectiveDate { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public string ReasonForDeactivation { get; set; }

        public string Note { get; set; }

        public bool IsActive { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsDeleted { get; set; }
        public int DeletedBy { get; set; }

        public int FranchiseId { get; set; }

        public int PositionId { get; set; }
        public virtual Franchise Franchise { get; set; }
        public virtual Company Company { get; set; }
        public virtual Position Position { get; set; }
        public string WSIBCode { get; set; }

        public virtual ICollection<Quotation> Quotations { get; set; }
    }
}
