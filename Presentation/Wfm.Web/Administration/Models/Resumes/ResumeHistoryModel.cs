using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Wfm.Admin.Validators.Messages;
using Wfm.Web.Framework.Mvc;


namespace Wfm.Admin.Models.Messages
{
    [Validator(typeof(ResumeHistoryValidator))]
    public class ResumeHistoryModel : BaseWfmEntityModel
    {
        public int ResumeId { get; set; }

        public int AccountId { get; set; }
        public string Who { get; set; }

        public DateTime ContactedOnUtc { get; set; }
        [UIHint("ISO_Date")]
        public DateTime ContactedOn { get; set; }

        public string Via { get; set; }

        public string Result { get; set; }

        public string Note { get; set; }
    }
}
