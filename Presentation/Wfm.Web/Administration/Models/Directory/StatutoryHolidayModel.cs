using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Directory
{
    public class StatutoryHolidayModel : BaseWfmEntityModel
    {
        public int StateProvinceId { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.StatutoryHoliday.Fields.StatutoryHolidayName")]
        public string StatutoryHolidayName { get; set; }
        [WfmResourceDisplayName("Admin.Configuration.StatutoryHoliday.Fields.HolidayDate")]
        [UIHint("DateNullable")]
        public DateTime HolidayDate { get; set; }
        [WfmResourceDisplayName("Common.Note")]
        public string Note { get; set; }
        [WfmResourceDisplayName("Common.IsActive")]
        public bool IsActive { get; set; }
        [WfmResourceDisplayName("Common.EnteredBy")]
        public int EnteredBy { get; set; }
        
        [WfmResourceDisplayName("Common.Year")]
        public int Year
        {
            get
            {
                return HolidayDate.Year;
            }
        }
    }
}
