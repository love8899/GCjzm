using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Admin.Validators.Company;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Companies
{
    [Validator(typeof(CompanyActivityValidator))]
    public class CompanyActivityModel : BaseWfmEntityModel
    {
        public int CompanyId { get; set; }
        public int ActivityTypeId { get; set; }
        [UIHint("DateNullable")]
        public DateTime ActivityDate { get; set; }
        public string Note { get; set; }
    }
}