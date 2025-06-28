using Wfm.Core.Domain.TimeSheet;

namespace Wfm.Admin.Models.TimeSheet
{
    public class EditCandidateWorkTimeAdjustmentModel_BL
    {
        public EditCandidateWorkTimeAdjustmentModel GetEditCandidateWorkTimeAdjustmentModel(CandidateWorkTime workTime, int id)
        {
            var model = new EditCandidateWorkTimeAdjustmentModel()
            {
                CandidateWorkTimeId = id,
                GrossWorkTimeInMinutes = workTime.GrossWorkTimeInMinutes,
                GrossWorkTimeInHours = workTime.GrossWorkTimeInHours,
                NetWorkTimeInMinutes = workTime.NetWorkTimeInMinutes,
                NetWorkTimeInHours = workTime.NetWorkTimeInHours,
                AdjustmentInMinutes = workTime.AdjustmentInMinutes,
                Note = workTime.Note,

                CandidateWorkTimeStatusId = workTime.CandidateWorkTimeStatusId,
                EmployeeLastName = workTime.Candidate.LastName,
                EmployeeFirstName = workTime.Candidate.FirstName,
                CompanyName = workTime.JobOrder.Company.CompanyName,
                JobTitle = workTime.JobOrder.JobTitle,
                JobStartDateTime = workTime.JobStartDateTime,
                JobEndDateTime = workTime.JobEndDateTime,
            };
            return model;
        }
    }
}