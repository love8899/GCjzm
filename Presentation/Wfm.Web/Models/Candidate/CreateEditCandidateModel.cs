using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using Wfm.Web.Framework;
using Wfm.Web.Framework.Mvc;
using Wfm.Web.Validators.Candidate;

namespace Wfm.Web.Models.Candidate
{
    [Validator(typeof(CreateEditCandidateValidator))]
    public class CreateEditCandidateModel : BaseWfmEntityModel
    {
        public CreateEditCandidateModel()
        {
            this.CandidateModel = new CandidateModel();
        }
        public CandidateModel CandidateModel { get; set; }


        [WfmResourceDisplayName("Common.KeySkill")]        
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string KeySkill1 { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.YearsOfExperience")]
        public decimal YearsOfExperience1 { get; set; }

        [WfmResourceDisplayName("Web.Candidate.Candidate.Fields.LastUsedDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
      //  [RegularExpression(@"^(((((0[13578])|([13578])|(1[02]))[\-\/\s]?((0[1-9])|([1-9])|([1-2][0-9])|(3[01])))|((([469])|(11))[\-\/\s]?((0[1-9])|([1-9])|([1-2][0-9])|(30)))|((02|2)[\-\/\s]?((0[1-9])|([1-9])|([1-2][0-9]))))[\-\/\s]?\d{4})(\s(((0[1-9])|([1-9])|(1[0-2]))\:([0-5][0-9])((\s)|(\:([0-5][0-9])\s))([AM|PM|am|pm]{2,2})))?$", ErrorMessage = "Date should be entered as mm/dd/yyyy")]
        public DateTime? LastUsedDate1 { get; set; }


        [WfmResourceDisplayName("Common.UnitNumber")]       
        public string UnitNumber { get; set; }

        [WfmResourceDisplayName("Common.AddressLine1")]       
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
        public string AddressLine1 { get; set; }

        [WfmResourceDisplayName("Common.AddressLine2")]
        [RegularExpression(@"[ a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'?,./]+", ErrorMessage = "Invalid characters detected")]
       
        public string AddressLine2 { get; set; }

        [WfmResourceDisplayName("Common.Country")]
        public int CountryId { get; set; }

        [WfmResourceDisplayName("Common.StateProvince")]
        public int StateProvinceId { get; set; }

        [WfmResourceDisplayName("Common.City")]
        public int CityId { get; set; }

        [WfmResourceDisplayName("Common.PostalCode")]
        [MaxLength(7)]
        [RegularExpression(@"[ABCEGHJKLMNPRSTVXYabceghjklmnprstvxy][0-9][ABCEGHJKLMNPRSTVWXYZabceghjklmnprstvwxyz] ?[0-9][ABCEGHJKLMNPRSTVWXYZabceghjklmnprstvwxyz][0-9]", ErrorMessage = "Invalid Postal Code!")]
        [DataType(DataType.PostalCode)]
        [Display(Prompt = "N3G 1E9")]
        public string PostalCode { get; set; }

    }

}