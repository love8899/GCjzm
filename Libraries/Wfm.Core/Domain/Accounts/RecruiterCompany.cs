using Wfm.Core.Domain.Companies;

namespace Wfm.Core.Domain.Accounts
{
    public class RecruiterCompany:BaseEntity
    {
        public int AccountId { get; set; }
        public int CompanyId { get; set; }
        //public bool IsActive { get; set; }
        public virtual Account Account { get; set; }
        public virtual Company Company { get; set; }
    }
}
