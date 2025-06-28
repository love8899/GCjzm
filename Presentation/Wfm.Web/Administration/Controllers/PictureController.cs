using System;
using System.Web.Mvc;
using Wfm.Services.Media;
using Wfm.Shared.Models.Common;

namespace Wfm.Admin.Controllers
{
    public partial class PictureController : BaseAdminController
    {
        private readonly IPictureService _pictureService;

        public PictureController(IPictureService pictureService)
        {
            this._pictureService = pictureService;
        }

        [HttpPost]
        public ActionResult AsyncUpload()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
            //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

            string fileName;
            string fileExtension;
            string contentType;

            var fileHelper = new FileUploadDownload();
            var fileBinary = fileHelper.ReadFile(Request, out fileName, out fileExtension, out contentType);
            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            if (String.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
                {
                    case ".bmp":
                        contentType = "image/bmp";
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".tiff":
                    case ".tif":
                        contentType = "image/tiff";
                        break;
                    default:
                        break;
                }
            }


            var picture = _pictureService.InsertPicture(fileBinary, contentType, null, true);


            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new { success = true, pictureId = picture.Id,
                imageUrl = _pictureService.GetPictureUrl(picture, 100) },
                "text/plain");
        }
    }
}
