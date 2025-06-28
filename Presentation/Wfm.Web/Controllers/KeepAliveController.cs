using System.Web.Mvc;
//using Wfm.Services.Logging;

namespace Wfm.Web.Controllers
{
    public partial class KeepAliveController : BasePublicController
    {
        #region Fields
        //private readonly ILogger _logger;
        #endregion

        #region Ctor

        //public KeepAliveController(ILogger logger)
        //{
        //    _logger = logger; 
        //}

        #endregion

        public ActionResult Index()
        {
            return Content("I am alive!");
        }
    }
}
