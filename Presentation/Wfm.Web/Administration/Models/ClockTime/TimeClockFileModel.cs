using System;
using Wfm.Web.Framework;

namespace Wfm.Admin.Models.ClockTime
{
    /// <summary>
    /// To display punch clock file information awaiting to be loade
    /// </summary>
    public class TimeClockFileModel
    {
        [WfmResourceDisplayName("Common.Id")]
        public int FileId { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.TimeClockFile.Fields.FileLocation")]
        public string FileLocation { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.TimeClockFile.Fields.FileName")]
        public string FileName { get; set; }

        [WfmResourceDisplayName("Common.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [WfmResourceDisplayName("Admin.TimeClocks.TimeClockFile.Fields.ModifiedOn")]
        public DateTime ModifiedOn { get; set; }
    }
}
