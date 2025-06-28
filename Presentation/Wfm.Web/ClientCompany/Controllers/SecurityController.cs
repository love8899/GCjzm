using System.Web.Mvc;
using Wfm.Core;
using Wfm.Services.Logging;

namespace Wfm.Client.Controllers
{
    public partial class SecurityController : BaseClientController
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public SecurityController(
            ILogger logger,
            IWorkContext workContext
            )
        {
            this._logger = logger;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult AccessDenied(string pageUrl)
        {
            var currentAccount = _workContext.CurrentAccount;
            if (currentAccount == null)
            {
                _logger.Information(string.Format("Access denied to anonymous request on {0}", pageUrl), userAgent: Request.UserAgent);

                return View();
            }

            _logger.Information(string.Format("Access denied to user #{0} '{1}' on {2}", currentAccount.Email, currentAccount.Email, pageUrl), 
                                userAgent: Request.UserAgent);

            return View();
        }

        #endregion
    } 
} 
