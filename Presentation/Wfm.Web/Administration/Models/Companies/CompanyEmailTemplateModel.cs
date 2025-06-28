using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wfm.Admin.Validators.Company;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Companies
{
    [Validator(typeof(CompanyEmailTemplateModelValidator))]
    public class CompanyEmailTemplateModel : BaseWfmEntityModel
    {
        public int Type { get; set; }
        public int CompanyId { get; set; }
        public Guid CompanyGuid { get; set; }
        public int CompanyLocationId { get; set; }
        public int CompanyDepartmentId { get; set; }
        [AllowHtml]
        public string Subject { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public bool IsDeleted { get; set; }
        public string AttachmentFileName { get; set; }
        public int? AttachmentTypeId { get; set; }
        public byte[] AttachmentFile { get; set; }
        public string AttachmentFileName2 { get; set; }
        public int? AttachmentTypeId2 { get; set; }
        public byte[] AttachmentFile2 { get; set; }
        public string AttachmentFileName3 { get; set; }
        public int? AttachmentTypeId3 { get; set; }
        public byte[] AttachmentFile3 { get; set; }
        public bool KeepFile1 { get; set; }
        public bool KeepFile2 { get; set; }
        public bool KeepFile3 { get; set; }
    }
}