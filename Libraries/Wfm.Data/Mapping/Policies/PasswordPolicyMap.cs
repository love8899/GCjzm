using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Policies;

namespace Wfm.Data.Mapping.Policies
{
    public class PasswordPolicyMap : EntityTypeConfiguration<PasswordPolicy>
    {
        public PasswordPolicyMap()
        {
            this.ToTable("PasswordPolicy");
            this.HasKey(m => m.Id);

            this.Property(m => m.Code).HasMaxLength(50);
        }
    }
}
