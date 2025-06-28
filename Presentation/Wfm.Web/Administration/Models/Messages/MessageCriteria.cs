using System;
using System.Collections.Generic;


namespace Wfm.Admin.Models.Messages
{
    public class MessageCriteria
    {
        public string SubjectKeyword { get; set; }

        public List<string> Categories { get; set; }

        public List<int> Status { get; set; }

        public bool WithCC { get; set; }
    }

}
