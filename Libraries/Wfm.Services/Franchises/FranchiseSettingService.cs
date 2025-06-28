using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Wfm.Core;
using Wfm.Core.Caching;
using Wfm.Core.Configuration;
using Wfm.Core.Data;
using Wfm.Core.Domain.Franchises;
using Wfm.Services.Events;


namespace Wfm.Services.Franchises
{
    public partial class FranchiseSettingService : IFranchiseSettingService
    {
        #region Fields

        private readonly IRepository<FranchiseSetting> _settingRepository;

        #endregion


        #region Ctor

        public FranchiseSettingService(IRepository<FranchiseSetting> settingRepository)
        {
            this._settingRepository = settingRepository;
        }

        #endregion


        #region Methods

        public void InsertSetting(FranchiseSetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Insert(setting);
        }


        public void UpdateSetting(FranchiseSetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Update(setting);
        }


        public void InsertOrUpdateSetting(int franchiseId, string name, string value)
        {
            var setting = GetSettingByKey(franchiseId, name);
            if (setting == null)
            {
                setting = new FranchiseSetting()
                {
                    FranchiseId = franchiseId,
                    Name = name,
                    Value = value
                };

                InsertSetting(setting);
            }
            else
            {
                setting.Value = value;

                UpdateSetting(setting);
            }
        }


        public void DeleteSetting(FranchiseSetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Delete(setting);
        }


        public FranchiseSetting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            return _settingRepository.GetById(settingId);
        }


        public IQueryable<FranchiseSetting> GetAllSettings(int franchiseId = 0)
        {
            var query = _settingRepository.Table;
            if (franchiseId > 0)
                query = query.Where(x => x.FranchiseId == franchiseId);

            return query.OrderBy(x => x.FranchiseId).ThenBy(x => x.Name);
        }


        public FranchiseSetting GetSettingByKey(int franchiseId, string key)
        {
            if (String.IsNullOrWhiteSpace(key))
                return null;

            var settings = GetAllSettings(franchiseId);

            return settings.FirstOrDefault(x => x.Name == key);
        }


        public T GetSettingByKey<T>(int franchiseId, string key, T defaultValue = default(T))
        {
            var settingByKey = GetSettingByKey(franchiseId, key);

            return settingByKey != null ? CommonHelper.To<T>(settingByKey.Value) : defaultValue;
        }

        #endregion
    }

}