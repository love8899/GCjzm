using System;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Franchises;
using Wfm.Services.Franchises;

namespace Wfm.Web.Framework
{
    /// <summary>
    /// Franchise context for web application
    /// </summary>
    public partial class WebFranchiseContext : IFranchiseContext
    {
        private readonly IFranchiseService _franchiseService;
        private readonly IWebHelper _webHelper;

        private Franchise _cachedFranchise;

        public WebFranchiseContext(IFranchiseService franchiseService, IWebHelper webHelper)
        {
            this._franchiseService = franchiseService;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Gets or sets the current franchise
        /// </summary>
        public virtual Franchise CurrentFranchise
        {
            get
            {
                if (_cachedFranchise != null)
                    return _cachedFranchise;

                //ty to determine the current franchise by HTTP_HOST
                var host = _webHelper.ServerVariables("HTTP_HOST");
                var allFranchises = _franchiseService.GetAllFranchises();
                var franchise = allFranchises.FirstOrDefault(s => s.ContainsHostValue(host));

                if (franchise == null)
                {
                    //load the first found franchise
                    franchise = allFranchises.FirstOrDefault();
                }
                if (franchise == null)
                    throw new Exception("No franchise could be loaded");

                _cachedFranchise = franchise;
                return _cachedFranchise;
            }
        }
    }
}
