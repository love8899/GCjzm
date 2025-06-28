using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Franchises;

namespace Wfm.Core.Domain.Incident
{
    /// <summary>
    /// File stream of incident report template
    /// </summary>
    public class IncidentReportTemplate : BaseEntity
    {
        public int IncidentCategoryId { get; set; }
        public virtual IncidentCategory IncidentCategory { get; set; }
        public int? FranchiseId { get; set; }
        public virtual Franchise Franchise { get; set; }
        public string FileName { get; set; }
        public string Note { get; set; }
        public int? MajorVersion { get; set; }
        public int? MinorVersion { get; set; }
        public byte[] TemplateStream { get; set; }
        public bool IsActive { get; set; }
    }
}
