using System;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Kendo.Mvc.UI;
using Wfm.Client.Validators.Employees;
using Wfm.Core.Domain.Employees;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Wfm.Client.Models.Employees
{
    [Validator(typeof(EmployeeAvailabilityValidator))]
    public class EmployeeAvailabilityModel : BaseWfmEntityModel, ISchedulerEvent
    {
        [WfmResourceDisplayName("Common.EmployeeId")]
        public int EmployeeIntId { get; set; }
        [WfmResourceDisplayName("Admin.Accounts.WorktimePreference.Fields.EmployeeAvailabilityType")]
        public EmployeeAvailabilityType EmployeeAvailabilityType { get; set; }
        [WfmResourceDisplayName("Admin.Accounts.WorktimePreference.Fields.DayOfWeek")]
        public DayOfWeek DayOfWeek { get; set; }
        [WfmResourceDisplayName("Common.IsFullDay")]
        public bool IsAllDay { get; set; }
        [WfmResourceDisplayName("Common.StartTime")]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? StartTimeOfDay { get; set; }
        [WfmResourceDisplayName("Common.EndTime")]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? EndTimeOfDay { get; set; }

        [WfmResourceDisplayName("Common.StartDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [WfmResourceDisplayName("Common.EndDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }


        //public virtual EmployeeModel Employee { get; set; }
        public string DaysOfWeekText
        {
            get
            {
                return this.DayOfWeek.ToString();
            }
        }

        public SelectList EmployeeAvailabilityTypeList
        {
            get
            {
                return this.EmployeeAvailabilityType.ToSelectList(true);
            }
        }
        public SelectList StartTimeOfDayList
        {
            get
            {
                return new SelectList(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, this.Start.Hour);
            }
        }

        public SelectList DayOfWeekList
        {
            get
            {
                return this.DayOfWeek.ToSelectList();
            }
        }

        public SelectList EndTimeOfDayList
        {
            get
            {
                return new SelectList(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, this.End.Hour);
            }
        }

        #region ISchedulerEvent
        public string Title
        {
            get
            {
                return EmployeeAvailabilityType.ToString();
            }
            set
            {
                //EmployeeAvailabilityType result;
                //if (Enum.TryParse<EmployeeAvailabilityType>(value, out result))
                //{
                //    EmployeeAvailabilityType = result;
                //}
            }
        }

        public string Description
        {
            get
            {
                switch(this.EmployeeAvailabilityType)
                {
                    case EmployeeAvailabilityType.Available:
                        return "Indicate the selection of preferred work hours";
                    case EmployeeAvailabilityType.Unavailable:
                        return "Indicate the selection of unavailable hours";
                    default:
                        return string.Empty;
                }
            }
            set
            {
            }
        }

        public DateTime Start
        {
            get
            {
                return this.StartTimeOfDay.GetValueOrDefault();
            }
            set
            {
            }
        }

        public DateTime End
        {
            get
            {
                return this.EndTimeOfDay.GetValueOrDefault();
            }
            set
            {
            }
        }

        public string TextColor
        {
            get
            {
                switch (this.EmployeeAvailabilityType)
                {
                    case EmployeeAvailabilityType.Unavailable:
                        return "red";
                    case EmployeeAvailabilityType.Available:
                        return "darkgreen";
                    default:
                        return "inherit";
                }
            }
        }

        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }

        public string RecurrenceRule
        {
            get; set;
        }

        public string RecurrenceException
        {
            get; set;
        }
        #endregion
    }
}