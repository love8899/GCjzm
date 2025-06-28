using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;


namespace Wfm.Services.Companies
{
    public partial class CompanyContactService : ICompanyContactService
    {
        #region Fields

        private readonly IRepository<CompanyLocation> _locationRepository;
        private readonly IRepository<Account> _contactRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CompanyDepartment> _departmentRepository;

        #endregion

        #region Ctor

        public CompanyContactService(
            IRepository<CompanyLocation> locationRepository,
            IRepository<Account> contactRepository,
            IRepository<Company> companyRepository,
            IRepository<CompanyDepartment> companyDepartmentRepository)
        {
            _locationRepository = locationRepository;
            _contactRepository = contactRepository;
            _companyRepository = companyRepository;
            _departmentRepository = companyDepartmentRepository;
        }

        #endregion

        #region CRUD

        public void InsertCompanyContact(Account companyContact)
        {
            if (companyContact == null) throw new ArgumentNullException("companyContact");
            _contactRepository.Insert(companyContact);
        }

        public void UpdateCompanyContact(Account companyContact)
        {
            if (companyContact == null) throw new ArgumentException("companyContact");
            _contactRepository.Update(companyContact);
        }

        public void DeleteCompanyContact(Account companyContact)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region CompanyContact

        public Account GetCompanyContactByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _contactRepository.Table
                        where c.Email == email
                        select c;

            return query.FirstOrDefault();
        }

        public Account GetCompanyContactById(int id)
        {
            if (id == 0)
                return null;

            return _contactRepository.GetById(id);
        }

        public void DeleteAllCompanyContactsByCompanyGuid(Guid? guid)
        {
            var company = _companyRepository.TableNoTracking.Where(x => x.CompanyGuid == guid).FirstOrDefault();
            if (company == null)
                return;
            var contacts = _contactRepository.Table.Where(x => x.IsClientAccount && x.CompanyId == company.Id);
            if (contacts.Count() > 0)
                _contactRepository.Delete(contacts);
        }
        #endregion

        #region LIST

        public IList<Account> GetCompanyContactsByCompanyId(int companyId)
        {
            if (companyId == 0)
                return null;

            var query = _contactRepository.Table;

            query = from c in query
                    where c.IsClientAccount == true && c.CompanyId == companyId
                    orderby c.LastName, c.FirstName
                    select c;

            return query.ToList();
        }


        public IList<Account> GetCompanyClientAdminsByCompanyId(int companyId)
        {
            if (companyId == 0)
                return null;

            var query = _contactRepository.Table;

            query = query.Where(x => x.CompanyId == companyId)
                    .Where(x => x.AccountRoles.Any(r => r.SystemName == AccountRoleSystemNames.ClientAdministrators))
                    .OrderBy(x => x.LastName).ThenBy(x => x.FirstName);

            return query.ToList();
        }


        public IList<SelectListItem> GetCompanyContactsByCompanyIdAsSelectList(int companyId)
        {
            var contacts = GetCompanyContactsByCompanyId(companyId);
            var result = new List<SelectListItem>();

            foreach (var c in contacts)
            {
                var item = new SelectListItem()
                {
                    Text = c.FirstName + " " + c.LastName,
                    Value = c.Id.ToString()
                };

                result.Add(item);
            }

            return result;
        }


        public IList<Account> GetCompanyContactsByCompanyIdAndLocationIdAndDepartmentId(int companyId, int locationId, int departmentId)
        {
            if (companyId == 0)
                return null;

            var query = this.GetAllCompanyContactsAsQueryable().Where(x => x.CompanyId == companyId && x.IsClientAccount == true);

            if (locationId != 0)
                query = query.Where(x => x.CompanyLocationId == locationId);

            if (departmentId != 0)
                query = query.Where(x => x.CompanyDepartmentId == departmentId);

            return query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
        }

        public IQueryable<Account> GetAllCompanyContactsAsQueryable(bool showInactive = false, bool showHidden = false)
        {
            var query = _contactRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            query = from c in query
                    where c.IsClientAccount == true
                    orderby c.LastName, c.FirstName
                    select c;

            return query.AsQueryable();
        }

        public IQueryable<CompanyContact> GetCompanyContactsAsQueryable(bool showInactive = false, bool showHidden = false)
        {
            var query = this.GetAllCompanyContactsAsQueryable(showInactive, showHidden);

            var companyContact = from c in query
                                 from cl in _locationRepository.TableNoTracking.Where(o => c.CompanyLocationId == o.Id).DefaultIfEmpty()
                                 from cd in _departmentRepository.TableNoTracking.Where(o => c.CompanyDepartmentId == o.Id).DefaultIfEmpty()
                                 from comp in _companyRepository.TableNoTracking.Where(o => c.CompanyId == o.Id).DefaultIfEmpty()
                                 where c.IsClientAccount == true
                                 orderby c.LastName, c.FirstName
                                 select new CompanyContact()
                                 {
                                     AccountGuid=c.AccountGuid,
                                     Id = c.Id,
                                     AccountRoleSystemName = c.AccountRoles.FirstOrDefault() != null ? c.AccountRoles.FirstOrDefault().SystemName : string.Empty,
                                     CompanyDepartmentId = c.CompanyDepartmentId,
                                     CompanyDepartmentName = cd.DepartmentName,
                                     CompanyId = c.CompanyId,
                                     CompanyLocationId = c.CompanyLocationId,
                                     CompanyLocationName = cl.LocationName,
                                     CompanyName = comp.CompanyName,
                                     Email = c.Email,
                                     EnteredBy = c.EnteredBy,
                                     FirstName = c.FirstName,
                                     IsActive = c.IsActive,
                                     IsDeleted = c.IsDeleted,
                                     LastName = c.LastName,
                                     ManagerId = c.ManagerId,
                                     //Title = c.Title,
                                     WorkPhone = c.WorkPhone,
                                     CreatedOnUtc=c.CreatedOnUtc,
                                     UpdatedOnUtc=c.UpdatedOnUtc,
                                     ShiftName=c.Shift!=null?c.Shift.ShiftName:string.Empty
                                 };
            return companyContact.AsQueryable();
        }


        public IQueryable<CompanyContact> GetCompanyClientAdminsAsQueryable(bool showInactive = false, bool showHidden = false)
        {
            var query = _contactRepository.Table;

            query = query.Where(x => x.AccountRoles.Any(r => r.SystemName == AccountRoleSystemNames.ClientAdministrators));

            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            var companyContact = from c in query
                                 from cl in _locationRepository.TableNoTracking.Where(o => c.CompanyLocationId == o.Id).DefaultIfEmpty()
                                 from cd in _departmentRepository.TableNoTracking.Where(o => c.CompanyDepartmentId == o.Id).DefaultIfEmpty()
                                 from comp in _companyRepository.TableNoTracking.Where(o => c.CompanyId == o.Id).DefaultIfEmpty()
                                 orderby c.LastName, c.FirstName
                                 select new CompanyContact()
                                 {
                                     AccountGuid = c.AccountGuid,
                                     Id = c.Id,
                                     AccountRoleSystemName = c.AccountRoles.FirstOrDefault() != null ? c.AccountRoles.FirstOrDefault().SystemName : string.Empty,
                                     CompanyDepartmentId = c.CompanyDepartmentId,
                                     CompanyDepartmentName = cd.DepartmentName,
                                     CompanyId = c.CompanyId,
                                     CompanyLocationId = c.CompanyLocationId,
                                     CompanyLocationName = cl.LocationName,
                                     CompanyName = comp.CompanyName,
                                     Email = c.Email,
                                     EnteredBy = c.EnteredBy,
                                     FirstName = c.FirstName,
                                     IsActive = c.IsActive,
                                     IsDeleted = c.IsDeleted,
                                     LastName = c.LastName,
                                     ManagerId = c.ManagerId,
                                     //Title = c.Title,
                                     WorkPhone = c.WorkPhone,
                                     CreatedOnUtc = c.CreatedOnUtc,
                                     UpdatedOnUtc = c.UpdatedOnUtc,
                                     ShiftName = c.Shift != null ? c.Shift.ShiftName : string.Empty
                                 };
            return companyContact.AsQueryable();
        }


        public IQueryable<Account> GetAllCompanyContactsByAccountAsQueryable(Account account, bool showInactive = false, bool showHidden = false)
        {
            if (account == null)
                return null;

            var query = _contactRepository.Table;

            // query within company
            query = query.Where(j => j.CompanyId == account.CompanyId);

            // active
            if (!showInactive)
                query = query.Where(j => j.IsActive == true);

            // deleted
            if (!showHidden)
                query = query.Where(j => j.IsDeleted == false);

            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}

            // Contacts for Location Manager
            else if (account.IsCompanyLocationManager())
                query = query.Where(cl =>
                    cl.CompanyLocationId > 0 &&
                    cl.CompanyLocationId == account.CompanyLocationId); // search within locatin

            // Contacts for Department Supervisor
            else if (account.IsCompanyDepartmentSupervisor())
                query = query.Where(cl =>
                    cl.CompanyLocationId > 0 &&
                    cl.CompanyLocationId == account.CompanyLocationId &&
                    cl.CompanyDepartmentId > 0 &&
                    cl.CompanyDepartmentId == account.CompanyDepartmentId &&    // search within department
                    cl.Id == account.Id);
            else if (account.IsCompanyDepartmentManager())
                query = query.Where(cl =>
                    cl.CompanyLocationId > 0 &&
                    cl.CompanyLocationId == account.CompanyLocationId &&
                    cl.CompanyDepartmentId > 0 &&
                    cl.CompanyDepartmentId == account.CompanyDepartmentId);
            else
                return null; // No role



            query = from c in query
                    orderby c.LastName, c.FirstName
                    select c;


            return query.AsQueryable();
        }

        #endregion

    }
}
