using System.Linq;
using Wfm.Core.Domain.Payroll;


namespace Wfm.Services.Payroll
{
    public partial interface IT4BaseService
    {
        #region T4

        T4_Base GetT4DataById(int id);

        IQueryable<T4_Base> GetAllT4s(int? franchiseId = null, bool includeSubmitted = true);

        #endregion

        
        #region T4 PDF

        string GetEmployeeT4PDF(int franchiseId, int t4Id, bool isEncrypted, out int candidateId);

        byte[] GetEmployeeT4PDFBytes(int franchiseId, int t4Id, bool isEncrypted, out int candidateId);

        #endregion
    }
}
