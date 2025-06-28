using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyEmailTemplate:BaseEntity
    {
        public int Type { get; set; }
        public int CompanyId { get; set; }
        public int? CompanyLocationId { get; set; }
        public int? CompanyDepartmentId { get; set; }
        public string Subject { get; set; }
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
        public virtual Company Company { get; set; }
        public virtual CompanyLocation CompanyLocation { get; set; }
        public virtual CompanyDepartment CompanyDepartment { get; set; }
    }
    public enum CompanyEmailTemplateType
    {
        Confirmation=1
    }
}
