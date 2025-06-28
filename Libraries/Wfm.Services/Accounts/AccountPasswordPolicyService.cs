using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Security;

namespace Wfm.Services.Accounts
{
    public class AccountPasswordPolicyService : IAccountPasswordPolicyService
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly IRepository<AccountPasswordPolicy> _accountPasswordPolicyRepository;

        private readonly IRepository<AccountPasswordHistory> _accountPasswordHistoryRepository;
        private readonly IRepository<CandidatePasswordHistory> _candidatePasswordHistoryRepository;

        private readonly IRepository<LocaleStringResource> _lsrRepository;
        private readonly IEncryptionService _encryptionService;
        #endregion

        #region Ctor
        public AccountPasswordPolicyService(ILogger logger, 
                                            IRepository<AccountPasswordPolicy> accountPasswordPolicyRepository,
                                            IRepository<AccountPasswordHistory> accountPasswordHistoryRepository,
                                            IRepository<CandidatePasswordHistory> candidatePasswordHistoryRepository,
                                            IRepository<LocaleStringResource> lsrRepository,
                                            IEncryptionService encryptionService)
        {
            this._logger = logger;
            this._accountPasswordPolicyRepository = accountPasswordPolicyRepository;

            this._accountPasswordHistoryRepository = accountPasswordHistoryRepository;
            this._candidatePasswordHistoryRepository = candidatePasswordHistoryRepository;

            this._lsrRepository = lsrRepository;
            this._encryptionService = encryptionService;
        }
        #endregion

        #region CRUD
        public void Create(AccountPasswordPolicy entity)
        {
            if (entity == null)
            {
                _logger.Error("Entity of AccountPasswordPolicy is null!");
                return;
            }
            _accountPasswordPolicyRepository.Insert(entity);
        }

        public AccountPasswordPolicy Retrieve(string type)
        {
            return _accountPasswordPolicyRepository.Table.Where(x => x.AccountType == type).FirstOrDefault();
        }

        public void Update(AccountPasswordPolicy entity)
        {
            if (entity == null)
            {
                _logger.Error("Entity of AccountPasswordPolicy is null!");
                return;
            }
            _accountPasswordPolicyRepository.Update(entity);
        }

        public void Delete(AccountPasswordPolicy entity)
        {
            if (entity == null)
            {
                _logger.Error("Entity of AccountPasswordPolicy is null!");
                return;
            }
            _accountPasswordPolicyRepository.Delete(entity);
        }
        #endregion

        #region Method
        public IQueryable<AccountPasswordPolicy> GetAllAccountPasswordPolicy()
        {
            return _accountPasswordPolicyRepository.Table;
        }

        public bool ApplyPasswordPolicy(int accountId, string accountType, string newPassword, string currentPassword, PasswordFormat currentPasswordFormat, string currentPasswordSalt, out StringBuilder errors)
        {
            var policy = Retrieve(accountType).PasswordPolicy;

            errors = new StringBuilder();
            var table = _lsrRepository.TableNoTracking;

            if (String.IsNullOrWhiteSpace(newPassword))
                errors.AppendLine(table.Where(x => x.ResourceName == "Admin.Candidate.Candidate.Fields.Password.Required").FirstOrDefault().ResourceValue);
            else
            {
                newPassword = newPassword.Trim();
                
                if (policy.MaxLength < newPassword.Length)
                    errors.AppendLine(String.Format(table.Where(x => x.ResourceName == "Common.PasswordPolicy.MaxLength").FirstOrDefault().ResourceValue, policy.MaxLength));

                if (policy.MinLength > newPassword.Length)
                    errors.AppendLine(String.Format(table.Where(x => x.ResourceName == "Common.PasswordPolicy.MinLength").FirstOrDefault().ResourceValue, policy.MinLength));

                if (policy.RequireLowerCase)
                {
                    if (!newPassword.Any(x => char.IsLower(x)))
                        errors.AppendLine(table.Where(x => x.ResourceName == "Common.PasswordPolicy.RequireLowerCase").FirstOrDefault().ResourceValue);
                }

                if (policy.RequireNumber)
                {
                    if (!newPassword.Any(x => char.IsNumber(x)))
                        errors.AppendLine(table.Where(x => x.ResourceName == "Common.PasswordPolicy.RequireNumber").FirstOrDefault().ResourceValue);
                }

                if (policy.RequireSymbol)
                {
                    if (!newPassword.Any(x => char.IsSymbol(x)))
                        errors.AppendLine(table.Where(x => x.ResourceName == "Common.PasswordPolicy.RequireSymbol").FirstOrDefault().ResourceValue);
                }

                if (policy.RequireUpperCase)
                {
                    if (!newPassword.Any(x => char.IsUpper(x)))
                        errors.AppendLine(table.Where(x => x.ResourceName == "Common.PasswordPolicy.RequireUpperCase").FirstOrDefault().ResourceValue);
                }

                if (policy.PasswordHistory > 0 && accountId > 0)
                {

                    IEnumerable<Tuple<int, string, string>> _passwordHistory; // Tuple's items are: PasswordFormatId, PasswordSalt, Password

                    if (accountType.Equals("Candidate", StringComparison.OrdinalIgnoreCase))
                    {
                        _passwordHistory = _candidatePasswordHistoryRepository.TableNoTracking.Where(x => x.CandidateId == accountId)
                                                                                         .OrderByDescending(x => x.CreatedOnUtc)
                                                                                         .Take(policy.PasswordHistory)
                                                                                         .ToList()
                                                                                         .Select(x => new Tuple<int, string, string>(x.PasswordFormatId, x.PasswordSalt, x.Password) );
                    }
                    else
                    {
                        _passwordHistory = _accountPasswordHistoryRepository.TableNoTracking.Where(x => x.AccountId == accountId)
                                                                                         .OrderByDescending(x => x.CreatedOnUtc)
                                                                                         .Take(policy.PasswordHistory)
                                                                                         .ToList()
                                                                                         .Select(x => new Tuple<int, string, string>(x.PasswordFormatId, x.PasswordSalt, x.Password));
                    }

                    foreach (var row in _passwordHistory)
                    {
                        // check if the new password matches the password in history
                        var _pwd = _encryptionService.ConvertPassword((PasswordFormat) row.Item1 /*PasswordFormatId*/, newPassword, row.Item2 /*PasswordSalt*/);
                        if (_pwd == row.Item3 /*Password*/)
                        {
                            errors.AppendLine(table.Where(x => x.ResourceName.Equals("Common.PasswordIsUsed", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().ResourceValue);
                            break;
                        }
                    }
                }

            }

            return errors.Length <= 0;
        }

        #endregion
    }
}
