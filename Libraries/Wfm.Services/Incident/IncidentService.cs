using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Incident;
using Wfm.Services.Events;

namespace Wfm.Services.Incident
{
    public class IncidentService : IIncidentService
    {
        #region Constants
        #endregion

        #region Fields

        private readonly IRepository<IncidentCategory> _incidentCategoryRepository;
        private readonly IRepository<IncidentReportTemplate> _incidentReportTemplateRepository;
        private readonly IRepository<IncidentReport> _incidentReportRepository;
        private readonly IRepository<IncidentReportFile> _incidentReportFileRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        #endregion 

        #region Ctor
        public IncidentService(ICacheManager cacheManager,
            IRepository<IncidentCategory> incidentCategoryRepository,
            IRepository<IncidentReportTemplate> incidentReportTemplateRepository,
            IRepository<IncidentReport> incidentReportRepository,
            IRepository<IncidentReportFile> incidentReportFileRepository,
            IRepository<Company> companyRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _incidentCategoryRepository = incidentCategoryRepository;
            _incidentReportTemplateRepository = incidentReportTemplateRepository;
            _incidentReportRepository = incidentReportRepository;
            _incidentReportFileRepository = incidentReportFileRepository;
            _companyRepository = companyRepository;
            _eventPublisher = eventPublisher;
        }
        #endregion

        #region CRUD - Cateogory and Template
        public IQueryable<IncidentCategory> GetAllIncidentCategories(Account account, bool activeOnly = false)
        {
            return _incidentCategoryRepository.Table
                .Where(x => !x.FranchiseId.HasValue || (x.FranchiseId == account.FranchiseId && account.IsLimitedToFranchises) || !account.IsLimitedToFranchises)
                .Where(x => !activeOnly || x.IsActive);
        }

        public void InsertNewCategory(IncidentCategory model)
        {
            model.CreatedOnUtc = model.UpdatedOnUtc = DateTime.UtcNow;
            _incidentCategoryRepository.Insert(model);
        }

        public void UpdateCategory(IncidentCategory model)
        {
            model.UpdatedOnUtc = DateTime.UtcNow;
            _incidentCategoryRepository.Update(model);
        }
        public IQueryable<IncidentReportTemplate> GetIncidentTemplates(int incidentCategoryId, bool activeOnly = false)
        {
            return _incidentReportTemplateRepository.Table
                .Where(x => x.IncidentCategoryId == incidentCategoryId)
                .Where(x => !activeOnly || x.IsActive);
        }
        public IQueryable<IncidentReportTemplate> GetIncidentTemplatesForCompany(int companyId, bool activeOnly = false)
        {
            return _incidentReportTemplateRepository.Table
                .Include(x => x.IncidentCategory)
                .Where(x => !activeOnly || x.IsActive);
        }
        public IncidentReportTemplate GetIncidentReportTemplate(int templateId)
        {
            return _incidentReportTemplateRepository.GetById(templateId);
        }
        public void InsertIncidentReportTemplate(IncidentReportTemplate template)
        {
            template.UpdatedOnUtc = template.CreatedOnUtc = DateTime.UtcNow;
            _incidentReportTemplateRepository.Insert(template);
        }
        public void RemoveIncidentReportTemplate(int templateId)
        {
            _incidentReportTemplateRepository.Delete(_incidentReportTemplateRepository.GetById(templateId));
        }
        public void UpdateIncidentReportTemplate(IncidentReportTemplate template)
        {
            template.UpdatedOnUtc = DateTime.UtcNow;
            _incidentReportTemplateRepository.Update(template);
        }
        #endregion

        #region Incident Report/File
        public IQueryable<IncidentReport> GetCandidateIncidentReports(int candidateId)
        {
            return _incidentReportRepository.Table
                .Include(x => x.IncidentCategory)
                .Include(x => x.Candidate)
                .Include(x => x.Company)
                .Include(x => x.JobOrder)
                .Include(x => x.Location)
                .Include(x => x.ReportedByAccount)
                .Where(x => x.CandidateId == candidateId);
        }
        public IncidentReport GetIncidentReport(int incidentReportId)
        {
            return _incidentReportRepository.Table
                .Include(x => x.IncidentCategory)
                .Include(x => x.Candidate)
                .Include(x => x.Company)
                .Include(x => x.JobOrder)
                .Include(x => x.Location)
                .Include(x => x.ReportedByAccount)
                .Where(x => x.Id == incidentReportId).First();
        }
        public IQueryable<IncidentReport> GetCompanyIncidentReports(int companyId)
        {
            return _incidentReportRepository.Table
                .Include(x => x.IncidentCategory)
                .Include(x => x.Candidate)
                .Include(x => x.Company)
                .Include(x => x.JobOrder)
                .Include(x => x.Location)
                .Include(x => x.ReportedByAccount)
                .Where(x => x.CompanyId == companyId);
        }
        public void InsertIncidentReport(IncidentReport report)
        {
            report.CreatedOnUtc = report.UpdatedOnUtc = DateTime.UtcNow;
            _incidentReportRepository.Insert(report);
        }
        public void UpdateIncidentReport(IncidentReport report)
        {
            report.UpdatedOnUtc = DateTime.UtcNow;
            _incidentReportRepository.Update(report);
        }
        public void DeleteIncidentReport(int reportId)
        {
            var report = this.GetIncidentReport(reportId);
            var files = GetIncidentReportFiles(reportId);
            _incidentReportFileRepository.Delete(files);
            _incidentReportRepository.Delete(report);
        }
        public IQueryable<IncidentReportFile> GetIncidentReportFiles(int incidentReportId)
        {
            return _incidentReportFileRepository.Table.Where(x => x.IncidentReportId == incidentReportId);
        }
        public IncidentReportFile GetIncidentReportFile(int incidentReportFileId)
        {
            return _incidentReportFileRepository.Table.Where(x => x.Id == incidentReportFileId).First();
        }
        public void InsertIncidentReportFile(IncidentReportFile file)
        {
            file.CreatedOnUtc = file.UpdatedOnUtc = DateTime.UtcNow;
            _incidentReportFileRepository.Insert(file);
        }
        public void RemoveIncidentReportFile(int fileId)
        {
            _incidentReportFileRepository.Delete(_incidentReportFileRepository.GetById(fileId));
        }

        #endregion

        #region Method for Incident Category
        public IncidentCategory GetIncidentCategoryByCategoryId(int categoryId)
        {
             return _incidentCategoryRepository.TableNoTracking.Where(x => x.Id == categoryId).FirstOrDefault();
        }
        #endregion
    }
}
