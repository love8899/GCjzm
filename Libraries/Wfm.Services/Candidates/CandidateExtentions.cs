using System;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Infrastructure;

namespace Wfm.Services.Candidates
{
    public static class CandidateExtentions
    {
        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="candidate">Account</param>
        /// <returns>Candidate full name</returns>
        public static string GetFullName(this Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException("account");
            var firstName = candidate.FirstName;
            var lastName = candidate.LastName;

            string fullName = "";
            if (!String.IsNullOrWhiteSpace(firstName) && !String.IsNullOrWhiteSpace(lastName))
                fullName = string.Format("{0} {1}", firstName, lastName);
            else
            {
                if (!String.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!String.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }
            return fullName;
        }

        public static string FormatPhoneNumber(this Candidate candidate, string phoneNumber)
        {
            if (candidate == null)
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

        public static string FormatSocialInsuranceNumber(this Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            string result = string.Empty;
            if (!String.IsNullOrWhiteSpace(candidate.SocialInsuranceNumber))
            {
                // clean up
                string tempStr = candidate.SocialInsuranceNumber.Trim().Replace(" ", "").Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "");
                // format
                int length = tempStr.Length;
                // get last first 9 digits
                if (length >= 9)
                {
                    result = string.Format("{0} {1} {2}", tempStr.Substring(0, 3), tempStr.Substring(3, 3), tempStr.Substring(6, 3));
                }
            }

            return result;
        }

        /// <summary>
        /// Formats the candidate name
        /// </summary>
        /// <param name="candidate">Source</param>
        /// <param name="stripTooLong">Strip too long account name</param>
        /// <param name="maxLength">Maximum account name length</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this Candidate candidate, bool stripTooLong = false, int maxLength = 0)
        {
            if (candidate == null)
                return string.Empty;

            string result = string.Empty;
            switch (EngineContext.Current.Resolve<CandidateSettings>().CandidateNameFormat)
            {
                case CandidateNameFormat.ShowEmails:
                    result = candidate.Email;
                    break;
                case CandidateNameFormat.ShowUsernames:
                    result = candidate.Username;
                    break;
                case CandidateNameFormat.ShowFullNames:
                    result = candidate.GetFullName();
                    break;
                case CandidateNameFormat.ShowFirstName:
                    result = candidate.FirstName;
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
