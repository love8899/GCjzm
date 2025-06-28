using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Common;

namespace Wfm.Admin.Models.Common
{
    [Validator(typeof(AddressModelValidator<AddressModel>))]
    public class AddressModel : BaseWfmEntityModel
    {
        //[WfmResourceDisplayName("Common.Name")]
        //public string Name { get; set; }

        [WfmResourceDisplayName("Common.UnitNumber")]
        public string UnitNumber { get; set; }

        [WfmResourceDisplayName("Common.AddressLine1")]
        public string AddressLine1 { get; set; }

        [WfmResourceDisplayName("Common.AddressLine2")]
        public string AddressLine2 { get; set; }

        [WfmResourceDisplayName("Common.AddressLine3")]
        public string AddressLine3 { get; set; }

        [UIHint("CityEditor")]
        [WfmResourceDisplayName("Common.City")]
        [Range(1, 200000, ErrorMessage = "Please select a city")]
        public int CityId { get; set; }

        [UIHint("StateProvinceEditor")]
        [WfmResourceDisplayName("Common.StateProvince")]
        [Range(1, 2000, ErrorMessage = "Please select a province")]
        public int StateProvinceId { get; set; }

        [UIHint("CountryEditor")]
        [WfmResourceDisplayName("Common.Country")]
        [Range(1, 2, ErrorMessage = "Please select a country")]
        public int CountryId { get; set; }

        [WfmResourceDisplayName("Common.PostalCode")]
        [RegularExpression(@"[ABCEGHJKLMNPRSTVXYabceghjklmnprstvxy][0-9][ABCEGHJKLMNPRSTVWXYZabceghjklmnprstvwxyz] ?[0-9][ABCEGHJKLMNPRSTVWXYZabceghjklmnprstvwxyz][0-9]", ErrorMessage = "Invalid Postal Code!")]
        [DataType(DataType.PostalCode)]
        [MaxLength(7)]
        public string PostalCode { get; set; }

        public virtual string CityName { get; set; }
        public virtual string StateProvinceName { get; set; }
        public virtual string CountryName { get; set; }

    }
}