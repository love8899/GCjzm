using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Payroll
{
    public class PayrollCalendarModel : BaseWfmEntityModel
    {
        [WfmResourceDisplayName("Common.PayGroup")]
        public int PayGroupId { get; set; }

        [WfmResourceDisplayName("Common.Number")]
        public int PayPeriodNumber { get; set; }

        [WfmResourceDisplayName("Common.PayPeriodStartDate")]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime PayPeriodStartDate { get; set; }

        [WfmResourceDisplayName("Common.PayPeriodEndDate")]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime PayPeriodEndDate { get; set; }

        [WfmResourceDisplayName("Common.PayDate")]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime PayPeriodPayDate { get; set; }

        [WfmResourceDisplayName("Payroll.LastCommitDate")]
        public Nullable<DateTime> PayPeriodCommitDate { get; set; }

        public int Year { get; set; }
    }
}