using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Wfm.Shared.Validators;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Employees
{
    [Validator(typeof(EmployeeTimeoffBookingValidator))]
    public class EmployeeTimeoffBookingModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.EmployeeName")]
        public string EmployeeName { get; set; }
        [WfmResourceDisplayName("Common.EmployeeId")]
        public int EmployeeIntId { get; set; }
        [WfmResourceDisplayName("Common.Type")]
        public int EmployeeTimeoffTypeId { get; set; }
        [WfmResourceDisplayName("Common.StartDate")]
        public DateTime TimeOffStartDateTime { get; set; }
        [WfmResourceDisplayName("Common.EndDate")]
        public DateTime TimeOffEndDateTime { get; set; }
        
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        public int BookedByAccountId { get; set; }
        //
        public IEnumerable<SelectListItem> TimeoffTypeList { get; set; }
    }
}
