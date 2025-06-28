
using Wfm.Web.Framework;
namespace Wfm.Admin.Models.Payroll
{
    public class PayrollItemModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        [WfmResourceDisplayName("Common.Name")]
        public string Name { get; set; }
        public string Type { get; set; }
        [WfmResourceDisplayName("Common.Vendor")]
        public string VendorName { get; set; }
    }
}