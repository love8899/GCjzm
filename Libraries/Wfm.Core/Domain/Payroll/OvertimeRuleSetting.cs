
namespace Wfm.Core.Domain.Payroll
{
    public partial class OvertimeRuleSetting
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public decimal ApplyAfter { get; set; }
        public int PayrollItemId { get; set; }
        public decimal Rate { get; set; }
        public virtual Payroll_Item Payroll_Item { get; set; }
        public virtual OvertimeType OvertimeType { get; set; }
    }
}
