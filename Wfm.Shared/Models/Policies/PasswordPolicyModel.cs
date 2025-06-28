using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Wfm.Shared.Validators;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Shared.Models.Policies
{
    [Validator(typeof(PasswordPolicyValidator))]
    public class PasswordPolicyModel : BaseWfmEntityModel
    {
        public string Code { get; set; }
        [Range(6,int.MaxValue)]
        public int MinLength { get; set; }
        [Range(6, int.MaxValue)]
        public int MaxLength { get; set; }
        public bool RequireUpperCase { get; set; }
        public bool RequireLowerCase { get; set; }
        public bool RequireNumber { get; set; }
        public bool RequireSymbol { get; set; }
        [Range(1, int.MaxValue)]
        public int PasswordLifeTime { get; set; }
        [Range(1, int.MaxValue)]
        public int PasswordHistory { get; set; }
    }
}