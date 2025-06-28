using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Companies;

namespace Wfm.Services.Companies
{
    public class CompanyEmailTemplateService:ICompanyEmailTemplateService
    {
        #region Field
        private readonly IRepository<CompanyEmailTemplate> _companyEmailTemplateRepository;

        #endregion

        #region Ctor
        public CompanyEmailTemplateService(IRepository<CompanyEmailTemplate> companyEmailTemplateRepository)
        {
            _companyEmailTemplateRepository = companyEmailTemplateRepository;
        }
        #endregion

        #region CRUD
        public void Create(CompanyEmailTemplate entity)
        {
            if(entity==null)
                throw new ArgumentNullException("CompanyEmailTemplate");
            if (entity.AttachmentFile.Length <= 0)
                entity.AttachmentFile = null;
            if (entity.AttachmentFile2.Length <= 0)
                entity.AttachmentFile2 = null;
            if (entity.AttachmentFile3.Length <= 0)
                entity.AttachmentFile3 = null;
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _companyEmailTemplateRepository.Insert(entity);
        }

        public CompanyEmailTemplate Retrieve(int id)
        {
            var emailTemplate = _companyEmailTemplateRepository.GetById(id);
            return emailTemplate;
        }

        public void Update(CompanyEmailTemplate entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyEmailTemplate");
            if (entity.AttachmentFile.Length <= 0)
                entity.AttachmentFile = null;
            if (entity.AttachmentFile2.Length <= 0)
                entity.AttachmentFile2 = null;
            if (entity.AttachmentFile3.Length <= 0)
                entity.AttachmentFile3 = null;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            _companyEmailTemplateRepository.Update(entity);
        }

        public void Delete(CompanyEmailTemplate entity)
        {
            if (entity == null)
                throw new ArgumentNullException("CompanyEmailTemplate");
            entity.UpdatedOnUtc = DateTime.UtcNow;
            entity.IsDeleted = true;
            _companyEmailTemplateRepository.Update(entity);
        }
        #endregion

        #region Method
        public IQueryable<CompanyEmailTemplate> GetAllEmailTemplateByCompanyId(int companyId)
        {
            var result = _companyEmailTemplateRepository.TableNoTracking.Where(x => x.CompanyId == companyId&&!x.IsDeleted);
            return result;
        }
        public IQueryable<CompanyEmailTemplate> GetAllEmailTemplate()
        {
            var result = _companyEmailTemplateRepository.TableNoTracking.Where(x => !x.IsDeleted);
            return result;
        }
        public IList<SelectListItem> GetAllEmailTemplateAsSelectedList()
        {
            var templates = GetAllEmailTemplate().ToList();
            List<SelectListItem> result = new List<SelectListItem>();
            StringBuilder text = new StringBuilder();
            foreach (var template in templates)
            {
                SelectListItem item = new SelectListItem();
                text.Clear();
                text.Append(Enum.GetName(typeof(CompanyEmailTemplateType), template.Type));
                text.AppendFormat(" - {0}",template.Company.CompanyName);
                if (template.CompanyLocation != null)
                    text.AppendFormat(" - {0}", template.CompanyLocation.LocationName);
                if (template.CompanyDepartment != null)
                    text.AppendFormat(" - {0}", template.CompanyDepartment.DepartmentName);
                item.Text = text.ToString();
                item.Value = String.Concat("CMP-", template.Id.ToString());
                result.Add(item);
            }
            //var result = GetAllEmailTemplate().Select(x => new SelectListItem() { Text = String.Concat(x.type, ": ", x.Subject), Value =String.Concat("CMP-", x.Id.ToString()) }).ToList();
            return result;
        }

        public CompanyEmailTemplate GetEmailTemplate(int type, int companyId, int locationId, int departmentId)
        {
            var result = _companyEmailTemplateRepository.TableNoTracking.Where(x => x.Type == type && x.CompanyId == companyId&&!x.IsDeleted);
            var companyTemplate = result.FirstOrDefault();
            result = result.Where(x => x.CompanyLocationId == locationId);
            var locationTemplate = result.FirstOrDefault();

            if (departmentId != 0)
            {
                result = result.Where(x => x.CompanyDepartmentId == departmentId);
                var departmentTemplate = result.FirstOrDefault();
                if (departmentTemplate != null)
                    return departmentTemplate;
                
            }
            
            if (locationTemplate != null)
                return locationTemplate;
            else if (companyTemplate != null)
                return companyTemplate;
            else
                return null;

        }

        public bool DuplicateEmailTemplate(CompanyEmailTemplate entity)
        {
            var result = _companyEmailTemplateRepository.TableNoTracking.Where(x => x.CompanyId == entity.CompanyId&& x.Type == entity.Type&&!x.IsDeleted).ToList();
            if (entity.CompanyDepartmentId != null)
                result = result.Where(x => x.CompanyLocationId == entity.CompanyLocationId && x.CompanyDepartmentId == entity.CompanyDepartmentId).ToList();
            else
            {
                if (entity.CompanyLocationId != null)
                    result = result.Where(x => x.CompanyLocationId == entity.CompanyLocationId&&x.CompanyDepartmentId==null).ToList();
                else
                    result = result.Where(x => x.CompanyLocationId==null && x.CompanyDepartmentId==null).ToList();
                
            }
            if (result.Count() <= 0)
                return false;
            else
                if (entity.Id > 0)
                {
                    //update
                    result = result.Where(x => x.Id != entity.Id).ToList();
                    return result.Count()>0;
                }
                else
                { 
                    //create 
                    return true;
                }
            
        }
        #endregion
    }
}
