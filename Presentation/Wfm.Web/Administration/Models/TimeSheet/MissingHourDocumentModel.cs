using FluentValidation.Attributes;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.TimeSheet;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Admin.Models.TimeSheet
{
    [Validator(typeof(MissingHourDocumentValidator))]
    public class MissingHourDocumentModel : BaseWfmEntityModel
    {
        public int CandidateMissingHourId { get; set; }
        public string FileName { get; set; }
    }
}
