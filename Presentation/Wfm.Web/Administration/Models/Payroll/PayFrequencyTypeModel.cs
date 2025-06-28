using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Payroll
{
    public class PayFrequencyTypeModel : BaseWfmEntityModel
    {
      //  public int Id { get; set; }
        [WfmResourceDisplayName("Common.Code")]
        public string Code { get; set; }
        [WfmResourceDisplayName("Common.Description")]
        public string Description { get; set; }
        [WfmResourceDisplayName("Common.PayFrequency")]
        public int Frequency { get; set; }
    }
}