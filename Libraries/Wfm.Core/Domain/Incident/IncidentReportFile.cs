using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wfm.Core.Domain.Incident
{
    /// <summary>
    /// File stream of incident reports
    /// </summary>
    public class IncidentReportFile : BaseEntity
    {
        public int IncidentReportId { get; set; }
        public virtual IncidentReport IncidentReport { get; set; }
        public string FileName { get; set; }
        public byte[] FileStream { get; set; }
    }
}
