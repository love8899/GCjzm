using System;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Infrastructure;
using Wfm.Services.Common;
using Wfm.Services.Localization;

namespace Wfm.Services.Accounts
{
    public static class AccountExtentions
    {
        ///// <summary>
        ///// Get full name
        ///// </summary>
        ///// <param name="account">Account</param>
        ///// <returns>Account full name</returns>
        //public static string GetFullName(this Account account)
        //{
        //    if (account == null)
        //        throw new ArgumentNullException("account");

        //    string fullName = "";
        //    if (!String.IsNullOrWhiteSpace(account.FirstName) && !String.IsNullOrWhiteSpace(account.LastName))
        //        fullName = String.Format("{0} {1}", account.FirstName, account.LastName);
        //    else
        //    {
        //        if (!String.IsNullOrWhiteSpace(account.FirstName))
        //            fullName = account.FirstName;

        //        if (!String.IsNullOrWhiteSpace(account.LastName))
        //            fullName = account.LastName;
        //    }
        //    return fullName;
        //}

        public static string FormatPhoneNumber(this Account account, string phoneNumber)
        {
            if (account == null)
                throw new ArgumentNullException("candidate");

            string result = string.Empty;
            if (!String.IsNullOrWhiteSpace(phoneNumber))
            {
                // clean up
                string tempStr = phoneNumber.Trim().Replace(" ", "").Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "");
                // format
                int length = tempStr.Length;
                // get last 10 digits
                if (length >= 10)
                {
                    result = string.Format("({0}) {1}-{2}", tempStr.Substring(length - 10, 3), tempStr.Substring(length - 7, 3), tempStr.Substring(length - 4, 4));
                }
            }

            return result;
        }

        /// <summary>
        /// Formats the account name
        /// </summary>
        /// <param name="account">Source</param>
        /// <param name="stripTooLong">Strip too long account name</param>
        /// <param name="maxLength">Maximum account name length</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this Account account, bool stripTooLong = false, int maxLength = 0)
        {
            if (account == null)
                return string.Empty;

            if (account.IsGuest())
            {
                return EngineContext.Current.Resolve<ILocalizationService>().GetResource("Account.Guest");
            }

            string result = string.Empty;
            switch (EngineContext.Current.Resolve<AccountSettings>().AccountNameFormat)
            {
                case AccountNameFormat.ShowEmails:
                    result = account.Email;
                    break;
                case AccountNameFormat.ShowUsernames:
                    result = account.Username;
                    break;
                case AccountNameFormat.ShowFullNames:
                    result = account.FullName; // account.GetFullName();
                    break;
                case AccountNameFormat.ShowFirstName:
                    result = account.GetAttribute<string>(SystemAccountAttributeNames.FirstName);
                    break;
                default:
                    break;
            }

            if (stripTooLong && maxLength > 0)
            {
                result = CommonHelper.EnsureMaximumLength(result, maxLength);
            }

            return result;
        }

    }
}
