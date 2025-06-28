using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Candidate
{
    public class CandidatePasswordHistoryModel: BaseWfmEntityModel
    {
        public int CandidateId { get; set; }
        public string Password { get; set; }
        public int PasswordFormatId { get; set; }
        public string PasswordSalt { get; set; }
        public int EnteredBy { get; set; }
    }

}
