using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Companies;


namespace Wfm.Services.ClockTime
{
     public partial interface IClockDeviceService
     {
        #region CRUD

        void Insert(CompanyClockDevice clockDevice);

        void Update(CompanyClockDevice clockDevice);

        #endregion


        #region Method

        CompanyClockDevice GetClockDeviceById(int id);

        CompanyClockDevice GetClockDeviceByClockDeviceUid(string clockDeviceUid);

        Company GetCompanyByClockDeviceUid(string clockDeviceUid);

        #endregion


        #region List

        IQueryable<CompanyClockDevice> GetAllClockDevicesAsQueryable(bool activeOnly = true, bool excludeEnrolment = true);

        IQueryable<CompanyClockDevice> GetClockDevicesByCompanyId(int companyId, bool excludeEnrolment = true);

        IList<CompanyClockDevice> GetClockDevicesByCompanyLocationId(int companyLocationId, bool excludeEnrolment = true);

        IQueryable<CompanyClockDevice> GetAllClockDevicesWithIPAddress(bool activeOnly = true, bool excludeEnrolment = true);

        #endregion

    }
}
