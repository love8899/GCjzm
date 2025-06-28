using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Services.Payroll
{
    public interface IPayrollSettingService
    {
        PayrollSetting GetPayrollSettingByFranchiseId(Guid? franchiseGuid);
        IList<SelectListItem> GetAllDDFileLayoutDataSource();
        bool UpdatePayrollSetting(PayrollSetting setting);
        List<EmailSetting> GetPayrollEmailSetting(Guid? franchiseGuid);
        bool UpdatePayrollEmailSetting(EmailSetting setting,int accountId);
        EmailSetting GetPayrollEmailSetting(Guid? franchiseGuid, string code);
    }
}
