using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Core.Domain.TimeSheet;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Web.Models.TimeSheet
{
    public class CandidateWorkTimeModel : BaseWfmEntityModel
    {
        public Guid UniqueId { get; set; }

        public int CandidateId { get; set; }
        public int JobOrderId { get; set; }

        public int CompanyId { get; set; }
        public int CompanyLocationId { get; set; }
        public int CompanyDepartmentId { get; set; }
        public int CompanyContactId { get; set; }

        public DateTime JobStartDateTime { get; set; }
        public DateTime JobEndDateTime { get; set; }

        public DateTime? ClockIn { get; set; }

        public DateTime? ClockOut { get; set; }

        [Range(typeof(decimal), "0.00", "1000.00")]
        public decimal NetWorkTimeInHours { get; set; }

        public int CandidateWorkTimeStatusId { get; set; }

        public DateTime StartDate { get { return JobStartDateTime.Date; } }
        public DateTime StartTime { get { return JobStartDateTime; } }

        public bool ReadOnly { get; set; }


        public CandidateWorkTimeModel()
        {
            UniqueId = Guid.NewGuid();
        }
    }
}