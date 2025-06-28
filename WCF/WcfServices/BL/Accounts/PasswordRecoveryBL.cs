using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Infrastructure;
using Wfm.Services.Accounts;
using Wfm.Services.Common;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Messages;


namespace WcfServices.Accounts
{
    public class PasswordRecoveryBL
    {
        private bool IsValidEmail(string strIn)
        {
            if (String.IsNullOrWhiteSpace(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }


            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;

            domainName = idn.GetAscii(domainName);

            return match.Groups[1].Value + domainName;
        }



        public bool SendPasswordResetMessage(string email)
        {
            if (!this.IsValidEmail(email) )
                return false;

            var _accountService = EngineContext.Current.Resolve<IAccountService>();
            var account = _accountService.GetAccountByEmail(email);

            if (account == null || !account.IsActive || account.IsDeleted)
                return true;

            if (String.IsNullOrWhiteSpace(account.SecurityQuestion1Answer) || String.IsNullOrWhiteSpace(account.SecurityQuestion2Answer))
                return false;

            var _logger = EngineContext.Current.Resolve<ILogger>();

            try
            {
                // token
                var _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
                var passwordRecoveryToken = System.Guid.NewGuid().ToString("N") + System.Guid.NewGuid().ToString("N");
                var tokenExpiryDate = DateTime.UtcNow.AddDays(3).ToString("u"); // Token will expire after three days
                _genericAttributeService.SaveAttribute(account, SystemAccountAttributeNames.PasswordRecoveryToken, passwordRecoveryToken.ToString());
                _genericAttributeService.SaveAttribute(account, SystemAccountAttributeNames.TokenExpiryDate, tokenExpiryDate.ToString());

                // send message
                var _workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                _workflowMessageService.SendAccountPasswordRecoveryMessage(account, languageId: 1);

                // activity log
                var _activityLogService = EngineContext.Current.Resolve<IActivityLogService>();
                var _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                _activityLogService.InsertActivityLog("RetrieveAccountPassword", _localizationService.GetResource("ActivityLog.RetrieveAccountPassword"), account, account.FullName);

                account.FailedSecurityQuestionAttempts = 0;
                _accountService.Update(account);

                return true;
            }

            catch (Exception ex)
            {
                _logger.Error("PasswordRecoveryBL(): ", ex);
                return false;
            }
        }

    }
}