using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Payroll;
using Wfm.Core.Domain.Franchises;
using System.Data.Entity;


namespace Wfm.Services.Payroll
{
    public class PayGroupService : IPayGroupService
    {
        #region Constants

        private const string PAYGROUP_PATTERN_KEY = "Wfm.PayGroup.";

        #endregion

        #region Fields

        private readonly IRepository<PayGroup> _PayGroupRepository;
        private readonly IRepository<Franchise> _FranchiseRepository;
        private readonly IRepository<Payroll_Calendar> _PayrollCalendarRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public PayGroupService(ICacheManager cacheManager, IRepository<PayGroup> PayGroupRepository, IRepository<Franchise> FranchiseRepository, IRepository<Payroll_Calendar> PayrollCalendarRepository)
        {
            _cacheManager = cacheManager;
            _PayGroupRepository = PayGroupRepository;
            _FranchiseRepository = FranchiseRepository;
            _PayrollCalendarRepository = PayrollCalendarRepository;
        }

        #endregion

        #region CRUD

        public void InsertPayGroup(PayGroup payGroup)
        {
            if (payGroup == null)
                throw new ArgumentNullException("PayGroup");

            payGroup.CreatedOnUtc = DateTime.UtcNow;
            payGroup.UpdatedOnUtc = DateTime.UtcNow;

            _PayGroupRepository.Insert(payGroup);

            //cache
            _cacheManager.RemoveByPattern(PAYGROUP_PATTERN_KEY);
        }

        public void UpdatePayGroup(PayGroup payGroup, string[] excludeList)
        {
            if (payGroup == null)
                throw new ArgumentNullException("PayGroup");

            payGroup.UpdatedOnUtc = DateTime.UtcNow;
            _PayGroupRepository.Update(payGroup, excludeList);

            //cache
            _cacheManager.RemoveByPattern(PAYGROUP_PATTERN_KEY);
        }

        public bool DeletePayGroupById(int payGroupId, out string errorMessage)
        {
            if (payGroupId <= 0)
                throw new ArgumentNullException("PayGroup");

            var count = _PayrollCalendarRepository.TableNoTracking.Count(x => x.PayGroupId == payGroupId && x.PayPeriodCommitDate.HasValue);
            if (count > 0)
            {
                errorMessage = String.Format("There are {0} payrolls committed for this pay group. You cannot delete this pay group", count);
                return false;
            }

            errorMessage = null;

            _PayrollCalendarRepository.Delete(_PayrollCalendarRepository.Table.Where(x => x.PayGroupId == payGroupId).ToList());
            _PayGroupRepository.Delete(_PayGroupRepository.Table.Where(x => x.Id == payGroupId).ToList());

            //cache
            _cacheManager.RemoveByPattern(PAYGROUP_PATTERN_KEY);

            return true;
        }

        #endregion

        #region PayGroup

        public PayGroup GetPayGroupById(int? id)
        {
            if (id == null || id == 0)
                return null;

            return _PayGroupRepository.TableNoTracking.Where(x => x.Id == id).FirstOrDefault();
        }

        public int GetPayGroupIdByName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return 0;

            var PayGroup = _PayGroupRepository.Table.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();

            return PayGroup != null ? PayGroup.Id : 0;
        }

        #endregion

        #region LIST
        public IQueryable<PayGroup> GetAllPayGroups()
        {
            return _PayGroupRepository.Table
                .Include(x => x.Vendor)
                .Include(x => x.PayFrequencyType)
                .OrderBy(x => x.Code);
        }

        public List<PayGroup> GetAllPayGroups(int year)
        {
            var query = _PayGroupRepository.Table
                .Include(x => x.Vendor)
                .Include(x => x.PayFrequencyType)
                .Include(x => x.Payroll_Calendar)
                .OrderBy(x => x.Code);


            var query2 = from x in query
                         from o in x.Payroll_Calendar.Where(x2 => x2.Year == year && x2.PayPeriodCommitDate != null).DefaultIfEmpty()
                         select new
                         {
                             x.Id,
                             x.Code,
                             x.CreatedOnUtc,
                             x.FranchiseId,
                             x.IsDefault,
                             x.Name,
                             x.PayFrequencyTypeId,
                             x.UpdatedOnUtc,
                             HasCommittedPayroll = (o == null ? false : true),
                             x.Vendor
                         };

            var data = query2.Distinct().ToList();

            var result = new List<PayGroup>();
            foreach (var x in data)
            {
                result.Add(new PayGroup()
                {
                    Id = x.Id,
                    Code = x.Code,
                    CreatedOnUtc = x.CreatedOnUtc,
                    FranchiseId = x.FranchiseId,
                    IsDefault = x.IsDefault,
                    Name = x.Name,
                    PayFrequencyTypeId = x.PayFrequencyTypeId,
                    UpdatedOnUtc = x.UpdatedOnUtc,
                    HasCommittedPayroll = x.HasCommittedPayroll,
                    Vendor = x.Vendor
                });

            }

            return result;
        }

        public IList<PayGroup> GetAllPayGroupsByFranchiseId(int franchiseId)
        {
            var query = this.GetAllPayGroups().Where(x => x.FranchiseId == franchiseId);

            return query.ToList();
        }

        #endregion


    }
}
