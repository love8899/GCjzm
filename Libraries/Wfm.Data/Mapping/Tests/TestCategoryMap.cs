using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Tests;

namespace Wfm.Data.Mapping.Tests
{
    public partial class TestCategoryMap : EntityTypeConfiguration<TestCategory>
    {
        public TestCategoryMap()
        {
            this.ToTable("TestCategory");
            this.HasKey(c => c.Id);

            this.Property(c => c.TestCategoryName).IsRequired().HasMaxLength(255);
        }
    }
}
