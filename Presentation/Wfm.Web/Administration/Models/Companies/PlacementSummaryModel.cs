using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Companies
{
    public class PlacementSummaryModel
    {
        public Guid JobOrderGuid { get; set; }
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Depatment { get; set; }
        public string Position { get; set; }
        public string Shift { get; set; }
        public string Supervisor { get; set; }
        public DateTime RefDate { get; set; }
        
        //[Range(0, int.MaxValue)]
        [UIHint("RangedInteger")]
        public int Opening { get; set; }

        public int Placed { get; set; }
        public int Shortage { get { return Opening - Placed; } }
        public bool CanSendEmail { get; set; }
    }
}
