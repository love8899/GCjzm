using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Services.Accounts;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.ExportImport;
using Wfm.Services.TimeSheet;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Companies
{
    public class DailyAttendance_BL
    {
        public IQueryable<DailyAttendanceModel>  GetDailyAttendance(ICandidateJobOrderService _candidateJobOrderService, 
                                                                    IAccountService _accountService, 
                                                                    ICompanyContactService _companyContactService,
                                                                    IRepository<CandidateWorkTime> _workTimeRepository, 
                                                                    Account account, int companyId, DateTime refDate)
        {
            var placement = _candidateJobOrderService.GetCandidateJobOrdersByCompanyIdAndDateAsQueryable(companyId, refDate);
            if (account.IsLimitedToFranchises)
                placement = placement.Where(x => x.JobOrder.FranchiseId == account.FranchiseId);
            var supervisors = _companyContactService.GetCompanyContactsAsQueryable();

            var result = from p in placement
                         from s in supervisors.Where(s => s.Id == p.JobOrder.CompanyContactId).DefaultIfEmpty()
                         select new DailyAttendanceModel
                         {
                             JobOrderId = p.JobOrderId,
                             JobOrderGuid = p.JobOrder.JobOrderGuid,
                             JobTitle = p.JobOrder.JobTitle,
                             Supervisor = s == null ? null : s.FirstName + " " + s.LastName,
                             CandidateId = p.CandidateId,
                             CandidateGuid = p.Candidate.CandidateGuid,
                             FirstName = p.Candidate.FirstName,
                             LastName = p.Candidate.LastName,
                             StartDate = refDate,
                             ShiftStart = p.JobOrder.StartTime,
                             ShiftEnd = p.JobOrder.EndTime,
                             CandidateJobOrderId=p.Id,
                            EmployeeId = p.Candidate.EmployeeId
                         };
            
            return result;
        }

        
        public FileContentResult GetDailyAttendantList(ICandidateService _candidateService, 
                                                       IExportManager _exportManager,
                                                       Account account, int companyId, DateTime refDate, int[] ids)
        {
            string fileName = string.Empty;
            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                fileName = _exportManager.ExportDialyAttendantListToXlsx(stream, companyId, refDate, ids);
                bytes = stream.ToArray();
            }

            var file = new FileContentResult(bytes, "text/xls");
            file.FileDownloadName = fileName;

            return file;
        }

    }

}
