using System.Data;
using Wfm.Core.Data;
using Wfm.Core.Domain.Localization;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Xml;
using Wfm.Core.Caching;
using Wfm.Data;

namespace Wfm.Services.Localization
{
    public partial class LanguageService : ILanguageService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        private const string LANGUAGES_BY_ID_KEY = "Wfm.language.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string LANGUAGES_ALL_KEY = "Wfm.language.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string LANGUAGES_PATTERN_KEY = "Wfm.language.";

        #endregion

        #region Fields

        IRepository<Language> _languageRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Ctor

        public LanguageService(
            ICacheManager cacheManager,
            IDataProvider dataProvider,
            IDbContext dbContext,
            IRepository<Language> languageRepository)

        {
            _cacheManager = cacheManager;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
            _languageRepository = languageRepository;
        }

        #endregion

        #region CRUD

        public void Insert(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageRepository.Insert(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

        }

        public void Update(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageRepository.Update(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
        }

        public void Delete(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageRepository.Delete(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            // _eventPublisher.EntityDeleted(localeStringResource);
        }

        #endregion

        #region Language

        public Language GetLanguageById(int id)
        {
            if (id == 0)
                return null;

            //no cache
            //Language language = _languageRepository.GetById(id);
            //return language;

            //using cache
            string key = string.Format(LANGUAGES_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _languageRepository.GetById(id));
        }

        public int GetLanguageIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var language = _languageRepository.Table.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();

            return language != null ? language.Id : 0;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="franchiseId">Load records allows only in specified franchise; pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        public virtual IList<Language> GetAllLanguages(bool showHidden = false)
        {
            //no cache
            //var query = _languageRepository.Table;
            //query = from l in query
            //        orderby l.Name
            //        select l;

            //var languages = query.ToList();
            //return languages;


            //using cache
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            var languages = _cacheManager.Get(key, () =>
            {
                var query = _languageRepository.Table;
                if (!showHidden)
                    query = query.Where(l => l.IsActive);
                query = query.OrderBy(l => l.DisplayOrder);
                return query.ToList();
            });

            return languages;
        }


        public string ExportLanguageToXml(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Language");
            xmlWriter.WriteAttributeString("Name", language.Name);


            IList<Language> resources = GetAllLanguages();
            foreach (var resource in resources)
            {
                xmlWriter.WriteStartElement("Language");
                xmlWriter.WriteAttributeString("Name", resource.Name);
                xmlWriter.WriteElementString("Value", null, resource.Id.ToString());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Import language resources from XML file
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="xml">XML</param>
        public void ImportLanguageFromXml(Language language, string xml)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            if (String.IsNullOrEmpty(xml))
                return;
            if (_dataProvider.StoredProceduredSupported)
            {
                //SQL 2005 insists that your XML schema incoding be in UTF-16.
                //Otherwise, you'll get "XML parsing: line 1, character XXX, unable to switch the encoding"
                //so let's remove XML declaration
                var inDoc = new XmlDocument();
                inDoc.LoadXml(xml);
                var sb = new StringBuilder();
                using (var xWriter = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true }))
                {
                    inDoc.Save(xWriter);
                    xWriter.Close();
                }
                var outDoc = new XmlDocument();
                outDoc.LoadXml(sb.ToString());
                xml = outDoc.OuterXml;

                //stored procedures are enabled and supported by the database.
                var pLanguageId = _dataProvider.GetParameter();
                pLanguageId.ParameterName = "LanguageId";
                pLanguageId.Value = language.Id;
                pLanguageId.DbType = DbType.Int32;

                var pXmlPackage = _dataProvider.GetParameter();
                pXmlPackage.ParameterName = "XmlPackage";
                pXmlPackage.Value = xml;
                pXmlPackage.DbType = DbType.Xml;

                //long-running query. specify timeout (600 seconds)
                _dbContext.ExecuteSqlCommand("EXEC [LanguagePackImport] @LanguageId, @XmlPackage", false, 600, pLanguageId, pXmlPackage);
            }
            else
            {
                //stored procedures aren't supported
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                var nodes = xmlDoc.SelectNodes(@"//Language/LocaleResource");
                foreach (XmlNode node in nodes)
                {
                    string name = node.Attributes["Name"].InnerText.Trim();
                    string value = "";
                    var valueNode = node.SelectSingleNode("Value");
                    if (valueNode != null)
                        value = valueNode.InnerText;

                    if (String.IsNullOrEmpty(name))
                        continue;

                    //do not use "Insert"/"Update" methods because they clear cache
                    //let's bulk insert
                    var resource = language.LocaleStringResources.Where(x => x.ResourceName.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (resource != null)
                        resource.ResourceValue = value;
                    else
                    {
                        language.LocaleStringResources.Add(
                            new LocaleStringResource()
                            {
                                ResourceName = name,
                                ResourceValue = value
                            });
                    }
                }
                // _languageService.UpdateLanguage(language);
            }

            //clear cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
        }

        #endregion
    }
}
