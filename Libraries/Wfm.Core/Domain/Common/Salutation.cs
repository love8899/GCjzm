namespace Wfm.Core.Domain.Common
{
    public class Salutation : BaseEntity
    {
        public string SalutationName { get; set; }

        public int GenderId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Gender Gender { get; set; }
    }
}
