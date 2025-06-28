using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Helpers;
using Wfm.Services.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Services.Security;
using Wfm.Services.Companies;
using Wfm.Services.Franchises;

namespace Wfm.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        #region Fields

        private readonly IGenericHelper _genericHelper;
        private readonly IPermissionService _permissionService;
        private readonly ICandidateService _candidateService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<EmployeeAvailability> _employeeAvailabilityRepository;
        private readonly IRepository<EmployeeType> _employeeTypeRepository;
        private readonly IRepository<EmployeePayrollSetting> _employeePayrollSettingRepository;
        private readonly IRepository<EmployeeJobRole> _employeeJobRoleRepository;
        private readonly IWorkContext _workContext;
        private readonly IFranchiseService _franchiseService;
        #endregion

        #region Ctor

        public EmployeeService(
            IGenericHelper genericHelper,
            IPermissionService permissionService,
            ICandidateService candidateService,
            ICompanyCandidateService companyCandidateService,
            IRepository<Account> accountRepository,
            IRepository<Employee> employeeRepository,
            IRepository<EmployeeAvailability> employeeAvailabilityRepository,
            IRepository<EmployeeType> employeeTypeRepository,
            IRepository<EmployeePayrollSetting> employeePayrollSettingRepository,
            IRepository<EmployeeJobRole> employeeJobRoleRepository,
            IWorkContext workContext,
            IFranchiseService franchiseService)
        {
            _genericHelper = genericHelper;
            _permissionService = permissionService;
            _candidateService = candidateService;
            _companyCandidateService = companyCandidateService;
            _accountRepository = accountRepository;
            _employeeRepository = employeeRepository;
            _employeeAvailabilityRepository = employeeAvailabilityRepository;
            _employeeTypeRepository = employeeTypeRepository;
            _employeePayrollSettingRepository = employeePayrollSettingRepository;
            _employeeJobRoleRepository = employeeJobRoleRepository;
            _workContext = workContext;
            _franchiseService = franchiseService;
        }


        #endregion

        #region List
        public IQueryable<Employee> GetAllEmployeesByCompany(int companyId, bool activeOnly = false, bool includeRegular = true)
        {
            return _employeeRepository.Table.Where(x => x.CompanyId == companyId)
                        .Where(x => (!activeOnly || x.IsActive) && (includeRegular || x.EmployeeType != "Regular"))
                        .Include(x => x.EmployeeJobRoles.Select(r => r.CompanyJobRole))
                        .OrderByDescending(x => x.Id);
        }

        public Employee GetEmployeeByCandidateId(int candidateId)
        {
            var query = _employeeRepository.Table.Where(x => x.Id == candidateId)
                .Include(x => x.Gender)
                .Include(x => x.CompanyLocation)
                .Include(x => x.EnteredByAccount)
                .Include(x => x.VetranType)
                .Include(x => x.EmployeePayrollSettings)
                .Include(x => x.EmployeeJobRoles.Select(r => r.CompanyJobRole));
            return query.FirstOrDefault();
        }
        public Employee GetEmployeeByGuid(Guid guid)
        {
            var query = _employeeRepository.Table.Where(x => x.CandidateGuid == guid);
            //query= query.Include(x => x.Gender);
            //query= query.Include(x => x.CompanyLocation);
            //query= query.Include(x => x.EnteredByAccount);
            //query= query.Include(x => x.VetranType);
            //query = query.Include(x => x.EmployeePayrollSettings);
            //query= query.Include(x => x.EmployeeJobRoles.Select(r => r.CompanyJobRole));
            return query.FirstOrDefault();
        }
        #endregion

        #region Worktime Preference
        public IQueryable<EmployeeAvailability> GetWorktimePreference(int id)
        {
            return _employeeAvailabilityRepository.Table.Where(x => x.EmployeeId == id).OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTimeOfDayTicks);
        }

        public EmployeeAvailability GetWorktimePreferenceById(int id)
        {
            return _employeeAvailabilityRepository.GetById(id);
        }
        public void AddWorktimePreference(EmployeeAvailability entity)
        {
            entity.CreatedOnUtc = entity.UpdatedOnUtc = DateTime.UtcNow;
            _employeeAvailabilityRepository.Insert(entity);
        }
        public void UpdateWorktimePreference(EmployeeAvailability entity)
        {
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _employeeAvailabilityRepository.Update(entity);
        }
        public void DeleteWorktimePreference(int id)
        {
            _employeeAvailabilityRepository.Delete(_employeeAvailabilityRepository.GetById(id));
        }
        public bool CheckOverlaping(int employeeId, DayOfWeek dayOfWeek, long startTime, long endTime, int exclusiveId, EmployeeAvailabilityType employeeAvailabilityType, 
            DateTime? startDate, DateTime? endDate)
        {
            startDate = startDate.GetValueOrDefault(new DateTime(1900, 1, 1));
            endDate = endDate.GetValueOrDefault(new DateTime(2999, 12, 31));
            var query = _employeeAvailabilityRepository.Table
                .Where(x => (x.StartDate.HasValue && x.StartDate <= startDate && (!x.EndDate.HasValue || x.EndDate >= startDate)) ||
                    (x.StartDate.HasValue && x.StartDate <= endDate && (!x.EndDate.HasValue || x.EndDate >= endDate)) ||
                    (x.StartDate.HasValue && x.StartDate <= startDate && (!x.EndDate.HasValue || x.EndDate >= endDate)) ||
                    (x.StartDate.HasValue && x.StartDate >= startDate && (!x.EndDate.HasValue || x.EndDate <= endDate)))
                .Where(x => x.EmployeeId == employeeId && x.DayOfWeek == dayOfWeek && x.Id != exclusiveId 
                    && x.EmployeeAvailabilityType != employeeAvailabilityType)
                .Where(x => (x.StartTimeOfDayTicks <= startTime && x.EndTimeOfDayTicks >= startTime) ||
                    (x.StartTimeOfDayTicks <= endTime && x.EndTimeOfDayTicks >= endTime) ||
                    (x.StartTimeOfDayTicks <= startTime && x.EndTimeOfDayTicks >= endTime) ||
                    (x.StartTimeOfDayTicks >= startTime && x.EndTimeOfDayTicks <= endTime));

            return query.Any();
        }

        public IEnumerable<EmployeeType> GetAllEmployeeTypes()
        {
            return _employeeTypeRepository.Table;
        }

        private void DefaultCandidate(Candidate candidate)
        {
            candidate.SalutationId = 1;
            candidate.EnteredBy = _workContext.CurrentAccount.Id;
            candidate.Username = candidate.Email;
        }

        public void UpdateEmployee(Candidate candidate, DateTime? hireDate, DateTime? terminationDate, int? companyLocationId, int? primaryJobRoleId)
        {
            //DefaultCandidate(candidate);
            _candidateService.UpdateCandidate(candidate, Tuple.Create(hireDate, terminationDate));
            var locationUpdated = false;
            var companyId = _workContext.CurrentAccount.CompanyId;
            var currentLocations = _companyCandidateService.GetCompanyCandidatesByCompanyIdAsQueryable(companyId)
                .Where(x => x.CandidateId == candidate.Id && (!x.EndDate.HasValue || x.EndDate.Value > DateTime.Today)).ToArray();
            foreach (var loc in currentLocations)
            {
                if (loc.PreferredLocationId == companyLocationId)
                {
                    loc.EndDate = null;
                    locationUpdated = true;
                }
                else
                {
                    loc.EndDate = DateTime.Today;
                }
                _companyCandidateService.UpdateCompanyCandidate(loc);
            }
            if (!locationUpdated && companyLocationId.HasValue)
            {
                _companyCandidateService.InsertCompanyCandidate(new CompanyCandidate()
                {
                    CandidateId = candidate.Id,
                    CompanyId = companyId,
                    PreferredLocationId = companyLocationId,
                    StartDate = DateTime.Today,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                });
            }
            if (hireDate.HasValue)
            {
                var currentSetting = _employeePayrollSettingRepository.Table.Where(x => x.EmployeeId == candidate.Id).OrderByDescending(x => x.LastHireDate).FirstOrDefault();
                if (currentSetting == null)
                {
                    _employeePayrollSettingRepository.Insert(new EmployeePayrollSetting
                    {
                        EmployeeId = candidate.Id,
                        //Year = DateTime.Today.Year,
                        UpdatedBy = _workContext.CurrentAccount.Id,
                        VacationRate = 0.04M,
                        UpdatedOnUtc = DateTime.UtcNow,
                        LastHireDate = hireDate,
                    });
                }
                else
                {
                    currentSetting.LastHireDate = hireDate;
                    _employeePayrollSettingRepository.Update(currentSetting);
                }
            }
            if (primaryJobRoleId.HasValue)
            {
                var currentRoles = _employeeJobRoleRepository.Table.Where(x => x.EmployeeId == candidate.Id).Include(x => x.CompanyJobRole).ToArray();
                var primaryRole = currentRoles.Where(x => x.IsPrimary).FirstOrDefault();
                var thisRole = currentRoles.Where(x => x.CompanyJobRoleId == primaryJobRoleId.Value).FirstOrDefault();
                bool doInsert = thisRole == null;
                bool needChanage = false;
                if (primaryRole == null)
                {
                    needChanage = true;
                }
                else if (primaryRole.CompanyJobRoleId != primaryJobRoleId)
                {
                    primaryRole.IsPrimary = false;
                    primaryRole.ExpiryDate = DateTime.Today.AddDays(-1);
                    _employeeJobRoleRepository.Update(primaryRole);
                    needChanage = true;
                }
                if (needChanage)
                {
                    if (doInsert)
                    {
                        _employeeJobRoleRepository.Insert(new EmployeeJobRole
                        {
                            EmployeeId = candidate.Id,
                            CompanyJobRoleId = primaryJobRoleId.Value,
                            CreatedOnUtc = DateTime.UtcNow,
                            UpdatedOnUtc = DateTime.UtcNow,
                            EffectiveDate = DateTime.Today,
                            IsPrimary = true,
                        });
                    }
                    else
                    {
                        thisRole.IsPrimary = true;
                        thisRole.ExpiryDate = null;
                        _employeeJobRoleRepository.Update(thisRole);
                    }
                }
            }
        }

       

        public void AddNewEmployee(Candidate candidate, int companyId, DateTime? hireDate, int? companyLocationId, int? primaryJobRoleId)
        {           
            //set Candidate FranchiseId
            var franchises = _franchiseService.GetAllFranchises();
            candidate.FranchiseId = franchises.Count > 0 ? franchises[0].Id : 0;
           
            DefaultCandidate(candidate);

            _candidateService.RegisterCandidate(candidate, true, true); //, false); //todo: add error handeling here
            
            _companyCandidateService.InsertCompanyCandidate(new CompanyCandidate()
                {
                    CandidateId = candidate.Id,
                    CompanyId = companyId,
                    PreferredLocationId = companyLocationId,
                    StartDate = DateTime.Today,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                });

            if (hireDate.HasValue)
            {
                _employeePayrollSettingRepository.Insert(new EmployeePayrollSetting
                {
                    EmployeeId = candidate.Id,
                    //Year = DateTime.Today.Year,
                    UpdatedBy = _workContext.CurrentAccount.Id,
                    VacationRate = 0.04M,
                    UpdatedOnUtc = DateTime.UtcNow,
                    LastHireDate = hireDate,
                });
            }
            if (primaryJobRoleId.HasValue)
            {
                _employeeJobRoleRepository.Insert(new EmployeeJobRole
                {
                    EmployeeId = candidate.Id,
                    CompanyJobRoleId = primaryJobRoleId.Value,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    EffectiveDate = DateTime.Today,
                    IsPrimary = true,
                });
            }
        }

        public IQueryable<EmployeeJobRole> GetEmployeJobRoles(int employeeId)
        {
            return _employeeJobRoleRepository.Table.Where(x => x.EmployeeId == employeeId)
                .Include(x => x.CompanyJobRole);
        }
        public EmployeeJobRole GetJobRoleById(int employeeJobRoleId)
        {
            return _employeeJobRoleRepository.Table.Where(x => x.Id == employeeJobRoleId)
                .Include(x => x.CompanyJobRole).FirstOrDefault();
        }
        public void AddEmployeeJobRole(int employeeId, EmployeeJobRole role)
        {
            role.EmployeeId = employeeId;
            role.CreatedOnUtc = DateTime.UtcNow;
            role.UpdatedOnUtc = DateTime.UtcNow;
            _employeeJobRoleRepository.Insert(role);
        }
        public void UpdateEmployeeJobRole(EmployeeJobRole role)
        {
            role.UpdatedOnUtc = DateTime.UtcNow;
            _employeeJobRoleRepository.Update(role);
        }
        public void DeleteEmployeeJobRole(int employeeJobRoleId)
        {
            _employeeJobRoleRepository.Delete(_employeeJobRoleRepository.GetById(employeeJobRoleId));
        }
        #endregion
    }
}
