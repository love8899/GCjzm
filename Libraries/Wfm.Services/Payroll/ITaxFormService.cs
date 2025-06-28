using System.Collections.Generic;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Services.Payroll
{
    public interface ITaxFormService
    {
        #region Method
        IEnumerable<TaxForm> GetAllT4sByCandidateIdAndYear(int candidateId, int year);
        IEnumerable<TaxForm> GetAllRL1sByCandidateIdAndYear(int candidateId, int year);
        IEnumerable<TaxForm> GetAllTaxFormsByCandidateIdAndYear(int candidateId, int year);
        #endregion
    }
}
