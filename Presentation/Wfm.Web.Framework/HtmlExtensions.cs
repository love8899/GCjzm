using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using Wfm.Core;
using Wfm.Core.Infrastructure;
using Wfm.Services.Localization;
using Wfm.Web.Framework.Localization;
using Wfm.Web.Framework.Mvc;
using Kendo.Mvc.UI;
using ZXing;
using ZXing.Common;


namespace Wfm.Web.Framework
{
    public static class HtmlExtensions
    {
        #region Admin area extensions

        public static MvcHtmlString ToSeoUrl(this HtmlHelper htmlHelper, string url)
        {
            // make the url lowercase
            string encodedUrl = (url ?? "").ToLower();
            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "-");
            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");
            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");
            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");
            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return MvcHtmlString.Create(encodedUrl);
        }


        /// <summary>
        /// Canonicals url tag for SEO
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static MvcHtmlString CanonicalUrl(this HtmlHelper html, string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                var rawUrl = html.ViewContext.RequestContext.HttpContext.Request.Url;
                path = String.Format("{0}://{1}{2}", rawUrl.Scheme, rawUrl.Host, rawUrl.AbsolutePath);
            }

            path = path.ToLower();

            if (path.Count(c => c == '/') > 3)
            {
                path = path.TrimEnd('/');
            }

            if (path.EndsWith("/index"))
            {
                path = path.Substring(0, path.Length - 6);
            }

            var canonical = new TagBuilder("link");
            canonical.MergeAttribute("href", path);
            canonical.MergeAttribute("rel", "canonical");

            var ogUrl = new TagBuilder("meta");
            ogUrl.MergeAttribute("content", path);
            ogUrl.MergeAttribute("name", "og:url");

            return new MvcHtmlString(ogUrl.ToString(TagRenderMode.SelfClosing) + Environment.NewLine + "    " + canonical.ToString(TagRenderMode.SelfClosing));
        }

        /// <summary>
        /// Canonicals url tag for SEO
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        public static MvcHtmlString CanonicalUrl(this HtmlHelper html)
        {
            var rawUrl = html.ViewContext.RequestContext.HttpContext.Request.Url;

            return CanonicalUrl(html, String.Format("{0}://{1}{2}", rawUrl.Scheme, rawUrl.Host, rawUrl.AbsolutePath));
        }


        public static MvcHtmlString ResolveUrl(this HtmlHelper htmlHelper, string url)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return MvcHtmlString.Create(urlHelper.Content(url));
        }

        public static MvcHtmlString Hint(this HtmlHelper helper, string value)
        {
            //// Create tag builder
            //var builder = new TagBuilder("img");

            //// Add attributes
            //var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            //var url = MvcHtmlString.Create(urlHelper.Content("~/Administration/Content/images/ico-help.gif")).ToHtmlString();

            //builder.MergeAttribute("src", url);
            //builder.MergeAttribute("alt", value);
            //builder.MergeAttribute("title", value);

            //// Render tag
            //return MvcHtmlString.Create(builder.ToString());

            //create tag builder
            var builder = new TagBuilder("div");
            builder.MergeAttribute("title", value);
            builder.MergeAttribute("class", "ico-help");
            var icon = new StringBuilder();
            icon.Append("<i class='fa fa-question-circle'></i>");
            builder.InnerHtml = icon.ToString();
            //render tag
            return MvcHtmlString.Create(builder.ToString());

        }

        public static HelperResult LocalizedEditor<T, TLocalizedModelLocal>(this HtmlHelper<T> helper, string name,
             Func<int, HelperResult> localizedTemplate,
             Func<T, HelperResult> standardTemplate)
            where T : ILocalizedModel<TLocalizedModelLocal>
            where TLocalizedModelLocal : ILocalizedModelLocal
        {
            return new HelperResult(writer =>
            {
                if (helper.ViewData.Model.Locales.Count > 1)
                {
                    var tabStrip = helper.Kendo().TabStrip().Name(name).Items(x =>
                    {
                        x.Add().Text("Standard").Content(standardTemplate(helper.ViewData.Model).ToHtmlString()).Selected(true);
                        for (int i = 0; i < helper.ViewData.Model.Locales.Count; i++)
                        {
                            var locale = helper.ViewData.Model.Locales[i];
                            var language = EngineContext.Current.Resolve<ILanguageService>().GetLanguageById(locale.LanguageId);
                            x.Add().Text(language.Name)
                                .Content(localizedTemplate
                                    (i).
                                    ToHtmlString
                                    ())
                                .ImageUrl("~/Content/images/flags/" + language.FlagImageFileName);
                        }
                    }).ToHtmlString();
                    writer.Write(tabStrip);
                }
                else
                {
                    standardTemplate(helper.ViewData.Model).WriteTo(writer);
                }
            });
        }

        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string buttonsSelector) where T : BaseWfmEntityModel
        {
            return DeleteConfirmation(helper, "", buttonsSelector);
        }

        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string actionName,
            string buttonsSelector) where T : BaseWfmEntityModel
        {
            if (String.IsNullOrEmpty(actionName))
                actionName = "Delete";

            var modalId =  MvcHtmlString.Create(helper.ViewData.ModelMetadata.ModelType.Name.ToLower() + "-delete-confirmation")
                .ToHtmlString();

            var deleteConfirmationModel = new DeleteConfirmationModel
            {
                Id = helper.ViewData.Model.Id,
                ControllerName = helper.ViewContext.RouteData.GetRequiredString("controller"),
                ActionName = actionName,
                WindowId = modalId
            };

            var window = new StringBuilder();
            window.AppendLine(string.Format("<div id='{0}' style='display:none;'>", modalId));
            window.AppendLine(helper.Partial("Delete", deleteConfirmationModel).ToHtmlString());
            window.AppendLine("</div>");
            window.AppendLine("<script>");
            window.AppendLine("$(document).ready(function() {");
            window.AppendLine(string.Format("$('#{0}').click(function (e) ", buttonsSelector));
            window.AppendLine("{");
            window.AppendLine("e.preventDefault();");
            window.AppendLine(string.Format("var window = $('#{0}');", modalId));
            window.AppendLine("if (!window.data('kendoWindow')) {");
            window.AppendLine("window.kendoWindow({");
            window.AppendLine("modal: true,");
            window.AppendLine(string.Format("title: '{0}',", EngineContext.Current.Resolve<ILocalizationService>().GetResource("Common.Confirm")));
            window.AppendLine("actions: ['Close']");
            window.AppendLine("});");
            window.AppendLine("}");
            window.AppendLine("window.data('kendoWindow').center().open();");
            window.AppendLine("});");
            window.AppendLine("});");
            window.AppendLine("</script>");

            return MvcHtmlString.Create(window.ToString());
        }


        public static MvcHtmlString WfmLabel<TModel>(this HtmlHelper<TModel> helper, string expression, string labelText = null, bool displayHint = true)
        {
            var result = new StringBuilder();

            var metadata = ModelMetadata.FromStringExpression(expression, helper.ViewData);
            var hintResource = string.Empty;
            object value;

            if (String.IsNullOrWhiteSpace(labelText))
                result.Append(helper.Label(expression, new { title = hintResource, @class = "control-label" }));
            else
                result.Append(helper.Label(expression, labelText, new { title = hintResource, @class = "control-label" }));

            if (metadata.AdditionalValues.TryGetValue("WfmResourceDisplayName", out value))
            {
                var resourceDisplayName = value as WfmResourceDisplayName;
                if (resourceDisplayName != null && displayHint)
                {
                    var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                    hintResource = EngineContext.Current.Resolve<ILocalizationService>()
                        .GetResource(resourceDisplayName.ResourceKey + ".Hint", langId, returnEmptyIfNotFound: true, logIfNotFound: false);
                    if (!String.IsNullOrEmpty(hintResource))
                    {
                        result.Append(helper.Hint(hintResource).ToHtmlString());
                    }
                }
            }

            var laberWrapper = new TagBuilder("div");
            laberWrapper.Attributes.Add("class", "label-wrapper");
            laberWrapper.InnerHtml = result.ToString();

            return MvcHtmlString.Create(laberWrapper.ToString());
        }


        public static MvcHtmlString WfmLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool displayHint = true, 
            string labelText = null)
        {
            var result = new StringBuilder();
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var hintResource = string.Empty;
            object value;

            if (String.IsNullOrWhiteSpace(labelText))
                result.Append(helper.LabelFor(expression, new { title = hintResource, @class = "control-label" }));
            else
                result.Append(helper.LabelFor(expression, labelText, new { title = hintResource, @class = "control-label" }));

            if (metadata.AdditionalValues.TryGetValue("WfmResourceDisplayName", out value))
            {
                var resourceDisplayName = value as WfmResourceDisplayName;
                if (resourceDisplayName != null && displayHint)
                {
                    var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                    hintResource = EngineContext.Current.Resolve<ILocalizationService>()
                        .GetResource(resourceDisplayName.ResourceKey + ".Hint", langId, returnEmptyIfNotFound: true, logIfNotFound: false);
                    if (!String.IsNullOrEmpty(hintResource))
                    {
                        result.Append(helper.Hint(hintResource).ToHtmlString());
                    }
                }
            }

            var laberWrapper = new TagBuilder("div");
            laberWrapper.Attributes.Add("class", "label-wrapper");
            laberWrapper.InnerHtml = result.ToString();

            return MvcHtmlString.Create(laberWrapper.ToString());
        }


        public static MvcHtmlString WfmEditorFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, string postfix = "",
            bool? renderFormControlClass = null, bool required = false)
        {
            var result = new StringBuilder();

            object htmlAttributes = null;
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            if ((!renderFormControlClass.HasValue && metadata.ModelType.Name.Equals("String")) ||
                (renderFormControlClass.HasValue && renderFormControlClass.Value))
                htmlAttributes = new { @class = "form-control" };

            if (required)
                result.AppendFormat(
                    "<div class=\"input-group input-group-required\">{0}<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>",
                    helper.EditorFor(expression, new { htmlAttributes, postfix }));
            else
                result.Append(helper.EditorFor(expression, new { htmlAttributes, postfix }));

            return MvcHtmlString.Create(result.ToString());
        }


        public static MvcHtmlString WfmDropDownList<TModel>(this HtmlHelper<TModel> helper, string name,
            IEnumerable<SelectListItem> itemList, object htmlAttributes = null,
            bool renderFormControlClass = true, bool required = false)
        {
            var result = new StringBuilder();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (renderFormControlClass)
                attrs = AddFormControlClassToHtmlAttributes(attrs);

            if (required)
                result.AppendFormat(
                    "<div class=\"input-group input-group-required\">{0}<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>",
                    helper.DropDownList(name, itemList, attrs));
            else
                result.Append(helper.DropDownList(name, itemList, attrs));

            return MvcHtmlString.Create(result.ToString());
        }


        public static MvcHtmlString WfmDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> itemList,
            object htmlAttributes = null, bool renderFormControlClass = true, bool required = false)
        {
            var result = new StringBuilder();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (renderFormControlClass)
                attrs = AddFormControlClassToHtmlAttributes(attrs);

            if (required)
                result.AppendFormat(
                    "<div class=\"input-group input-group-required\">{0}<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>",
                    helper.DropDownListFor(expression, itemList, attrs));
            else
                result.Append(helper.DropDownListFor(expression, itemList, attrs));

            return MvcHtmlString.Create(result.ToString());
        }


        public static MvcHtmlString WfmTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null,
            bool renderFormControlClass = true, int rows = 4, int columns = 20, bool required = false)
        {
            var result = new StringBuilder();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (renderFormControlClass)
                attrs = AddFormControlClassToHtmlAttributes(attrs);

            if (required)
                result.AppendFormat(
                    "<div class=\"input-group input-group-required\">{0}<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>",
                    helper.TextAreaFor(expression, rows, columns, attrs));
            else
                result.Append(helper.TextAreaFor(expression, rows, columns, attrs));

            return MvcHtmlString.Create(result.ToString());
        }


        public static MvcHtmlString WfmDisplayFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var result = new TagBuilder("div");
            result.Attributes.Add("class", "form-text-row");
            result.InnerHtml = helper.DisplayFor(expression).ToString();

            return MvcHtmlString.Create(result.ToString());
        }


        public static MvcHtmlString WfmDisplay<TModel>(this HtmlHelper<TModel> helper, string expression)
        {
            var result = new TagBuilder("div");
            result.Attributes.Add("class", "form-text-row");
            result.InnerHtml = expression;

            return MvcHtmlString.Create(result.ToString());
        }


        public static RouteValueDictionary AddFormControlClassToHtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes["class"] == null || string.IsNullOrEmpty(htmlAttributes["class"].ToString()))
                htmlAttributes["class"] = "form-control";
            else
                if (!htmlAttributes["class"].ToString().Contains("form-control"))
                htmlAttributes["class"] += " form-control";

            return htmlAttributes as RouteValueDictionary;
        }


        /// <summary>
        /// Render CSS styles of selected index 
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <param name="currentIndex">Current tab index (where appropriate CSS style should be rendred)</param>
        /// <param name="indexToSelect">Tab index to select</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString RenderSelectedTabIndex(this HtmlHelper helper, int currentIndex, int indexToSelect)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            //ensure it's not negative
            if (indexToSelect < 0)
                indexToSelect = 0;
            
            //required validation
            if (indexToSelect == currentIndex)
            {
            return new MvcHtmlString(" class='k-state-active'");
            }

            return new MvcHtmlString("");
        }


        /// <summary>
        /// Render CSS styles of selected index 
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <param name="currentTabName">Current tab name (where appropriate CSS style should be rendred)</param>
        /// <param name="content">Tab content</param>
        /// <param name="isDefaultTab">Indicates that the tab is default</param>
        /// <param name="tabNameToSelect">Tab name to select</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString RenderBootstrapTabContent(this HtmlHelper helper, string currentTabName,
            HelperResult content, bool isDefaultTab = false, string tabNameToSelect = "")
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            if (string.IsNullOrEmpty(tabNameToSelect))
                tabNameToSelect = helper.GetSelectedTabName();

            if (string.IsNullOrEmpty(tabNameToSelect) && isDefaultTab)
                tabNameToSelect = currentTabName;

            var tag = new TagBuilder("div")
            {
                InnerHtml = content.ToHtmlString(),
                Attributes =
                {
                    new KeyValuePair<string, string>("class", string.Format("tab-pane{0}", tabNameToSelect == currentTabName ? " active" : "")),
                    new KeyValuePair<string, string>("id", string.Format("{0}", currentTabName))
                }
            };

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }


        /// <summary>
        /// Render CSS styles of selected index 
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <param name="currentTabName">Current tab name (where appropriate CSS style should be rendred)</param>
        /// <param name="title">Tab title</param>
        /// <param name="isDefaultTab">Indicates that the tab is default</param>
        /// <param name="tabNameToSelect">Tab name to select</param>
        /// <param name="customCssClass">Tab name to select</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString RenderBootstrapTabHeader(this HtmlHelper helper, string currentTabName,
            LocalizedString title, bool isDefaultTab = false, string tabNameToSelect = "", string customCssClass = "")
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            if (string.IsNullOrEmpty(tabNameToSelect))
                tabNameToSelect = helper.GetSelectedTabName();

            if (string.IsNullOrEmpty(tabNameToSelect) && isDefaultTab)
                tabNameToSelect = currentTabName;

            var a = new TagBuilder("a")
            {
                Attributes =
                {
                    new KeyValuePair<string, string>("data-tab-name", currentTabName),
                    new KeyValuePair<string, string>("href", string.Format("#{0}", currentTabName)),
                    new KeyValuePair<string, string>("data-toggle", "tab"),
                },
                InnerHtml = title.Text
            };
            var liClassValue = "";
            if (tabNameToSelect == currentTabName)
            {
                liClassValue = "active";
            }
            if (!String.IsNullOrEmpty(customCssClass))
            {
                if (!String.IsNullOrEmpty(liClassValue))
                    liClassValue += " ";
                liClassValue += customCssClass;
            }

            var li = new TagBuilder("li")
            {
                Attributes =
                {
                    new KeyValuePair<string, string>("class", liClassValue),
                },
                InnerHtml = a.ToString(TagRenderMode.Normal)
            };

            return MvcHtmlString.Create(li.ToString(TagRenderMode.Normal));
        }


        /// <summary>
        /// Gets a selected tab name (used in admin area to store selected tab name)
        /// </summary>
        /// <returns>Name</returns>
        public static string GetSelectedTabName(this HtmlHelper helper)
        {
            //keep this method synchornized with
            //"SaveSelectedTab" method of \Administration\Controllers\BaseAdminController.cs
            var tabName = string.Empty;
            const string dataKey = "wfm.selected-tab-name";

            if (helper.ViewData.ContainsKey(dataKey))
                tabName = helper.ViewData[dataKey].ToString();

            if (helper.ViewContext.Controller.TempData.ContainsKey(dataKey))
                tabName = helper.ViewContext.Controller.TempData[dataKey].ToString();

            return tabName;
        }

        #endregion


        #region Common extensions

        public static MvcHtmlString RequiredHint(this HtmlHelper helper, string additionalText = null)
        {
            // Create tag builder
            var builder = new TagBuilder("span");
            builder.AddCssClass("required");
            var innerText = "*";
            //add additional text if specified
            if (!String.IsNullOrEmpty(additionalText))
                innerText += " " + additionalText;
            builder.SetInnerText(innerText);
            // Render tag
            return MvcHtmlString.Create(builder.ToString());
        }

        public static string FieldNameFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            return html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }
        public static string FieldIdFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            var id = html.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            // because "[" and "]" aren't replaced with "_" in GetFullHtmlFieldId
            return id.Replace('[', '_').Replace(']', '_');
        }

        /// <summary>
        /// Creates a days, months, years drop down list using an HTML select control. 
        /// The parameters represent the value of the "name" attribute on the select control.
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="dayName">"Name" attribute of the day drop down list.</param>
        /// <param name="monthName">"Name" attribute of the month drop down list.</param>
        /// <param name="yearName">"Name" attribute of the year drop down list.</param>
        /// <param name="beginYear">Begin year</param>
        /// <param name="endYear">End year</param>
        /// <param name="selectedDay">Selected day</param>
        /// <param name="selectedMonth">Selected month</param>
        /// <param name="selectedYear">Selected year</param>
        /// <param name="localizeLabels">Localize labels</param>
        /// <returns></returns>
        public static MvcHtmlString DatePickerDropDowns(this HtmlHelper html,
            string dayName, string monthName, string yearName,
            int? beginYear = null, int? endYear = null,
            int? selectedDay = null, int? selectedMonth = null, int? selectedYear = null, bool localizeLabels = true)
        {
            var daysList = new TagBuilder("select");
            var monthsList = new TagBuilder("select");
            var yearsList = new TagBuilder("select");

            daysList.Attributes.Add("name", dayName);
            monthsList.Attributes.Add("name", monthName);
            yearsList.Attributes.Add("name", yearName);

            var days = new StringBuilder();
            var months = new StringBuilder();
            var years = new StringBuilder();

            string dayLocale, monthLocale, yearLocale;
            if (localizeLabels)
            {
                var locService = EngineContext.Current.Resolve<ILocalizationService>();
                dayLocale = locService.GetResource("Common.Day");
                monthLocale = locService.GetResource("Common.Month");
                yearLocale = locService.GetResource("Common.Year");
            }
            else
            {
                dayLocale = "Day";
                monthLocale = "Month";
                yearLocale = "Year";
            }

            days.AppendFormat("<option value='{0}'>{1}</option>", "0", dayLocale);
            for (int i = 1; i <= 31; i++)
                days.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (selectedDay.HasValue && selectedDay.Value == i) ? " selected=\"selected\"" : null);


            months.AppendFormat("<option value='{0}'>{1}</option>", "0", monthLocale);
            for (int i = 1; i <= 12; i++)
            {
                months.AppendFormat("<option value='{0}'{1}>{2}</option>",
                                    i, 
                                    (selectedMonth.HasValue && selectedMonth.Value == i) ? " selected=\"selected\"" : null,
                                    CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(i));
            }


            years.AppendFormat("<option value='{0}'>{1}</option>", "0", yearLocale);

            if (beginYear == null)
                beginYear = DateTime.UtcNow.Year - 100;
            if (endYear == null)
                endYear = DateTime.UtcNow.Year;

            if (endYear > beginYear)
            {
                for (int i = beginYear.Value; i <= endYear.Value; i++)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (selectedYear.HasValue && selectedYear.Value == i) ? " selected=\"selected\"" : null);
            }
            else
            {
                for (int i = beginYear.Value; i >= endYear.Value; i--)
                    years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                        (selectedYear.HasValue && selectedYear.Value == i) ? " selected=\"selected\"" : null);
            }

            daysList.InnerHtml = days.ToString();
            monthsList.InnerHtml = months.ToString();
            yearsList.InnerHtml = years.ToString();

            return MvcHtmlString.Create(string.Concat(daysList, monthsList, yearsList));
        }

        public static MvcHtmlString Widget(this HtmlHelper helper, string widgetZone, object additionalData = null)
        {
            return helper.Action("WidgetsByZone", "Widget", new { widgetZone = widgetZone, additionalData = additionalData });
        }

        /// <summary>
        /// Renders the standard label with a specified suffix added to label text
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        /// <typeparam name="TValue">Value</typeparam>
        /// <param name="html">HTML helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="htmlAttributes">HTML attributes</param>
        /// <param name="suffix">Suffix</param>
        /// <returns>Label</returns>
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes, string suffix)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string resolvedLabelText = metadata.DisplayName ?? (metadata.PropertyName ?? htmlFieldName.Split(new [] { '.' }).Last());
            if (string.IsNullOrEmpty(resolvedLabelText))
            {
                return MvcHtmlString.Empty;
            }
            var tag = new TagBuilder("label");
            tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName)));
            if (!String.IsNullOrEmpty(suffix))
            {
                resolvedLabelText = String.Concat(resolvedLabelText, suffix);
            }
            tag.SetInnerText(resolvedLabelText);

            var dictionary = ((IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            tag.MergeAttributes(dictionary, true);

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Wfm the validation message for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="helper">The helper.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MvcHtmlString WfmValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            // add br tag
            // -------------------
            //var containerDivBuilder = new TagBuilder("br");
            //containerDivBuilder.InnerHtml += containerDivBuilder.ToString(TagRenderMode.SelfClosing); // close br

            //// Validation Html message
            //containerDivBuilder.InnerHtml = helper.ValidationMessageFor(expression).ToString();

            //return MvcHtmlString.Create(containerDivBuilder.ToString());


            // no additional tag
            // -------------------
            return MvcHtmlString.Create(helper.ValidationMessageFor(expression).ToString());
        }


        public static MvcHtmlString WfmValidationMessageFor_NOTUSED<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            // KENDO style sample
            //======================================
            //<tr class="field-error-box">
            //    <td class="error-title"></td>
            //    <td class="error-message">
            //        <span class="k-widget k-tooltip k-tooltip-validation k-invalid-msg field-validation-error" data-for="Salutation" data-val-msg-for="Salutation" id="Salutation_validationMessage" role="alert">
            //            <span class="k-icon k-warning"> </span> 
            //            The salutation is required
            //        </span>
            //    </td>
            //</tr>

            TagBuilder containerDivBuilder = new TagBuilder("tr");
            containerDivBuilder.AddCssClass("field-error-box");

            TagBuilder ltdDivBuilder = new TagBuilder("td"); // open left td
            ltdDivBuilder.AddCssClass("error-title");
            containerDivBuilder.InnerHtml += ltdDivBuilder.ToString(TagRenderMode.Normal); // close left td

            TagBuilder rtdDivBuilder = new TagBuilder("td"); // open right td
            rtdDivBuilder.AddCssClass("error-message");

            // Validation Html message
            rtdDivBuilder.InnerHtml = helper.ValidationMessageFor(expression).ToString();
            //rtdDivBuilder.InnerHtml = helper.ValidationMessageFor(expression, String.Empty, new { @class = "k-widget k-tooltip k-tooltip-validation k-invalid-msg" }).ToString();

            containerDivBuilder.InnerHtml += rtdDivBuilder.ToString(TagRenderMode.Normal); // close right td

            return MvcHtmlString.Create(containerDivBuilder.ToString(TagRenderMode.Normal));
        }


        public static MvcHtmlString Video(this HtmlHelper helper, string id, string sourceUrl, bool controls)
        {
            TagBuilder builder = new TagBuilder("video");
            builder.MergeAttribute("id", id);
            builder.MergeAttribute("src", sourceUrl);
            if (controls)
            {
                builder.MergeAttribute("controls", "controls");
            }
            return new MvcHtmlString(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString SubmitLink(this HtmlHelper helper, string text, string formId)
        {
            return new MvcHtmlString(string.Format("<a class='k-button' onclick='$(\"#{0}\").submit();'>{1}</a>", formId, text));
        }

        #endregion


        #region Bar Codes

        public static MvcHtmlString GenerateQrCode(this HtmlHelper html, string value, int height = 250, int width = 250, int margin = 4, string text = null)
        {
            return MvcHtmlString.Create(CommonHelper.ToQrCodeImg(value, height, width, margin, text));
        }

        #endregion


        #region Custom Label

        /// <summary>
        /// Renders a custom label
        /// </summary>
        public static MvcHtmlString TwLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression,
            string label = null, string hint = null, object htmlAttr = null, bool wrapper = true)
        {
            var result = new StringBuilder();

            htmlAttr = CommonHelper.Merge(new { title = hint, @class = "control-label" }, htmlAttr);
            result.Append(helper.LabelFor(expression, label, htmlAttr));

            if (wrapper)
            {
                var laberWrapper = new TagBuilder("div");
                laberWrapper.Attributes.Add("class", "label-wrapper");
                laberWrapper.InnerHtml = result.ToString();
                return MvcHtmlString.Create(laberWrapper.ToString());
            }
            else
                return MvcHtmlString.Create(result.ToString());
        }


        /// <summary>
        /// Renders a custom label
        /// </summary>
        public static MvcHtmlString TwLabel<TModel>(this HtmlHelper<TModel> helper, string expression, 
            string label = null, string hint = null, object htmlAttr = null, bool wrapper = true)
        {
            var result = new StringBuilder();

            htmlAttr = CommonHelper.Merge(new { title = hint, @class = "control-label" }, htmlAttr);
            result.Append(helper.Label(expression, label, htmlAttr));

            if (wrapper)
            {
                var laberWrapper = new TagBuilder("div");
                laberWrapper.Attributes.Add("class", "label-wrapper");
                laberWrapper.InnerHtml = result.ToString();
                return MvcHtmlString.Create(laberWrapper.ToString());
            }
            else
                return MvcHtmlString.Create(result.ToString());
        }

        #endregion


        #region Custom Editor

        /// <summary>
        /// Renders a custom editor
        /// </summary>
        public static MvcHtmlString TwEditorFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, 
            string tmplt = null, object viewData = null, bool validation = true, string validationMessage = null)
        {
            //return helper.EditorFor(expression, tmplt, viewData);
            var result = new StringBuilder();
            result.Append(helper.EditorFor(expression, tmplt, viewData));
            if (validation)
                result.Append(helper.ValidationMessageFor(expression, validationMessage));

            return MvcHtmlString.Create(result.ToString());
        }


        /// <summary>
        /// Renders a custom editor
        /// </summary>
        public static MvcHtmlString TwEditor<TModel>(this HtmlHelper<TModel> helper, string expression,
            string tmplt = null, object viewData = null, bool validation = true, string validationMessage = null)
        {
            //return helper.Editor(expression, tmplt, viewData);
            var result = new StringBuilder();
            result.Append(helper.Editor(expression, tmplt, viewData));
            if (validation)
                result.Append(helper.ValidationMessage(expression, validationMessage));

            return MvcHtmlString.Create(result.ToString());
        }

        #endregion


        #region Input Line

        private static void _AddContent2Container(TagBuilder container, string content, string contentDivClass = null)
        {
            if (String.IsNullOrWhiteSpace(contentDivClass))
                container.InnerHtml += content;
            else
            {
                var contentDiv = new TagBuilder("div");
                contentDiv.AddCssClass(contentDivClass);
                contentDiv.InnerHtml += content;
                container.InnerHtml += contentDiv;
            }
        }


        private static MvcHtmlString _InputLine(string labelContent, string fieldContent,
            string labelDivClass = null, string fieldDivClass = null, string containerClass = null,
            object containerHtmlAttributes = null)
        {
            var container = new TagBuilder("div");
            if (containerHtmlAttributes != null)
                container.MergeAttributes(new RouteValueDictionary(containerHtmlAttributes));
            container.AddCssClass(containerClass);

            _AddContent2Container(container, labelContent, labelDivClass);
            _AddContent2Container(container, Environment.NewLine);  // mock new line in manual html, for alignment
            _AddContent2Container(container, fieldContent, fieldDivClass);

            return MvcHtmlString.Create(container.ToString());
        }


        private static void _GetInputDivClasses(string containerClass, ref string labelDivClass, ref string fieldDivClass, 
            out bool labelWrapper)
        {
            labelWrapper = false;
            if (containerClass == "form-group")
            {
                labelWrapper = true;
                if (String.IsNullOrWhiteSpace(labelDivClass)) labelDivClass = "col-sm-4";
                if (String.IsNullOrWhiteSpace(fieldDivClass)) fieldDivClass = "col-sm-8";
            }
        }


        /// <summary>
        /// Renders an input line including lable and editor
        /// </summary>
        public static MvcHtmlString InputLineFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, 
            string label = null, string hint = null, string tmplt = null, bool val = true, string valMsg = null, 
            object labelHtmlAttr = null, object viewData = null, object rowHtmlAttr = null,
            string labelDivCls = null, string fieldDivCls = null, string rowCls = "form-group")
        {
            _GetInputDivClasses(rowCls, ref labelDivCls, ref fieldDivCls, out bool labelWrapper);

            return _InputLine(TwLabelFor(helper, expression, label, hint, labelHtmlAttr, labelWrapper).ToString(),
                TwEditorFor(helper, expression, tmplt, viewData, val, valMsg).ToString(), 
                labelDivClass: labelDivCls, fieldDivClass: fieldDivCls, containerClass: rowCls);
        }


        /// <summary>
        /// Renders an input line including lable and editor
        /// </summary>
        public static MvcHtmlString InputLine<TModel>(this HtmlHelper<TModel> helper, string expression,
            string label = null, string hint = null, string tmplt = null, bool val = true, string valMsg = null,
            object labelHtmlAttr = null, object viewData = null, object rowHtmlAttr = null,
            string labelDivCls = null, string fieldDivCls = null, string rowCls = "form-group")
        {
            _GetInputDivClasses(rowCls, ref labelDivCls, ref fieldDivCls, out bool labelWrapper);

            return _InputLine(TwLabel(helper, expression, label, hint, labelHtmlAttr, labelWrapper).ToString(),
                TwEditor(helper, expression, tmplt, viewData, val, valMsg).ToString(),
                labelDivClass: labelDivCls, fieldDivClass: fieldDivCls, containerClass: rowCls);
        }

        #endregion


        #region Input

        /// <summary>
        /// Renders an input, with optional template and view data
        /// </summary>
        public static MvcHtmlString InputFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression,
            string tmplt = null, object viewData = null)
        {
            return InputLineFor(helper, expression, tmplt: tmplt, viewData: viewData);
        }


        /// <summary>
        /// Renders an input, with optional template and view data
        /// </summary>
        public static MvcHtmlString Input<TModel>(this HtmlHelper<TModel> helper, string expression,
            string tmplt = null, object viewData = null)
        {
            return InputLine(helper, expression, tmplt: tmplt, viewData: viewData);
        }

        #endregion


        #region Custom Display

        /// <summary>
        /// Renders a custom display
        /// </summary>
        public static MvcHtmlString TwDisplayFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, 
            string display = null, string tmplt = null, object viewData = null, string style = null)
        {
            var result = new TagBuilder("div");
            result.Attributes.Add("class", "form-text-row");
            if (!String.IsNullOrWhiteSpace(style))
                result.Attributes.Add("style", style);
            result.InnerHtml = display ?? helper.DisplayFor(expression, tmplt, viewData).ToString();

            return MvcHtmlString.Create(result.ToString());
        }


        /// <summary>
        /// Renders a custom display
        /// </summary>
        public static MvcHtmlString TwDisplay<TModel>(this HtmlHelper<TModel> helper, string expression, 
            string display = null, string tmplt = null, object viewData = null, string style = null)
        {
            var result = new TagBuilder("div");
            result.Attributes.Add("class", "form-text-row");
            if (!String.IsNullOrWhiteSpace(style))
                result.Attributes.Add("style", style);
            result.InnerHtml = display ?? helper.Display(expression, tmplt, viewData).ToString();

            return MvcHtmlString.Create(result.ToString());
        }

        #endregion


        #region Display Line

        /// <summary>
        /// Renders an input line including lable and editor
        /// </summary>
        public static MvcHtmlString DisplayLineFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression,
            string label = null, string hint = null, string display = null, string tmplt = null, object viewData = null, string style = null, 
            object labelHtmlAttr = null, object rowHtmlAttr = null,
            string labelDivCls = null, string fieldDivCls = null, string rowCls = "form-group")
        {
            _GetInputDivClasses(rowCls, ref labelDivCls, ref fieldDivCls, out bool labelWrapper);

            return _InputLine(TwLabelFor(helper, expression, label, hint, labelHtmlAttr, labelWrapper).ToString(),
                TwDisplayFor(helper, expression, display, tmplt, viewData, style).ToString(),
                labelDivCls, fieldDivCls, rowCls, rowHtmlAttr);
        }


        /// <summary>
        /// Renders an input line including lable and editor
        /// </summary>
        public static MvcHtmlString DisplayLine<TModel>(this HtmlHelper<TModel> helper, string expression,
            string label = null, string hint = null, string display = null, string tmplt = null, object viewData = null, string style = null,
            object labelHtmlAttr = null, object rowHtmlAttr = null,
            string labelDivCls = null, string fieldDivCls = null, string rowCls = "form-group")
        {
            _GetInputDivClasses(rowCls, ref labelDivCls, ref fieldDivCls, out bool labelWrapper);

            return _InputLine(TwLabel(helper, expression, label, hint, labelHtmlAttr, labelWrapper).ToString(),
                TwDisplay(helper, expression, display, tmplt, viewData, style).ToString(),
                labelDivCls, fieldDivCls, rowCls, rowHtmlAttr);
        }

        #endregion


        #region Line

        /// <summary>
        /// Renders an input, with optional template and view data
        /// </summary>
        public static MvcHtmlString LineFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression,
            string display = null)
        {
            return DisplayLineFor(helper, expression, display: display);
        }


        /// <summary>
        /// Renders an input, with optional template and view data
        /// </summary>
        public static MvcHtmlString Line<TModel>(this HtmlHelper<TModel> helper, string expression,
            string display = null)
        {
            return DisplayLine(helper, expression, display: display);
        }

        #endregion

    }
}
