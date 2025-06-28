using System;
using System.ComponentModel.DataAnnotations;
using Kendo.Mvc.UI;
using Wfm.Core.Domain.Candidates;
using Wfm.Web.Framework;


namespace Wfm.Admin.Models.Candidate
{
    public class CandidateScheduleModel : ISchedulerEvent
    {
        public int CandidateJobOrderId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid candidate")]
        public int CandidateId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public int ContactId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid job order")]
        public int JobOrderId { get; set; }
        public Guid? JobOrderGuid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string RecruiterName { get; set; }

        private DateTime start;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a valid start date")]
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;
            }
        }

        //public DateTime Start { get; set; }

        public string StartTimezone { get; set; }

        private DateTime end;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
            }
        }

        //public DateTime End { get; set; }

        public string EndTimezone { get; set; }

        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        [WfmResourceDisplayName("Common.IsFullDay")]
        public bool IsAllDay { get; set; }
        public string Timezone { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid status")]
        public int StatusId { get; set; }
        public string Status { get { return Enum.GetName(typeof(CandidateJobOrderStatusEnum), StatusId); } }

        public bool ReadOnly { get; set; }
        public int AvailableOpening { get; set; }
        public CandidateJobOrder ToEntity()
        {
            return new CandidateJobOrder
            {
                Id = CandidateJobOrderId,
                CandidateId = CandidateId,
                JobOrderId = JobOrderId,
                CandidateJobOrderStatusId = (int)CandidateJobOrderStatusEnum.Placed,
                StartDate = Start,
                EndDate = End
            };
        }
    }
}
