using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Common;

namespace Wfm.Data.Mapping.Common
{
    public partial class SecurityQuestionMap : EntityTypeConfiguration<SecurityQuestion>
    {
        public SecurityQuestionMap()
        {
            this.ToTable("SecurityQuestion");
            this.HasKey(c => c.Id);
        }
    }
}
