﻿using System.Web.Mvc;
using System.Web.Routing;
using Wfm.Web.Framework.Localization;
using Wfm.Web.Framework.Mvc.Routes;

namespace Wfm.Web.Infrastructure
{
    //Routes used for backward compatibility with 2.x versions of wfmCommerce
    public partial class BackwardCompatibility2XRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //products
            routes.MapLocalizedRoute("", "p/{productId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectProductById", SeName = UrlParameter.Optional },
                new { productId = @"\d+" },
                new[] { "Wfm.Web.Controllers" });

            //categories
            routes.MapLocalizedRoute("", "c/{categoryId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectCategoryById", SeName = UrlParameter.Optional },
                new { categoryId = @"\d+" },
                new[] { "Wfm.Web.Controllers" });

            //manufacturers
            routes.MapLocalizedRoute("", "m/{manufacturerId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectManufacturerById", SeName = UrlParameter.Optional },
                new { manufacturerId = @"\d+" },
                new[] { "Wfm.Web.Controllers" });

            //news
            routes.MapLocalizedRoute("", "news/{newsItemId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectNewsItemById", SeName = UrlParameter.Optional },
                new { newsItemId = @"\d+" },
                new[] { "Wfm.Web.Controllers" });

            //blog
            routes.MapLocalizedRoute("", "blog/{blogPostId}/{SeName}",
                new { controller = "BackwardCompatibility2X", action = "RedirectBlogPostById", SeName = UrlParameter.Optional },
                new { blogPostId = @"\d+" },
                new[] { "Wfm.Web.Controllers" });
        }

        public int Priority
        {
            get
            {
                //register it after all other IRouteProvider are processed
                return -1000;
            }
        }
    }
}
