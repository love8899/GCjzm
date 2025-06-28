using System;
using System.Linq;
using System.Collections.Generic;
using Wfm.Core.Domain.Franchises;
using Wfm.Admin.Extensions;
using Wfm.Services.Franchises;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Accounts;
using Wfm.Services.Companies;
using Wfm.Services.Messages;
using Wfm.Admin.Infrastructure.WcfHelper;
using System.Web;
using System.IO;
using Wfm.Core.Infrastructure;
using Wfm.Core.Domain.Common;


namespace Wfm.Admin.Models.Franchises
{
    public class CreateEditVendor_BL
    {
        public FranchiseModel GetNewVendorModel(Account account)
        {
            FranchiseModel franchiseModel = new FranchiseModel();

            return PopulateVendorModel(franchiseModel, account);
        }

        public FranchiseModel PopulateVendorModel(FranchiseModel franchiseModel, Account account)
        {
            if (franchiseModel == null)
                return null;

            franchiseModel.IsDeleted = false;
            franchiseModel.EnableStandAloneJobOrders = false;
            franchiseModel.IsDefaultManagedServiceProvider = false;
            franchiseModel.IsHot = false;
            franchiseModel.IsActive = true;
            franchiseModel.OwnerId = account.Id;
            franchiseModel.EnteredBy = account.Id;
            franchiseModel.CreatedOnUtc = System.DateTime.UtcNow;
            franchiseModel.UpdatedOnUtc = System.DateTime.UtcNow;
            franchiseModel.EnteredName = string.Format("{0} {1}", account.FirstName, account.LastName);

            return franchiseModel;
        }

        public FranchiseModel GetVendorModel(Guid? guid, IFranchiseService _franchiseService,IAccountService _accountService, out string error)
        {
            error = string.Empty;
            Franchise franchise = _franchiseService.GetFranchiseByGuid(guid);

            if (franchise == null)
            {
                error = "No such vendor exists!";
                return null;
            }

            FranchiseModel franchiseModel = franchise.ToModel();
            franchiseModel.KeepCurrentLogo = true;
            Account account = _accountService.GetAccountById(franchise.EnteredBy);
            if(account!=null)
                franchiseModel.EnteredName = _accountService.GetAccountById(franchise.EnteredBy).FullName;
            return franchiseModel;
        }

        public Franchise SaveNewVendor(IFranchiseService _franchiseService, FranchiseModel franchiseModel, IEnumerable<HttpPostedFileBase> files)
        {
            var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

            if (!commonSettings.DisplayVendor)
            {
                // This is standalone mode.
                franchiseModel.IsDefaultManagedServiceProvider = true;
                franchiseModel.EnableStandAloneJobOrders = true;
            }

            SaveFilesIntoModel(franchiseModel, files);
            Franchise franchise = franchiseModel.ToEntity();
            _franchiseService.InsertFranchise(franchise);

            Franchise franchise_update = _franchiseService.GetFranchiseById(franchise.Id);
            franchise.FranchiseId = "GCFR" + System.DateTime.Now.ToString("yy") + "-" + franchise_update.Id.ToString("0000");
            _franchiseService.UpdateFranchise(franchise_update);
            return franchise;
        }

        public Franchise UpdateVendor(IFranchiseService _franchiseService,  FranchiseModel franchiseModel, IEnumerable<HttpPostedFileBase> files)
        {
            SaveFilesIntoModel(franchiseModel, files);
            franchiseModel.UpdatedOnUtc = System.DateTime.UtcNow;

            Franchise franchise = _franchiseService.GetFranchiseById(franchiseModel.Id);
            franchise = franchiseModel.ToEntity(franchise);

            _franchiseService.UpdateFranchise(franchise);

            return franchise;
        }

        private void SaveFilesIntoModel(FranchiseModel franchiseModel, IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null && files.Count() > 0)
            {
                var file = files.FirstOrDefault();
                using (Stream inputStream = file.InputStream)
                {

                    var fileBinary = new byte[inputStream.Length];
                    inputStream.Read(fileBinary, 0, fileBinary.Length);
                    franchiseModel.FranchiseLogo = fileBinary;
                    franchiseModel.FranchiseLogoFileName =Path.GetFileName(file.FileName);

                }
            }
            else
            {
                if (!franchiseModel.KeepCurrentLogo)
                {
                    franchiseModel.FranchiseLogo = null;
                    franchiseModel.FranchiseLogoFileName = null;
                }
            }
        }

        #region Mass Email

        public IQueryable<Account> GetVendorAccountListBySelector(IAccountService _accountService, IRecruiterCompanyService _recruiterCompanyService, Account account, VendorAccountSelector selector)
        {
            var accounts = _accountService.GetAllAccountsAsQueryable(account).Where(x => !x.IsClientAccount && x.IsLimitedToFranchises);

            if (selector != null)
            {
                if (selector.VendorIds != null)
                    accounts = accounts.Where(x => selector.VendorIds.Contains(x.FranchiseId));

                if (selector.RoleIds != null)
                    accounts = accounts.Where(x => selector.RoleIds.Contains(x.AccountRoles.OrderByDescending(y => y.UpdatedOnUtc).FirstOrDefault().Id));

                if (selector.CompanyIds != null)
                {
                    var recruiterIds = _recruiterCompanyService.GetAllRecruitersAsQueryable(account)
                                       .Where(x => selector.CompanyIds.Contains(x.CompanyId))
                                       .Select(x => x.Account.Id);
 
                    accounts = accounts.Where(x => recruiterIds.Contains(x.Id));
                }
            }

            return accounts;
        }


        public void SendMassEmailToSelectedVendorAccounts(IEmailAccountService _emailAccountService, VendorMassEmailSettings _vendorMassEmailSettings, Account account, IList<int> ids, string subject, string message, bool systemAccount = true)
        {
            var from = account.Email;
            var fromName = account.FullName;
            if (systemAccount)
            {
                var emailAccount = _emailAccountService.GetEmailAccountById(_vendorMassEmailSettings.FromEmailAccountId);
                if (emailAccount != null)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.FriendlyName;
                }
            }

            var vendors = String.Join<int>(",", ids);

            string svcUser; string svcPassword;
            ClientServiceReference.WfmServiceClient svcClient = WcfHelper.GetClientService(out svcUser, out svcPassword);
            svcClient.SendMassEmailAsync(svcUser, svcPassword, account.AccountGuid.ToString(), account.FranchiseId, vendors, "Vendor", from, fromName, subject, message, 0);
            svcClient.Close();
        }

            

        #endregion
    
    }
}
