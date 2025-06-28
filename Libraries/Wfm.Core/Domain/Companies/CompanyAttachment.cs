using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Media;

namespace Wfm.Core.Domain.Companies
{
    public class CompanyAttachment:BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CompanyAttachmentGuid { get; set; }
        public int CompanyId { get; set; }
        public byte[] AttachmentFile { get; set; }
        public string AttachmentFileName { get; set; }
        public int AttachmentTypeId { get; set; }
        public int? EnteredBy { get; set; }
        public bool IsRestricted { get; set; }
        public virtual Company Company { get; set; }
        public virtual AttachmentType AttachmentType { get; set; }
    }
}
