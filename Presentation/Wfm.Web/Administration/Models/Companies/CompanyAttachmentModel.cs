using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Companies
{
    public class CompanyAttachmentModel : BaseWfmEntityModel
    {
        public Guid CompanyAttachmentGuid { get; set; }
        public int CompanyId { get; set; }
        public string AttachmentFileName { get; set; }
        public int AttachmentTypeId { get; set; }
        public bool IsRestricted { get; set; }
        public bool Readable { get; set; }
        public bool Writeable { get; set; }
        public bool Deletable { get; set; }
    }
}