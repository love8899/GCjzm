using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Data;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Services.Franchises
{
    public interface IVendorCertificateService
    {
        #region CRUD
        /// <summary>
        /// Insert entity and return Guid
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns>return New entity's Id</returns>
        void Create(VendorCertificate certificate);
        /// <summary>
        /// Retrive entity 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>entity</returns>
        VendorCertificate Retrive(Guid? guid);
        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns>Guid</returns>
        //Guid? Update(VendorCertificate certificate);

        Guid? Update(VendorCertificate certificate, string[] exclude);
        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns>sucess or not</returns>
        bool Delete(VendorCertificate certificate);

        #endregion

        #region Method
        IQueryable<VendorCertificate> GetAllCertificatesByVendorId(int id);

        IQueryable<VendorCertificate> GetAllCertificates();
        IQueryable<SimpleVendorCertificate> GetAllLastestCertificates();
        #endregion
    }
}
