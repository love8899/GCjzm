//Contributor : MVCContrib

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Wfm.Core.Infrastructure;
using Wfm.Services.Localization;

namespace Wfm.Web.Framework.UI.Paging
{
    /// <summary>
    /// Renders a pager component from an IPageableModel datasource.
    /// </summary>
    public partial class Pager : IHtmlString
    {
        protected readonly IPageableModel model;
        protected readonly ViewContext viewContext;
        protected string pageQueryName = "page";
        protected string customQueryKey = "";
        protected string customQueryValue = "";
        protected bool showTotalSummary;
        protected bool showPagerItems = true;
        protected bool showFirst = true;
        protected bool showPrevious = true;
        protected bool showNext = true;
        protected bool showLast = true;
        protected bool showIndividualPages = true;
        protected int individualPagesDisplayedCount = 5;
        protected Func<int, string> urlBuilder;
        protected IList<string> booleanParameterNames;

        public Pager(IPageableModel model, ViewContext context)
        {
            this.model = model;
            this.viewContext = context;
            this.urlBuilder = CreateDefaultUrl;
            this.booleanParameterNames = new List<string>();
        }

        protected ViewContext ViewContext
        {
            get { return viewContext; }
        }

        public Pager QueryParam(string value)
        {
            this.pageQueryName = value;
            return this;
        }
        public Pager CustomQueryParam(string key, string value)
        {
            this.customQueryKey = key;
            this.customQueryValue = value;
            return this;
        }

        public Pager ShowTotalSummary(bool value)
        {
            this.showTotalSummary = value;
            return this;
        }
        public Pager ShowPagerItems(bool value)
        {
            this.showPagerItems = value;
            return this;
        }
        public Pager ShowFirst(bool value)
        {
            this.showFirst = value;
            return this;
        }
        public Pager ShowPrevious(bool value)
        {
            this.showPrevious = value;
            return this;
        }
        public Pager ShowNext(bool value)
        {
            this.showNext = value;
            return this;
        }
        public Pager ShowLast(bool value)
        {
            this.showLast = value;
            return this;
        }
        public Pager ShowIndividualPages(bool value)
        {
            this.showIndividualPages = value;
            return this;
        }
        public Pager IndividualPagesDisplayedCount(int value)
        {
            this.individualPagesDisplayedCount = value;
            return this;
        }
        public Pager Link(Func<int, string> value)
        {
            this.urlBuilder = value;
            return this;
        }
        //little hack here due to ugly MVC implementation
        //find more info here: http://www.mindstorminteractive.com/blog/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
        public Pager BooleanParameterName(string paramName)
        {
            booleanParameterNames.Add(paramName);
            return this;
        }

        public override string ToString()
        {
            return ToHtmlString();
        }
        public virtual string ToHtmlString()
        {
            if (model.TotalItems == 0)
                return null;
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var links = new StringBuilder();

            if (showTotalSummary && (model.TotalPages > 0))
            {
                links.Append("<li class=\"total-summary\">");
                links.Append(string.Format(localizationService.GetResource("Pager.CurrentPage"), model.PageIndex + 1, model.TotalPages, model.TotalItems));
                links.Append("</li>");
            }
            if (showPagerItems && (model.TotalPages > 1))
            {
                if (showFirst)
                {
                    if ((model.PageIndex >= 3) && (model.TotalPages > individualPagesDisplayedCount))
                    {
                        //if (showIndividualPages)
                        //{
                        //    links.Append("&nbsp;");
                        //}

                        links.Append(CreatePageLink(1, localizationService.GetResource("Pager.First"), "first-page"));

                        if ((showIndividualPages || (showPrevious && (model.PageIndex > 0))) || showLast)
                        {
                            links.Append("<li>&nbsp;...&nbsp;</li>");
                        }
                    }
                }
                if (showPrevious)
                {
                    if (model.PageIndex > 0)
                    {
                        links.Append(CreatePageLink(model.PageIndex, localizationService.GetResource("Pager.Previous"), "previous-page"));

                        //if ((showIndividualPages || showLast) || (showNext && ((model.PageIndex + 1) < model.TotalPages)))
                        //{
                        //    links.Append("&nbsp;");
                        //}
                    }
                }
                if (showIndividualPages)
                {
                    int firstIndividualPageIndex = GetFirstIndividualPageIndex();
                    int lastIndividualPageIndex = GetLastIndividualPageIndex();
                    for (int i = firstIndividualPageIndex; i <= lastIndividualPageIndex; i++)
                    {
                        if (model.PageIndex == i)
                        {
                            links.AppendFormat("<li class=\"current-page\"><span>{0}</span></li>", (i + 1));
                        }
                        else
                        {
                            links.Append(CreatePageLink(i + 1, (i + 1).ToString(), "individual-page"));
                        }
                        //if (i < lastIndividualPageIndex)
                        //{
                        //    links.Append("&nbsp;");
                        //}
                    }
                }
                if (showNext)
                {
                    if ((model.PageIndex + 1) < model.TotalPages)
                    {
                        //if (showIndividualPages)
                        //{
                        //    links.Append("&nbsp;");
                        //}

                        links.Append(CreatePageLink(model.PageIndex + 2, localizationService.GetResource("Pager.Next"), "next-page"));
                    }
                }
                if (showLast)
                {
                    if (((model.PageIndex + 3) < model.TotalPages) && (model.TotalPages > individualPagesDisplayedCount))
                    {
                        if (showIndividualPages || (showNext && ((model.PageIndex + 1) < model.TotalPages)))
                        {
                            links.Append("<li>&nbsp;...&nbsp;</li>");
                        }

                        links.Append(CreatePageLink(model.TotalPages, localizationService.GetResource("Pager.Last"), "last-page"));
                    }
                }
            }

            var result = links.ToString();
            if (!String.IsNullOrEmpty(result))
            {
                result = "<ul>" + result + "</ul>";
            }
            return result;
        }


        public virtual bool IsEmpty()
        {
            var html = ToString();
            return string.IsNullOrEmpty(html);
        }


        protected virtual int GetFirstIndividualPageIndex()
        {
            if ((model.TotalPages < individualPagesDisplayedCount) ||
                ((model.PageIndex - (individualPagesDisplayedCount / 2)) < 0))
            {
                return 0;
            }
            if ((model.PageIndex + (individualPagesDisplayedCount / 2)) >= model.TotalPages)
            {
                return (model.TotalPages - individualPagesDisplayedCount);
            }
            return (model.PageIndex - (individualPagesDisplayedCount / 2));
        }

        protected virtual int GetLastIndividualPageIndex()
        {
            int num = individualPagesDisplayedCount / 2;
            if ((individualPagesDisplayedCount % 2) == 0)
            {
                num--;
            }
            if ((model.TotalPages < individualPagesDisplayedCount) ||
                ((model.PageIndex + num) >= model.TotalPages))
            {
                return (model.TotalPages - 1);
            }
            if ((model.PageIndex - (individualPagesDisplayedCount / 2)) < 0)
            {
                return (individualPagesDisplayedCount - 1);
            }
            return (model.PageIndex + num);
        }
        protected virtual string CreatePageLink(int pageNumber, string text, string cssClass)
        {
            var liBuilder = new TagBuilder("li");
            if (!String.IsNullOrWhiteSpace(cssClass))
                liBuilder.AddCssClass(cssClass);

            var aBuilder = new TagBuilder("a");
            aBuilder.SetInnerText(text);
            aBuilder.MergeAttribute("href", urlBuilder(pageNumber));

            liBuilder.InnerHtml += aBuilder;

            return liBuilder.ToString(TagRenderMode.Normal);
        }

        protected virtual string CreateDefaultUrl(int pageNumber)
        {
            var routeValues = new RouteValueDictionary();

            foreach (var key in viewContext.RequestContext.HttpContext.Request.QueryString.AllKeys.Where(key => key != null))
            {
                var value = viewContext.RequestContext.HttpContext.Request.QueryString[key];
                if (booleanParameterNames.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                {
                    //little hack here due to ugly MVC implementation
                    //find more info here: http://www.mindstorminteractive.com/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
                    if (!String.IsNullOrEmpty(value) && value.Equals("true,false", StringComparison.InvariantCultureIgnoreCase))
                    {
                        value = "true";
                    }
                }
                routeValues[key] = value;
            }

            if (pageNumber > 1)
            {
                routeValues[pageQueryName] = pageNumber;
            }
            else
            {
                //SEO. we do not render pageindex query string parameter for the first page
                if (routeValues.ContainsKey(pageQueryName))
                {
                    routeValues.Remove(pageQueryName);
                }
            }

            // Add custom Query key / value
            routeValues[customQueryKey] = customQueryValue;

            var url = UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, viewContext.RequestContext, true);
            return url;
        }
    }
}