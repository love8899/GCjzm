//code from Telerik MVC Extensions
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Routing;
using System.Xml;
using Wfm.Core;
using Wfm.Core.Infrastructure;
using Wfm.Services.Localization;
using Wfm.Services.Security;


namespace Wfm.Web.Framework.Menu
{
    public class XmlSiteMap
    {
        public XmlSiteMap(string siteMapType = "Admin", List<string> features = null)
        {
            Features = features != null && features.Count > 0 ? features : null;
            SiteMapType = siteMapType;
            RootNode = new SiteMapNode();
        }

        private static string SiteMapType;
        private static List<string> Features = new List<string>() {"Dashboard", "Job Order", "Time Sheet" };
        public SiteMapNode RootNode { get; set; }

        public virtual void LoadFrom(string physicalPath)
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string filePath = webHelper.MapPath(physicalPath);
            string content = File.ReadAllText(filePath);

            if (!string.IsNullOrEmpty(content))
            {
                using (var sr = new StringReader(content))
                {
                    using (var xr = XmlReader.Create(sr,
                            new XmlReaderSettings
                            {
                                CloseInput = true,
                                IgnoreWhitespace = true,
                                IgnoreComments = true,
                                IgnoreProcessingInstructions = true
                            }))
                    {
                        var doc = new XmlDocument();
                        doc.Load(xr);

                        if ((doc.DocumentElement != null) && doc.HasChildNodes)
                        {
                            XmlNode xmlRootNode = doc.DocumentElement.FirstChild;
                            Iterate(RootNode, xmlRootNode, topLevel: true);
                        }
                    }
                }
            }
        }

        private static void Iterate(SiteMapNode siteMapNode, XmlNode xmlNode, bool topLevel = false)
        {
            PopulateNode(siteMapNode, xmlNode);
            var nodes = xmlNode.ChildNodes.Cast<XmlNode>();
            //only sort for reports
            if (siteMapNode.Title == "Reports")
            {
                nodes = nodes.OrderBy(x => x.Attributes[0].Value);
                
            }
            foreach (XmlNode xmlChildNode in nodes)
            {
                var featureName = GetStringValueFromAttribute(xmlChildNode, "featureName");
                var featureIncluded = Features == null || !Features.Any() || Features.Contains(featureName) || string.IsNullOrEmpty(featureName);
                if ((!topLevel || featureIncluded) && (xmlChildNode.LocalName.Equals("siteMapNode", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var siteMapChildNode = new SiteMapNode();
                    if (featureIncluded)
                    {
                        siteMapNode.ChildNodes.Add(siteMapChildNode);
                    }
                    Iterate(siteMapChildNode, xmlChildNode);
                }
            }
        }

        private static void PopulateNode(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            //system name
            siteMapNode.SystemName = GetStringValueFromAttribute(xmlNode, "SystemName");

            //title
            var wfmResource = GetStringValueFromAttribute(xmlNode, "wfmResource");
            if (!string.IsNullOrEmpty(wfmResource))
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                siteMapNode.Title = localizationService.GetResource(wfmResource);
            }
            else
            {
                siteMapNode.Title = GetStringValueFromAttribute(xmlNode, "title");
            }

            //routes, url
            string controllerName = GetStringValueFromAttribute(xmlNode, "controller");
            string actionName = GetStringValueFromAttribute(xmlNode, "action");
            string url = GetStringValueFromAttribute(xmlNode, "url");
            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
            {
                siteMapNode.ControllerName = controllerName;
                siteMapNode.ActionName = actionName;

                siteMapNode.RouteValues = new RouteValueDictionary { { "area", SiteMapType } };
            }
            else if (!string.IsNullOrEmpty(url))
            {
                siteMapNode.Url = url;
            }

            //image URL
            siteMapNode.IconClass = GetStringValueFromAttribute(xmlNode, "IconClass");

            //permission name
            var permissionNames = GetStringValueFromAttribute(xmlNode, "PermissionNames");
            if (!string.IsNullOrEmpty(permissionNames))
            {
                var permissionService = EngineContext.Current.Resolve<IPermissionService>();
                siteMapNode.Visible = permissionNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                   .Any(permissionName => permissionService.Authorize(permissionName.Trim()));
            }
            else
            {
                siteMapNode.Visible = true;
            }

            // Open URL in new tab
            var openUrlInNewTabValue = GetStringValueFromAttribute(xmlNode, "OpenUrlInNewTab");
            bool booleanResult;
            if (!string.IsNullOrWhiteSpace(openUrlInNewTabValue) && bool.TryParse(openUrlInNewTabValue, out booleanResult))
            {
                siteMapNode.OpenUrlInNewTab = booleanResult;
            }
        }

        private static string GetStringValueFromAttribute(XmlNode node, string attributeName)
        {
            //string value = null;

            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                XmlAttribute attribute = node.Attributes[attributeName];

                if (attribute != null)
                {
                    return attribute.Value;
                }
            }

            return null;
        }
    }
}
