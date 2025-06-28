using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Services.Payroll
{
    public partial interface IOvertimeRuleSettingService
    {
        #region CRUD

        void Insert(OvertimeRuleSetting overtimeRuleSetting);
        void Update(OvertimeRuleSetting overtimeRuleSetting);
        void Delete(OvertimeRuleSetting overtimeRuleSetting);

        #endregion

        #region OvertimeRuleSetting

        OvertimeRuleSetting GetOvertimeRuleSettingById(int id);

        #endregion

        #region LIST

        IList<OvertimeRuleSetting> GetAllOvertimeRuleSettings();
        
        IPagedList<OvertimeRuleSetting> GetAllOvertimeRuleSettings(int pageIndex = 0, int PageSize = int.MaxValue);

        IQueryable<OvertimeRuleSetting> GetAllOvertimeRuleSettingsAsQueryable();

        IList<SelectListItem> GetAllOvertimeRuleSettingsForDropDownList();


        #endregion
    }
}
