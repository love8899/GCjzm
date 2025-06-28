using System;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Seo
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial class SitemapGenerator : BaseSitemapGenerator, ISitemapGenerator
    {
        private readonly CommonSettings _commonSettings;
        private readonly IWebHelper _webHelper;

        public SitemapGenerator(
            CommonSettings commonSettings, IWebHelper webHelper)
        {
            this._commonSettings = commonSettings;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Method that is overridden, that handles creation of child urls.
        /// Use the method WriteUrlLocation() within this method.
        /// </summary>
        protected override void GenerateUrlNodes()
        {
            //if (_commonSettings.SitemapIncludeCategories)
            //{
            //    WriteCategories(0);
            //}

            //if (_commonSettings.SitemapIncludeManufacturers)
            //{
            //    WriteManufacturers();
            //}

            //if (_commonSettings.SitemapIncludeProducts)
            //{
            //    WriteProducts();
            //}

            //if (_commonSettings.SitemapIncludeTopics)
            //{
            //    WriteTopics();
            //}
        }

        //private void WriteCategories(int parentCategoryId)
        //{
        //    var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
        //    foreach (var category in categories)
        //    {
        //        //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
        //        var url = string.Format("{0}{1}", _webHelper.GetStoreLocation(false), category.GetSeName());
        //        var updateFrequency = UpdateFrequency.Weekly;
        //        var updateTime = category.UpdatedOnUtc;
        //        WriteUrlLocation(url, updateFrequency, updateTime);

        //        WriteCategories(category.Id);
        //    }
        //}

        //private void WriteManufacturers()
        //{
        //    var manufacturers = _manufacturerService.GetAllManufacturers(false);
        //    foreach (var manufacturer in manufacturers)
        //    {
        //        //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
        //        var url = string.Format("{0}{1}", _webHelper.GetStoreLocation(false),manufacturer.GetSeName());
        //        var updateFrequency = UpdateFrequency.Weekly;
        //        var updateTime = manufacturer.UpdatedOnUtc;
        //        WriteUrlLocation(url, updateFrequency, updateTime);
        //    }
        //}

        //private void WriteProducts()
        //{
        //    var products = _productService.GetAllProducts(false);
        //    foreach (var product in products)
        //    {
        //        //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
        //        var url = string.Format("{0}{1}", _webHelper.GetStoreLocation(false), product.GetSeName());
        //        var updateFrequency = UpdateFrequency.Weekly;
        //        var updateTime = product.UpdatedOnUtc;
        //        WriteUrlLocation(url, updateFrequency, updateTime);
        //    }
        //}

        //private void WriteTopics()
        //{
        //    var topics = _topicService.GetAllTopics().ToList().FindAll(t => t.IncludeInSitemap);
        //    foreach (var topic in topics)
        //    {
        //        //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
        //        var url = string.Format("{0}t/{1}", _webHelper.GetStoreLocation(false), topic.SystemName.ToLowerInvariant());
        //        var updateFrequency = UpdateFrequency.Weekly;
        //        var updateTime = DateTime.UtcNow;
        //        WriteUrlLocation(url, updateFrequency, updateTime);
        //    }
        //}
    }
}
