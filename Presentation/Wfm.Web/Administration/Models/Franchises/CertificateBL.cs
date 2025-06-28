using System;
using System.IO;
using System.Web;
using Wfm.Core.Domain.Media;
using Wfm.Services.Franchises;
using Wfm.Services.Media;

namespace Wfm.Admin.Models.Franchises
{
    public class CertificateBL
    {
        private readonly IVendorCertificateService _vendorCertificateService;
        private readonly IAttachmentTypeService _attachmentTypeService;
        public CertificateBL(IVendorCertificateService vendorCertificateService,
                                IAttachmentTypeService attachmentTypeService)
        {
            _vendorCertificateService = vendorCertificateService;
            _attachmentTypeService = attachmentTypeService;

        }
        public bool SaveCertificateFiles(HttpPostedFileBase attachment, Guid? guid,out string errorMessage)
        {
            errorMessage = String.Empty;
            if (guid == null)
            {
                errorMessage="The certificate does not exist!";
                return false;
            }
            var entity = _vendorCertificateService.Retrive(guid);
            if (entity == null)
            {
                errorMessage = "The certificate does not exist!";
                return false;
            }
            // The Name of the Upload component is "attachments"
            //foreach (var file in attachments)
            //{
                // prepare
            var fileName = Path.GetFileName(attachment.FileName);
            var contentType = attachment.ContentType;

                // not supported file format
                AttachmentType attachmentType = _attachmentTypeService.GetAttachmentTypeByFileExtension(Path.GetExtension(fileName));
                if (attachmentType == null)
                {
                    errorMessage = "File format is not supported, please contact administrator.";
                    return false;
                }
                using (Stream stream = attachment.InputStream)
                {
                    var fileBinary = new byte[stream.Length];
                    stream.Read(fileBinary, 0, fileBinary.Length);

                    entity.CertificateFile = fileBinary;
                    entity.ContentType = contentType;
                    entity.UpdatedOnUtc = DateTime.UtcNow;
                    entity.CertificateFileName = fileName;
                    // upload attachment
                    _vendorCertificateService.Update(entity,null);
                    return true;
                }
        }
    }
}