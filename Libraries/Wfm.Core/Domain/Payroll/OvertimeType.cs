
namespace Wfm.Core.Domain.Payroll
{
    public partial class OvertimeType
    {
        //public OvertimeType()
        //{
        //    this.OvertimeRuleSettings = new List<OvertimeRuleSetting>();
        //}

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        //public virtual ICollection<OvertimeRuleSetting> OvertimeRuleSettings { get; set; }
    }
}
