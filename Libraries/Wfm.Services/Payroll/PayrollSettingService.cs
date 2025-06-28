using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Domain.Payroll;
using Wfm.Data;
using Wfm.Services.Franchises;
using Wfm.Services.Logging;

namespace Wfm.Services.Payroll
{
    public class PayrollSettingService:IPayrollSettingService
    {
        #region Fields
        private readonly IDbContext _dbContext;
        private readonly IFranchiseService _franchiseService;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext; 
        #endregion

        #region Ctor
        public PayrollSettingService(IDbContext dbContext, IFranchiseService franchiseService, ILogger logger, IWorkContext workContext)
        {
            _dbContext = dbContext;
            _franchiseService = franchiseService;
            _logger = logger;
            _workContext = workContext;
        }
        #endregion

        #region Methods
        public PayrollSetting GetPayrollSettingByFranchiseId(Guid? franchiseGuid)
        {
            var franchise = _franchiseService.GetFranchiseByGuid(franchiseGuid);
            if (franchise == null)
                return null;
            else
            {
                StringBuilder query = new StringBuilder();
                query.Append(@"Select fran.Id as FranchiseId,fran.FranchiseGuid,fba.Client_Number, fba.Transmission_Header, fran.BusinessNumber, fran.EIRate, fran.NEQ, 
                                  fran.QuebecIdentificationNumber, fran.RL1TransmitterNumber, bfl.Code as DDFileLayout,bfl.Id as DDFileLayoutId 
                                  ,fran.RL1SequentialNumber,fran.RL1XMLSequentialNumber,fba.InstitutionNumber,fba.TransitNumber,fba.AccountNumber
                            from Franchise as fran
                               left join FranchiseBankAccount fba on fba.FranchiseId=fran.Id
							   left join BankFileLayout bfl on fba.BankFileLayoutId = bfl.Id
                            where fran.FranchiseGuid = @franchiseGuid");
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("franchiseGuid", franchiseGuid);
                return _dbContext.SqlQuery<PayrollSetting>(query.ToString(), parameters).FirstOrDefault();
            }
        }

        public IList<SelectListItem> GetAllDDFileLayoutDataSource()
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"Select Code as Value, Description as Text from  BankFileLayout ");

            return _dbContext.SqlQuery<SelectListItem>(query.ToString()).ToList();
        }

        public bool UpdatePayrollSetting(PayrollSetting setting)
        {
            setting.DDFileLayoutId = GetBankFileLayoutIdByCode(setting.DDFileLayout);

            SqlParameter[] parameters = new SqlParameter[14];
            parameters[0] = new SqlParameter("clientNumber", setting.Client_Number);

            if (String.IsNullOrWhiteSpace(setting.Transmission_Header))
            {
                parameters[1] = new SqlParameter("transmissionHeader", DBNull.Value);
            }
            else
            {
                parameters[1] = new SqlParameter("transmissionHeader", setting.Transmission_Header);
            }

            if (String.IsNullOrWhiteSpace(setting.BusinessNumber))
            {
                parameters[2] = new SqlParameter("BusinessNum", DBNull.Value);
            }
            else
            {
                parameters[2] = new SqlParameter("BusinessNum", setting.BusinessNumber);
            }

            if (!setting.EIRate.HasValue || setting.EIRate.Value<=0)
            {
                parameters[3] = new SqlParameter("EIRate", DBNull.Value);
            }
            else
            {
                parameters[3] = new SqlParameter("EIRate", setting.EIRate.Value);
            }

            if (String.IsNullOrWhiteSpace(setting.NEQ))
            {
                parameters[4] = new SqlParameter("NEQ", DBNull.Value);
            }
            else
            {
                parameters[4] = new SqlParameter("NEQ", setting.NEQ);
            }

            if (String.IsNullOrWhiteSpace(setting.QuebecIdentificationNumber) || setting.QuebecIdentificationNumber.Contains("_"))
            {
                parameters[5] = new SqlParameter("QuebecIdentificationNumber", DBNull.Value);
            }
            else
            {
                parameters[5] = new SqlParameter("QuebecIdentificationNumber", setting.QuebecIdentificationNumber);
            }

            if (String.IsNullOrWhiteSpace(setting.RL1TransmitterNumber) || setting.RL1TransmitterNumber.Contains("_"))
            {
                parameters[6] = new SqlParameter("RL1TransmitterNumber", DBNull.Value);
            }
            else
            {
                parameters[6] = new SqlParameter("RL1TransmitterNumber", setting.RL1TransmitterNumber);
            }

            parameters[7] = new SqlParameter("franchiseId", setting.FranchiseId);
            if (setting.DDFileLayoutId.HasValue)
            {
                parameters[8] = new SqlParameter("ddFileLayoutId", setting.DDFileLayoutId.Value);
            }
            else
            {
                parameters[8] = new SqlParameter("ddFileLayoutId", DBNull.Value);
            }
            
            if (String.IsNullOrWhiteSpace(setting.RL1SequentialNumber))
            {
                parameters[9] = new SqlParameter("RL1SequentialNumber", DBNull.Value);
            }
            else
            {
                parameters[9] = new SqlParameter("RL1SequentialNumber", setting.RL1SequentialNumber);
            }
            if (String.IsNullOrWhiteSpace(setting.RL1XMLSequentialNumber))
            {
                parameters[10] = new SqlParameter("RL1XMLSequentialNumber", DBNull.Value);
            }
            else
            {
                parameters[10] = new SqlParameter("RL1XMLSequentialNumber", setting.RL1XMLSequentialNumber);
            }
            if (String.IsNullOrWhiteSpace(setting.InstitutionNumber))
            {
                parameters[11] = new SqlParameter("InstitutionNumber", DBNull.Value);
            }
            else
            {
                parameters[11] = new SqlParameter("InstitutionNumber", setting.InstitutionNumber);
            }
            if (String.IsNullOrWhiteSpace(setting.TransitNumber))
            {
                parameters[12] = new SqlParameter("TransitNumber", DBNull.Value);
            }
            else
            {
                parameters[12] = new SqlParameter("TransitNumber", setting.TransitNumber);
            }
            if (String.IsNullOrWhiteSpace(setting.AccountNumber))
            {
                parameters[13] = new SqlParameter("AccountNumber", DBNull.Value);
            }
            else
            {
                parameters[13] = new SqlParameter("AccountNumber", setting.AccountNumber);
            }


            String sUpdateFranchiseBankAccount_Table = @"if not exists (Select 1 from FranchiseBankAccount where FranchiseId = @franchiseId)
                                                        begin
	                                                        Insert Into FranchiseBankAccount
                                                             (FileCreationNumber,Client_Number,Transmission_Header,FranchiseId,BankFileLayoutId,InstitutionNumber,TransitNumber,AccountNumber)
	                                                        Values ('0000',@clientNumber,@transmissionHeader,@franchiseId, @ddFileLayoutId,@InstitutionNumber,@TransitNumber,@AccountNumber) 
                                                        end 
                                                        else
                                                        begin
	                                                        Update FranchiseBankAccount
	                                                        Set Client_Number=@clientNumber, Transmission_Header=@transmissionHeader, BankFileLayoutId = @ddFileLayoutId
	                                                            ,InstitutionNumber=@InstitutionNumber,TransitNumber=@TransitNumber,AccountNumber=@AccountNumber
                                                            where FranchiseId=@franchiseId
                                                        end
                                                        Update Franchise Set BusinessNumber=@BusinessNum, EIRate=@EIRate, NEQ=@NEQ, QuebecIdentificationNumber=@QuebecIdentificationNumber 
                                                            , RL1TransmitterNumber=@RL1TransmitterNumber,RL1SequentialNumber=@RL1SequentialNumber,RL1XMLSequentialNumber=@RL1XMLSequentialNumber
                                                            Where Id=@franchiseId";

            int recCount = 0;
            try
            {
                recCount = _dbContext.ExecuteSqlCommand(sUpdateFranchiseBankAccount_Table, false, null, parameters);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("UpdatePayrollSetting():", ex);
                return false;
            }
            
        }

        private int GetBankFileLayoutIdByCode(string code)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"Select Id from  BankFileLayout Where Code=@code");
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("code", code);
            return _dbContext.SqlQuery<int>(query.ToString(),parameters).FirstOrDefault();
        }


        public List<EmailSetting> GetPayrollEmailSetting(Guid? franchiseGuid)
        {
            var franchise = _franchiseService.GetFranchiseByGuid(franchiseGuid);
            if (franchise == null)
                return null;
            else
            {
                StringBuilder query = new StringBuilder();
                query.Append(@"select fr.Id as FranchiseId,ea.Email as EmailAddress, Host as [EmailSmtpClient],Port as [EmailPortNumber],
	                                [Password] as EmailPassword,EnableSsl, EmailSubject,EmailBody,Username,Code
                                from EmailAccount ea
                                inner join Franchise fr on fr.Id = ea.FranchiseId
                                where Code in ('PayStub','T4','RL1','Accounting') and fr.FranchiseGuid=@franchiseGuid");
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("franchiseGuid", franchiseGuid);
                return _dbContext.SqlQuery<EmailSetting>(query.ToString(), parameters).ToList();
            }
        }

        public EmailSetting GetPayrollEmailSetting(Guid? franchiseGuid,string code)
        {
            var franchise = _franchiseService.GetFranchiseByGuid(franchiseGuid);
            if (franchise == null)
                return null;
            else
            {
                StringBuilder query = new StringBuilder();
                query.Append(@"select fr.Id as FranchiseId,fr.FranchiseGuid,ea.Email as EmailAddress, Host as [EmailSmtpClient],Port as [EmailPortNumber],
	                                [Password] as EmailPassword,EnableSsl, EmailSubject,EmailBody,Username,Code
                                from EmailAccount ea
                                inner join Franchise fr on fr.Id = ea.FranchiseId
                                where Code=@code and fr.FranchiseGuid=@franchiseGuid");
                SqlParameter[] parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("franchiseGuid", franchiseGuid);
                parameters[1] = new SqlParameter("code", code);
                return _dbContext.SqlQuery<EmailSetting>(query.ToString(), parameters).FirstOrDefault();
            }
        }

        public bool UpdatePayrollEmailSetting(EmailSetting setting,int accountId)
        {
            SqlParameter[] paras = new SqlParameter[11];
            paras[0] = new SqlParameter("emailAddress", setting.EmailAddress);
            paras[1] = new SqlParameter("smtpClient", setting.EmailSmtpClient);
            paras[2] = new SqlParameter("portNumber", setting.EmailPortNumber);
            paras[3] = new SqlParameter("emailPassword", setting.EmailPassword);
            paras[4] = new SqlParameter("enableSsl", setting.EnableSsl);
            paras[5] = new SqlParameter("emailSubject", setting.EmailSubject);
            paras[6] = new SqlParameter("emailBody", setting.EmailBody);
            paras[7] = new SqlParameter("franchiseId", setting.FranchiseId);
            paras[8] = new SqlParameter("userName", setting.UserName);
            paras[9] = new SqlParameter("EnteredBy", accountId);
            paras[10] = new SqlParameter("code", setting.Code);
            StringBuilder query = new StringBuilder();
            query.AppendLine(@"If Exists(Select 1 From EmailAccount Where Code=@code and FranchiseId=@franchiseId) 
                                    BEGIN
                                        Update EmailAccount Set Email=@emailAddress,
                                                                Host=@smtpClient,
                                                                Port=@portNumber,
                                                                Password=@emailPassword,
                                                                EnableSsl=@enableSsl,
                                                                EmailSubject=@emailSubject,
                                                                EmailBody=@emailBody,
                                                                UpdatedOnUtc=Getdate(),
                                                                UserName=@userName
                                    Where Code=@code and FranchiseId=@franchiseId                     
                                  END
                                ELSE 
                                BEGIN
                                        Insert into EmailAccount(
                                                   Email,         
                                                   Host,         
                                                   Port,         
                                                   Password,         
                                                   EnableSsl,         
                                                   EmailSubject,         
                                                   EmailBody,                                                            
                                                   Code,                                                            
                                                   FranchiseId,                                                            
                                                   CreatedOnUtc,                                                            
                                                   UserName,                                                           
                                                   UseDefaultCredentials, 
                                                   EnteredBy, 
                                                   IsActive, 
                                                   DisplayOrder 
                                                    )
                                        Values(
                                                @emailAddress,
                                                @smtpClient,
                                                @portNumber,
                                                @emailPassword,
                                                @enableSsl,
                                                @emailSubject,
                                                @emailBody,
                                                @code,
                                                @franchiseId,
                                                Getdate(),
                                                @userName,    
                                                'false',
                                               @EnteredBy,
                                                'true',
                                                0
                                              )
                                END            
                            ");
            int count = _dbContext.ExecuteSqlCommand(query.ToString(), false, null, paras);
            return count > 0;
        }
        #endregion
    }
}
