using System.Collections.Generic;
using System.Web.Mvc;
using Wfm.Core.Domain.Messages;

namespace Wfm.Services.Messages
{
    /// <summary>
    /// Message template service
    /// </summary>
    public partial interface IMessageTemplateService
    {
        /// <summary>
        /// Delete a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        void DeleteMessageTemplate(MessageTemplate messageTemplate);

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        void InsertMessageTemplate(MessageTemplate messageTemplate);

        void InsertMessageTemplate(MessageTemplate messageTemplate, int[] accountRoleIds);

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        void UpdateMessageTemplate(MessageTemplate messageTemplate, int[] accountRoleIds);

        /// <summary>
        /// Gets a message template by identifier
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        MessageTemplate GetMessageTemplateById(int messageTemplateId);

        /// <summary>
        /// Gets a message template by name
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <param name="franchiseId">Franchise identifier</param>
        /// <returns>Message template</returns>
        MessageTemplate GetMessageTemplateByName(string messageTemplateName, int franchaseId);

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <param name="franchiseId">Franchise identifier; pass 0 to load all records</param>
        /// <returns>Message template list</returns>
        IList<MessageTemplate> GetAllMessageTemplates(int franchaseId);

        /// <summary>
        /// Create a copy of message template with all depended data
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Message template copy</returns>
        MessageTemplate CopyMessageTemplate(MessageTemplate messageTemplate);
        IList<SelectListItem> GetAllMassEmailTemplates();
    }
}
