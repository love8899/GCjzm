using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Messages;
using Wfm.Services.Events;
using Wfm.Services.Localization;

namespace Wfm.Services.Messages
{
    public partial class MessageTemplateService : IMessageTemplateService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : franchise ID
        /// </remarks>
        private const string MESSAGETEMPLATES_ALL_KEY = "Wfm.messagetemplate.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : franchise ID
        /// </remarks>
        private const string MESSAGETEMPLATES_BY_NAME_KEY = "Wfm.messagetemplate.name-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string MESSAGETEMPLATES_PATTERN_KEY = "Wfm.messagetemplate.";

        #endregion

        #region Fields

        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<MessageTemplateAccountRole> _messageTemplateAccountRoleRepository;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="franchiseMappingRepository">Franchise mapping repository</param>
        /// <param name="languageService">Language service</param>
        /// <param name="localizedEntityService">Localized entity service</param>
        /// <param name="franchiseMappingService">Franchise mapping service</param>
        /// <param name="messageTemplateRepository">Message template repository</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        public MessageTemplateService(ICacheManager cacheManager,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IRepository<MessageTemplate> messageTemplateRepository,
            IEventPublisher eventPublisher,
             IRepository<MessageTemplateAccountRole> messageTemplateAccountRoleRepository
            )
        {
            this._cacheManager = cacheManager;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._messageTemplateRepository = messageTemplateRepository;
            this._eventPublisher = eventPublisher;
            this._messageTemplateAccountRoleRepository = messageTemplateAccountRoleRepository; 
        }

        #endregion 

        #region Methods

        /// <summary>
        /// Delete a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void DeleteMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            _messageTemplateRepository.Delete(messageTemplate);

            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            ////event notification
            //_eventPublisher.EntityDeleted(messageTemplate);
        }

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void InsertMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");
            messageTemplate.CreatedOnUtc = messageTemplate.UpdatedOnUtc = DateTime.UtcNow;

            _messageTemplateRepository.Insert(messageTemplate);
            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            ////event notification
            //_eventPublisher.EntityInserted(messageTemplate);
        }
        public virtual void InsertMessageTemplate(MessageTemplate messageTemplate, int[] accountRoleIds)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            this.InsertMessageTemplate(messageTemplate);          
            var MessageTemplateRoles = new List<MessageTemplateAccountRole>();
            for (int i = 0; i < accountRoleIds.Length; i++)
            {
                MessageTemplateRoles.Add(new MessageTemplateAccountRole
                {
                    AccountRoleId = accountRoleIds[i],
                    MessageTemplateId = messageTemplate.Id,
                });
            }
            _messageTemplateAccountRoleRepository.Insert(MessageTemplateRoles);
          
        }

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void UpdateMessageTemplate(MessageTemplate messageTemplate, int[] accountRoleIds)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");
            messageTemplate.UpdatedOnUtc = DateTime.UtcNow;

            var toDeleteMessageTemplateRoles = messageTemplate.AccountRoles.Where(x =>x.MessageTemplateId==messageTemplate.Id).ToList();
            toDeleteMessageTemplateRoles.ForEach(x =>
            {
                _messageTemplateAccountRoleRepository.Delete(x);
            });

            for (int i = 0; i < accountRoleIds.Length; i++)
            {
                messageTemplate.AccountRoles.Add(new MessageTemplateAccountRole
                {
                    AccountRoleId = accountRoleIds[i],
                    MessageTemplateId=messageTemplate.Id,
                });
            }

            _messageTemplateRepository.Update(messageTemplate);

            _cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            ////event notification
            //_eventPublisher.EntityUpdated(messageTemplate);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        public virtual MessageTemplate GetMessageTemplateById(int messageTemplateId)
        {
            if (messageTemplateId == 0)
                return null;

            return _messageTemplateRepository.GetById(messageTemplateId);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <param name="franchiseId">Franchise identifier</param>
        /// <returns>Message template</returns>
        public virtual MessageTemplate GetMessageTemplateByName(string messageTemplateName, int franchiseId)
        {
            if (string.IsNullOrWhiteSpace(messageTemplateName))
                throw new ArgumentException("messageTemplateName");

            //without cache
            //var query = _messageTemplateRepository.Table;
            //query = query.Where(t => t.TagName == messageTemplateName);
            //query = query.OrderBy(t => t.Id);
            //return query.FirstOrDefault();


            //using cache
            string key = string.Format(MESSAGETEMPLATES_BY_NAME_KEY, messageTemplateName, franchiseId);
            return _cacheManager.Get(key, () =>
            {
                var query = _messageTemplateRepository.Table;
                query = query.Where(t => t.TagName == messageTemplateName);
                query = query.OrderBy(t => t.Id);
                var templates = query.ToList();


                return templates.FirstOrDefault();
            });
        }

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <param name="franchiseId">Franchise identifier; pass 0 to load all records</param>
        /// <returns>Message template list</returns>
        public virtual IList<MessageTemplate> GetAllMessageTemplates(int franchiseId)
        {
            //without cache
            //var query = _messageTemplateRepository.Table;
            //query = query.OrderBy(t => t.TagName);
            //return query.ToList();


            //using cache
            string key = string.Format(MESSAGETEMPLATES_ALL_KEY, franchiseId);
            return _cacheManager.Get(key, () =>
            {
                var query = _messageTemplateRepository.Table;
                query = query.OrderBy(t => t.TagName);


                return query.ToList();
            });
        }

        /// <summary>
        /// Create a copy of message template with all depended data
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Message template copy</returns>
        public virtual MessageTemplate CopyMessageTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            var mtCopy = new MessageTemplate()
                             {
                                 TagName = messageTemplate.TagName,
                                 BccEmailAddresses = messageTemplate.BccEmailAddresses,
                                 Subject = messageTemplate.Subject,
                                 Body = messageTemplate.Body,
                                 IsActive = messageTemplate.IsActive,
                                 EmailAccountId = messageTemplate.EmailAccountId,
                                 //LimitedToFranchises = messageTemplate.LimitedToFranchises,
                             };

            InsertMessageTemplate(mtCopy);

            var languages = _languageService.GetAllLanguages(true);

            ////localization
            //foreach (var lang in languages)
            //{
            //    var bccEmailAddresses = messageTemplate.GetLocalized(x => x.BccEmailAddresses, lang.Id, false, false);
            //    if (!String.IsNullOrEmpty(bccEmailAddresses))
            //        _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.BccEmailAddresses, bccEmailAddresses, lang.Id);

            //    var subject = messageTemplate.GetLocalized(x => x.Subject, lang.Id, false, false);
            //    if (!String.IsNullOrEmpty(subject))
            //        _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.Subject, subject, lang.Id);

            //    var body = messageTemplate.GetLocalized(x => x.Body, lang.Id, false, false);
            //    if (!String.IsNullOrEmpty(body))
            //        _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.Body, body, lang.Id);

            //    var emailAccountId = messageTemplate.GetLocalized(x => x.EmailAccountId, lang.Id, false, false);
            //    if (emailAccountId > 0)
            //        _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.EmailAccountId, emailAccountId, lang.Id);
            //}

            return mtCopy;
        }

        public IList<SelectListItem> GetAllMassEmailTemplates()
        {
            List<SelectListItem> result = _messageTemplateRepository.TableNoTracking
                                            .Where(x => x.IsGeneral&&x.IsActive&&!x.IsDeleted)
                                            .Select(x => new SelectListItem() { Text = x.TagName, Value = String.Concat("GE-",x.Id.ToString()) }).ToList();
            result.Add(new SelectListItem() { Text = "None", Value = "0" });
            return result.OrderBy(x => x.Value).ToList();
        }

        #endregion
    }
}
