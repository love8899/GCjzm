using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Data;
using Wfm.Core.Domain.Franchises;
using Wfm.Data;
using Wfm.Services.Logging;

namespace Wfm.Services.Franchises
{
    public class VendorCertificateService:IVendorCertificateService
    {
        #region Fields
        private readonly IRepository<VendorCertificate> _vendorCertificateRepository;
        private readonly ILogger _logger;
        private readonly IDbContext _dbContext;
        #endregion
        
        #region Ctor
        public VendorCertificateService(IRepository<VendorCertificate> vendorCertificateRepository,IDbContext dbContext,
                                        ILogger logger)
        {
            _vendorCertificateRepository = vendorCertificateRepository;
            _logger = logger;
            _dbContext = dbContext;
        }
        #endregion

        #region CRUD
        public void Create(VendorCertificate certificate)
        {
            if (certificate == null)
                return;
            try
            {
                  _vendorCertificateRepository.Insert(certificate);
//                SqlParameter[] paras = new SqlParameter[7];
//                paras[0] = new SqlParameter("certificateGuid", certificate.CertificateGuid);
//                paras[1] = new SqlParameter("franchiseId", certificate.FranchiseId);
//                paras[2] = new SqlParameter("generalLiabilityCoverage", certificate.GeneralLiabilityCoverage);
//                paras[3] = new SqlParameter("generalLiabilityCertificateExpiryDate", certificate.GeneralLiabilityCertificateExpiryDate);
//                if (certificate.CertificateFile == null)
//                {
//                    paras[4] = new SqlParameter("certificateFile", System.Data.SqlDbType.VarBinary, -1);
//                    paras[4].Value = DBNull.Value;
//                }
//                else
//                    paras[4] = new SqlParameter("certificateFile", certificate.CertificateFile);
//                paras[5] = new SqlParameter("createdOnUtc", certificate.CreatedOnUtc);
//                paras[6] = new SqlParameter("updatedOnUtc", certificate.UpdatedOnUtc);
//                _dbContext.ExecuteSqlCommand(@"INSERT INTO [dbo].[VendorCertificate]
//                                                    ([CertificateGuid]
//                                                    ,[FranchiseId]
//                                                    ,[GeneralLiabilityCoverage]
//                                                    ,[GeneralLiabilityCertificateExpiryDate]
//                                                    ,[CertificateFile]
//                                                    ,[CreatedOnUtc]
//                                                    ,[UpdatedOnUtc])
//                                                VALUES
//                                                    (@certificateGuid
//                                                    ,@franchiseId
//                                                    ,@generalLiabilityCoverage
//                                                    ,@generalLiabilityCertificateExpiryDate
//                                                    ,@certificateFile
//                                                    ,@createdOnUtc
//                                                    ,@updatedOnUtc)", false, null, paras);
            }
            catch (Exception ex)
            {
                _logger.Error("VendorCertificateService:Create():", ex);
            }
        }

        public VendorCertificate Retrive(Guid? guid)
        {
            if (guid == null)
                return null;
            var entity = _vendorCertificateRepository.Table.Where(x => x.CertificateGuid == guid).FirstOrDefault();
            if (entity == null)
                return null;
            return entity;
        }
        //public Guid? Update(VendorCertificate certificate)
        //{
        //    if (certificate == null)
        //        return null;
        //    try
        //    {
        //        _vendorCertificateRepository.Update(certificate);
        //        return certificate.CertificateGuid;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("VendorCertificateService:Update():", ex);
        //        return null;
        //    }
        //}

        public Guid? Update(VendorCertificate certificate, string[] exclude)
        {
            if (certificate == null)
                return null;
            try
            {
                _vendorCertificateRepository.Update(certificate,exclude);
                return certificate.CertificateGuid;
            }
            catch (Exception ex)
            {
                _logger.Error("VendorCertificateService:Update():", ex);
                return null;
            }
        }
        public bool Delete(VendorCertificate certificate)
        {
            if (certificate == null)
                return false;
            try
            {
                
                _vendorCertificateRepository.Delete(certificate);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("VendorCertificateService:Delete():", ex);
                return false;
            }
        }

        
        #endregion

        #region Method
        public IQueryable<VendorCertificate> GetAllCertificatesByVendorId(int id)
        {
            return _vendorCertificateRepository.TableNoTracking.Where(x => x.FranchiseId == id);
        }

        public IQueryable<VendorCertificate> GetAllCertificates()
        {
            return _vendorCertificateRepository.TableNoTracking;
        }

        public IQueryable<SimpleVendorCertificate> GetAllLastestCertificates()
        {
            return _vendorCertificateRepository.TableNoTracking
                .Where(x => x.GeneralLiabilityCertificateExpiryDate.HasValue)
                .Select(x => new SimpleVendorCertificate() { Description = x.Description, FranchiseId = x.FranchiseId, GeneralLiabilityCertificateExpiryDate = x.GeneralLiabilityCertificateExpiryDate.Value })
                .GroupBy(x => new { x.FranchiseId, x.Description })
                .Select(x => new SimpleVendorCertificate() { Description = x.Key.Description, FranchiseId = x.Key.FranchiseId, GeneralLiabilityCertificateExpiryDate = x.Max(y => y.GeneralLiabilityCertificateExpiryDate) });
                //.GroupBy(x => new { x.Item1, x.Item3 })                                
        }
        #endregion

    }
}
