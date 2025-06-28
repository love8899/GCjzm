using System.IO;
using System.Text;
using System.Xml;

namespace Wfm.Services.Seo
{
    /// <summary>
    /// Represents a base sitemap generator
    /// </summary>
    public abstract partial class BaseSitemapGenerator
    {
        #region Fields

        private const string DateFormat = @"yyyy-MM-dd";
       

        #endregion

        #region Utilities

        /// <summary>
        /// Method that is overridden, that handles creation of child urls.
        /// Use the method WriteUrlLocation() within this method.
        /// </summary>
        protected abstract void GenerateUrlNodes();

        #endregion

        #region Methods

        /// <summary>
        /// This will build an xml sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <returns>Sitemap.xml as string</returns>
        public string Generate()
        {
            using (var stream = new MemoryStream())
            {
                Generate(stream);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// This will build an xml sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="stream">Stream of sitemap.</param>
        public void Generate(Stream stream)
        {
            using (var _writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                _writer.Formatting = Formatting.Indented;
                _writer.WriteStartDocument();
                _writer.WriteStartElement("urlset");
                _writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
                _writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                _writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

                GenerateUrlNodes();

                _writer.WriteEndElement();
                _writer.Flush();
                //_writer.Close();
            }
            
        }

        #endregion

    }
}
