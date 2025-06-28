using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Candidates
{
    public partial class CandidateAddressService : ICandidateAddressService
    {

        #region Fields

        private readonly IRepository<CandidateAddress> _candidateAddressRepository;
        private readonly IWorkContext _workContext;
        private readonly CommonSettings _commonSettings;
        #endregion

        #region Ctor

        public CandidateAddressService(
            IRepository<CandidateAddress> candidateAddressRepository,
            IWorkContext workContext,
            CommonSettings commonSettings
            )
        {
            _candidateAddressRepository = candidateAddressRepository;
            _workContext = workContext;
            _commonSettings = commonSettings;
        }

        #endregion

        #region CRUD

        public void InsertCandidateAddress(CandidateAddress candidateAddress)
        {
            if (candidateAddress == null)
                throw new ArgumentNullException("candidateAddress");

            candidateAddress.CreatedOnUtc = DateTime.UtcNow;
            candidateAddress.UpdatedOnUtc = DateTime.UtcNow;

            candidateAddress.CleanUp();

            _candidateAddressRepository.Insert(candidateAddress);
        }

        public void UpdateCandidateAddress(CandidateAddress candidateAddress)
        {
            if (candidateAddress == null)
                throw new ArgumentNullException("candidateAddress");
            
            candidateAddress.UpdatedOnUtc = DateTime.UtcNow;
            candidateAddress.CleanUp();

            _candidateAddressRepository.Update(candidateAddress);
        }

        public void DeleteCandidateAddress(CandidateAddress candidateAddress)
        {
            if (candidateAddress == null)
                throw new ArgumentNullException("candidateAddress");

            candidateAddress.UpdatedOnUtc = DateTime.UtcNow;

            _candidateAddressRepository.Delete(candidateAddress);
        }

        #endregion

        #region CandidateAddress

        public CandidateAddress GetCandidateAddressById(int id)
        {
            if (id == 0)
                return null;

            return _candidateAddressRepository.GetById(id);
        }

        public CandidateAddress GetCandidateAddressByCandidateIdAndType(int candidateId, int typeId)
        {
            if (candidateId == 0 || typeId == 0)
                return null;

            var query = _candidateAddressRepository.Table;
            if (_commonSettings.DisplayVendor && !_workContext.CurrentAccount.IsPayrollAdministrator())
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            query = from ca in query
                    where ca.CandidateId == candidateId && ca.IsDeleted == false && ca.AddressTypeId == typeId &&
                          !ca.Candidate.IsDeleted
                    orderby ca.UpdatedOnUtc descending
                    select ca;

            return query.FirstOrDefault();
        }

        public CandidateAddress GetCandidateHomeAddressByCandidateId(int candidateId)
        {
            return this.GetCandidateAddressByCandidateIdAndType(candidateId, (int)AddressTypeEnum.Residential);
        }

        public CandidateAddress GetCandidateMailingAddressByCandidateId(int candidateId)
        {
            return this.GetCandidateAddressByCandidateIdAndType(candidateId, (int)AddressTypeEnum.MailingOrPostal);
        }

        #endregion

        private IQueryable<CandidateAddress> getCandidateAddressQuery()
        {
            var query = _candidateAddressRepository.TableNoTracking;
            query = query.Where(x => !x.IsDeleted && !x.Candidate.IsDeleted);

            if (_workContext.CurrentAccount != null)
            {
                if (_commonSettings.DisplayVendor && !_workContext.CurrentAccount.IsPayrollAdministrator())
                    query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);

                if (_workContext.CurrentAccount.IsVendor())
                    query = query.Where(x => x.Candidate.FranchiseId == _workContext.CurrentAccount.FranchiseId);
            }

            query = from a in query
                    orderby a.UpdatedOnUtc descending
                    select a;

            return query;
        }

        #region LIST

        public IList<CandidateAddress> GetAllCandidateAddressesByCandidateId(int candidateId, bool showInactive = false)
        {
            var query = this.getCandidateAddressQuery();
            
            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);

            query = query.Where(a => a.CandidateId == candidateId);

            return query.ToList();
        }


        public IQueryable<CandidateAddress> GetAllCandidateAddressesAsQueryable()
        {
            var query = this.getCandidateAddressQuery();

            return query.AsQueryable();
        }


        public IQueryable<CandidateAddressWithName> GetAllCandidateAddressesForListAsQueryable()
        {
            var query = this.getCandidateAddressQuery();

            var result = from a in query
                         orderby a.UpdatedOnUtc descending
                         select new CandidateAddressWithName()
                         {
                             Id = a.Id,
                             CandidateId = a.CandidateId,
                             EmployeeId = a.Candidate.EmployeeId,
                             FullAddress = String.Concat(a.UnitNumber , " " , a.AddressLine1 , " " , a.AddressLine2),
                             CityId = a.CityId,
                             StateProvinceId = a.StateProvinceId,
                             CountryId = a.CountryId,
                             AddressTypeId = a.AddressTypeId,
                             AddressLine1 = a.AddressLine1,
                             AddressLine2 = a.AddressLine2,
                             UnitNumber = a.UnitNumber,
                             EmployeeName = String.Concat(a.Candidate.FirstName , " " , a.Candidate.LastName),
                             UpdatedOnUtc = a.UpdatedOnUtc,
                             CreatedOnUtc = a.CreatedOnUtc,
                             Candidate = a.Candidate,
                             FranchiseId = a.Candidate.FranchiseId
                         };

            return result;
        }

        #endregion

    }
}
