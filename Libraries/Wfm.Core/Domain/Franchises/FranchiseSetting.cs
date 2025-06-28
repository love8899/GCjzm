namespace Wfm.Core.Domain.Franchises
{
    public partial class FranchiseSetting : BaseEntity
    {
        public FranchiseSetting() { }

        public FranchiseSetting(int franchiseId, string name, string value)
        {
            this.FranchiseId = franchiseId;
            this.Name = name;
            this.Value = value;
        }

        public int FranchiseId { get; set; }
        
        public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
