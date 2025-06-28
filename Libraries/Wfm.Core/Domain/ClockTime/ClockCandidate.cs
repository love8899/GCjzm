using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Wfm.Core.Domain.ClockTime
{
    public class ClockCandidate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClockDeviceId { get; set; }
        public int CandidateId { get; set; }
        public DateTime AddedOnUtc { get; set; }

        public virtual CompanyClockDevice ClockDevice { get; set; }
    }
}
