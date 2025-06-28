using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Validators.Common;

namespace Wfm.Admin.Models.Common
{
    [Validator(typeof(PositionModelValidator))]
    public class PositionModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
    }
}