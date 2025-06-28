using System;
using System.IO;
using System.Web.Mvc;
using Wfm.Core.Domain.Media;
using Wfm.Services.Media;
using Wfm.Shared.Models.Common;

namespace Wfm.Admin.Controllers
{
    public partial class DownloadController : BaseAdminController
    {
        private readonly IDownloadService _downloadService;

        public DownloadController(IDownloadService downloadService)
        {
            this._downloadService = downloadService;
        }

        public ActionResult DownloadFile(Guid downloadGuid)
        {
            var download = _downloadService.GetDownloadByGuid(downloadGuid);
            if (download == null)
                return Content("No download record found with the specified id");

            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //use stored data
            if (download.DownloadBinary == null)
                return Content(string.Format("Download data is not available any more. Download GD={0}", download.Id));

            string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
            string contentType = !String.IsNullOrWhiteSpace(download.ContentType)
                ? download.ContentType
                : "application/octet-stream";
            return new FileContentResult(download.DownloadBinary, contentType)
            {
                FileDownloadName = fileName + download.Extension
            };
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveDownloadUrl(string downloadUrl)
        {
            //insert
            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = true,
                DownloadUrl = downloadUrl,
                IsNew = true
              };
            _downloadService.InsertDownload(download);

            return Json(new { downloadId = download.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AsyncUpload()
        {
            string fileName;
            string fileExtension;
            string contentType;

            var fileHelper = new FileUploadDownload();
            var fileBinary = fileHelper.ReadFile(Request, out fileName, out fileExtension, out contentType);

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = Path.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new { success = true, 
                downloadId = download.Id, 
                downloadUrl = Url.Action("DownloadFile", new { downloadGuid= download.DownloadGuid }) },
                "text/plain");
        }
    }
}
