using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wfm.Core.Domain.Candidates
{
    public class CandidateKeySkill : BaseEntity
    {
        public int CandidateId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CandidateKeySkillGuid { get; set; }
        public string KeySkill { get; set; }
        public decimal? YearsOfExperience { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Note { get; set; }
    
        public virtual Candidate Candidate { get; set; }
    }

    public class CandidateKeySkillExtended
    {
        public int Id { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public Guid CandidateGuid { get; set; }
        public Guid CandidateKeySkillGuid { get; set; }
        public int CandidateId { get; set; }
        public string EmployeeId { get; set; }
        public string KeySkill { get; set; }
        public decimal? YearsOfExperience { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public string Note { get; set; }
        public string CandidateName { get; set; }
    }
}