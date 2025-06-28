using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ImageResizer;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Media;
using Wfm.Services.Configuration;
using Wfm.Services.Events;
using Wfm.Services.Logging;
using Wfm.Services.Helpers;

namespace Wfm.Services.Candidates
{
    /// <summary>
    /// CandidatePicture service
    /// </summary>
    public partial class CandidatePictureService : ICandidatePictureService
    {
        #region Const

        private const int NUMBER_OF_PICTURES_PER_DIRECTORY = 5000;

        #endregion

        #region Fields

        private static readonly object s_lock = new object();

        private readonly IRepository<CandidatePicture> _candidatePictureRepository;
        private readonly ICandidateService _candidateService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ILogger _logger;
        private readonly IEventPublisher _eventPublisher;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="candidatePictureRepository">CandidatePicture repository</param>
        /// <param name="candidateCandidatePictureRepository">Candidate candidatePicture repository</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="logger">Logger</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="mediaSettings">Media settings</param>
        public CandidatePictureService(IRepository<CandidatePicture> candidatePictureRepository,
            ICandidateService candidateService,
            ISettingService settingService, IWebHelper webHelper,
            ILogger logger, IEventPublisher eventPublisher,
            MediaSettings mediaSettings)
        {
            this._candidatePictureRepository = candidatePictureRepository;
            this._candidateService = candidateService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._logger = logger;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities


        /// <summary>
        /// Loads a candidatePicture from file
        /// </summary>
        /// <param name="candidatePictureId">CandidatePicture identifier</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>CandidatePicture binary</returns>
        protected virtual byte[] LoadCandidatePictureFromFile(int candidatePictureId, string mimeType)
        {
            MediaHelper _helper = new MediaHelper();
            string lastPart = _helper.GetFileExtensionFromMimeType(mimeType);
            string fileName = string.Format("{0}_0.{1}", candidatePictureId.ToString("000000000000"), lastPart);
            var filePath = GetCandidatePictureLocalPath(fileName);
            if (!File.Exists(filePath))
                return new byte[0];
            return File.ReadAllBytes(filePath);
        }


        protected virtual byte[] LoadCandidatePictureFromDatabase(CandidatePicture candidatePicture)
        {
            return candidatePicture.PictureFile;
        }


        /// <summary>
        /// Save candidatePicture on file system
        /// </summary>
        /// <param name="candidatePictureId">CandidatePicture identifier</param>
        /// <param name="candidatePictureBinary">CandidatePicture binary</param>
        /// <param name="mimeType">MIME type</param>
        protected virtual string SaveCandidatePictureInFile(int candidatePictureId, byte[] candidatePictureBinary, string mimeType)
        {
            MediaHelper _helper = new MediaHelper();
            string lastPart = _helper.GetFileExtensionFromMimeType(mimeType);
            string fileName = string.Format("{0}_0.{1}", candidatePictureId.ToString("000000000000"), lastPart);
            File.WriteAllBytes(GetCandidatePictureLocalPath(fileName), candidatePictureBinary);
            return _mediaSettings.CandidatePictureLocation + @"\" + GetSubDirectoryName(candidatePictureId) + @"\" + fileName;
        }

        /// <summary>
        /// Delete a candidatePicture on file system
        /// </summary>
        /// <param name="candidatePicture">CandidatePicture</param>
        protected virtual void DeleteCandidatePictureOnFileSystem(CandidatePicture candidatePicture)
        {
            if (candidatePicture == null)
                throw new ArgumentNullException("candidatePicture");

            MediaHelper _helper = new MediaHelper();
            string lastPart = _helper.GetFileExtensionFromMimeType(candidatePicture.MimeType);
            string fileName = string.Format("{0}_0.{1}", candidatePicture.Id.ToString("000000000000"), lastPart);
            string filePath = GetCandidatePictureLocalPath(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Delete candidatePicture thumbs
        /// </summary>
        /// <param name="candidatePicture">CandidatePicture</param>
        protected virtual void DeleteCandidatePictureThumbs(CandidatePicture candidatePicture)
        {
            var fileName = candidatePicture.PictureFile == null ? candidatePicture.Id.ToString("000000000000") : candidatePicture.CandidatePictureGuid.ToString();
            string filter = string.Format("{0}*.*", fileName);

            string webRootDirecroty = _webHelper.GetRootDirectory();
            var thumbsDirectoryPath = Path.Combine(webRootDirecroty, _mediaSettings.CandidatePictureLocation, "Thumbs");

            string[] currentFiles = System.IO.Directory.GetFiles(thumbsDirectoryPath, filter, SearchOption.AllDirectories);
            foreach (string currentFileName in currentFiles)
            {
                var thumbFilePath = GetThumbLocalPath(currentFileName, candidatePicture.Id);
                File.Delete(thumbFilePath);
            }
        }

        /// <summary>
        /// Get candidatePicture (thumb) local path
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <returns>Local candidatePicture thumb path</returns>
        protected virtual string GetThumbLocalPath(string thumbFileName, int candidatePictureId = 0)
        {
            string webRootDirecroty = _webHelper.GetRootDirectory();
            var thumbsDirectoryPath = Path.Combine(webRootDirecroty, _mediaSettings.CandidatePictureLocation, "Thumbs");
         
            // get picture id from file name
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(thumbFileName);
            if (!string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                var pictureIdString = fileNameWithoutExtension.Split('_')[0];
                int pictureId;
                if (!int.TryParse(pictureIdString, out pictureId))
                    pictureId = candidatePictureId;
                if (pictureId > 0)
                {
                    thumbsDirectoryPath = Path.Combine(thumbsDirectoryPath, GetSubDirectoryName(pictureId));
                    if (!System.IO.Directory.Exists(thumbsDirectoryPath))
                    {
                        System.IO.Directory.CreateDirectory(thumbsDirectoryPath);
                    }
                }
            }
            
            var thumbFilePath = Path.Combine(thumbsDirectoryPath, thumbFileName);
            return thumbFilePath;
        }

        /// <summary>
        /// Get candidatePicture (thumb) URL 
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <param name="franchiseLocation">Franchise location URL; null to use determine the current franchise location automatically</param>
        /// <returns>Local candidatePicture thumb path</returns>
        protected virtual string GetThumbUrl(string thumbFileName, string franchiseLocation = null, int candidatePictureId = 0)
        {
            

            franchiseLocation = !String.IsNullOrEmpty(franchiseLocation)
                                    ? franchiseLocation
                                    : _webHelper.GetFranchiseUrl();
            string thumbWebPath = _mediaSettings.CandidatePictureLocation.Replace(@"\", "/") + "/Thumbs/";
            var url = franchiseLocation + thumbWebPath;

            // get picture id from file name
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(thumbFileName);
            if (!string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                var pictureIdString = fileNameWithoutExtension.Split('_')[0];
                int pictureId;
                if (!int.TryParse(pictureIdString, out pictureId))
                    pictureId = candidatePictureId;
                if (pictureId > 0)
                {
                    url = url + GetSubDirectoryName(pictureId) + "/";
                }
            }

            url = url + thumbFileName;
            return url;
        }

        /// <summary>
        /// Get candidatePicture local path. Used when images franchised on file system (not in the database)
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="imagesDirectoryPath">Directory path with images; if null, then default one is used</param>
        /// <returns>Local candidatePicture path</returns>
        protected virtual string GetCandidatePictureLocalPath(string fileName, string imagesDirectoryPath = null)
        {
            if (String.IsNullOrEmpty(imagesDirectoryPath))
            {
                // old - hard coded
                //imagesDirectoryPath = _webHelper.MapPath("~/uploads/candidatepictures/");
                // new - configurable
                string webRootDirecroty = _webHelper.GetRootDirectory();
                imagesDirectoryPath = Path.Combine(webRootDirecroty, _mediaSettings.CandidatePictureLocation);
            }

            // get sub directory from picture file id
            // NUMBER_OF_PICTURES_PER_DIRECTORY
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (!string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                var pictureIdString = fileNameWithoutExtension.Split('_')[0];
                int pictureId;
                if (int.TryParse(pictureIdString, out pictureId))
                {
                    imagesDirectoryPath = imagesDirectoryPath + @"\" + GetSubDirectoryName(pictureId);
                    if (!System.IO.Directory.Exists(imagesDirectoryPath))
                    {
                        System.IO.Directory.CreateDirectory(imagesDirectoryPath);
                    }
                }
            }

            var filePath = Path.Combine(imagesDirectoryPath, fileName);
            return filePath;
        }

        /// <summary>
        /// Gets the name of the sub directory.
        /// </summary>
        /// <param name="pictureId">The picture identifier.</param>
        /// <returns></returns>
        protected virtual string GetSubDirectoryName(int pictureId)
        {
            var subDirectoryName = ((pictureId / NUMBER_OF_PICTURES_PER_DIRECTORY) + 1) * NUMBER_OF_PICTURES_PER_DIRECTORY;
            return subDirectoryName.ToString("000000000000");
        }

        #endregion

        #region CRUD methods

        public virtual void InsertCandidatePicture(CandidatePicture candidatePicture)
        {
            if (candidatePicture == null)
                throw new ArgumentNullException("candidatePicture");

            //save into database
            _candidatePictureRepository.Insert(candidatePicture);
        }

        public virtual void UpdateCandidatePicture(CandidatePicture candidatePicture)
        {
            if (candidatePicture == null)
                throw new ArgumentNullException("candidatePicture");

            //save into database
            _candidatePictureRepository.Update(candidatePicture);
        }

        public virtual void DeleteCandidatePicture(CandidatePicture candidatePicture)
        {
            if (candidatePicture == null)
                throw new ArgumentNullException("candidatePicture");

            //delete thumbs
            DeleteCandidatePictureThumbs(candidatePicture);

            //delete from file system
            if (candidatePicture.FilePath != null)
                DeleteCandidatePictureOnFileSystem(candidatePicture);

            //delete from database
            _candidatePictureRepository.Delete(candidatePicture);
        }

        public virtual CandidatePicture InsertCandidatePicture(byte[] candidatePictureBinary, string mimeType, int candidateId, bool isNew, int displayOrder = 0, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            if (validateBinary)
                candidatePictureBinary = ValidateCandidatePicture(candidatePictureBinary, mimeType);

            var candidatePicture = new CandidatePicture
            {
                CandidatePictureGuid = Guid.NewGuid(),
                CandidateId = candidateId,
                MimeType = mimeType,
                PictureFile = candidatePictureBinary,
                DisplayOrder = displayOrder,
                IsNew = isNew,
                IsActive = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };

            _candidatePictureRepository.Insert(candidatePicture);

            return candidatePicture;
        }

        public virtual CandidatePicture UpdateCandidatePicture(int id, byte[] candidatePictureBinary, string mimeType, bool isNew, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            if (validateBinary)
                candidatePictureBinary = ValidateCandidatePicture(candidatePictureBinary, mimeType);

            var candidatePicture = GetCandidatePictureById(id);
            if (candidatePicture == null)
                return null;

            //delete old thumbs if a candidatePicture has been changed
            if (mimeType != candidatePicture.MimeType)
                DeleteCandidatePictureThumbs(candidatePicture);

            candidatePicture.MimeType = mimeType;
            candidatePicture.IsNew = isNew;

            if (candidatePicture.PictureFile == null)
                candidatePicture.FilePath = SaveCandidatePictureInFile(candidatePicture.Id, candidatePictureBinary, mimeType);
            else
                candidatePicture.PictureFile = candidatePictureBinary;

            _candidatePictureRepository.Update(candidatePicture);

            return candidatePicture;
        }


        /// <summary>
        /// Validates input candidatePicture dimensions
        /// </summary>
        /// <param name="candidatePictureBinary">CandidatePicture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>CandidatePicture binary or throws an exception</returns>
        public virtual byte[] ValidateCandidatePicture(byte[] candidatePictureBinary, string mimeType)
        {
            using(var destStream = new MemoryStream())
            {
                ImageBuilder.Current.Build(candidatePictureBinary, destStream, new ResizeSettings
                {
                    MaxWidth = _mediaSettings.MaximumImageSize,
                    MaxHeight = _mediaSettings.MaximumImageSize,
                    Quality = _mediaSettings.DefaultImageQuality
                });

                return destStream.ToArray();
            }
        }

        #endregion

        #region CandidatePicture

        /// <summary>
        /// Gets a candidatePicture
        /// </summary>
        /// <param name="id">CandidatePicture identifier</param>
        /// <returns>CandidatePicture</returns>
        public virtual CandidatePicture GetCandidatePictureById(int id)
        {
            if (id == 0)
                return null;

            return _candidatePictureRepository.GetById(id);
        }

        #endregion


        #region Getting candidatePicture local path/URL methods

        public virtual byte[] LoadCandidatePictureBinary(CandidatePicture candidatePicture)
        {
            if (candidatePicture == null)
                throw new ArgumentNullException("candidatePicture");

            return candidatePicture.PictureFile == null ?
                LoadCandidatePictureFromFile(candidatePicture.Id, candidatePicture.MimeType) :
                LoadCandidatePictureFromDatabase(candidatePicture);
        }

        public virtual string GetDefaultCandidatePictureUrl(int targetSize = 0, 
            CandidatePictureType defaultCandidatePictureType = CandidatePictureType.Entity,
            string franchiseLocation = null)
        {
            string defaultImageFileName;
            switch (defaultCandidatePictureType)
            {
                case CandidatePictureType.Entity:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.DefaultImageName", "default-image.gif");
                    break;
                case CandidatePictureType.Avatar:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.Candidate.DefaultAvatarImageName", "default-avatar.jpg");
                    break;
                default:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.DefaultImageName", "default-image.gif");
                    break;
            }

            string filePath = GetCandidatePictureLocalPath(defaultImageFileName,
                imagesDirectoryPath: _settingService.GetSettingByKey<string>("Media.DefaultImageDirectoryPath"));

            if (!File.Exists(filePath))
            {
                return "";
            }
            if (targetSize == 0)
            {
                // new - configurable
                string imageWebPath = _mediaSettings.CandidatePictureLocation.Replace(@"\", "/") + "/";
                string url = (!String.IsNullOrEmpty(franchiseLocation)
                                 ? franchiseLocation
                                 : _webHelper.GetFranchiseUrl())
                                 + imageWebPath + defaultImageFileName;
                                // +"uploads/candidatepictures/" + defaultImageFileName;
                return url;
            }
            else
            {
                string fileExtension = Path.GetExtension(filePath);
                string thumbFileName = string.Format("{0}_{1}{2}",
                    Path.GetFileNameWithoutExtension(filePath),
                    targetSize,
                    fileExtension);
                var thumbFilePath = GetThumbLocalPath(thumbFileName);
                if (!File.Exists(thumbFilePath))
                {
                    using (var b = new Bitmap(filePath))
                    {
                        var newSize = MediaHelper.CalculateDimensions(b.Size, targetSize);

                        using (var destStream = new MemoryStream())
                        {
                            ImageBuilder.Current.Build(b, destStream, new ResizeSettings
                            {
                                Width = newSize.Width,
                                Height = newSize.Height,
                                Scale = ScaleMode.Both,
                                Quality = _mediaSettings.DefaultImageQuality
                            });
                            var destBinary = destStream.ToArray();
                            File.WriteAllBytes(thumbFilePath, destBinary);
                        }
                    }
                }
                var url = GetThumbUrl(thumbFileName, franchiseLocation);
                return url;
            }
        }

        public virtual string GetCandidatePictureUrl(int candidatePictureId,
            int targetSize = 0,
            bool showDefaultCandidatePicture = true, 
            string franchiseLocation = null, 
            CandidatePictureType defaultCandidatePictureType = CandidatePictureType.Entity)
        {
            var candidatePicture = GetCandidatePictureById(candidatePictureId);
            return GetCandidatePictureUrl(candidatePicture, targetSize, showDefaultCandidatePicture, franchiseLocation, defaultCandidatePictureType);
        }
        
        /// <summary>
        /// Get a candidatePicture URL
        /// </summary>
        /// <param name="candidatePicture">CandidatePicture instance</param>
        /// <param name="targetSize">The target candidatePicture size (longest side)</param>
        /// <param name="showDefaultCandidatePicture">A value indicating whether the default candidatePicture is shown</param>
        /// <param name="franchiseLocation">Franchise location URL; null to use determine the current franchise location automatically</param>
        /// <param name="defaultCandidatePictureType">Default candidatePicture type</param>
        /// <returns>CandidatePicture URL</returns>
        public virtual string GetCandidatePictureUrl(CandidatePicture candidatePicture, 
            int targetSize = 0,
            bool showDefaultCandidatePicture = true, 
            string franchiseLocation = null, 
            CandidatePictureType defaultCandidatePictureType = CandidatePictureType.Entity)
        {
            string url = string.Empty;
            byte[] candidatePictureBinary = null;
            if (candidatePicture != null)
                candidatePictureBinary = LoadCandidatePictureBinary(candidatePicture);
            if (candidatePicture == null || candidatePictureBinary == null || candidatePictureBinary.Length == 0)
            {
                if(showDefaultCandidatePicture)
                {
                    url = GetDefaultCandidatePictureUrl(targetSize, defaultCandidatePictureType, franchiseLocation);
                }
                return url;
            }

            MediaHelper _helper = new MediaHelper();
            string lastPart = _helper.GetFileExtensionFromMimeType(candidatePicture.MimeType);
            string thumbFileName;
            if (candidatePicture.IsNew)
            {
                DeleteCandidatePictureThumbs(candidatePicture);

                //we do not validate candidatePicture binary here to ensure that no exception ("Parameter is not valid") will be thrown
                candidatePicture = UpdateCandidatePicture(candidatePicture.Id, 
                    candidatePictureBinary, 
                    candidatePicture.MimeType, 
                    false, 
                    false);
            }
            lock (s_lock)
            {
                var fileName = candidatePicture.PictureFile == null ? candidatePicture.Id.ToString("000000000000") : candidatePicture.CandidatePictureGuid.ToString();
                if (targetSize == 0)
                {
                    thumbFileName = string.Format("{0}.{1}", fileName, lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName, candidatePicture.Id);
                    if (!File.Exists(thumbFilePath))
                    {
                        File.WriteAllBytes(thumbFilePath, candidatePictureBinary);
                    }
                }
                else
                {
                    thumbFileName = string.Format("{0}_{1}.{2}", fileName, targetSize, lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName, candidatePicture.Id);
                    if (!File.Exists(thumbFilePath))
                    {
                        using (var stream = new MemoryStream(candidatePictureBinary))
                        {
                            Bitmap b = null;
                            try
                            {
                                //try-catch to ensure that candidatePicture binary is really OK. Otherwise, we can get "Parameter is not valid" exception if binary is corrupted for some reasons
                                b = new Bitmap(stream);
                            }
                            catch (ArgumentException exc)
                            {
                                _logger.Error(string.Format("Error generating candidatePicture thumb. ID={0}", candidatePicture.Id), exc);
                            }
                            if (b == null)
                            {
                                //bitmap could not be loaded for some reasons
                                return url;
                            }

                            var newSize = MediaHelper.CalculateDimensions(b.Size, targetSize);

                            using (var destStream = new MemoryStream())
                            {
                                ImageBuilder.Current.Build(b, destStream, new ResizeSettings
                                {
                                    Width = newSize.Width,
                                    Height = newSize.Height,
                                    Scale = ScaleMode.Both,
                                    Quality = _mediaSettings.DefaultImageQuality
                                });
                                var destBinary = destStream.ToArray();
                                File.WriteAllBytes(thumbFilePath, destBinary);
                            }

                            b.Dispose();
                        }
                    }
                }
            }
            url = GetThumbUrl(thumbFileName, franchiseLocation, candidatePicture.Id);
            return url;
        }

        /// <summary>
        /// Get a candidatePicture local path
        /// </summary>
        /// <param name="candidatePicture">CandidatePicture instance</param>
        /// <param name="targetSize">The target candidatePicture size (longest side)</param>
        /// <param name="showDefaultCandidatePicture">A value indicating whether the default candidatePicture is shown</param>
        /// <returns></returns>
        public virtual string GetThumbLocalPath(CandidatePicture candidatePicture, int targetSize = 0, bool showDefaultCandidatePicture = true)
        {
            string url = GetCandidatePictureUrl(candidatePicture, targetSize, showDefaultCandidatePicture);
            if(String.IsNullOrEmpty(url))
                return String.Empty;
            
            return GetThumbLocalPath(Path.GetFileName(url), candidatePicture==null?0:candidatePicture.Id);
        }

        #endregion


        #region LIST

        /// <summary>
        /// Gets candidatePictures by candidate identifier
        /// </summary>
        /// <param name="candidateId">Candidate identifier</param>
        /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        /// <returns>CandidatePictures</returns>
        public virtual IList<CandidatePicture> GetCandidatePicturesByCandidateId(int candidateId, int recordsToReturn = 0)
        {
            if (candidateId == 0)
                return new List<CandidatePicture>();

            var query = _candidatePictureRepository.Table;

            query = from cp in query
                    where cp.CandidateId == candidateId
                    orderby cp.DisplayOrder descending , cp.UpdatedOnUtc descending
                    select cp;

            if (recordsToReturn > 0)
                query = query.Take(recordsToReturn);

            var pics = query.ToList();
            return pics;
        }

        #endregion


        #region Properties
        
        /// <summary>
        /// Gets or sets a value indicating whether the images should be stored in data base.
        /// </summary>
        public virtual bool StoreInDb
        {
            get
            {
                return _settingService.GetSettingByKey("Media.Images.StoreInDB", true);
            }
            set
            {
                //check whether it's a new value
                if (this.StoreInDb != value)
                {
                    //save the new setting value
                    _settingService.SetSetting("Media.Images.StoreInDB", value);

                    ////update all candidatePicture objects
                    //var candidatePictures = this.GetCandidatePictures(0, int.MaxValue);
                    //foreach (var candidatePicture in candidatePictures)
                    //{
                    //    var candidatePictureBinary = LoadCandidatePictureBinary(candidatePicture, !value);

                    //    //delete from file system
                    //    if (value)
                    //        DeleteCandidatePictureOnFileSystem(candidatePicture);

                    //    //just update a candidatePicture (all required logic is in UpdateCandidatePicture method)
                    //    UpdateCandidatePicture(candidatePicture.Id,
                    //                  candidatePictureBinary,
                    //                  candidatePicture.MimeType,
                    //                  candidatePicture.SeoFilename,
                    //                  true,
                    //                  false);
                    //    //we do not validate candidatePicture binary here to ensure that no exception ("Parameter is not valid") will be thrown when "moving" candidatePictures
                    //}
                }
            }
        }

        #endregion
    }
}
