using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Tests;

namespace Wfm.Data.Mapping.Tests
{
    public partial class TestQuestionMap : EntityTypeConfiguration<TestQuestion>
    {
        public TestQuestionMap()
        {
            this.ToTable("TestQuestion");
            this.HasKey(c => c.Id);
            this.HasRequired(c => c.TestCategory).WithMany(c => c.TestQuestions).HasForeignKey(c => c.TestCategoryId);
        }
    }
}
