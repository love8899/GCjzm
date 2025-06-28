using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Common
{
    public class CandidatePipelineSimpleModel : BaseWfmEntityModel
    {
        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }
        public string EmployeeId { get; set; }
        public int JobOrderId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get { return String.Concat(LastName, " ", FirstName); } }

        public int AvailableShiftId { get; set; }

        public string HomePhone { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public int? RatingValue { get; set; }
        public string RatingComment { get; set; }
        public bool IsArrivedToday { get; set; }
        public decimal? JobDuration { get; set; }
        public string StatusName { get; set; }
        public int CandidateJobOrderId { get; set; }

        public decimal? TotalHours { get; set; }    // total hours in last 1 year

        public string ToggleButtonText
        {
            get
            {
                return StatusName.Equals("Placed", StringComparison.OrdinalIgnoreCase) ? "Deactivate" : "Activate";
            }
        }
        public string StatusDisplayText
        {
            get
            {
                switch (StatusName.ToLower())
                {
                    case "placed":
                        return "Placed";
                    default:
                        return "Standby";
                }
            }
        }
    }
}