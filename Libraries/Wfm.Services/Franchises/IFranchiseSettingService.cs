using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Wfm.Core.Configuration;
using Wfm.Core.Domain.Franchises;


namespace Wfm.Services.Franchises
{
    public partial interface IFranchiseSettingService
    {
        void InsertSetting(FranchiseSetting setting);

        void UpdateSetting(FranchiseSetting setting);

        void InsertOrUpdateSetting(int franchiseId, string name, string value);
        
        void DeleteSetting(FranchiseSetting setting);

        FranchiseSetting GetSettingById(int settingId);

        IQueryable<FranchiseSetting> GetAllSettings(int franchiseId = 0);

        FranchiseSetting GetSettingByKey(int franchiseId, string key);

        T GetSettingByKey<T>(int franchiseId, string key, T defaultValue = default(T));
    }
}
