using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Companies
{
    public class PlacementDetailsModel
    {
        public Guid CandidateGuid { get; set; }
        public int CandidateId { get; set; }
        public string EmployeeId { get; set; }
        public string CandidateName { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public IList<string> Skills { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string LastJobOrder { get; set; }
        public int LastCaniddateJobOrderId { get; set; }
        public int CurrentJobOrderId { get; set; }
        public int CurrentCandidateJobOrderId { get; set; }
        public int AvailableDays { get; set; }
        public bool Editable { get; set; }
    }
}
