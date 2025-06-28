using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;
using Wfm.Core;

namespace Wfm.Services.ClockTime
{
    public partial class ClockDeviceService : IClockDeviceService
    {
        #region Fields

        IRepository<CompanyClockDevice> _clockDeviceRepository;
        IRepository<Company> _companyRepository;
        IRepository<CompanyLocation> _companyLocationRepository;

        #endregion

        #region Ctor

        public ClockDeviceService (IRepository<CompanyClockDevice> clockDeviceRepository,
            IRepository<Company> companyRepository,
            IRepository<CompanyLocation> companyLocationRepository
            )
        {
            _clockDeviceRepository = clockDeviceRepository;
            _companyRepository = companyRepository;
            _companyLocationRepository = companyLocationRepository;
        }

        #endregion


        #region CRUD

        public void Insert(CompanyClockDevice clockDevice)
        {
            if (clockDevice == null)
                throw new ArgumentNullException("Clock Device is null");

            //insert
            _clockDeviceRepository.Insert(clockDevice);
        }

        public void Update(CompanyClockDevice clockDevice)
        {
            if (clockDevice == null)
                throw new ArgumentNullException("Clock Device is null");

            //update
            _clockDeviceRepository.Update(clockDevice);
        }

        #endregion

        #region Method

        public CompanyClockDevice GetClockDeviceById(int id)
        {
            if (id == 0)
                return null;

            return _clockDeviceRepository.GetById(id);
        }

        public CompanyClockDevice GetClockDeviceByClockDeviceUid(string clockDeviceUid)
        {
            if (string.IsNullOrWhiteSpace(clockDeviceUid))
                return null;

            var query = _clockDeviceRepository.Table;

            query = from c in query
                    where c.ClockDeviceUid == clockDeviceUid
                    select c;

            return query.FirstOrDefault();
        }


        public Company GetCompanyByClockDeviceUid(string clockDeviceUid)
        {
            if (string.IsNullOrWhiteSpace(clockDeviceUid))
                return null;

            var query = _companyRepository.Table;

            query = from c in query
                    join cl in _companyLocationRepository.Table on c.Id equals cl.CompanyId
                    join cc in _clockDeviceRepository.Table on cl.Id equals cc.CompanyLocationId
                    where cc.ClockDeviceUid == clockDeviceUid && cc.IsActive && !cc.IsDeleted
                    select c;


            return query.FirstOrDefault();
        }

        #endregion


        #region List

        public IQueryable<CompanyClockDevice> GetAllClockDevicesAsQueryable(bool activeOnly = true, bool excludeEnrolment = true)
        {
            return _clockDeviceRepository.Table.Where(x => !activeOnly || x.IsActive)
                .Where(x => !excludeEnrolment || x.AddOnEnroll);
        }


        public IQueryable<CompanyClockDevice> GetClockDevicesByCompanyId(int companyId, bool excludeEnrolment = true)
        {
            if (companyId == 0)
                return Enumerable.Empty<CompanyClockDevice>().AsQueryable();

            var query = GetAllClockDevicesAsQueryable(true, excludeEnrolment)
                .Where(x => x.CompanyLocation.CompanyId == companyId);

            return query.OrderByDescending(x => x.CreatedOnUtc);
        }


        public IList<CompanyClockDevice> GetClockDevicesByCompanyLocationId(int companyLocationId, bool excludeEnrolment = true)
        {
            if (companyLocationId == 0)
                return Enumerable.Empty<CompanyClockDevice>().ToList();

            var query = GetAllClockDevicesAsQueryable(true, excludeEnrolment)
                .Where(x => x.CompanyLocationId == companyLocationId);

            return query.OrderByDescending(x => x.CreatedOnUtc).ToList();
        }


        public IQueryable<CompanyClockDevice> GetAllClockDevicesWithIPAddress(bool activeOnly = true, bool excludeEnrolment = true)
        {
            return GetAllClockDevicesAsQueryable(activeOnly, excludeEnrolment).Where(x => x.IPAddress != null && x.IPAddress != "");
        }

        #endregion

    }
}
