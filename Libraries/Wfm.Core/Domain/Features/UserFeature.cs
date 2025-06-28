using System;


namespace Wfm.Core.Domain.Features
{
    public class UserFeature : BaseEntity
    {
        public string Area { get; set; }
        public int UserId { get; set; }
        public int FeatureId { get; set; }
        public bool IsActive { get; set; }

        public virtual Feature Feature { get; set; }
    }
}
