using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Employees
{
    public class EmployeeTimeoffBookingHistoryModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.EmployeeName")]
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int? EmployeeTimeoffBalanceId { get; set; }
        [WfmResourceDisplayName("Common.Type")]
        public string EmployeeTimeoffTypeName { get; set; }
        [WfmResourceDisplayName("Admin.Employee.TimeoffEntitlement.Fields.BookedHours")]
        public decimal BookedTimeoffInHours { get; set; }
        public decimal ApprovedTimeoffInHours { get; set; }
        public bool? IsRejected { get; set; }
        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime TimeOffStartDateTime { get; set; }
        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime TimeOffEndDateTime { get; set; }
        [WfmResourceDisplayName("Common.EnteredBy")]
        public string BookedByAccountName { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        public int? ApprovedByAccountId { get; set; }
        public bool CanEdit
        {
            get
            {
                return true; // TimeOffStartDateTime >= DateTime.Today.AddDays(1);
            }
        }

        [WfmResourceDisplayName("Admin.Employee.TimeoffEntitlement.Fields.LatestBalanceInHours")]
        public decimal? LatestBalanceInHours { get; set; }
    }
}
