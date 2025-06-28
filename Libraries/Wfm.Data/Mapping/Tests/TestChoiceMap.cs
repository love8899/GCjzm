using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Tests;

namespace Wfm.Data.Mapping.Tests
{
    public partial class TestChoiceMap : EntityTypeConfiguration<TestChoice>
    {
        public TestChoiceMap()
        {
            this.ToTable("TestChoice");
            this.HasKey(c => c.Id);

        }
    }
}
