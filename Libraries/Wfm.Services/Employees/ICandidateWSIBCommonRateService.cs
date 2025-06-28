using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.WSIB;

namespace Wfm.Services.Employees
{
    public interface ICandidateWSIBCommonRateService
    {
        #region CRUD
        void Create(CandidateWSIBCommonRate entity);
        CandidateWSIBCommonRate Retrieve(int id);
        void Update(CandidateWSIBCommonRate entity);
        void Delete(CandidateWSIBCommonRate entity);
        #endregion

        #region Method
        IQueryable<CandidateWSIBCommonRate> GetAllWSIBCommonRateByCandidateId(int candidate);
        bool ValidateWSIBCommonRates(List<CandidateWSIBCommonRate> rates, out string errors);
        #endregion
    }
}
