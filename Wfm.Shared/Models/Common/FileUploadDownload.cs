using System;
using System.IO;
using System.Web;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Candidates;

namespace Wfm.Shared.Models.Common
{
    public class FileUploadDownload
    {
        public byte[] ReadFile(HttpRequestBase request, out string fileName, out string fileExtension, out string contentType)
        {
            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload

            Stream stream = null;
            fileName = "";
            contentType = "";
            byte[] fileBinary;

            try
            {
                if (String.IsNullOrEmpty(request["qqfile"]))
                {
                    // IE
                    HttpPostedFileBase httpPostedFile = request.Files[0];
                    if (httpPostedFile == null)
                        throw new ArgumentException("No file uploaded");
                    stream = httpPostedFile.InputStream;
                    fileName = Path.GetFileName(httpPostedFile.FileName);
                    contentType = httpPostedFile.ContentType;
                }
                else
                {
                    //Webkit, Mozilla
                    stream = request.InputStream;
                    fileName = request["qqfile"];
                }

                fileBinary = new byte[stream.Length];
                stream.Read(fileBinary, 0, fileBinary.Length);

                fileExtension = Path.GetExtension(fileName);
                if (!String.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return fileBinary;
        }
        public CandidatePicture AsyncUploadCandidatePicture(Guid? guid, HttpRequestBase request, ICandidateService _candidateService, ICandidatePictureService _candidatePictureService, out string error)
        {
            error = string.Empty;
            var candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate == null)
            {
                error = "The candidate does not exist!";
                return null;
            }
            string fileName;
            string fileExtension;
            string contentType;

            var fileBinary = ReadFile(request, out fileName, out fileExtension, out contentType);

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


            var picture = _candidatePictureService.InsertCandidatePicture(fileBinary, contentType, candidate.Id, true);

            return picture;
        }
    }
}
