﻿using System.Web.Mvc;

namespace Wfm.Web.Framework.UI
{
    public partial interface IPageHeadBuilder
    {
        void AddTitleParts(string part);
        void AppendTitleParts(string part);
        string GenerateTitle(bool addDefaultTitle);

        void AddMetaDescriptionParts(string part);
        void AppendMetaDescriptionParts(string part);
        string GenerateMetaDescription();

        void AddMetaKeywordParts(string part);
        void AppendMetaKeywordParts(string part);
        string GenerateMetaKeywords();

        void AddScriptParts(ResourceLocation location, string part, bool excludeFromBundle);
        void AppendScriptParts(ResourceLocation location, string part, bool excludeFromBundle);
        string GenerateScripts(UrlHelper urlHelper, ResourceLocation location, bool? bundleFiles = null);

        void AddCssFileParts(ResourceLocation location, string part);
        void AppendCssFileParts(ResourceLocation location, string part);
        string GenerateCssFiles(UrlHelper urlHelper, ResourceLocation location, bool? bundleFiles = null);


        void AddCanonicalUrlParts(string part);
        void AppendCanonicalUrlParts(string part);
        string GenerateCanonicalUrls();

        void AddHeadCustomParts(string part);
        void AppendHeadCustomParts(string part);
        string GenerateHeadCustom();

        void AddPageCssClassParts(string part);
        void AppendPageCssClassParts(string part);
        string GeneratePageCssClasses();

        /// <summary>
        /// Specify system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <param name="systemName">System name</param>
        void SetActiveMenuItemSystemName(string systemName);
        /// <summary>
        /// Get system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <returns>System name</returns>
        string GetActiveMenuItemSystemName();
    }
}
