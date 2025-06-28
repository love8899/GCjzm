
using System.Collections.Generic;
namespace Wfm.Core.Domain.Messages
{
    public class MessageTemplate : BaseEntity
    {
       

        /// <summary>
        /// Gets or sets the used email account identifier
        /// </summary>
        public int EmailAccountId { get; set; }

        public string TagName { get; set; }

        public int MessageCategoryId { get; set; }

        public string CCEmailAddresses { get; set; }
        
        /// <summary>
        /// Gets or sets the BCC Email addresses
        /// </summary>
        public string BccEmailAddresses { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }
        public string PossibleVariables { get; set; }

        public string Note { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int EnteredBy { get; set; }
        public int FranchiseId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsGeneral { get; set; }

        public virtual ICollection<MessageTemplateAccountRole> AccountRoles { get; set; }

        public virtual MessageCategory MessageCategory { get; set; }


    }
}