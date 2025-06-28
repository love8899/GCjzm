using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wfm.Admin.Extensions;
using Wfm.Core;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.DirectoryLocation;
using Wfm.Services.JobOrders;
using Wfm.Services.TimeSheet;


namespace Wfm.Admin.Models.TimeSheet
{
    public class CandidateAttendanceModel_BL
    {
        public DataSourceResult GetAllAttendanceList(DateTime? firstDateOfWeek, int? companyId, IJobOrderService _jobOrderService, IWorkContext _workContext, IRecruiterCompanyService _recruiterCompanyService)
        {
            DateTime startDate = firstDateOfWeek ?? DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
            //int theYear = CommonHelper.GetYear(startDate);
            //int theWeek = CommonHelper.GetWeekNumber(startDate);
            DateTime endDate = startDate.AddDays(+6);
            var attendancePreview = _jobOrderService.GetAttendancePreviewByDate(startDate, endDate, companyId);
            var gridModel = new DataSourceResult()
            {
                Data = attendancePreview,
                Total = attendancePreview.Count(),
            };
            return gridModel;
        }


        public IEnumerable<SelectListItem> GetCompanyList(IWorkContext _workContext, ICompanyService _companyService, IRecruiterCompanyService _recruiterCompanyService)
        {
            var companies = _companyService.Secure_GetAllCompanies(_workContext.CurrentAccount, true)
                .OrderBy(x => x.CompanyName)
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem
                            {
                                Value = x.Id.ToString(),
                                Text = x.CompanyName
                            });

            return companies;
        }


        public IList<CandidateWorkTimeModel> GetJobOrderWorkTime(
            IStateProvinceService _stateProvinceService, ICompanyDivisionService _companyLocationService, ICandidateJobOrderService _candidateJobOrderService, 
            IWorkTimeService _workTimeService, ICandidateService _candidatesService, Wfm.Core.Domain.JobOrders.JobOrder jo, DateTime firstDateOfWeek)
        {
            var firstDate = firstDateOfWeek;
            var lastDate = firstDate.AddDays(6);
            var allCandidates = _workTimeService.GetJobOrderPlacedCandidateIds(jo.Id, firstDate);
            var allWorkTimes = new List<CandidateWorkTimeModel>();

            var location = _companyLocationService.GetCompanyLocationById(jo.CompanyLocationId);
            var holidays = _stateProvinceService.GetAllStatutoryHolidaysOfStateProvince(location != null ? location.StateProvinceId : 0)
                           .Where(x => x.HolidayDate >= firstDate && x.HolidayDate <= lastDate).Select(x => x.HolidayDate);
            bool[] dailySwitches = {jo.SundaySwitch && (jo.IncludeHolidays || !holidays.Contains(firstDate)), 
                                    jo.MondaySwitch && (jo.IncludeHolidays || !holidays.Contains(firstDate.AddDays(1))), 
                                    jo.TuesdaySwitch && (jo.IncludeHolidays || !holidays.Contains(firstDate.AddDays(2))), 
                                    jo.WednesdaySwitch && (jo.IncludeHolidays || !holidays.Contains(firstDate.AddDays(3))), 
                                    jo.ThursdaySwitch && (jo.IncludeHolidays || !holidays.Contains(firstDate.AddDays(4))), 
                                    jo.FridaySwitch && (jo.IncludeHolidays || !holidays.Contains(firstDate.AddDays(5))), 
                                    jo.SaturdaySwitch && (jo.IncludeHolidays || !holidays.Contains(lastDate))};

            foreach (int candidateId in allCandidates)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (firstDate.AddDays(i) > DateTime.Now.Date)
                        break;

                    if (!_candidateJobOrderService.IsCandidatePlacedInJobOrderWithinDateRange(jo.Id, candidateId, firstDate.AddDays(i), firstDate.AddDays(i)))
                        continue;

                    var cwt = _workTimeService.GetWorkTimeByCandidateIdAndJobOrderIdAndJobStartDate(candidateId, jo.Id, firstDate.AddDays(i));

                    if (cwt != null)
                    {
                        cwt.JobStartDateTime = firstDate.AddDays(i).Date + jo.StartTime.TimeOfDay;
                        cwt.JobEndDateTime = firstDate.AddDays(i).Date + jo.EndTime.TimeOfDay;
                        if (jo.EndTime < jo.StartTime)
                            cwt.JobEndDateTime = cwt.JobEndDateTime.AddDays(1);
                        cwt.NetWorkTimeInHours = cwt.NetWorkTimeInHours < 0 ? 0 : cwt.NetWorkTimeInHours;
                        if (cwt.ClockIn != null && cwt.ClockOut != null)
                        {
                            var inTime = (DateTime)cwt.ClockIn;
                            var outTime = (DateTime)cwt.ClockOut;
                            if (outTime.Subtract(inTime).TotalMinutes < 3)
                                if (Math.Abs((decimal)cwt.JobStartDateTime.Subtract(inTime).TotalMinutes) <
                                    Math.Abs((decimal)cwt.JobEndDateTime.Subtract(outTime).TotalMinutes))
                                    cwt.ClockOut = null;
                                else
                                    cwt.ClockIn = null;
                        }
                        allWorkTimes.Add(cwt.ToModel());
                    }
                    else if (dailySwitches[i])
                    {
                        var candidate = _candidatesService.GetCandidateById(candidateId);
                        if (candidate != null)
                            allWorkTimes.Add(new CandidateWorkTimeModel()
                            {
                                CandidateId = candidateId,
                                CandidateGuid = candidate.CandidateGuid,
                                JobOrderId = jo.Id,
                                JobOrderGuid = jo.JobOrderGuid,
                                CompanyId = jo.CompanyId,
                                CompanyName = jo.Company.CompanyName,
                                CompanyLocationId = jo.CompanyLocationId,
                                CompanyDepartmentId = jo.CompanyDepartmentId,
                                CompanyContactId = jo.CompanyContactId,
                                Year = CommonHelper.GetYearAndWeekNumber(firstDate, out int weekOfYear),
                                WeekOfYear = weekOfYear,
                                JobTitle = jo.JobTitle,
                                ShiftId = jo.ShiftId,
                                JobStartDateTime = firstDate.AddDays(i).Date + jo.StartTime.TimeOfDay,
                                JobEndDateTime = firstDate.AddDays(jo.EndTime.TimeOfDay > jo.StartTime.TimeOfDay ? i : i + 1).Date + jo.EndTime.TimeOfDay,
                                EmployeeId = candidate.EmployeeId,
                                EmployeeFirstName = candidate.FirstName,
                                EmployeeLastName = candidate.LastName,
                            });
                    }
                }
            }

            return allWorkTimes;
        }
    
    }
}
