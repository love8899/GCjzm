using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Validators.Company;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Companies
{
    [Validator(typeof(CompanyRecruiterValidator))]
    public class RecruiterCompanySimpleModel : BaseWfmEntityModel
    {
        public int AccountId { get; set; }
        public int CompanyId { get; set; }
        public int FranchiseId { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid FranchiseGuid { get; set; }
    }
}