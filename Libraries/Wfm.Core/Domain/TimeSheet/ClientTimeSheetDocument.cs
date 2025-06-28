using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.JobOrders;

namespace Wfm.Core.Domain.TimeSheet
{
    /// <summary>
    /// Represents a document from client company which will be attached to Candidate Worktime
    /// </summary>
    public class ClientTimeSheetDocument : BaseEntity
    {
        public int JobOrderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Version { get; set; }
        public DocumentFileType FileType { get; set; }
        public string FileName { get; set; }
        public byte[] Stream { get; set; }
        public DocumentSource Source { get; set; }

        public virtual JobOrder JobOrder { get; set; }
    }

    public enum DocumentFileType
    {
        Unknown,
        PDF,
        Excel,
        Text,
        Html,
    }
    public enum DocumentSource
    {
        ImportFile,
        Attachment,
    }
}
