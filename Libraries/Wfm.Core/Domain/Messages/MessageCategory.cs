using System;


namespace Wfm.Core.Domain.Messages
{
    public class MessageCategory : BaseEntity
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
