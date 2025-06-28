using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Web.Framework.Mvc;

namespace Wfm.Admin.Models.Test
{
    public class TestMaterialModel : BaseWfmEntityModel
    {
        public Guid TestMaterialGuid { get; set; }
        public int TestCategoryId { get; set; }
        public int AttachmentTypeId { get; set; }
        public string AttachmentFileName { get; set; }
        public bool IsActive { get; set; }
    }
}