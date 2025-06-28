using System;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;


namespace Wfm.Core.Domain.Messages
{
    public class ClientNotification : BaseEntity
    {
        public int MessageTemplateId { get; set; }
        public int CompanyId { get; set; }
        public int AccountRoleId { get; set; }
        public bool IsActive { get; set; }

        public virtual MessageTemplate MessageTemplate { get; set; }
        public virtual Company Company { get; set; }
        public virtual AccountRole AccountRole { get; set; }
    }
}
