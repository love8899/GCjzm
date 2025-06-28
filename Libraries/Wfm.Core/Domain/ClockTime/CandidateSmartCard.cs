using System;
using System.ComponentModel.DataAnnotations.Schema;
using Wfm.Core.Domain.Candidates;


namespace Wfm.Core.Domain.ClockTime
{
    public class CandidateSmartCard : BaseEntity
    {
        public CandidateSmartCard()
        {
            this.CandidateSmartCardGuid = Guid.NewGuid();         
        }

        public Guid CandidateSmartCardGuid { get; set; }
        public int CandidateId { get; set; }
        public string SmartCardUid { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public string ReasonForDeactivation { get; set; }
        public string Note { get; set; }
        public int EnteredBy { get; set; }

        // Wiegand 26 format
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int FacilityCode { get; private set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int CardNumber { get; private set; }

        public virtual Candidate Candidate { get; set; }
    }
}