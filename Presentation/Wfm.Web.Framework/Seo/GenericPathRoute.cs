﻿using System.Web;
using System.Web.Routing;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Infrastructure;
using Wfm.Services.Seo;
using Wfm.Web.Framework.Localization;

namespace Wfm.Web.Framework.Seo
{
    /// <summary>
    /// Provides properties and methods for defining a SEO friendly route, and for getting information about the route.
    /// </summary>
    public partial class GenericPathRoute : LocalizedRoute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern and handler class.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public GenericPathRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class and default parameter values.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public GenericPathRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values and constraints.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public GenericPathRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values, 
        /// constraints,and custom values.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="dataTokens">Custom values that are passed to the route handler, but which are not used to determine whether the route matches a specific URL pattern. The route handler might need these values to process the request.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public GenericPathRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Returns information about the requested route.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition.
        /// </returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData data = base.GetRouteData(httpContext);
            if (data != null && DataSettingsHelper.DatabaseIsInstalled())
            {
                var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
                var slug = data.Values["generic_se_name"] as string;
                var urlRecord = urlRecordService.GetBySlug(slug);
                if (urlRecord == null)
                {
                    //no URL record found

                    //var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    //var response = httpContext.Response;
                    //response.Status = "302 Found";
                    //response.RedirectLocation = webHelper.GetStoreLocation(false);
                    //response.End();
                    //return null;

                    data.Values["controller"] = "Common";
                    data.Values["action"] = "PageNotFound";
                    return data;
                }
                if (!urlRecord.IsActive)
                {
                    //URL record is not active. let's find the latest one
                    var activeSlug = urlRecordService.GetActiveSlug(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
                    if (!string.IsNullOrWhiteSpace(activeSlug))
                    {
                        //the active one is found
                        var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                        var response = httpContext.Response;
                        response.Status = "301 Moved Permanently";
                        response.RedirectLocation = string.Format("{0}{1}", webHelper.GetFranchiseUrl(false), activeSlug);
                        response.End();
                        return null;
                    }
                    else
                    {
                        //no active slug found
                        
                        //var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                        //var response = httpContext.Response;
                        //response.Status = "302 Found";
                        //response.RedirectLocation = webHelper.GetStoreLocation(false);
                        //response.End();
                        //return null;
                        
                        data.Values["controller"] = "Common";
                        data.Values["action"] = "PageNotFound";
                        return data;
                    }
                }

                //process URL
                switch (urlRecord.EntityName.ToLowerInvariant())
                {
                    case "candidate":
                        {
                            data.Values["controller"] = "Candidate";
                            data.Values["action"] = "Index";
                        }
                        break;

                    case "blogpost":
                        {
                            data.Values["controller"] = "Blog";
                            data.Values["action"] = "BlogPost";
                            data.Values["blogPostId"] = urlRecord.EntityId;
                            data.Values["SeName"] = urlRecord.Slug;
                        }
                        break;

                    case "joborder":
                        {
                            data.Values["controller"] = "JobPost";
                            data.Values["action"] = "JobDetails";
                            data.Values["JobOrderId"] = urlRecord.EntityId;
                            data.Values["SeName"] = urlRecord.Slug;
                        }
                        break;

                    default:
                        {
                            //no record found
                            throw new WfmException(string.Format("Not supported EntityName for UrlRecord: {0}", urlRecord.EntityName));
                        }
                }
            }
            return data;
        }

        //public override RouteData GetRouteData(HttpContextBase httpContext){
          
        //    RouteData data = base.GetRouteData(httpContext);

        //    return data;
        //}

        #endregion
    }
}