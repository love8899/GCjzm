using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.Employee;
using FluentValidation.Attributes;
namespace Wfm.Admin.Models.Employee
{
    [Validator(typeof(CandidateWSIBCommonRateValidator<CandidateWSIBCommonRateModel>))]
    public class CandidateWSIBCommonRateModel : BaseWfmEntityModel
    {
        public int CandidateId { get; set; }
        [UIHint("Date")]
        public DateTime StartDate { get; set; }
        [UIHint("Date")]
        public DateTime EndDate { get; set; }
        public decimal Ratio { get; set; }
        public int EnteredBy { get; set; }
        public string Code { get; set; }
        public int ProvinceId { get; set; }
    }
}