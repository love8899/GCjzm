using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Employees
{
    public class EmployeeTimeoffBalanceModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.EmployeeId")]
        public int EmployeeId { get; set; }
        [WfmResourceDisplayName("Common.Year")]
        public int Year { get; set; }
        [WfmResourceDisplayName("Common.Type")]
        public int EmployeeTimeoffTypeId { get; set; }
        [WfmResourceDisplayName("Admin.Employee.TimeoffEntitlement.Fields.EntitledTimeoffInHours")]
        public decimal EntitledTimeoffInHours { get; set; }
        [WfmResourceDisplayName("Admin.EmployeeTimeoffType.Field.AllowNegative")]
        public bool AllowNegative { get; set; }
        [WfmResourceDisplayName("Admin.Employee.TimeoffEntitlement.Fields.LatestBalanceInHours")]
        public decimal? LatestBalanceInHours { get; set; }
        [WfmResourceDisplayName("Admin.Employee.TimeoffEntitlement.Fields.EmployeeTimeoffTypeName")]
        public virtual string EmployeeTimeoffTypeName { get; set; }
        [WfmResourceDisplayName("Common.EmployeeName")]
        public virtual string EmployeeName { get; set; }
        [WfmResourceDisplayName("Admin.Employee.TimeoffEntitlement.Fields.BookedHours")]
        public decimal? BookedHours
        {
            get
            {
                return EntitledTimeoffInHours - LatestBalanceInHours;
            }
        }
    }
}
