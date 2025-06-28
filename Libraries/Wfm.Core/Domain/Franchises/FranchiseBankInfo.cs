using Wfm.Core.Domain.Common;


namespace Wfm.Core.Domain.Franchises
{
    public class FranchiseBankInfo : BaseEntity
    {
        public int FranchiseId { get; set; }
        public int BankId { get; set; }
        public int BankAccountTypeId { get; set; }
        public string BankCode { get; set; }
        public string ClientNumber { get; set; }
        public string ClientLongName { get; set; }
        public string ClientShortName { get; set; }
        public int NextSequenceNo { get; set; }
        public string TransactionCode { get; set; }
        public string OriginatorId { get; set; }
        public string HeaderString { get; set; }
        public string InstitutionNumber  { get; set; }
        public string TransitNumber { get; set; }

        
        public string Note { get; set; }
        public int EnteredBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int DisplayOrder { get; set; }




        public BankAccountType BankAccountType
        {
            get
            {
                return (BankAccountType)this.BankAccountTypeId;
            }
            set
            {
                this.BankAccountTypeId = (int)value;
            }
        }





        public virtual Franchise Franchise { get; set; }
    }
}