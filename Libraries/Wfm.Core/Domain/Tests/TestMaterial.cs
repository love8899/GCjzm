using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Media;

namespace Wfm.Core.Domain.Tests
{
    public class TestMaterial:BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TestMaterialGuid { get; set; }
        public int? TestCategoryId { get; set; }
        public int AttachmentTypeId { get; set; }
        public string AttachmentFileName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] AttachmentFile { get; set; }
        public string ContentType { get; set; }

        public virtual AttachmentType AttachmentType { get; set; }
    }
}
