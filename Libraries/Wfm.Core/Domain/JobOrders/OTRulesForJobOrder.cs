using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.JobOrders
{
    public class OTRulesForJobOrder
    {
        public int JobOrderId { get; set; }
        public int OvertimeRuleSettingId { get; set; }
        public string OvertimeCode { get; set; }
        public string ProvinceCode { get; set; }
        public decimal Threshold { get; set; }
        public decimal OvertimeRate { get; set; }
        public int PayrollItemId { get; set; }
        public int OvertimeTypeId { get; set; }
        public decimal? OvertimeHoursCeiling { get; set; }
        public decimal? RegularHoursCeiling { get; set; }
        public bool UseLastDayOTRate { get; set; }
    }
}
