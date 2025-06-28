using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Web.Framework;

namespace Wfm.Admin.Models.Payroll
{
    public class PayrollEmailSettingModel
    {
        public int FranchiseId { get; set; }
        public Guid? FranchiseGuid { get; set; }
        [WfmResourceDisplayName("Common.Subject")]
        public string EmailSubject { get; set; }
        [WfmResourceDisplayName("Common.Body")]
        public string EmailBody { get; set; }
        public string Code { get; set; }
        [WfmResourceDisplayName("Common.Email")]
        public string EmailAddress { get; set; }
        public bool Simple { get; set; }
    }

    public class PayrollEmailSettingDetailModel:PayrollEmailSettingModel
    {
        [WfmResourceDisplayName("Common.Password")]
        public string EmailPassword { get; set; }
        [WfmResourceDisplayName("Common.EmailSmtpClient")]
        public string EmailSmtpClient { get; set; }
        [WfmResourceDisplayName("Common.EmailPortNumber")]
        public int EmailPortNumber { get; set; }
        [WfmResourceDisplayName("Common.EnableSsl")]
        public bool EnableSsl { get; set; }
        [WfmResourceDisplayName("Common.UserName")]
        public string UserName { get; set; }
    }
}