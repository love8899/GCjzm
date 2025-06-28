using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.WSIBS;
using FluentValidation.Attributes;

namespace Wfm.Admin.Models.WSIBs
{
    [Validator(typeof(WSIBValidator))]
    public class WSIBModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public decimal Rate { get; set; }

        [UIHint("Date")]
        public DateTime StartDate { get; set; }
        [UIHint("Date")]
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }
}