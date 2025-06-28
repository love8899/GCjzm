using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Tests;

namespace Wfm.Data.Mapping.Tests
{
    public class TestMaterialMap : EntityTypeConfiguration<TestMaterial>
    {
        public TestMaterialMap()
        {
            this.ToTable("TestMaterial");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.AttachmentType).WithMany().HasForeignKey(c => c.AttachmentTypeId);

        }
    }
}
