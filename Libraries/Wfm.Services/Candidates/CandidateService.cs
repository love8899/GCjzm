using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Transactions;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Employees;
using Wfm.Data;
using Wfm.Services.Accounts;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.Helpers;
using Wfm.Services.Localization;
using Wfm.Services.Messages;
using Wfm.Services.Security;


namespace Wfm.Services.Candidates
{
    public partial class CandidateService : ICandidateService
    {

        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IGenericHelper _genericHelper;
        private readonly IRepository<Candidate> _candidateRepository;
        private readonly ICandidateBlacklistService _candidateBlacklistService;
        private readonly IEncryptionService _encryptionService;
        private readonly CandidateSettings _candidateSettings;
        private readonly ICityService _cityService;
        private readonly IRepository<CandidateAddress> _candidateAddressRepository;
        private readonly IRepository<CandidateKeySkill> _candidateKeySkillsRepository;
        private readonly IRepository<EmployeePayrollSetting> _employeePayrollSettingRepository;
        private readonly ITokenizer _tokenizer;
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly CommonSettings _commonSettings;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor

        public CandidateService(
            IPermissionService permissionService,
            IGenericHelper genericHelper,
            IRepository<Candidate> candidateRepository,
            ICandidateBlacklistService candidateBlacklistService,
            IEncryptionService encryptionService,
            CandidateSettings candidateSettings,
            ICityService cityService,
            IRepository<CandidateAddress> candidateAddress,
            IRepository<CandidateKeySkill> candidateKeySkillRepository,
            IRepository<EmployeePayrollSetting> employeeDateRepository,
            ITokenizer tokenizer,
            IDbContext dbContext,
            IWorkContext workContext,
            IAccountPasswordPolicyService accountPasswordPolicyService,
            CommonSettings commonSettings,
            ILocalizationService localizationService
            )
        {
            _permissionService = permissionService;
            _genericHelper = genericHelper;
            _candidateRepository = candidateRepository;
            _candidateBlacklistService = candidateBlacklistService;
            _encryptionService = encryptionService;
            _candidateSettings = candidateSettings;
            _cityService = cityService;
            _candidateAddressRepository = candidateAddress;
            _candidateKeySkillsRepository = candidateKeySkillRepository;
            _employeePayrollSettingRepository = employeeDateRepository;
            _tokenizer = tokenizer;
            _dbContext = dbContext;
            _workContext = workContext;
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _commonSettings = commonSettings;
            _localizationService = localizationService;
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Inserts a candidate
        /// </summary>
        /// <param name="candidate">Candidate</param>
        public void InsertCandidate(Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            //insert
            candidate.CreatedOnUtc = System.DateTime.UtcNow;
            candidate.UpdatedOnUtc = System.DateTime.UtcNow;

            CleanUpCandidateData(candidate);

            _candidateRepository.Insert(candidate);
        }

        /// <summary>
        /// Updates the candidate
        /// </summary>
        /// <param name="candidate">candidate</param>
        public void UpdateCandidate(Candidate candidate, Tuple<DateTime?, DateTime?> dates = null)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            List<string> excludeList = new List<string>();
            if (candidate.Password == null || candidate.Password.Equals("******"))
            {
                excludeList.Add("Password");
                excludeList.Add("LastPasswordUpdateDate");
            }
            if (candidate.SocialInsuranceNumber == null || candidate.SocialInsuranceNumber.Contains("*"))
                excludeList.Add("SocialInsuranceNumber");


            this.UpdateCandidate(candidate, excludeList.ToArray());

            if (dates != null)
            {
                var setting = _employeePayrollSettingRepository.Table.Where(x => x.EmployeeId == candidate.Id).OrderByDescending(x => x.LastHireDate).FirstOrDefault();
                if (setting == null)
                {
                    _employeePayrollSettingRepository.Insert(new EmployeePayrollSetting
                    {
                        EmployeeId = candidate.Id,
                        //Year = DateTime.Today.Year,
                        LastHireDate = dates.Item1,
                        TerminationDate = dates.Item2,
                    });
                }
                else
                {
                    setting.LastHireDate = dates.Item1;
                    setting.TerminationDate = dates.Item2;
                    _employeePayrollSettingRepository.Update(setting);
                }
            }
        }
        /// <summary>
        /// Updates the candidate record but will not updated the properties that are defined in the exclude list
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="excludeList"></param>
        public void UpdateCandidate(Candidate candidate, string[] excludeList)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            candidate.UpdatedOnUtc = DateTime.UtcNow;
            CleanUpCandidateData(candidate);
            _candidateRepository.Update(candidate, excludeList);
        }

        /// <summary>
        /// Deletes the candidate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <exception cref="System.ArgumentNullException">candidate</exception>
        public virtual void DeleteCandidate(Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException("candidate");

            candidate.IsDeleted = true;
            UpdateCandidate(candidate);
        }


        public void UpdateCandidateSearchKeys(Candidate candidate)
        {
            if (candidate == null)
                return;

            var skills = _candidateKeySkillsRepository.Table.Where(x => x.CandidateId == candidate.Id && !x.IsDeleted).ToList();
            var addresses = _candidateAddressRepository.Table.Where(x => x.CandidateId == candidate.Id && x.IsActive && !x.IsDeleted).ToList();
            candidate.SearchKeys = _GetCandidateSearchKeyWords(skills, addresses);
            UpdateCandidate(candidate);
        }


        private string _GetCandidateSearchKeyWords(IList<CandidateKeySkill> skills, IList<CandidateAddress> addresses)
        {
            IList<String> keyWords = new List<String>();

            foreach (var item in skills)
            {
                keyWords.Add(item.KeySkill);
            }

            foreach (var item in addresses)
            {
                var city = _cityService.GetCityById(item.CityId);
                if (city != null)
                    keyWords.Add(city.CityName);
            }

            return string.Join(", ", keyWords);
        }


        public void SetCandidateToBannedStatus(Wfm.Core.Domain.Candidates.Candidate candidate, string reason)
        {
            candidate.IsBanned = true;
            candidate.BannedReason = reason;

            candidate.IsHot = false;
            candidate.IsActive = false;
            candidate.InactiveReason = "Other";


            UpdateCandidate(candidate);
        }


        public void ResetCandidateFromBannedStatus(Wfm.Core.Domain.Candidates.Candidate candidate)
        {
            candidate.IsBanned = false;
            candidate.BannedReason = null;

            candidate.IsActive = true;
            candidate.InactiveReason = null;

            UpdateCandidate(candidate);
        }


        public void UpdateEmployeeNumbers(IEnumerable<Tuple<int, string>> idNumbers)
        {
            // TODO: use batching if the number of rows > 1000 (maximum number of rows in a single statement)
            var candidateList = String.Join(",", idNumbers.Select(x => string.Format("({0},'{1}')", x.Item1, x.Item2)));
            var cmd = String.Format(@"update c set EmployeeId = n.value, UpdatedOnUtc = GETUTCDATE() from Candidate as c
                                      join (values {0}) as n(id, value) on n.id = c.Id
                                      where c.EmployeeId != n.value",
                                    candidateList);
            _dbContext.ExecuteSqlCommand(cmd);
        }

        #endregion

        #region Candidate

        /// <summary>
        /// Gets Candidate
        /// </summary>
        /// <param name="id">Candidate identifier</param>
        /// <returns>Candidate</returns>
        public Candidate GetCandidateById(int id)
        {
            var candidate = _candidateRepository.GetById(id);
            if (!_commonSettings.DisplayVendor || candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG)
                return candidate;
            else
                return null;
        }

        /// <summary>
        /// Gets the candidate by id including regular employee
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return candidate including regular employees</returns>
        public Candidate GetCandidateByIdForClient(int id)
        {
            var candidate = _candidateRepository.GetById(id);           
            return candidate;
        }


        /// <summary>
        /// Get the candidate by guid
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Candidate</returns>
        public Candidate GetCandidateByGuid(Guid? guid)
        {
            if (guid == null)
                return null;
            var candidate = _candidateRepository.Table.Where(x => x.CandidateGuid == guid).FirstOrDefault();
            
            if (!_commonSettings.DisplayVendor || candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG || _workContext.CurrentAccount.IsPayrollAdministrator())
                return candidate;
            else
                return null;
        }

        /// <summary>
        /// Get the candidate by guid including regular employee
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Candidate</returns>
        public Candidate GetCandidateByGuidForClient(Guid? guid)
        {
            if (guid == null)
                return null;
            return _candidateRepository.TableNoTracking.Where(x => x.CandidateGuid == guid).FirstOrDefault();
        }

        public CandidateLoginResults AuthenticateCandidate(string username, string password, out TimeSpan time, out bool passwordIsExpired, out bool showPasswordExpiryWarning)
        {
            time = new TimeSpan();
            passwordIsExpired = false;
            showPasswordExpiryWarning = false;

            if (String.IsNullOrWhiteSpace(username)) return CandidateLoginResults.CandidateNotExist;

            Candidate candidate = GetCandidateByUsername(username);

            if (candidate == null) return CandidateLoginResults.CandidateNotExist;
            if (!candidate.IsActive) return CandidateLoginResults.NotActive;
            if (candidate.IsDeleted) return CandidateLoginResults.Deleted;

            string pwd = _encryptionService.ConvertPassword(candidate.PasswordFormat, password, candidate.PasswordSalt);
            bool isValid = candidate.Password == pwd;

            if (isValid)
            {
                var passpolicy = _accountPasswordPolicyService.Retrieve("Candidate").PasswordPolicy;

                if (passpolicy.PasswordLifeTime != 0)
                {
                    if (candidate.LastPasswordUpdateDate.AddDays(passpolicy.PasswordLifeTime) <= DateTime.UtcNow)
                    {
                        passwordIsExpired = true;
                    }
                    else if (candidate.LastPasswordUpdateDate.AddDays(passpolicy.PasswordLifeTime - 10) < DateTime.UtcNow)
                    {
                        time = candidate.LastPasswordUpdateDate.AddDays(passpolicy.PasswordLifeTime) - DateTime.UtcNow;
                        showPasswordExpiryWarning = true;
                    }
                }

                return CandidateLoginResults.Successful;
            }
            else 
                return CandidateLoginResults.WrongPassword;

        }

        /// <summary>
        /// Gets  Candidate By Email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>Candidate</returns>
        public Candidate GetCandidateByEmail(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
                return null;

            var query = _candidateRepository.Table;
            //query = query.Where(c => c.IsDeleted == false);
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            query = from c in query
                    where c.Email == email
                    select c;

            return query.FirstOrDefault();
        }


        public Candidate GetCandidateByUsername(string username)
        {
            if (String.IsNullOrWhiteSpace(username))
                return null;

            var query = _candidateRepository.Table;
            //query = query.Where(c => c.IsDeleted == false);
            query = from c in query
                    where c.Username == username 
                    select c;

            return query.FirstOrDefault();
        }


        public IList<Candidate> GetCandidatesBySin(string sin)
        {
            if (!CommonHelper.IsValidCanadianSin(sin))
                return null;

            sin = sin.Trim();

            var query = _candidateRepository.TableNoTracking;
            if (_commonSettings.DisplayVendor)
                query = query.Where(c => c.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            query = from c in query
                    where c.SocialInsuranceNumber == sin 
                    select c;

            return query.ToList();
        }


        public Candidate GetCandidateByVendorIdAndEmployeeId(int vendorId, string employeeId)
        {
            if (vendorId <= 0 || String.IsNullOrWhiteSpace(employeeId))
                return null;
            var candidate = _candidateRepository.Table;
            if(_commonSettings.DisplayVendor)
                candidate = candidate.Where(x=>x.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            var query =candidate.Where(x => x.FranchiseId == vendorId &&
                                                         x.EmployeeId.ToLower() == employeeId.Trim().ToLower());

            return query.FirstOrDefault();
        }


        /// <summary>
        /// Determines whether the specified candidate is duplicate.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns></returns>
        public bool IsDuplicate(Candidate candidate, out string duplicateCandidateIds)
        {
            duplicateCandidateIds = string.Empty;
            var query = _candidateRepository.TableNoTracking;

            // duplication check:
            // -------------------------------------
            // 1) email
            // 2) name + home phone
            // 3) name + SocialInsuranceNumber
            // 4) Name + birth date
            // 5) userName

            query = from c in query
                    where candidate.Username == c.Username ||
                    (candidate.Email != null && c.Email == candidate.Email) ||
                    (c.FirstName == candidate.FirstName && c.LastName == candidate.LastName && c.HomePhone == candidate.HomePhone) ||
                    (c.FirstName == candidate.FirstName && c.LastName == candidate.LastName && c.SocialInsuranceNumber != null && c.SocialInsuranceNumber == candidate.SocialInsuranceNumber) ||
                    (c.FirstName == candidate.FirstName && c.LastName == candidate.LastName && c.BirthDate.HasValue && c.BirthDate == candidate.BirthDate)
                    select c;
            if (query.Any())
                duplicateCandidateIds = String.Join(",", query.Select(x => x.Id));
            // TO DO: other validation
            return query.Any();
        }

        /// <summary>
        /// Determines whether the specified candidate is duplicate.
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public bool IsDuplicateWhenUpdate(Candidate candidate)
        {
            var query = _candidateRepository.TableNoTracking;

            // duplication check:
            // -------------------------------------
            // 1) email
            // 2) name + home phone
            // 3) name + SocialInsuranceNumber
            // 4) Name + birth date
            // 5) Franchise + EmployeeID

            query = from c in query
                    where candidate.Username == c.Username ||
                    (candidate.Email != null && c.Email == candidate.Email) ||
                    (c.FirstName == candidate.FirstName && c.LastName == candidate.LastName && c.HomePhone == candidate.HomePhone) ||
                    (c.FirstName == candidate.FirstName && c.LastName == candidate.LastName && c.SocialInsuranceNumber != null && c.SocialInsuranceNumber == candidate.SocialInsuranceNumber) ||
                    (c.FirstName == candidate.FirstName && c.LastName == candidate.LastName && c.BirthDate.HasValue && c.BirthDate == candidate.BirthDate) ||
                    (c.FranchiseId == candidate.FranchiseId && c.EmployeeId != null && c.EmployeeId == candidate.EmployeeId)
                    select c;
            return query.Count() > 1;
        }


        // if SIN is used
        public bool IsSinUsed(string sinString)
        {
            if (String.IsNullOrWhiteSpace(sinString)) return false;

            var query = _candidateRepository.TableNoTracking;

            query = from c in query
                    where c.SocialInsuranceNumber == sinString.Trim()
                    select c;

            return query.Any();
        }


        public string RegisterCandidate(Candidate candidate, bool isEmployee, bool applyPasswordPolicy)
        {
            if (candidate == null) throw new ArgumentNullException("candidate");

            // Use the hashed password for all new registerations
            var _newPassword = candidate.Password;
            candidate.Password = null;

            var result = this.GenerateNewPassword(new ChangePasswordRequest()
            {
                Candidate = candidate,
                NewPassword = _newPassword,
                NewPasswordFormat = _candidateSettings.DefaultPasswordFormat,
                ValidateOldPassword = false,
                ApplyPasswordPolicy = applyPasswordPolicy
            });

            if (!result.IsSuccess)
                return result.ErrorsAsString();

            candidate.SecurityAnswerFormatId =(int)PasswordFormat.Hashed;
            SetSecurityQuestionInformation(candidate);

            candidate.CandidateOnboardingStatusId = (int)CandidateOnboardingStatusEnum.NoStatus;
            candidate.IsEmployee = isEmployee;
            candidate.IsDeleted = false;
            candidate.IsBanned = false;

            using (var scope = new TransactionScope())
            {
                this.InsertCandidate(candidate);

                if (String.IsNullOrWhiteSpace(candidate.EmployeeId))
                {
                    var msg = SetEmployeeId(candidate);
                    if (!String.IsNullOrWhiteSpace(msg))
                        return msg;
                }
                scope.Complete();
            }
            return String.Empty;
        }


        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public  PasswordChangeResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = this.GenerateNewPassword(request);

            if (result.IsSuccess)
            {
                request.Candidate.FailedSecurityQuestionAttempts = 0;
                this.UpdateCandidate(request.Candidate);
            }

            return result;
        }

        private PasswordChangeResult GenerateNewPassword(ChangePasswordRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (request.Candidate == null) throw new ArgumentNullException("request.candidate");

            var result = new PasswordChangeResult();

            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(_localizationService.GetResource("Candidate.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            if (request.ValidateOldPassword)
            {
                //password
                string oldPwd = _encryptionService.ConvertPassword(request.Candidate.PasswordFormat, request.OldPassword, request.Candidate.PasswordSalt);

                bool oldPasswordIsValid = oldPwd == request.Candidate.Password;
                if (!oldPasswordIsValid)
                {
                    result.AddError(_localizationService.GetResource("account.resetpassword.fields.oldpassword.incorrect"));
                    return result;
                }
            }

            if (request.ApplyPasswordPolicy)
            {
                StringBuilder errors;
                if (!_accountPasswordPolicyService.ApplyPasswordPolicy( request.Candidate.Id, "Candidate", request.NewPassword, request.Candidate.Password, request.Candidate.PasswordFormat, request.Candidate.PasswordSalt, out errors))
                {
                    result.AddError(errors.ToString()); 
                    return result;
                }
            }

            // New password shouldn't be same as the current password
            var _pwd = _encryptionService.ConvertPassword(request.Candidate.PasswordFormat, request.NewPassword, request.Candidate.PasswordSalt);
            if (_pwd == request.Candidate.Password)
            {
                result.AddError(_localizationService.GetResource("Common.PasswordIsUsed"));
                return result;
            }

            //at this point request is valid
            request.Candidate.PasswordFormat = request.NewPasswordFormat;
            request.Candidate.PasswordSalt = _encryptionService.CreateSaltKey(5);
            request.Candidate.PasswordFormat = PasswordFormat.Hashed;
            request.Candidate.LastPasswordUpdateDate = DateTime.UtcNow;
            request.Candidate.Password = _encryptionService.ConvertPassword(request.NewPasswordFormat, request.NewPassword, request.Candidate.PasswordSalt);

            return result;
        }

        public void SetSecurityQuestionInformation(Candidate candidate)
        {
            string saltKey = _encryptionService.CreateSaltKey(5);
            candidate.SecurityQuestionSalt = saltKey;

            if (!candidate.SecurityQuestion1Id.HasValue || candidate.SecurityQuestion1Id <= 0)
                candidate.SecurityQuestion1Id = 1;


            if (!string.IsNullOrWhiteSpace(candidate.SecurityQuestion1Answer))
            {
                string cleanedString = System.Text.RegularExpressions.Regex.Replace(candidate.SecurityQuestion1Answer, @"\s+", "");
                candidate.SecurityQuestion1Answer = cleanedString.ToLower();
            }

            if (!candidate.SecurityQuestion2Id.HasValue || candidate.SecurityQuestion2Id <= 0)
                candidate.SecurityQuestion2Id = 1;

            if (!string.IsNullOrWhiteSpace(candidate.SecurityQuestion2Answer))
            {
                string cleanedString = System.Text.RegularExpressions.Regex.Replace(candidate.SecurityQuestion2Answer, @"\s+", "");
                candidate.SecurityQuestion2Answer = cleanedString.ToLower();
            }
            if (candidate.SecurityAnswerFormatId == (int)PasswordFormat.Hashed)
            {
                if (!string.IsNullOrWhiteSpace(candidate.SecurityQuestion1Answer))
                {
                    candidate.SecurityQuestion1Answer = _encryptionService.CreatePasswordHash(candidate.SecurityQuestion1Answer, saltKey, _candidateSettings.HashedPasswordFormat);
                }
                if (!string.IsNullOrWhiteSpace(candidate.SecurityQuestion2Answer))
                {
                    candidate.SecurityQuestion2Answer = _encryptionService.CreatePasswordHash(candidate.SecurityQuestion2Answer, saltKey, _candidateSettings.HashedPasswordFormat);
                }
            }
            else if (candidate.SecurityAnswerFormatId == (int)PasswordFormat.Encrypted)
            {
                if (!string.IsNullOrWhiteSpace(candidate.SecurityQuestion1Answer))
                {
                    candidate.SecurityQuestion1Answer = _encryptionService.EncryptText(candidate.SecurityQuestion1Answer);
                }
                if (!string.IsNullOrWhiteSpace(candidate.SecurityQuestion2Answer))
                {
                    candidate.SecurityQuestion1Answer = _encryptionService.EncryptText(candidate.SecurityQuestion2Answer);
                }
            }
        }


        public string SetEmployeeId(Candidate candidate)
        {
            if (candidate == null || candidate.Id == 0)
                throw new ArgumentNullException("candidate");

            var result = String.Empty;

            var employeeId = _candidateSettings.EmployeeIdTokens;
            var tokens = new List<Token>();
            this._AddEmployeeIdTokens(tokens, candidate);
            candidate.EmployeeId = _tokenizer.Replace(employeeId, tokens, htmlEncode: false);

            var anyDuplicate = this.GetCandidateByVendorIdAndEmployeeId(candidate.FranchiseId, candidate.EmployeeId);
            if (anyDuplicate == null)
                this.UpdateCandidate(candidate);
            else
                result = String.Format("The EmployeeId [{0}] exists already!", employeeId);

            return result;
        }


        private void _AddEmployeeIdTokens(IList<Token> tokens, Candidate candidate)
        {
            tokens.Add(new Token("FirstName", String.IsNullOrWhiteSpace(candidate.FirstName) ? "*" : candidate.FirstName));
            tokens.Add(new Token("LastName", String.IsNullOrWhiteSpace(candidate.LastName) ? "*" : candidate.LastName));
            tokens.Add(new Token("Id", candidate.Id.ToString("D8")));
            var sin = candidate.SocialInsuranceNumber;
            tokens.Add(new Token("SocialInsuranceNumber", String.IsNullOrWhiteSpace(sin) ? "xxx" : sin.Substring(sin.Length - 3)));
        }


        public void CleanUpCandidateData(Candidate candidate)
        {
            // clean up
            if (!String.IsNullOrWhiteSpace(candidate.Email))
            {
                candidate.Email = candidate.Email.Trim();
            }
            candidate.Username = candidate.Username.Trim();

            //else
            //    candidate.Username = //todo: unique random string

            candidate.FirstName = CommonHelper.ToPrettyName(candidate.FirstName);
            candidate.MiddleName = CommonHelper.ToPrettyName(candidate.MiddleName);
            candidate.LastName = CommonHelper.ToPrettyName(candidate.LastName);

            candidate.HomePhone = CommonHelper.ExtractNumericText(candidate.HomePhone);
            candidate.MobilePhone = CommonHelper.ExtractNumericText(candidate.MobilePhone);
            candidate.EmergencyPhone = CommonHelper.ExtractNumericText(candidate.EmergencyPhone);
            candidate.SocialInsuranceNumber = CommonHelper.ExtractNumericText(candidate.SocialInsuranceNumber);
        }

        public void ActivateCandidate(Candidate candidate, bool isEmployee = false)
        {
            if (candidate != null && !candidate.IsDeleted)
            {
                candidate.IsActive = true;
                candidate.IsEmployee = isEmployee;
                this.UpdateCandidate(candidate);
            }
        }

        public string ResetPassword(string newPassword, string confirmPassword, string oldPassword, string userName)
        {
            Candidate candidate = this.GetCandidateByUsername(userName);
            if (candidate == null)
                throw new  ArgumentException("Invalid user name");

            newPassword = newPassword.Trim();
            confirmPassword = confirmPassword.Trim();
            if (oldPassword != null)
                oldPassword = oldPassword.Trim();

            if (newPassword != confirmPassword)
                return _localizationService.GetResource("Common.EnteredPasswordsDoNotMatch");

            var request = new ChangePasswordRequest()
                                {
                                    Candidate = candidate,
                                    OldPassword = oldPassword,
                                    NewPassword = newPassword,
                                    NewPasswordFormat = _candidateSettings.DefaultPasswordFormat,
                                    ValidateOldPassword = !String.IsNullOrWhiteSpace(oldPassword),
                                    ApplyPasswordPolicy = true
                                };

            var result = this.ChangePassword(request);

            return result.ErrorsAsString();
        }


        public string ChangeSecurityQuestions(int question1Id, string answer1, int question2Id, string answer2, string userName)
        {
            Candidate candidate = this.GetCandidateByUsername(userName);
            if (candidate == null)
                return null;

            candidate.SecurityQuestion1Id = question1Id;
            candidate.SecurityQuestion2Id = question2Id;
            candidate.SecurityQuestion1Answer = answer1;
            candidate.SecurityQuestion2Answer = answer2;
            candidate.UpdatedOnUtc = System.DateTime.UtcNow;

            SetSecurityQuestionInformation(candidate);
            _candidateRepository.Update(candidate);
            return String.Empty;
        }
        public bool ValidateSecurityQuestions(Candidate candidate, string answer1, string answer2)
        {
            if (!String.IsNullOrWhiteSpace(answer1))
                answer1 = System.Text.RegularExpressions.Regex.Replace(answer1, @"\s+", "").ToLower();
            if (!String.IsNullOrWhiteSpace(answer2))
                answer2 = System.Text.RegularExpressions.Regex.Replace(answer2, @"\s+", "").ToLower();

            if (candidate.SecurityAnswerFormatId == (int)PasswordFormat.Hashed)
            {
                answer1 = _encryptionService.CreatePasswordHash(answer1, candidate.SecurityQuestionSalt, _candidateSettings.HashedPasswordFormat);
                answer2 = _encryptionService.CreatePasswordHash(answer2, candidate.SecurityQuestionSalt, _candidateSettings.HashedPasswordFormat);
            }
            else if (candidate.SecurityAnswerFormatId == (int)PasswordFormat.Encrypted)
            {
                answer1 = _encryptionService.EncryptText(answer1);
                answer2 = _encryptionService.EncryptText(answer2);
            }

            if (answer1 == candidate.SecurityQuestion1Answer && answer2 == candidate.SecurityQuestion2Answer)
            {
                return true;
            }
            else
            {
                candidate.FailedSecurityQuestionAttempts = candidate.FailedSecurityQuestionAttempts + 1;
                candidate.UpdatedOnUtc = System.DateTime.UtcNow;
                _candidateRepository.Update(candidate);
            }
            return false;
        }

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
        public IQueryable<Candidate> GetAllCandidatesAsQueryable(Account account = null, bool showInactive = false, bool showBanned = false, bool showHidden = false)
        {
            var query = this.getDefaultCandidateQuery(showInactive, showBanned, showHidden);

            if (account != null && !account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(c => c.FranchiseId == account.FranchiseId);

            query = query.OrderByDescending(x => x.UpdatedOnUtc)
                .Include(x => x.CandidateAddresses);

            return query.AsQueryable();
        }

        public IQueryable<Candidate> GetAllCandidatesAsQueryableByJobOrderIdAndCityIdFilters(Account account, DateTime inquiryDate, int companyId, string jobOrderId, string cityId)
        {
            var query = _candidateRepository.TableNoTracking;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            query = query.Where(x => x.IsActive && !x.IsBanned && !x.IsDeleted && x.FranchiseId == account.FranchiseId &&
                               (x.IsEmployee || x.CandidateOnboardingStatusId == 2) &&
                // no active job placement
                                x.SmartCards.Any(y=>y.IsActive&&!y.IsDeleted)&&
                                !x.CandidateJobOrders.Any(y => y.CandidateJobOrderStatusId == (int)(CandidateJobOrderStatusEnum.Placed) &&
                                (y.StartDate <= inquiryDate && (y.EndDate == null || y.EndDate >= inquiryDate))) && !x.UseForDirectPlacement)
                .OrderByDescending(x => x.UpdatedOnUtc)
                .Include(x => x.CandidateAddresses);

            // excluding those banned by company
            var bannedByCompany = _candidateBlacklistService.GetAllCandidateBlacklistsByDate(inquiryDate)
                                  .Where(x => !x.ClientId.HasValue || x.ClientId == companyId).Select(x => x.CandidateId);
            query = query.Where(x => !bannedByCompany.Contains(x.Id));

            if (!string.IsNullOrEmpty(cityId))
                query = query.Where(x => x.CandidateAddresses.Any(o => o.AddressTypeId == (int)AddressTypeEnum.Residential && o.CityId.ToString().Contains(cityId)));

            if (!string.IsNullOrEmpty(jobOrderId))
                query = query.Where(x => x.CandidateJobOrders.Any(o => (!o.EndDate.HasValue || o.EndDate > inquiryDate) && (o.JobOrderId.ToString().Contains(jobOrderId))));

            return query.AsQueryable();
        }

        public IQueryable<CandidateWithAddress> GetAllCandidatesWithAddressAsQueryable(Account account, bool showInactive = false, bool showBanned = false, bool showHidden = false, bool isSortingRequired = true)
        {
            var query = this.getDefaultCandidateQuery(showInactive, showBanned, showHidden);

            // IsLimitedToFranchises
            if (account.IsLimitedToFranchises)
                query = query.Where(c => c.FranchiseId == account.FranchiseId);

            var result =
                     from c in query
                     // where !regIds.Contains(c.Id)
                     from ca in _candidateAddressRepository.TableNoTracking.Where(o => c.Id == o.CandidateId && c.IsDeleted == false
                                                                       && o.AddressTypeId == (int)AddressTypeEnum.Residential
                                                                       ).Take(1)
                                                                       .DefaultIfEmpty()
                     orderby c.UpdatedOnUtc descending
                     select new CandidateWithAddress()
                     {
                         Id = c.Id,
                         Guid = c.CandidateGuid,
                         EmployeeId = c.EmployeeId,
                         FirstName = c.FirstName,
                         MiddleName = c.MiddleName,
                         LastName = c.LastName,
                         GenderId = c.GenderId,
                         BirthDate = c.BirthDate,
                         Email = c.Email,
                         Email2 = c.Email2,
                         SearchKeys = c.SearchKeys,
                         Note = c.Note,
                         HomePhone = c.HomePhone,
                         MobilePhone = c.MobilePhone,
                         EmergencyPhone = c.EmergencyPhone,
                         SocialInsuranceNumber = c.SocialInsuranceNumber,
                         CityId = ca.CityId,
                         StateProvinceId = ca.StateProvinceId,
                         MajorIntersection1 = c.MajorIntersection1,
                         MajorIntersection2 = c.MajorIntersection2,
                         PreferredWorkLocation = c.PreferredWorkLocation,
                         ShiftId = c.ShiftId,
                         TransportationId = c.TransportationId,
                         LicencePlate = c.LicencePlate,
                         FranchiseId = c.FranchiseId,
                         UpdatedOnUtc = c.UpdatedOnUtc,
                         CreatedOnUtc = c.CreatedOnUtc,
                         IsActive = c.IsActive,
                         IsHot = c.IsHot,
                         IsBanned = c.IsBanned,
                         IsEmployee = c.IsEmployee,
                         CandidateOnboardingStatusId = c.CandidateOnboardingStatusId,
                         SalutationId = c.SalutationId,
                         UseForDirectPlacement = c.UseForDirectPlacement,
                         OnBoarded = c.IsEmployee == true ? true :
                                         (c.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started || c.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Finished)
                                  ? true : false
                     };

            if (isSortingRequired)
                result = result.OrderByDescending(x => x.UpdatedOnUtc);

            return result;
        }

        public IQueryable<Candidate> GetAllEmployeesAsQueryable(Account account, bool showInactive = false, bool showBanned = false, bool showHidden = false)
        {
            var query = _candidateRepository.Table;

            query = query.Where(c => c.IsEmployee == true);

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);

            // banned
            if (!showBanned)
                query = query.Where(c => c.IsBanned == false);

            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            if (!account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(c => c.FranchiseId == account.FranchiseId);

            return query.AsQueryable();
        }
        #endregion

        #region Candidate Search

        public IList<Candidate> SearchCandidates(string searchKey, int maxRecordsToReturn = 100, bool showInactive = false, bool showBanned = false, bool showHidden = false, bool employeeOnly = false)
        {
            if (String.IsNullOrWhiteSpace(searchKey))
            {
                IList<Candidate> candidates = new List<Candidate>();
                return candidates;
            }

            var query = this.getDefaultCandidateQuery(showInactive, showBanned, showHidden, employeeOnly);

            var account = _workContext.CurrentAccount;
            if (!account.IsClientAccount && account.IsLimitedToFranchises)
                query = query.Where(x => x.FranchiseId == account.FranchiseId);

            // ************************************************
            // Analyze the search key
            // ************************************************
            bool isNum = _genericHelper.IsSearchableDigits(searchKey);
            string sKeyWord = _genericHelper.ToSearchableString(searchKey);
            // End of analysis
            // ************************************************

            if (isNum)
            {
                query = (from c in query
                         where c.Id.ToString() == sKeyWord
                         || c.EmployeeId.Contains(sKeyWord)
                         || c.HomePhone.Contains(sKeyWord)
                         || c.MobilePhone.Contains(sKeyWord)
                             //|| c.EmergencyPhone.Contains(sKeyWord)
                         || c.SocialInsuranceNumber.Contains(sKeyWord)
                         select c);
            }
            else
            {
                query = (from c in query
                         where c.Email.Contains(sKeyWord)
                         || c.Username.Contains(sKeyWord)
                         || c.EmployeeId.Contains(sKeyWord)
                         || c.LastName.Contains(sKeyWord)
                         || c.FirstName.Contains(sKeyWord)
                         || (c.FirstName + " " + c.LastName).Contains(sKeyWord)
                         || (c.LastName + " " + c.FirstName).Contains(sKeyWord)
                         || c.Note.Contains(sKeyWord)
                         //|| c.SearchKeys.Contains(sKeyWord)
                         select c);
            }

            query = query.OrderByDescending(c => c.UpdatedOnUtc);

            if (maxRecordsToReturn < 1 || maxRecordsToReturn > 500)
                maxRecordsToReturn = 500;

            return query.Take(maxRecordsToReturn).ToList();
        }

        private IQueryable<Candidate> getDefaultCandidateQuery(bool showInactive = false, bool showBanned = false, bool showHidden = false, bool employeeOnly = false)
        {
            var query = _candidateRepository.TableNoTracking;

            if (employeeOnly)
                query = query.Where(x => x.IsEmployee);
            else if (_commonSettings.DisplayVendor)
                query = query.Where(c => c.EmployeeTypeId != (int)EmployeeTypeEnum.REG);

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // banned
            if (!showBanned)
                query = query.Where(c => c.IsBanned == false);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            return query;
        }
        #endregion

        #region Statistics
        public int GetTotalCandidateByDate(DateTime datetime)
        {
            var query = _candidateRepository.TableNoTracking;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            int total = (from c in query
                         where c.CreatedOnUtc.Value.Month == datetime.Month
                         select c).Count();

            return total;
        }
        #endregion


        public Candidate GetNewCandidateEntity()
        {
            return new Candidate()
            {
                SalutationId = (int)SalutationEnum.Mr,
                DisabilityStatus = false,
                IsHot = false,
                IsBanned = false,
                IsDeleted = false,
                Entitled = true,
                CanRelocate = false,
                LanguageId = 1,
                ShiftId = (int)ShiftEnum.Any,
                CandidateOnboardingStatusId = (int)CandidateOnboardingStatusEnum.NoStatus,
                IsEmployee = false
            };
        }
        public Guid? GetCandidateGuidByCandidateId(int id)
        {
            Candidate entity = _candidateRepository.TableNoTracking.Where(x => x.Id == id).FirstOrDefault();
            if (entity == null)
                return null;
            else
                return entity.CandidateGuid;
        }


        #region validation for placement

        public bool IsCandidateActive(Candidate candidate)
        {
            return candidate.IsActive && !candidate.IsBanned && !candidate.IsDeleted;
        }


        public bool IsCandidateBannedByCompanyAndDateRange(int candidateId, int companyId, DateTime startDate, DateTime? endDate)
        {
            var query = _candidateBlacklistService.GetAllCandidateBlacklistsByCandidateId(candidateId).Where(x => x.ClientId == companyId);

            query = query.Where(x => x.EffectiveDate <= startDate || !endDate.HasValue || endDate >= x.EffectiveDate);

            return query.Any();
        }


        public bool IsCanidateOnboarded(Candidate candidate)
        {
            return candidate.IsEmployee ||
                   candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started ||
                   candidate.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Finished;
        }

        #endregion


        #region Employee Payroll Setting

        public void InsertEmployeePayrollSetting(EmployeePayrollSetting employeePayrollSetting)
        {
            if (employeePayrollSetting == null)
                throw new ArgumentNullException("employeePayrollSetting");

            _employeePayrollSettingRepository.Insert(employeePayrollSetting);
        }


        public void UpdateEmployeePayrollSetting(EmployeePayrollSetting employeePayrollSetting)
        {
            if (employeePayrollSetting == null)
                throw new ArgumentNullException("employeePayrollSetting");
            
            _employeePayrollSettingRepository.Update(employeePayrollSetting);
        }


        public void DeleteEmployeePayrollSetting(EmployeePayrollSetting employeePayrollSetting)
        {
            if (employeePayrollSetting == null)
                throw new ArgumentNullException("employeePayrollSetting");
            
            _employeePayrollSettingRepository.Delete(employeePayrollSetting);
        }


        public EmployeePayrollSetting GetEmployeePayrollSettingById(int id)
        {
            return _employeePayrollSettingRepository.GetById(id);
        }


        public EmployeePayrollSetting GetEmployeePayrollSettingByEmployeeId(int employeeId, out int employeeTypeId)
        {
            var candidate = _candidateRepository.GetById(employeeId);
            employeeTypeId = candidate != null && candidate.EmployeeTypeId.HasValue ? candidate.EmployeeTypeId.Value : 0;

            var data = _employeePayrollSettingRepository.Table.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
            if (data != null && candidate != null)
                data.FranchiseId = candidate.FranchiseId;

            return data;
        }


        public EmployeePayrollSetting GetEmployeePayrollSettingByCandidateGuid(Guid guid, out int employeeTypeId)
        {
            var candidate = _candidateRepository.TableNoTracking.FirstOrDefault(x => x.CandidateGuid == guid);
            employeeTypeId = candidate != null && candidate.EmployeeTypeId.HasValue ? candidate.EmployeeTypeId.Value : 0;

            var data = _employeePayrollSettingRepository.Table.FirstOrDefault(x => x.EmployeeId == candidate.Id);
            if (data != null && candidate != null)
                data.FranchiseId = candidate.FranchiseId;

            return data;
        }

        #endregion


        #region Barcode

        public string GetCandidateQrCodeStr(Candidate candidate)
        {
            return String.Format("{0};{1}", candidate.Id, candidate.CandidateGuid);
        }

        #endregion

    }
}
