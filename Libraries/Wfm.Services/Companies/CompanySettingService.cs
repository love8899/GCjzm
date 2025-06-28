using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Wfm.Core;
using Wfm.Core.Caching;
using Wfm.Core.Configuration;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Events;


namespace Wfm.Services.Companies
{
    public partial class CompanySettingService : ICompanySettingService
    {
        #region Fields

        private readonly IRepository<CompanySetting> _settingRepository;

        #endregion


        #region Ctor

        public CompanySettingService(IRepository<CompanySetting> settingRepository)
        {
            this._settingRepository = settingRepository;
        }

        #endregion


        #region Methods

        public void InsertSetting(CompanySetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Insert(setting);
        }


        public void UpdateSetting(CompanySetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Update(setting);
        }


        public void InsertOrUpdateSetting(int companyId, string name, string value)
        {
            var setting = GetSettingByKey(companyId, name);
            if (setting == null)
            {
                setting = new CompanySetting()
                {
                    CompanyId = companyId,
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


        public void DeleteSetting(CompanySetting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Delete(setting);
        }


        public CompanySetting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            return _settingRepository.GetById(settingId);
        }


        public IQueryable<CompanySetting> GetAllSettings(int companyId = 0)
        {
            var query = _settingRepository.Table;
            if (companyId > 0)
                query = query.Where(x => x.CompanyId == companyId);

            return query.OrderBy(x => x.CompanyId).ThenBy(x => x.Id);
        }


        public CompanySetting GetSettingByKey(int companyId, string key)
        {
            if (String.IsNullOrWhiteSpace(key))
                return null;

            var settings = GetAllSettings(companyId);

            return settings.FirstOrDefault(x => x.Name == key);
        }


        public MaxAnnualHours GetMaxAnnualHours(int companyId)
        {
            DateTime startDate = DateTime.MinValue, endDate = startDate.AddYears(1).AddDays(-1);
            int maxHours = 0, threshold = 0;
            var settings = this.GetAllSettings(companyId).Where(x => x.Name.Contains("MaxAnnualHours."));

            if (settings.Any())
            {
                var startDateSetting = settings.FirstOrDefault(x => x.Name.Contains("StartDate"));
                if (startDateSetting != null)
                    DateTime.TryParse(startDateSetting.Value, out startDate);
                
                var endDateSetting = settings.FirstOrDefault(x => x.Name.Contains("EndDate"));
                if (endDateSetting != null)
                    DateTime.TryParse(endDateSetting.Value, out endDate);
                
                var maxHoursSetting = settings.FirstOrDefault(x => x.Name.Contains("MaxHours"));
                if (maxHoursSetting != null)
                    Int32.TryParse(maxHoursSetting.Value, out maxHours);
                
                var thresholdSetting = settings.FirstOrDefault(x => x.Name.Contains("Threshold"));
                if (thresholdSetting != null)
                    Int32.TryParse(thresholdSetting.Value, out threshold);
            }

            return new MaxAnnualHours() { StartDate = startDate, EndDate = endDate, MaxHours = maxHours, Threshold = threshold };
        }

        #endregion
    }

}