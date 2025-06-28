using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Employees;
using Wfm.Core.Domain.JobOrders;
using Wfm.Data;
using Wfm.Services.Helpers;


namespace Wfm.Services.Companies
{
    public partial class CompanyService : ICompanyService
    {
        #region Fields

        private readonly IGenericHelper _genericHelper;

        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<JobOrder> _jobOrderRepository;
        private readonly IRepository<CompanyJobRole> _jobRoleRepository;
        private readonly IRepository<CompanyJobRoleSkill> _jobRoleSkillRepository;
        private readonly IRepository<Skill> _skillRepository;
        private readonly IRepository<EmployeeJobRole> _employeeJobRoleRepository;
        private readonly IRepository<CompanyShift> _companyShiftRepository;
        private readonly IRepository<CompanyShiftJobRole> _companyShiftJobRoleRepository;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IRepository<CompanyVendor> _companyVendorRepository;
        //private readonly ILogger _logger;

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyService"/> class.
        /// </summary>
        /// <param name="companyRepository">The company repository.</param>
        public CompanyService(IGenericHelper genericHelper,
            IRepository<Company> companyRepository,
            IRepository<JobOrder> jobOrderRepository,
            IRepository<CompanyJobRole> jobRoleRepository,
            IRepository<CompanyJobRoleSkill> jobRoleSkillRepository,
            IRepository<Skill> skillRepository,
            IRepository<EmployeeJobRole> employeeJobRoleRepository,
            IRepository<CompanyShift> companyShiftRepository,
            IRepository<CompanyShiftJobRole> companyShiftJobRoleRepository,
            IWorkContext workContext,
            IDbContext dbContext,
            IRepository<CompanyVendor> companyVendorRepository
            )
        {
            _genericHelper = genericHelper;
            _companyRepository = companyRepository;
            _jobOrderRepository = jobOrderRepository;
            _jobRoleRepository = jobRoleRepository;
            _jobRoleSkillRepository = jobRoleSkillRepository;
            _skillRepository = skillRepository;
            _employeeJobRoleRepository = employeeJobRoleRepository;
            _companyShiftRepository = companyShiftRepository;
            _companyShiftJobRoleRepository = companyShiftJobRoleRepository;
            _workContext = workContext;
            _dbContext = dbContext;
            _companyVendorRepository = companyVendorRepository;
        }

        #endregion

        #region CRUD
        /// <summary>
        /// Inserts the company.
        /// </summary>
        /// <param name="company">The company.</param>
        /// <exception cref="System.ArgumentNullException">company</exception>
        public virtual void InsertCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException("company");

            _companyRepository.Insert(company);
        }

        /// <summary>
        /// Updates the company.
        /// </summary>
        /// <param name="company">The company.</param>
        /// <exception cref="System.ArgumentNullException">company</exception>
        public virtual void UpdateCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException("company");

            _companyRepository.Update(company);
        }

        /// <summary>
        /// Deletes the company.
        /// </summary>
        /// <param name="company">The company.</param>
        /// <exception cref="System.ArgumentNullException">company</exception>
        public void DeleteCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException("company");
            company.IsActive = false;
            _companyRepository.Update(company);
        }

        #endregion

        #region Company

        /// <summary>
        /// Gets a Company
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns>Company</returns>
        public Company GetCompanyById(int Id, bool enableTracking = false)
        {
            if (Id <= 0)
                return null;

            return this.Secure_GetCompanyById(_workContext.CurrentAccount, Id, enableTracking);
        }

        public Company GetCompanyByCode(string code)
        {
            if (String.IsNullOrWhiteSpace(code))
                return null;

            return _companyRepository.TableNoTracking.Where(x => x.CompanyCode == code).FirstOrDefault();
        }

        public Company GetCompanyByGuid(Guid? guid)
        {
            if (guid == null)
                return null;
            Company company = _companyRepository.TableNoTracking.Where(x => x.CompanyGuid == guid).FirstOrDefault();
            return company;
        }
        public Company GetCompanyByIdForScheduleTask(int Id, bool enableTracking = false)
        {
            if (Id <= 0)
                return null;

            return _companyRepository.Table.Where(x => x.Id == Id).FirstOrDefault();
        }

        public Company GetAdminCompany()
        {
            Company company = _companyRepository.TableNoTracking.Where(x => x.IsAdminCompany).FirstOrDefault();
            return company;
        }

        public void DeleteCompanyByGuid(Guid? guid)
        {
            var company = GetCompanyByGuid(guid);
            if (company == null)
                return;
            _companyRepository.Delete(company);
        }
        #endregion

        #region LIST
        public IList<SelectListItem> GetCompanyListForCandidate(bool showInactive=false,bool showHidden=false)
        {
            var query = _companyRepository.TableNoTracking;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);

            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            var list = query.Select(x=>new SelectListItem(){Text=x.CompanyName,Value=x.Id.ToString()}).ToList();
            return list;

        }
        /// <summary>
        /// Gets all companies.
        /// </summary>
        /// <returns></returns>
        public IList<SelectListItem> GetAllCompanies()
        {
            return this.GetAllCompanies(_workContext.CurrentAccount);
        }

        public IList<SelectListItem> GetAllCompanies(Account account)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            StringBuilder query = new StringBuilder(@"Select *
                                                      FROM Company c
                                                      Where c.IsDeleted = 0
                                                    ");

            if (account.IsClientAccount)
                query = query.Append(String.Format(" and c.Id = {0} ", account.CompanyId));
            else if (account.IsLimitedToFranchises) // vendor only
                query = query.Append(String.Format(" and c.Id in (Select distinct CompanyId from CompanyVendor Where VendorId = {0} and IsActive = 1) ", account.FranchiseId));

            query.Append(" Order by c.CompanyName");

            var data = _dbContext.SqlQuery<Company>(query.ToString());
            var companies = data.AsQueryable<Company>();

            foreach (var c in companies)
            {
                var item = new SelectListItem()
                {
                    Text=c.CompanyName,
                    Value=c.Id.ToString()
                };
                result.Add(item);
            }

            return result;
        }
        /// <summary>
        /// Gets all companies.
        /// </summary>
        /// <param name="FilterCols">The filter cols.</param>
        /// <param name="FiltersCommand">The filters command.</param>
        /// <param name="SortCommand">The sort command.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IPagedList<Company> GetAllCompanies(int pageIndex = 0, int PageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _companyRepository.TableNoTracking;

            query = from c in query
                    orderby c.DisplayOrder, c.UpdatedOnUtc descending
                    select c;

            int total = _companyRepository.Table.Count(); // counting total records

            IPagedList<Company> companies = new PagedList<Company>(query, pageIndex, PageSize, total);
            return companies;
        }

        /// <summary>
        /// Gets all companies asynchronous queryable.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="showHidden">if set to <c>true</c> [show hidden].</param>
        /// <returns></returns>
        public IQueryable<Company> GetAllCompaniesAsQueryable(Account account, bool showInactive = false, bool showHidden = false)
        {
            var query = _companyRepository.TableNoTracking;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);

            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);
            if (account != null)
            {
                // vendor only
                if (account.IsLimitedToFranchises)
                {
                    query = from c in query
                            join d in _companyVendorRepository.TableNoTracking on c.Id equals d.CompanyId
                            where d.VendorId == account.FranchiseId && d.IsActive
                            select c;
                }
            }
            query = from c in query
                    orderby c.DisplayOrder, c.UpdatedOnUtc descending
                    select c;

            return query.AsQueryable();
        }

        /// <summary>
        /// Gets all companies asynchronous queryable. The result has tracking disable, so it is suitable for read-only lists (grids, dropdowns, etc.)
        /// </summary>
        /// <param name="account">The current user account.</param>
        /// <param name="showHidden">if set to <c>true</c> [show deleted].</param>
        /// <returns></returns>
        public IQueryable<Company> Secure_GetAllCompanies(Account account, bool showInactive = false, bool showHidden = false)
        {
            StringBuilder query = new StringBuilder(@"Select c.Id
                                      ,CompanyGuid
                                      ,CompanyName
                                      ,WebSite
                                      ,KeyTechnology
                                      ,c.Note
                                      ,c.IsHot
                                      ,c.IsActive
                                      ,c.IsDeleted
                                      ,IsAdminCompany
                                      ,AdminName
                                      ,c.OwnerId
                                      ,c.EnteredBy
                                      ,c.DisplayOrder
                                      ,c.CreatedOnUtc
                                      ,c.UpdatedOnUtc
                                      ,InvoiceAddress1
                                      ,InvoiceAddress2
                                      ,InvoiceTermId
                                      ,CompanyCode
                                      ,CompanyStatusId
                                      ,InvoiceIntervalId
                                      ,IndustryId
                               FROM Company c
                                    ");

            bool onlyMappedUsers = false;
            if (
                account.IsVendorAdmin() == false && !account.IsOperator(true) &&
                account.IsAdministrator(true) == false && account.IsPayrollAdministrator(true) == false && account.IsCompanyHrManager(true) == false)
            {
                query.AppendLine(" inner join Account_Company_Mapping mappedUsers on c.Id = mappedUsers.CompanyId ");
                onlyMappedUsers = true;
            }

            // deleted
            if (!showHidden)
                query = query.Append(" Where c.IsDeleted = 0 ");
            else
                query = query.Append(" Where c.Id > 0 "); //this line is added only to add the WHERE clause to the query

            // active
            if (!showInactive)
                query = query.Append(" and c.IsActive = 1 ");

            if (account.IsClientAccount)
                query = query.Append(String.Format(" and c.Id = {0} ", account.CompanyId));
            else if (account.IsLimitedToFranchises) // vendor only
            {
                query = query.Append(String.Format(" and c.Id in (Select distinct CompanyId from CompanyVendor Where VendorId = {0} and IsActive =1) ", account.FranchiseId));
            }


            if (onlyMappedUsers)
                query.Append(String.Format(" and mappedUsers.AccountId = {0} ", account.Id));

            query.Append(" Order by c.IsAdminCompany desc, c.CompanyName,c.UpdatedOnUtc desc");

            var data = _dbContext.SqlQuery<Company>(query.ToString());
            var result = data.AsQueryable<Company>();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Company Secure_GetCompanyById(Account account, int Id, bool enableTracking)
        {
            if (Id == 0 || account == null)
                return null;

            StringBuilder query = new StringBuilder(@"
                                Select c.Id
                                      ,CompanyGuid
                                      ,CompanyName
                                      ,WebSite
                                      ,KeyTechnology
                                      ,c.Note
                                      ,c.IsHot
                                      ,c.IsActive
                                      ,c.IsDeleted
                                      ,IsAdminCompany
                                      ,AdminName
                                      ,c.OwnerId
                                      ,c.EnteredBy
                                      ,c.DisplayOrder
                                      ,c.CreatedOnUtc
                                      ,c.UpdatedOnUtc
                                      ,InvoiceAddress1
                                      ,InvoiceAddress2
                                      ,InvoiceTermId
                                      ,CompanyCode
                                      ,CompanyStatusId
                                      ,InvoiceIntervalId
                                      ,c.IndustryId
                               FROM Company c
                                    ");

            //bool onlyMappedUsers = false;
            //if (account.IsAdministrator(true) == false && account.IsPayrollAdministrator(true) == false && account.IsCompanyHrManager(true) == false)
            //{
            //    query.AppendLine(" inner join Account_Company_Mapping mappedUsers on c.Id = mappedUsers.CompanyId ");
            //    onlyMappedUsers = true;
            //}

            query.AppendLine(" Where c.Id = @Id  ");


            if (account.IsClientAccount)
                query = query.Append(String.Format(" and c.Id = {0} ", account.CompanyId));
            else if (account.IsLimitedToFranchises) // vendor only
                query = query.Append(String.Format(" and c.Id in (Select distinct CompanyId from CompanyVendor Where VendorId = {0} and IsActive=1) ", account.FranchiseId));
           

            //if (onlyMappedUsers)
            //    query.Append(String.Format(" and mappedUsers.AccountId = {0} ", account.Id));

            query.Append(" Order by c.DisplayOrder, c.UpdatedOnUtc desc");

            Company result = null;
            if (enableTracking)
            {
                result = _companyRepository.GetByQuery(query.ToString(), new SqlParameter("Id", Id));
            }
            else
            {
                IEnumerable<Company> data = _dbContext.SqlQuery<Company>(query.ToString(), new SqlParameter("Id", Id));
                result = data.FirstOrDefault<Company>();
            }

            return result;
        }

        #endregion

        #region Company Search

        public IList<Company> SearchCompanies(string searchKey, int maxRecordsToReturn = 100, bool showInactive = false, bool showHidden = false)
        {
            if (String.IsNullOrWhiteSpace(searchKey))
            {
                IList<Company> companies = new List<Company>();
                return companies;
            }

            var query = _companyRepository.TableNoTracking;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);


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
                         select c);
            }
            else
            {
                query = (from c in query
                         where c.CompanyName.Contains(sKeyWord)
                         || c.WebSite.Contains(sKeyWord)
                         || c.KeyTechnology.Contains(sKeyWord)
                         || c.Note.Contains(sKeyWord)
                         select c);
            }

            query = from c in query
                    orderby c.DisplayOrder, c.CreatedOnUtc descending
                    select c;

            if (maxRecordsToReturn < 1 || maxRecordsToReturn > 500)
                maxRecordsToReturn = 500;

            return query.Take(maxRecordsToReturn).ToList();
        }

        #endregion

        #region Statistics
        public int GetTotalCompanyByDate(DateTime date)
        {
            var total = (from c in _companyRepository.TableNoTracking
                         where c.CreatedOnUtc.Value.Month == date.Month
                         select c).Count();
            return total;
        }
        #endregion

        #region Job Roles
        private readonly string[] ResourceColors = new string[] {
                    "#E1F5FE", "#E0F7FA", "#E3F2FD", "#E0F2F1",
                    "#B3E5FC", "#B2EBF2", "#BBDEFB", "#B2DFDB",
                    "#81D4FA", "#80DEEA", "#90CAF9", "#80CBC4",
                    "#4FC3F7", "#4DD0E1", "#64B5F6", "#4DB6AC",
                    "#29B6F6", "#26C6DA", "#42A5F5", "#26A69A"
                };
        public IQueryable<CompanyJobRole> GetAllJobRoles(int companyId)
        {
            return this._jobRoleRepository.Table.Where(x => x.CompanyId == companyId)
                .Include(x => x.Company)
                .Include(x => x.CompanyLocation)
                .Include(x => x.RequiredSkills.Select(y => y.Skill));
        }

        public IQueryable<CompanyJobRole> GetAllJobRolesByAccount(Account account)
        {
            var result = GetAllJobRoles(account.CompanyId);

            if (!account.IsCompanyAdministrator() && !account.IsCompanyHrManager())
                result = result.Where(x => x.CompanyLocationId == account.CompanyLocationId);

            return result;
        }
        public IEnumerable GetAllJobRolesSelectList(int companyId)
        {
            var range = this.ResourceColors.Length;
            return GetAllJobRoles(_workContext.CurrentAccount.CompanyId).ToArray()
                .Select(x => new { Text = x.Name, Value = x.Id.ToString(), Color = this.ResourceColors[x.Id % range] });
        }
        public CompanyJobRole GetJobRoleById(int jobRoleId)
        {
            return this._jobRoleRepository.Table.Where(x => x.Id == jobRoleId)
                .Include(x => x.Company)
                .Include(x => x.CompanyLocation)
                .Include(x => x.RequiredSkills.Select(y => y.Skill)).FirstOrDefault();
        }
        public void InsertCompanyJobRole(CompanyJobRole jobRole, int[] requiredSkillId)
        {
            jobRole.CreatedOnUtc = jobRole.UpdatedOnUtc = DateTime.UtcNow;
            jobRole.StandardCostHourlyRate = 15.0m;
            var _skills = _skillRepository.Table.Where(x => requiredSkillId.Contains(x.Id)).ToArray();
            jobRole.RequiredSkills = _skills.Select(x => new CompanyJobRoleSkill
            {
                Skill = x,
                SkillId = x.Id,
                RequiredSkillProficiencyLevel = SkillProficiencyLevelEnum.Intermediate,
            }).ToArray();
            this._jobRoleRepository.Insert(jobRole);
        }
        public void UpdateCompanyJobRole(CompanyJobRole jobRole, int[] requiredSkillId)
        {
            var _skills = _skillRepository.Table.Where(x => requiredSkillId.Contains(x.Id)).ToArray();
            var _currentRecord = GetJobRoleById(jobRole.Id);
            _currentRecord.UpdatedOnUtc = DateTime.UtcNow;
            _currentRecord.Name = jobRole.Name;
            _currentRecord.Description = jobRole.Description;
            _currentRecord.CompanyLocationId = jobRole.CompanyLocationId;
            //
            var currentRequiredSkills = _currentRecord.RequiredSkills.Select(x => x.SkillId).ToArray();
            var toDeleteSkills = _currentRecord.RequiredSkills.Where(x => !requiredSkillId.Contains(x.SkillId)).ToList();
            toDeleteSkills.ForEach(x =>
            {
                _jobRoleSkillRepository.Delete(x);
            });
            var toAddSkills = _skills.Where(x => !currentRequiredSkills.Contains(x.Id)).ToList();
            toAddSkills.ForEach(x => _currentRecord.RequiredSkills.Add(new CompanyJobRoleSkill
            {
                Skill = x,
                SkillId = x.Id,
                RequiredSkillProficiencyLevel = SkillProficiencyLevelEnum.Intermediate,
            }));
            // no update needed this stage
            this._jobRoleRepository.Update(_currentRecord);
        }
        public void DeleteCompanyJobRole(int jobRoleId)
        {
            var role = this._jobRoleRepository.Table.Where(x => x.Id == jobRoleId)
                .Include(x => x.RequiredSkills)
                .Include(x => x.EmployeeJobRoles)
                .First();
            var skillsToRemove = role.RequiredSkills.ToArray();
            foreach (var skill in skillsToRemove)
            {
                _jobRoleSkillRepository.Delete(skill);
            }
            var empRolesToRemove = role.EmployeeJobRoles.ToArray();
            foreach (var eRole in empRolesToRemove)
            {
                _employeeJobRoleRepository.Delete(eRole);
            }
            this._jobRoleRepository.Delete(role);
        }
        #endregion

        #region Shifts

        public IQueryable<CompanyShift> GetAllShifts(int companyId)
        {
            return _companyShiftRepository.Table.Where(x => x.CompanyId == companyId)
                .Include(x => x.CompanyDepartment.CompanyLocation)
                .Include(x => x.Shift)
                .Include(x => x.CompanyShiftJobRoles);
        }
        public IEnumerable GetGetAllShiftsSelectList(int companyId)
        {
            var range = this.ResourceColors.Length;
            return GetAllShifts(_workContext.CurrentAccount.CompanyId).ToArray()
                .Select(x => new { Text = x.Shift.ShiftName, Value = x.Id.ToString(), Color = this.ResourceColors[x.Id % range] });
        }

        public IQueryable<CompanyShift> GetAllShiftsByAccount(Account account)
        {
            var result = GetAllShifts(account.CompanyId);

            if (account.IsCompanyLocationManager())
                result = result.Where(x => x.CompanyDepartment.CompanyLocation.Id == account.CompanyLocationId);

            else if (account.IsCompanyDepartmentManager())
                result = result.Where(x => x.CompanyDepartmentId == account.CompanyDepartmentId);

            return result;
        }

        public CompanyShift GetCompanyShiftById(int companyShiftId)
        {
            return _companyShiftRepository.Table.Where(x => x.Id == companyShiftId)
                .Include(x => x.CompanyDepartment.CompanyLocation)
                .Include(x => x.Shift)
                .Include(x => x.CompanyShiftJobRoles.Select(r => r.CompanyJobRole))
                .FirstOrDefault();
        }
        public void InsertNewCompanyShift(CompanyShift shift)
        {
            shift.UpdatedOnUtc = shift.CreatedOnUtc = DateTime.UtcNow;
            _companyShiftRepository.Insert(shift);
        }
        public void UpdateCompanyShift(CompanyShift shift)
        {
            shift.UpdatedOnUtc = DateTime.UtcNow;
            _companyShiftRepository.Update(shift);
        }
        public void DeleteCompanyShift(int companyShiftId)
        {
            _companyShiftRepository.Delete(_companyShiftRepository.Table.Where(x => x.Id == companyShiftId));
        }

        public IEnumerable<CompanyShiftJobRole> GetJobRolesOfShift(int companyShiftId)
        {
            return _companyShiftJobRoleRepository.Table.Where(x => x.CompanyShiftId == companyShiftId && 
                x.CompanyJobRole.CompanyLocationId == x.CompanyShift.CompanyDepartment.CompanyLocationId).Include(x => x.CompanyJobRole)
                .Include(x => x.Supervisor);
        }

        public void InsertCompanyShiftJobRoles(int companyShiftId, IEnumerable<CompanyShiftJobRole> roles)
        {
            var entity = _companyShiftRepository.Table.Where(x => x.Id == companyShiftId).Include(x => x.CompanyShiftJobRoles).First();
            if (entity.CompanyShiftJobRoles == null)
            {
                entity.CompanyShiftJobRoles = new List<CompanyShiftJobRole>(roles);
            }
            else
            {
                foreach (var role in roles) entity.CompanyShiftJobRoles.Add(role);
            }
            _companyShiftRepository.Update(entity);
        }
        public void UpdateCompanyShiftJobRoles(int companyShiftId, IEnumerable<CompanyShiftJobRole> roles)
        {
            var entity = _companyShiftRepository.Table.Where(x => x.Id == companyShiftId).Include(x => x.CompanyShiftJobRoles).First();
            foreach (var role in roles)
            {
                var _role = entity.CompanyShiftJobRoles.Where(x => x.Id == role.Id).FirstOrDefault();
                if (_role != null)
                {
                    _role.CompanyJobRoleId = role.CompanyJobRoleId;
                    _role.MandantoryRequiredCount = role.MandantoryRequiredCount;
                    _role.ContingencyRequiredCount = role.ContingencyRequiredCount;
                    _role.SupervisorId = role.SupervisorId;
                }
            }
            _companyShiftRepository.Update(entity);
        }
        public void DeleteCompanyShiftJobRoles(IEnumerable<CompanyShiftJobRole> roles)
        {
            foreach (var role in roles)
            {
                var _role = _companyShiftJobRoleRepository.Table.Where(x => x.Id == role.Id);
                _companyShiftJobRoleRepository.Delete(_role);
            }
        }
        #endregion
    }
}
