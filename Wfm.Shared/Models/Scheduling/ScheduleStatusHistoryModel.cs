using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Scheduling
{
    public class ScheduleStatusHistoryModel : BaseWfmEntityModel
    {
        public int SchedulePeriodId { get; set; }
        public int CompanyShiftId { get; set; }
        public int JobRoleId { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public int? SubmittedById { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? ApprovedById { get; set; }
        public DateTime? RevisionRequestedDate { get; set; }
        public int? RevisionRequestedById { get; set; }
        public DateTime? ResubmittedDate { get; set; }
        public int? ResubmittedById { get; set; }
        public ScheduleStatus ScheduleStatus
        {
            get
            {
                return ApprovedById.HasValue ? ScheduleStatus.Approved :
                    (ResubmittedById.HasValue ? ScheduleStatus.Resubmitted :
                    (RevisionRequestedById.HasValue ? ScheduleStatus.RevisionRequested :
                    (SubmittedById.HasValue ? ScheduleStatus.Submitted : ScheduleStatus.Draft)));
            }
        }
    }

    public enum ScheduleStatus
    {
        Draft,
        Submitted,
        Approved,
        RevisionRequested,
        Resubmitted,
    }
}
