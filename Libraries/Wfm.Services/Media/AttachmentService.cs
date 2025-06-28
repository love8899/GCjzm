using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.IO;
using Aspose.OCR;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using Aspose.Pdf.Text;
using Aspose.Words;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Media;
using Wfm.Core.Domain.Candidates;
using LoadFormat = Aspose.Words.LoadFormat;
using Wfm.Services.Candidates;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Common;


namespace Wfm.Services.Media
{
    public partial class AttachmentService : IAttachmentService
    {
        #region Fields

        private readonly IRepository<CandidateAttachment> _attachmentRepository;
        private readonly IAttachmentTypeService _attachmentTypeService;
        private readonly MediaSettings _mediaSettings;
        private readonly IWebHelper _webHelper;
        private readonly ICandidateService _candidateService;
        private readonly IWorkContext _workContext;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly CommonSettings _commonSettings;
        #endregion

        #region Ctor

        public AttachmentService(
            IRepository<CandidateAttachment> attachmentRepository,
            IAttachmentTypeService attachmentTypeService,
            MediaSettings mediaSettings,
            IWebHelper webHelper,
            ICandidateService candidateService,
            IWorkContext workContext,
            IDocumentTypeService documentTypeService,
            CommonSettings commonSettings
            )
        {
            _attachmentRepository = attachmentRepository;
            _attachmentTypeService = attachmentTypeService;
            _mediaSettings = mediaSettings;
            _webHelper = webHelper;
            _candidateService = candidateService;
            _workContext = workContext;
            _documentTypeService = documentTypeService;
            _commonSettings = commonSettings;
        }

        #endregion


        #region Utilities

        protected string GetAttachmentSubDirectory()
        {
            string year = System.DateTime.Now.ToString("yyyy");
            string month = System.DateTime.Now.ToString("MM");
            string day = System.DateTime.Now.ToString("dd");

            return Path.Combine(_mediaSettings.AttachmentLocation, year, month, day);
        }

        protected virtual void DeleteAttachmentOnFileSystem(CandidateAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");

            // web root directory
            string rootDirectory = _webHelper.GetRootDirectory();
            string attachmentFileWithPath = Path.Combine(rootDirectory, attachment.StoredPath, attachment.StoredFileName);

            // delete the file
            if (File.Exists(attachmentFileWithPath))
            {
                File.Delete(attachmentFileWithPath);
            }
        }


        //protected string GetDocFileContentText(string filePath)
        //{
        //    var sb = new StringBuilder();

        //    using (var doc = new Document())
        //    {
        //        doc.Open(filePath);
        //        for (int i = 0; i < doc.ParagraphCount; i++)
        //        {
        //            Paragraph p = doc.GetParagraph(i);
        //            if (p != null)
        //                sb.Append(CommonHelper.SanitizeContent(p.Text));
        //        }
        //    }

        //    return sb.ToString();
        //}

        //protected string GetTxtFileContentText(string filePath)
        //{
        //    var sb = new StringBuilder();

        //    using (var sr = new StreamReader(filePath))
        //    {
        //        string line;
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            sb.Append(CommonHelper.SanitizeContent(line));
        //        }
        //    }

        //    return sb.ToString();
        //}


        protected string GetTxtFileContentText(byte[] byteArray)
        {
            var listContent = new List<string>();

            using (var sr = new StreamReader(new MemoryStream(byteArray)))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    listContent.Add(CommonHelper.SanitizeContent(line));
                }
            }

            return String.Join(" ", listContent);
        }

        //protected string GetDocFileContentText(byte[] byteArray)
        //{
        //    var sbContent = new StringBuilder();

        //    using (var doc = new Bytescout.Document.Document())
        //    {
        //        doc.Open(new MemoryStream(byteArray));
        //        for (int i = 0; i < doc.ParagraphCount; i++)
        //        {
        //            Bytescout.Document.Paragraph para = doc.GetParagraph(i);
        //            if (para != null)
        //                sbContent.Append(CommonHelper.SanitizeContent(para.Text));
        //        }
        //    }

        //    return sbContent.ToString();
        //}

        //protected string GetPdfFileContentText(byte[] byteArray)
        //{
        //    var sbContent = new StringBuilder();

        //    using (var reader = new PdfReader(byteArray))
        //    {
        //        for (int page = 1; page <= reader.NumberOfPages; page++)
        //        {
        //            ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
        //            String s = PdfTextExtractor.GetTextFromPage(reader, page, its);

        //            s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));
        //            sbContent.Append(CommonHelper.SanitizeContent(s));

        //        }
        //        reader.Close();
        //    }

        //    return sbContent.ToString();
        //}

        protected string GetDocFileContentText(byte[] byteArray)
        {
            var listContent = new List<string>();

            using (var stream = new MemoryStream(byteArray))
            {
                var doc = new Aspose.Words.Document(stream);
                NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);
                foreach (Paragraph paragraph in paragraphs)
                {
                    if (paragraph != null)
                        listContent.Add(CommonHelper.SanitizeContent(paragraph.GetText()));

                    //NodeCollection runs = paragraph.GetChildNodes(NodeType.Run, true);
                    //foreach (Run run in runs)
                    //{
                    //    listContent.Add(CommonHelper.SanitizeContent(run.GetText()));
                    //}
                }
            }

            return String.Join(" ", listContent);
        }

        protected string GetPdfFileContentText(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return null;

            string content = null;
            using (var stream = new MemoryStream(byteArray))
            {
                try
                {
                    //open document
                    var pdfDocument = new Aspose.Pdf.Document(stream);
                    //create TextAbsorber object to extract text
                    var textAbsorber = new TextAbsorber();
                    //accept the absorber for all the pages
                    pdfDocument.Pages.Accept(textAbsorber);
                    //get the extracted text
                    content = CommonHelper.SanitizeContent(textAbsorber.Text);
                }
                catch (Exception e)
                {
                    // NullReferenceException at Aspose.Pdf.Document(stream), for SIGNED pdf
                    // Fixed in new versions of Aspose.Pdf (like 21.10 or earlier)
                    ;
                }
            }

            return content;
        }

        protected string GetPdfFileContentTextWithImage(byte[] byteArray)
        {
            var sbContent = new StringBuilder();

            // open PDF
            var pdfExtractor = new PdfExtractor();

            using (var stream = new MemoryStream(byteArray))
            {
                pdfExtractor.BindPdf(stream);
            }

            //use parameterless ExtractText method
            pdfExtractor.ExtractText();

            using (var memoryStream = new MemoryStream())
            {
                pdfExtractor.GetText(memoryStream);

                //specify Unicode encoding type in StreamReader constructor
                using (var streamReader = new StreamReader(memoryStream, Encoding.Unicode))
                {
                    streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    sbContent.Append(CommonHelper.SanitizeContent(streamReader.ReadToEnd()));
                }
            }

            //Specify Image Extraction Mode
            pdfExtractor.ExtractImageMode = ExtractImageMode.ActuallyUsed;

            //Extract Images based on Image Extraction Mode
            pdfExtractor.ExtractImage();

            //Get all the extracted images
            while (pdfExtractor.HasNextImage())
            {
                using (var stream = new MemoryStream())
                {
                    pdfExtractor.GetNextImage(stream, ImageFormat.Png);
                    //var imageBinary = new byte[stream.Length];
                    //stream.Read(imageBinary, 0, imageBinary.Length);
                    //stream.Close();

                    //initialize OcrEngine
                    var ocrEngine = new OcrEngine();
                    //set the image
                    //ocrEngine.Image = ImageStream.FromMemoryStream(new MemoryStream(imageBinary), ImageStreamFormat.Png);
                    ocrEngine.Image = ImageStream.FromMemoryStream(stream, ImageStreamFormat.Png);
                    //add language and other attributes
                    ocrEngine.Languages.AddLanguage(Language.Load("english"));
                    //ocrEngine.Config.NeedRotationCorrection = true;
                    //ocrEngine.Config.UseDefaultDictionaries = true;
                    //load the resource file
                    //ocrEngine.Resource = new FileStream("2011.07.02 v1.0 Aspose.OCR.Resouces.zip", FileMode.Open);
                    //process the whole image
                    if (ocrEngine.Process())
                    {
                        sbContent.Append(CommonHelper.SanitizeContent(ocrEngine.Text.ToString()));
                    }
                }

                //pdfExtractor.GetNextImage(_webHelper.GetRootDirectory() + "\\uploads\\" +DateTime.Now.Ticks.ToString() + ".jpg");
            }

            return sbContent.ToString();
        }


        protected string ExtractFileContentText(byte[] fileBinary, string fileExtension)
        {
            // parse file content if available
            string fileContentText;
            switch (fileExtension)
            {
                case ".txt":
                    fileContentText = GetTxtFileContentText(fileBinary);
                    break;
                case ".pdf":
                    fileContentText = GetPdfFileContentText(fileBinary);
                    break;
                default:
                    using (var stream = new MemoryStream(fileBinary))
                    {
                        // try to detect file format
                        FileFormatInfo info = FileFormatUtil.DetectFileFormat(stream);
                        // process the document type
                        switch (info.LoadFormat)
                        {
                            case LoadFormat.Doc:
                            // Microsoft Word 97-2003 document
                            case LoadFormat.Dot:
                            // Microsoft Word 97-2003 template
                            case LoadFormat.Docx:
                            // Office Open XML WordprocessingML Macro-Free Document
                            case LoadFormat.Docm:
                            // Office Open XML WordprocessingML Macro-Enabled Document
                            case LoadFormat.Dotx:
                            // Office Open XML WordprocessingML Macro-Free Template
                            case LoadFormat.Dotm:
                            // Office Open XML WordprocessingML Macro-Enabled Template
                            case LoadFormat.FlatOpc:
                            // Flat OPC document
                            case LoadFormat.Rtf:
                            // RTF format
                            case LoadFormat.WordML:
                            // Microsoft Word 2003 WordprocessingML format
                            case LoadFormat.Html:
                            // HTML format
                            case LoadFormat.Mhtml:
                            // MHTML (Web archive) format
                            case LoadFormat.Odt:
                            // OpenDocument Text
                            case LoadFormat.Ott:
                            // OpenDocument Text Template
                            case LoadFormat.DocPreWord97:
                                // MS Word 6 or Word 95 format

                                // try to extract content text
                                fileContentText = GetDocFileContentText(fileBinary);
                                break;
                            case LoadFormat.Unknown:
                            default:
                                fileContentText = null;
                                break;
                        }
                    }
                    break;
            }


            return fileContentText;
        }

        #endregion


        #region CRUD

        public void InsertAttachment(CandidateAttachment attachment)
        {
            if (attachment == null) throw new ArgumentException("attachment");
            _attachmentRepository.Insert(attachment);
        }

        public void UpdateAttachment(CandidateAttachment attachment)
        {
            if (attachment == null) throw new ArgumentException("attachment");
            _attachmentRepository.Update(attachment);
        }

        public void DeleteAttachment(CandidateAttachment attachment)
        {
            if (attachment == null) throw new ArgumentException("attachment");

            //delete from file system
            DeleteAttachmentOnFileSystem(attachment);

            //delete from database
            _attachmentRepository.Delete(attachment);
        }


        public string UploadCandidateAttachment(int candidateId, byte[] attachmentBinary, string fileName, string contentType, int documentTypeId, DateTime? expiryDate = null, int? companyId = null)
        {
            var result = string.Empty;

            if (candidateId == 0)
                return result;
            if (attachmentBinary == null || attachmentBinary.Length == 0)
                return result;
            if (string.IsNullOrWhiteSpace(fileName))
                return result;

            var documentType = _documentTypeService.GetDocumentTypeById(documentTypeId);
            if (documentType != null && !string.IsNullOrEmpty(documentType.FileName))
            {
                if (!fileName.ToUpper().StartsWith(documentType.FileName.Replace("*.*", "").ToUpper()))
                {
                    result = "File name must start with " + documentType.FileName.Replace("*.*", "");
                    return result;
                }
            }

            // get stored file name
            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            var sourceFileName = Path.GetFileName(fileName);
            var destinationFileName = Guid.NewGuid().ToString() + fileExtension;

            // *** not supported file format ***
            var attachmentType = _attachmentTypeService.GetAttachmentTypeByFileExtension(fileExtension);
            if (attachmentType == null)
                return result;

            // parse file content if available
            var fileContentText = ExtractFileContentText(attachmentBinary, fileExtension);

            var isCompressed = false;
            var storedPath = string.Empty;
            byte[] attachmentFile = null;
            if (_mediaSettings.SaveAttachmentAsFile)
            {
                storedPath = GetAttachmentSubDirectory();
                var attachmentRootDirectory = Path.Combine(_webHelper.GetRootDirectory(), storedPath);
                if (!Directory.Exists(attachmentRootDirectory))
                {
                    Directory.CreateDirectory(attachmentRootDirectory);
                }
                File.WriteAllBytes(Path.Combine(attachmentRootDirectory, destinationFileName), attachmentBinary);
            }
            else
            {
                attachmentFile = attachmentBinary;
                if (attachmentType.TypeName == "PDF" || attachmentType.TypeName == "Document")
                {
                    var binaryZipped = CommonHelper.Compress(attachmentBinary);
                    // save only if with gzip magic number
                    if (binaryZipped.Length >= 2 && binaryZipped[0] == 0x1F && binaryZipped[1] == 0x8B)
                    {
                        isCompressed = true;
                        attachmentFile = binaryZipped;
                    }
                }
            }

            // add new attachment
            var newAttachment = new CandidateAttachment()
            {
                CandidateAttachmentGuid = Guid.NewGuid(),
                CandidateId = candidateId,
                AttachmentTypeId = attachmentType.Id,
                OriginalFileName = sourceFileName,
                StoredFileName = destinationFileName,
                StoredPath = storedPath,
                ContentType = contentType,
                ContentText = fileContentText,
                FileSizeInKB = attachmentBinary.Length,
                IsActive = true,
                IsDeleted = false,
                CreatedOnUtc = System.DateTime.UtcNow,
                UpdatedOnUtc = System.DateTime.UtcNow,
                DocumentTypeId = documentTypeId,
                AttachmentFile = attachmentFile,
                IsCompressed = isCompressed,
                ExpiryDate = expiryDate,
                CompanyId = companyId
            };

            InsertAttachment(newAttachment);

            return result;
        }

        #endregion


        #region Attachment

        private void _TryDecompress(CandidateAttachment attachment)
        {
            if (attachment != null && attachment.AttachmentFile != null && attachment.IsCompressed)
                attachment.AttachmentFile = CommonHelper.Decompress(attachment.AttachmentFile);
        }

        public CandidateAttachment GetAttachmentById(int id, bool noTracking = true)
        {
            if (id == 0)
                return null;

            var allAttachments = noTracking ? _attachmentRepository.TableNoTracking : _attachmentRepository.Table;
            var result = allAttachments.FirstOrDefault(x => x.Id == id);
            _TryDecompress(result);

            return result;
        }

        public CandidateAttachment GetAttachmentByGuid(Guid? guid, bool noTracking = true)
        {
            if (guid == null)
                return null;

            var allAttachments = noTracking ? _attachmentRepository.TableNoTracking : _attachmentRepository.Table;
            var result = allAttachments.Where(x => x.CandidateAttachmentGuid == guid).FirstOrDefault();
            _TryDecompress(result);
            
            return result;
        }

        public CandidateAttachment GetCandidateResumeByCandidateGuid(Guid? guid, bool noTracking = true)
        {
            if (guid == null)
                return null;

            var allAttachments = noTracking ? _attachmentRepository.TableNoTracking : _attachmentRepository.Table;
            var result = allAttachments.Where(x => x.Candidate.CandidateGuid == guid && x.DocumentTypeId == (int)CandidateDocumentTypeEnum.RESUME).FirstOrDefault();
            _TryDecompress(result);
            
            return result;
        }

        #endregion

        #region LIST

        public List<CandidateAttachment> GetAttachmentsByCandidateId(int candidateId, bool isPublic = false)
        {
            if (candidateId == 0)
                return null;

            var query = _attachmentRepository.Table;
            if (isPublic)
                query = query.Where(c => c.DocumentType.IsPublic==true);
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            query = from a in query
                    where a.CandidateId == candidateId
                    select a;

            return query.ToList();
        }

        public IQueryable<CandidateAttachment> GetAllCandidateAttachmentsAsQueryable(bool showInactive = false, bool showHidden = false)
        {
            //if (account == null)
            //    throw new ArgumentNullException("account is null");

            var query = _attachmentRepository.Table;
            if (_commonSettings.DisplayVendor)
                query = query.Where(x => x.Candidate.EmployeeTypeId != (int)EmployeeTypeEnum.REG);
            // active
            if (!showInactive)
                query = query.Where(c => c.IsActive == true);
            // deleted
            if (!showHidden)
                query = query.Where(c => c.IsDeleted == false);

            // IsLimitedToFranchises
            Account account = _workContext.CurrentAccount;
            if (account.IsVendor())
                query = query.Where(c => c.Candidate.FranchiseId == account.FranchiseId);

            query = query.OrderByDescending(c => c.UpdatedOnUtc);

            query = from c in query
                    select c;

            return query.AsQueryable();
        }

        #endregion

        #region Helpers

        public string GetMimeType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            string mime = "application/octetstream";

            string fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension);
            if (rk != null && rk.GetValue("Content Type") != null)
                mime = rk.GetValue("Content Type").ToString();

            return mime;
        }

        public void ReloadContentTextById(int id)
        {
            CandidateAttachment attachment = GetAttachmentById(id);
            if (attachment == null)
                return;

            // get stored file name
            string fileExtension = Path.GetExtension(attachment.OriginalFileName);
            if (String.IsNullOrEmpty(fileExtension))
                return;

            fileExtension = fileExtension.ToLowerInvariant();



            // web root directory
            string rootDirectory = _webHelper.GetRootDirectory();
            // read file as byte array
            string storedFile = Path.Combine(rootDirectory, attachment.StoredPath, attachment.StoredFileName);
            if (String.IsNullOrEmpty(storedFile) || !File.Exists(storedFile))
                return;

            byte[] attachmentBinary = File.ReadAllBytes(storedFile);



            // get new content text
            attachment.ContentText = ExtractFileContentText(attachmentBinary, fileExtension);



            UpdateAttachment(attachment);
        }


        public string ExtractFileText(byte[] fileBinary, string fileExtension)
        {
            return ExtractFileContentText(fileBinary, fileExtension);
        }

        #endregion

    }
}
