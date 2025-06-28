
namespace Wfm.Core.Domain.Candidates
{
    public class CandidatePasswordHistory : BaseEntity
    {
        public int CandidateId { get; set; }
        public string Password { get; set; }
        public int PasswordFormatId { get; set; }
        public string PasswordSalt { get; set; }
        public int EnteredBy { get; set; }
    }
}