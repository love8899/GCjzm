using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Wfm.Client.Validators.JobOrder;

namespace Wfm.Client.Models.JobOrder
{
    [Validator(typeof(BudgetForcastingValidator))]
    public class BudgetForcastingModel
    {
        public int JobOrderId { get; set; }
        public string JobTitle { get; set; }
        public string Position { get; set; }
        public string Shift { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        //public string BillingRateCode { get; set; }
        public int NumberOfEmployees { get; set; }
        public decimal TotalCost { get; set; }
    }
}