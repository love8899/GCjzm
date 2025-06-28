using System.Data.Entity.ModelConfiguration;
using Wfm.Core.Domain.Logging;

namespace Wfm.Data.Mapping.Logging
{
    public partial class CandidateActivityLogMap : EntityTypeConfiguration<CandidateActivityLog>
    {
        public CandidateActivityLogMap()
        {
            this.ToTable("CandidateActivityLog");
            this.HasKey(c => c.Id);
        }
    }
}
