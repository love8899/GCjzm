using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Common
{
    public partial class SystemWarningModel : BaseWfmModel
    {
        public SystemWarningLevel Level { get; set; }

        public string Text { get; set; }
    }

    public enum SystemWarningLevel
    {
        Pass,
        Warning,
        Fail
    }
}