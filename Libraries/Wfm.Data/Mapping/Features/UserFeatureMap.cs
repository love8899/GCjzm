using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Features;


namespace Wfm.Data.Mapping.Companies
{
    public class UserFeatureMap : EntityTypeConfiguration<UserFeature>
    {
        public UserFeatureMap()
        {
            this.ToTable("UserFeature");
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.Feature)
                .WithMany()
                .HasForeignKey(x => x.FeatureId);
        }
    }
}
