using System;


namespace Wfm.Core.Domain.Features
{
    public class Feature : BaseEntity
    {
        public string Area { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
