using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;


namespace Wfm.Services.Companies
{
    public class CompanyDepartmentService :ICompanyDepartmentService
    {
        #region Fields

        private readonly IRepository<CompanyDepartment> _companyDepartmentRepository;

        #endregion

        #region Cotr

        public CompanyDepartmentService(IRepository<CompanyDepartment> companyDepartment)
        {
            _companyDepartmentRepository = companyDepartment;
        }

        #endregion

        #region CRUD

        public void Insert(CompanyDepartment companyDepartment)
        {
            if (companyDepartment == null)
            {
                throw new ArgumentNullException("companyDepartment");
            }
            _companyDepartmentRepository.Insert(companyDepartment);

        }

        public void Update(CompanyDepartment companyDepartment)
        {
            if (companyDepartment == null)
            {
                throw new ArgumentNullException("companyDepartment");
            }
            _companyDepartmentRepository.Update(companyDepartment);
        }

        public void Delete(CompanyDepartment companyDepartment)
        {
            if (companyDepartment == null)
            {
                throw new ArgumentNullException("companyDepartment");
            }
            _companyDepartmentRepository.Delete(companyDepartment);
        }

        #endregion

        #region CompanyDepartment

        public CompanyDepartment GetCompanyDepartmentById(int id)
        {
            if (id == 0)
                return null;

            return _companyDepartmentRepository.GetById(id);
        }

        public void DeleteAllCompanyDepartmentByCompanyGuid(Guid? guid)
        {
            if (guid == null || guid == Guid.Empty)
                return;
            var departments = _companyDepartmentRepository.Table.Where(x => x.Company.CompanyGuid == guid);
            if (departments.Count() > 0)
                _companyDepartmentRepository.Delete(departments);
        }
        #endregion

        #region LIST

        public IQueryable<CompanyDepartment> GetAllCompanyDepartmentsAsQueryable(bool notTracking = true)
        {
            return notTracking ? _companyDepartmentRepository.TableNoTracking :
                                 _companyDepartmentRepository.Table;
        }

        public IList<CompanyDepartment> GetAllCompanyDepartmentsByCompanyId(int companyId, bool activeOnly)
        {
            var query = _companyDepartmentRepository.Table;

            query = from c in _companyDepartmentRepository.Table
                    where c.CompanyId == companyId && c.IsDeleted == false
                    select c;

            if (activeOnly)
                query = query.Where(c => c.IsActive);

            query = query.OrderBy(c => c.DisplayOrder);
            query = query.OrderByDescending(c => c.UpdatedOnUtc);

            return query.ToList();
        }

        public IList<CompanyDepartment> GetAllCompanyDepartmentsByCompanyGuid(Guid? companyGuid, bool activeOnly = true)
        {
            List<CompanyDepartment> list = new List<CompanyDepartment>();
            if (companyGuid == null || companyGuid == Guid.Empty)
                return list;
            var query = _companyDepartmentRepository.Table;

            query = from c in _companyDepartmentRepository.Table
                    where c.Company.CompanyGuid == companyGuid && c.IsDeleted == false
                    select c;

            if (activeOnly)
                query = query.Where(c => c.IsActive);

            query = query.OrderBy(c => c.DisplayOrder);
            query = query.OrderByDescending(c => c.UpdatedOnUtc);

            return query.ToList();
        }

        public IList<SelectListItem> GetAllCompanyDepartmentsByCompanyIdAsSelectList(int companyId, bool activeOnly = true)
        {
            var departments = GetAllCompanyDepartmentsByCompanyId(companyId, activeOnly).OrderBy(x=>x.DepartmentName);

            var result = new List<SelectListItem>();
            foreach (var d in departments)
            {
                var item = new SelectListItem()
                {
                    Text = String.Concat(d.DepartmentName,"(",d.CompanyLocation.LocationName,")"),
                    Value = d.Id.ToString()
                };

                result.Add(item);
            }

            return result;
        }

        public IList<SelectListItem> GetAllCompanyDepartmentsByCompanyGuidAsSelectList(Guid? companyGuid, bool activeOnly = true)
        {
            var departments = GetAllCompanyDepartmentsByCompanyGuid(companyGuid, activeOnly);


            var result = departments.Select(x => new SelectListItem() { Text = x.DepartmentName, Value = x.Id.ToString() });


            return result.ToList();
        }

        public IList<CompanyDepartment> GetAllCompanyDepartmentsByAccount(Account account, bool showInactive = false, bool showHidden = false)
        {
            var query = _companyDepartmentRepository.Table;

            // query within company
            query = query.Where(d => d.CompanyId == account.CompanyId);

            // active
            if (!showInactive)
                query = query.Where(d => d.IsActive == true);

            // deleted
            if (!showHidden)
                query = query.Where(d => d.IsDeleted == false);

            // Check account role and determine search range
            //----------------------------------------------------
            if (account.IsCompanyAdministrator() || account.IsCompanyHrManager()) { ;}
                
            // department for Location Manager
            else if (account.IsCompanyLocationManager())
                query = query.Where(cd =>
                    cd.CompanyLocationId > 0 &&
                    cd.CompanyLocationId == account.CompanyLocationId); // search within locatin

            // department for Department Supervisor
            else if (account.IsCompanyDepartmentSupervisor()||account.IsCompanyDepartmentManager())
                query = query.Where(cd => 
                    cd.Id == account.CompanyDepartmentId); // search within locatin
            else
                return null; // No role


            query = from b in query
                    orderby b.UpdatedOnUtc descending
                    select b;


            return query.ToList();
        }


        public IList<CompanyDepartment> GetAllCompanyDepartmentByLocationId(int locationId, bool activeOnly = true)
        {
            var query = from c in _companyDepartmentRepository.Table
                        where c.CompanyLocationId == locationId
                        select c;

            if (activeOnly)
                query = query.Where(c => c.IsActive&&!c.IsDeleted);

            return query.ToList();
        }


        public IList<CompanyDepartment> GetAllCompanyDepartmentByLocationName(string location, bool activeOnly = true)
        {
            var query = _companyDepartmentRepository.TableNoTracking
                .Where(x => x.CompanyLocation.LocationName == location)
                .Where(x => !activeOnly || (x.IsActive && !x.IsDeleted));

            return query.ToList();
        }


        public IList<SelectListItem> GetAllCompanyDepartmentsForDropDownList(bool showInactive = false, bool showHidden = false)
        {
            var query = _companyDepartmentRepository.Table;

            // active
            if (!showInactive)
                query = query.Where(d => d.IsActive == true);

            // deleted
            if (!showHidden)
                query = query.Where(d => d.IsDeleted == false);

            return query.Select(x => new SelectListItem() { Text = x.DepartmentName, Value = x.Id.ToString() }).ToList();
        }
        #endregion

    }
}
