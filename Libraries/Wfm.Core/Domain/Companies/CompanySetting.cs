namespace Wfm.Core.Domain.Companies
{
    public partial class CompanySetting : BaseEntity
    {
        public CompanySetting() { }

        public CompanySetting(int companyId, string name, string value)
        {
            this.CompanyId = companyId;
            this.Name = name;
            this.Value = value;
        }

        public int CompanyId { get; set; }
        
        public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
