using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Payroll;
using Wfm.Services.Messages;


namespace Wfm.Services.Payroll
{
    public partial class OvertimeRuleSettingService : IOvertimeRuleSettingService
    {
        #region Fields

        private readonly IRepository<OvertimeRuleSetting> _overtimeRuleSettingRepository;

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyBillingService"/> class.
        /// </summary>
        /// <param name="CompanyBillingRepository">The CompanyBilling repository.</param>
        public OvertimeRuleSettingService(
            IRepository<OvertimeRuleSetting> OvertimeRuleSettingRepository
            )
        {
            _overtimeRuleSettingRepository = OvertimeRuleSettingRepository;
        }

        #endregion

        #region CRUD

        public virtual void Insert(OvertimeRuleSetting overtimeRuleSetting)
        {
            if (overtimeRuleSetting == null)
                throw new ArgumentNullException("overtimeRuleSetting");

            _overtimeRuleSettingRepository.Insert(overtimeRuleSetting);

        }

        public virtual void Update(OvertimeRuleSetting overtimeRuleSetting)
        {
            if (overtimeRuleSetting == null)
                throw new ArgumentNullException("overtimeRuleSetting");

            _overtimeRuleSettingRepository.Update(overtimeRuleSetting);
        }

        public virtual void Delete(OvertimeRuleSetting overtimeRuleSetting)
        {
            if (overtimeRuleSetting == null)
                throw new ArgumentNullException("overtimeRuleSetting");

            _overtimeRuleSettingRepository.Delete(overtimeRuleSetting);
        }

        #endregion


        #region OvertimeRuleSetting

        public OvertimeRuleSetting GetOvertimeRuleSettingById(int Id)
        {
            if (Id == 0)
                return null;

            return _overtimeRuleSettingRepository.GetById(Id);
        }

        #endregion


        #region LIST

        public IList<OvertimeRuleSetting> GetAllOvertimeRuleSettings()
        {
            var query = _overtimeRuleSettingRepository.Table;

            query = from o in query
                    select o;

            return query.ToList();
        }

        public IPagedList<OvertimeRuleSetting> GetAllOvertimeRuleSettings(int pageIndex = 0, int PageSize = int.MaxValue)
        {
            var query = _overtimeRuleSettingRepository.Table; //no where filter. get all records.

            int total = _overtimeRuleSettingRepository.Table.Count(); // counting total records

            IPagedList<OvertimeRuleSetting> settings = new PagedList<OvertimeRuleSetting>(query, pageIndex, PageSize, total);
            return settings;
        }

        public IQueryable<OvertimeRuleSetting> GetAllOvertimeRuleSettingsAsQueryable()
        {
            var query = _overtimeRuleSettingRepository.Table;

            query = from o in query
                    select o;

            return query.AsQueryable();
        }

        public IList<SelectListItem> GetAllOvertimeRuleSettingsForDropDownList()
        {
            var query = _overtimeRuleSettingRepository.TableNoTracking;
            var list = query.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = String.Concat(x.Code, " - ", x.Description) }).ToList();
            list.Add(new SelectListItem() { Text = "None", Value = "0" });
            return list.ToList();
        }
        #endregion

    }
}
