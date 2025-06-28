using System.Linq;
using Wfm.Core.Domain.TaxForm.RL1;


namespace Wfm.Services.Payroll
{
    public partial interface IRL1BaseService
    {
        #region RL1

        RL1_Base GetRL1DataById(int id);
     
        IQueryable<RL1_Base> GetAllRL1s(int? franchiseId = null);

        #endregion


        #region RL1 PDF

        string GetEmployeeRL1PDF(int franchiseId, int rl1Id, bool isEncrypted, out int candidateId);


        byte[] GetEmployeeRL1PDFBytes(int franchiseId, int rl1Id, bool isEncrypted, out int candidateId);

        #endregion
    }
}
