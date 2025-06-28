using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;

namespace Wfm.Core.Domain.Messages 
{
    public class MessageTemplateAccountRole 
    {
        public int MessageTemplateId { get; set; }
        public int AccountRoleId { get; set; }
        public virtual MessageTemplate MessageTemplate { get; set; }
        public virtual AccountRole AccountRole { get; set; } 
    }

  
}
