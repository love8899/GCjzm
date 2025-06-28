using System;

namespace Wfm.Web.Models.Test
{
    public class TestMaterialModel
    {
        public Guid TestMaterialGuid { get; set; }
        public int TestCategoryId { get; set; }
        public int AttachmentTypeId { get; set; }
        public string AttachmentFileName { get; set; }
        public bool IsActive { get; set; }
    }
}