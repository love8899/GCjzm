using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Data;
using Wfm.Core.Domain.Common;
using Wfm.Core.Caching;

namespace Wfm.Services.Common
{
    public partial class BankService : IBankService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : bank ID
        /// </remarks>
        private const string BANKS_BY_ID_KEY = "Wfm.bank.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string BANKS_ALL_KEY = "Wfm.bank.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string BANKS_PATTERN_KEY = "Wfm.bank.";

        #endregion

        #region Fields

        private readonly IRepository<Bank> _bankRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public BankService(
            IRepository<Bank> bankRepository,
            ICacheManager cacheManager
        )
        {
            _bankRepository = bankRepository;
            _cacheManager = cacheManager;
        }

        #endregion 

        
        #region CRUD

        public void InsertBank(Bank bank)
        {
            if (bank == null)
                throw new ArgumentNullException("bank");

            //insert
            _bankRepository.Insert(bank);

            //cache
            _cacheManager.RemoveByPattern(BANKS_PATTERN_KEY);
        }

        public void UpdateBank(Bank bank)
        {
            if (bank == null)
                throw new ArgumentNullException("bank");

            _bankRepository.Update(bank);

            //cache
            _cacheManager.RemoveByPattern(BANKS_PATTERN_KEY);
        }

        public void DeleteBank(Bank bank)
        {
            if (bank == null)
                throw new ArgumentNullException("bank");

            _bankRepository.Delete(bank);

            //cache
            _cacheManager.RemoveByPattern(BANKS_PATTERN_KEY);
        }

        #endregion

        #region Bank

        public Bank GetBankById(int id)
        {
            if (id == 0)
                return null;

            // No caching
            //return _bankRepository.GetById(id);

            // Using caching
            string key = string.Format(BANKS_BY_ID_KEY, id);
            return _cacheManager.Get(key, () => _bankRepository.GetById(id));
        }

        #endregion

        #region LIST

        public IList<Bank> GetAllBanks(bool showInactive = false, bool showHidden = false)
        {
            //no caching
            //-----------------------------
            //var query = _bankRepository.Table;

            //// active
            //if (!showInactive)
            //    query = query.Where(c => c.IsActive == true);
            //// deleted
            //if (!showHidden)
            //    query = query.Where(c => c.IsDeleted == false);

            //query = from b in query
            //        orderby b.DisplayOrder, b.BankName
            //        select b;

            //return query.ToList();


            //using caching
            //-----------------------------
            string key = string.Format(BANKS_ALL_KEY, showInactive);
            return _cacheManager.Get(key, () =>
            {
                var query = _bankRepository.Table;

                // active
                if (!showInactive)
                    query = query.Where(c => c.IsActive == true);
                // deleted
                if (!showHidden)
                    query = query.Where(c => c.IsDeleted == false);

                query = from b in query
                        orderby b.DisplayOrder, b.BankName
                        select b;

                return query.ToList();
            });
        }

        #endregion
    }
}
