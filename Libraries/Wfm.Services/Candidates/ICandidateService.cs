using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Employees;


namespace Wfm.Services.Candidates
{
    /// <summary>
    /// Candidate Service Interface
    /// </summary>
    public partial interface ICandidateService
    {
        #region CRUD

        /// <summary>
        /// Deletes the candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        void DeleteCandidate(Candidate candidate);

        /// <summary>
        /// Inserts the candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        void InsertCandidate(Candidate candidate);

        /// <summary>
        /// Updates the candiate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        void UpdateCandidate(Candidate candidate, Tuple<DateTime?, DateTime?> dates = null);

        void UpdateCandidate(Candidate candidate, string[] excludeList);

        void UpdateCandidateSearchKeys(Candidate candidate);

        void SetCandidateToBannedStatus(Wfm.Core.Domain.Candidates.Candidate candidate, string reason);

        void ResetCandidateFromBannedStatus(Wfm.Core.Domain.Candidates.Candidate candidate);

        void UpdateEmployeeNumbers(IEnumerable<Tuple<int, string>> idNumbers);

        #endregion

        #region Candidate

        /// <summary>
        /// Gets the candidate by id.
        /// </summary>
        /// <param name="id">The candidate id.</param>
        /// <returns>Candidate</returns>
        Candidate GetCandidateById(int id);

        /// <summary>
        /// Gets the candidate by id including regular employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Candidate</returns>
        Candidate GetCandidateByIdForClient(int id);
        /// <summary>
        /// Get the candidate by guid
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Candidate</returns>
        Candidate GetCandidateByGuid(Guid? guid);
      
        /// <summary>
        /// Get the candidate by guid including regular employee
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Candidate</returns>
        Candidate GetCandidateByGuidForClient(Guid? guid);
        /// <summary>
        /// Validates the candidate.
        /// </summary>
        /// <param name="username">username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        CandidateLoginResults AuthenticateCandidate(string username, string password,out TimeSpan time, out bool passwordIsExpired, out bool showPasswordExpiryWarning );

        Candidate GetCandidateByEmail(string email);

        Candidate GetCandidateByUsername(string username);

        IList<Candidate> GetCandidatesBySin(string sin);

        Candidate GetCandidateByVendorIdAndEmployeeId(int vendorId, string employeeId);

        /// <summary>
        /// Determines whether the specified candidate is duplicate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns></returns>
        bool IsDuplicate(Candidate candidate,out string duplicateCandidateIds);
        
        /// <summary>
        /// Determine whether the candidate is duplicate when updating 
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        bool IsDuplicateWhenUpdate(Candidate candidate);

       // if SIN is used
        bool IsSinUsed(string sinString);

        string RegisterCandidate(Candidate candidate, bool isEmployee, bool applyPasswordPolicy);
        void CleanUpCandidateData(Candidate candidate);

        void ActivateCandidate(Candidate candidate, bool isEmployee = false);

        string ResetPassword(string newPassword, string confirmPassword, string oldPassword, string userName);

        string ChangeSecurityQuestions(int question1Id, string answer1, int question2Id, string answer2, string userName);

        void SetSecurityQuestionInformation(Candidate candidate);

        bool ValidateSecurityQuestions(Candidate candidate, string answer1, string answer2);

        string SetEmployeeId(Candidate candidate);
        #endregion

        #region LIST

        /// <summary>
        /// Gets all candidates as queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="showInactive">if set to <c>true</c> [show active].</param>
        /// <param name="showBanned">if set to <c>true</c> [show banned].</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        IQueryable<Candidate> GetAllCandidatesAsQueryable(Account account = null, bool showInactive = false, bool showBanned = false, bool showHidden = false);

        /// <summary>
        /// Gets all candidates as queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="showInactive">if set to <c>true</c> [show active].</param>
        /// <param name="showBanned">if set to <c>true</c> [show banned].</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns>
        /// IQueryable<Candidate> records that IsEmployee = true
        /// No sorting os applied to query result.
        /// </returns>
        IQueryable<Candidate> GetAllEmployeesAsQueryable(Account account, bool showInactive = false, bool showBanned = false, bool showHidden = false);

        IQueryable<Candidate> GetAllCandidatesAsQueryableByJobOrderIdAndCityIdFilters(Account account, DateTime inquiryDate, int companyId, string jobOrderId, string cityId);

        #endregion

        #region Search Candidates

        IList<Candidate> SearchCandidates(string searchKey, int maxRecordsToReturn = 100, bool showInactive = false, bool showBanned = false, bool showHidden = false, bool employeeOnly = false);

        #endregion

        #region Statistics

        int GetTotalCandidateByDate(DateTime datetime);


        #endregion

        IQueryable<CandidateWithAddress> GetAllCandidatesWithAddressAsQueryable(Account account, bool showInactive = false, bool showBanned = false, bool showHidden = false,bool isSortingRequired=true);

        Candidate GetNewCandidateEntity();
        Guid? GetCandidateGuidByCandidateId(int id);
       


        #region validation for placement

        bool IsCandidateActive(Candidate candidate);

        bool IsCandidateBannedByCompanyAndDateRange(int candidateId, int companyId, DateTime startDate, DateTime? endDate);

        bool IsCanidateOnboarded(Candidate candidate);

        #endregion


        #region Employee Payroll Setting

        void InsertEmployeePayrollSetting(EmployeePayrollSetting employeePayrollSetting);

        void UpdateEmployeePayrollSetting(EmployeePayrollSetting employeePayrollSetting);

        void DeleteEmployeePayrollSetting(EmployeePayrollSetting employeePayrollSetting);

        EmployeePayrollSetting GetEmployeePayrollSettingById(int id);

        EmployeePayrollSetting GetEmployeePayrollSettingByEmployeeId(int employeeId, out int employeeTypeId);

        EmployeePayrollSetting GetEmployeePayrollSettingByCandidateGuid(Guid guid, out int employeeTypeId);

        #endregion


        #region Barcode

        string GetCandidateQrCodeStr(Candidate candidate);

        #endregion
    }
}
