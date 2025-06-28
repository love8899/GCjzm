using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Wfm.Core.Configuration;
using Wfm.Core.Domain.Companies;


namespace Wfm.Services.Companies
{
    public partial interface ICompanySettingService
    {
        void InsertSetting(CompanySetting setting);

        void UpdateSetting(CompanySetting setting);

        void InsertOrUpdateSetting(int companyId, string name, string value);

        void DeleteSetting(CompanySetting setting);

        CompanySetting GetSettingById(int settingId);

        IQueryable<CompanySetting> GetAllSettings(int companyId = 0);

        CompanySetting GetSettingByKey(int companyId, string key);

        MaxAnnualHours GetMaxAnnualHours(int companyId);
    }
}
