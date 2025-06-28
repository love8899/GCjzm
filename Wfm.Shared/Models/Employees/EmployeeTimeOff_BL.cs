using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Core.Domain.Employees;
using Wfm.Services.Candidates;
using Wfm.Services.Employees;
using Wfm.Services.Scheduling;
using Wfm.Shared.Mapping;
using Wfm.Shared.Models.Scheduling;

namespace Wfm.Shared.Models.Employees
{
    public class EmployeeTimeOff_BL
    {
        private readonly ICandidateService _candidateService;
        private readonly ITimeoffService _timeoffService;
        private readonly IEmployeeService _employeeService;
        private readonly ISchedulingDemandService _schedulingDemandService;
        public EmployeeTimeOff_BL(ICandidateService candidateService, ITimeoffService timeoffService, IEmployeeService employeeService, ISchedulingDemandService schedulingDemandService)
        {
            _candidateService = candidateService;
            _timeoffService = timeoffService;
            _employeeService = employeeService;
            _schedulingDemandService = schedulingDemandService;
        }
        public EmployeeTimeoffBookingModel BookNewTimeoffPopup(Guid employeeId, int timeoffTypeId)
        {
            var candidateId = _candidateService.GetCandidateByGuidForClient(employeeId).Id;
            var model = new EmployeeTimeoffBookingModel()
            {
                EmployeeIntId = candidateId,
                EmployeeName = _employeeService.GetEmployeeByCandidateId(candidateId).ToString(),
                TimeoffTypeList = _timeoffService.GetAllTimeoffTypes(true)
                    .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == timeoffTypeId }).ToArray(),
                TimeOffStartDateTime = DateTime.Today,
                TimeOffEndDateTime = DateTime.Today,
            };
            return model;
        }
        public EmployeeSchedulePreviewModel[] GetEmployeeScheduleForTimeoffBooking(EmployeeTimeoffBookingModel model)
        {
            var employeeGuid = _employeeService.GetEmployeeByCandidateId(model.EmployeeIntId).CandidateGuid;
            var _model = _schedulingDemandService.GetEmployeeScheduleBaseline(employeeGuid);
            var schedule = _model.ToArray().SelectMany(x => EmployeeSchedulePreviewModel.FromShift(x).OrderBy(y => y.Title))
                .Where(x => (x.Start >= model.TimeOffStartDateTime && x.Start <= model.TimeOffEndDateTime) ||
                    (x.End >= model.TimeOffStartDateTime && x.End <= model.TimeOffEndDateTime))
                    .ToArray();
            var overridings = _schedulingDemandService.GetEmployeeScheduleOverride(employeeGuid).ToArray()
                .Where(x => (x.ScheduelDate.Add(x.StartTimeOfDay) >= model.TimeOffStartDateTime && x.ScheduelDate.Add(x.StartTimeOfDay) <= model.TimeOffEndDateTime) ||
                    (x.ScheduelDate.Add(x.EndTimeOfDay) >= model.TimeOffStartDateTime && x.ScheduelDate.Add(x.EndTimeOfDay) <= model.TimeOffEndDateTime))
                    .Select(x => x.ToModel()).ToArray();
            return EmployeeSchedulePreviewModel.MergeWithEmployeeScheduleDaily(schedule, overridings).ToArray();
        }
    }
}
