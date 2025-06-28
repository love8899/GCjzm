using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Payroll
{
    public class EmailSetting
    {
        public Guid FranchiseGuid { get; set; }
        public int FranchiseId { get; set; }
        public string EmailAddress { get; set; }
        public string EmailPassword { get; set; }
        public string EmailSmtpClient { get; set; }
        public int EmailPortNumber { get; set; }
        public bool EnableSsl { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string UserName { get; set; }
        public string Code { get; set; }
    }
}
