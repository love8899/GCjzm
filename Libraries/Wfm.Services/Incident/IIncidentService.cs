using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Incident;

namespace Wfm.Services.Incident
{
    public interface IIncidentService
    {
        #region CRUD - Cateogory and Template
        void InsertNewCategory(IncidentCategory model);
        void UpdateCategory(IncidentCategory model);
        IQueryable<IncidentCategory> GetAllIncidentCategories(Account account, bool activeOnly = false);
        IQueryable<IncidentReportTemplate> GetIncidentTemplates(int incidentCategoryId, bool activeOnly = false);
        IQueryable<IncidentReportTemplate> GetIncidentTemplatesForCompany(int companyId, bool activeOnly = false);
        IncidentReportTemplate GetIncidentReportTemplate(int templateId);
        void InsertIncidentReportTemplate(IncidentReportTemplate template);
        void RemoveIncidentReportTemplate(int templateId);
        void UpdateIncidentReportTemplate(IncidentReportTemplate template);
        #endregion

        #region CRUD - Report and File
        IQueryable<IncidentReport> GetCandidateIncidentReports(int candidateId);
        IQueryable<IncidentReport> GetCompanyIncidentReports(int companyId);
        IncidentReport GetIncidentReport(int incidentReportId);
        void InsertIncidentReport(IncidentReport report);
        void UpdateIncidentReport(IncidentReport report);
        void DeleteIncidentReport(int reportId);
        IQueryable<IncidentReportFile> GetIncidentReportFiles(int incidentReportId);
        void InsertIncidentReportFile(IncidentReportFile file);
        IncidentReportFile GetIncidentReportFile(int incidentReportFileId);
        void RemoveIncidentReportFile(int fileId);
        #endregion

        #region Method for Category 
        IncidentCategory GetIncidentCategoryByCategoryId(int categoryId);
        #endregion
    }
}
