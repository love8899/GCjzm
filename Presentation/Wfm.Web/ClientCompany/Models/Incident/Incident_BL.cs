using System.Collections.Generic;
using System.Web;
using Wfm.Core;
using Wfm.Services.Incident;
using Wfm.Core.Domain.Incident;
using System.IO;

namespace Wfm.Client.Models.Incident
{
    public class Incident_BL
    {
      
        private readonly IWorkContext _workContext;
        private readonly IIncidentService _incidentService;
        public Incident_BL(
            IIncidentService incidentService,
            IWorkContext workContext
            )
        {
            _incidentService = incidentService;
            _workContext = workContext;
        }

        public string SaveIncidentReportFiles(IEnumerable<HttpPostedFileBase> files, int incidentReportId)
        {
            // The Name of the Upload component is "attachments"
            foreach (var file in files)
            {
                // prepare
                var fileName = Path.GetFileName(file.FileName);
                var contentType = file.ContentType;

                using (Stream stream = file.InputStream)
                {
                    var fileBinary = new byte[stream.Length];
                    stream.Read(fileBinary, 0, fileBinary.Length);

                    // upload attachment
                    var fileEntity = new IncidentReportFile()
                    {
                        IncidentReportId = incidentReportId,
                        FileName = fileName,
                        FileStream = fileBinary,
                    };
                    _incidentService.InsertIncidentReportFile(fileEntity);
                }
            }
            // Return an empty string to signify success
            return "";
        }
        public void DownloadIncidentReportFile(int reportFileId, HttpResponseBase Response)
        {
            var file = _incidentService.GetIncidentReportFile(reportFileId);
            Response.ClearContent();
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            //  NOTE: If you get an "HttpCacheability does not exist" error on the following line, make sure you have
            //  manually added System.Web to this project's References.
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.AddHeader("content-disposition", "attachment; filename=" + file.FileName);
            Response.ContentType = MimeMapping.GetMimeMapping(file.FileName);

            Response.BinaryWrite(file.FileStream);
            Response.Flush();
            Response.End();
        }
    }
}