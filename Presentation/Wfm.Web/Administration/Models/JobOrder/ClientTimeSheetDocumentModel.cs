using FluentValidation.Attributes;
using System;
using Wfm.Web.Framework.Mvc;
using Wfm.Admin.Validators.JobOrder;
using Wfm.Core.Domain.TimeSheet;


namespace Wfm.Admin.Models.JobOrder
{
    [Validator(typeof(ClientTimeSheetDocumentValidator))]
    public class ClientTimeSheetDocumentModel : BaseWfmEntityModel
    {
        public int JobOrderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Version { get; set; }
        public DocumentFileType FileType { get; set; }
        public string FileName { get; set; }
        public DocumentSource Source { get; set; }
    }
}
