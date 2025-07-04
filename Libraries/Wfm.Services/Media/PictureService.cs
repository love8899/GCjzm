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
using Wfm.Services.Seo;
using Wfm.Services.Helpers;

namespace Wfm.Services.Media
{
    /// <summary>
    /// Picture service
    /// </summary>
    public partial class PictureService : IPictureService
    {
        #region Const

        private const int MULTIPLE_THUMB_DIRECTORIES_LENGTH = 3;

        #endregion

        #region Fields

        private static readonly object s_lock = new object();

        private readonly IRepository<Picture> _pictureRepository;
        private readonly IRepository<CandidatePicture> _candidatePictureRepository;
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
        /// <param name="pictureRepository">Picture repository</param>
        /// <param name="candidatePictureRepository">Candidate picture repository</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="logger">Logger</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="mediaSettings">Media settings</param>
        public PictureService(IRepository<Picture> pictureRepository,
            IRepository<CandidatePicture> candidatePictureRepository,
            ISettingService settingService, IWebHelper webHelper,
            ILogger logger, IEventPublisher eventPublisher,
            MediaSettings mediaSettings)
        {
            this._pictureRepository = pictureRepository;
            this._candidatePictureRepository = candidatePictureRepository;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._logger = logger;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Loads a picture from file
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary</returns>
        protected virtual byte[] LoadPictureFromFile(int pictureId, string mimeType)
        {
            MediaHelper _helper = new MediaHelper();
            string lastPart = _helper.GetFileExtensionFromMimeType(mimeType);
            string fileName = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);
            var filePath = GetPictureLocalPath(fileName);
            if (!File.Exists(filePath))
                return new byte[0];
            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// Save picture on file system
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        protected virtual void SavePictureInFile(int pictureId, byte[] pictureBinary, string mimeType)
        {
            MediaHelper _helper = new MediaHelper();
            string lastPart = _helper.GetFileExtensionFromMimeType(mimeType);
            string fileName = string.Format("{0}_0.{1}", pictureId.ToString("0000000"), lastPart);
            File.WriteAllBytes(GetPictureLocalPath(fileName), pictureBinary);
        }

        /// <summary>
        /// Delete a picture on file system
        /// </summary>
        /// <param name="picture">Picture</param>
        protected virtual void DeletePictureOnFileSystem(Picture picture)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            MediaHelper _helper = new MediaHelper();
            string lastPart = _helper.GetFileExtensionFromMimeType(picture.MimeType);
            string fileName = string.Format("{0}_0.{1}", picture.Id.ToString("0000000"), lastPart);
            string filePath = GetPictureLocalPath(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Delete picture thumbs
        /// </summary>
        /// <param name="picture">Picture</param>
        protected virtual void DeletePictureThumbs(Picture picture)
        {
            string filter = string.Format("{0}*.*", picture.Id.ToString("0000000"));
            var thumbDirectoryPath = _webHelper.MapPath("~/content/images/thumbs");
            string[] currentFiles = System.IO.Directory.GetFiles(thumbDirectoryPath, filter, SearchOption.AllDirectories);
            foreach (string currentFileName in currentFiles)
            {
                var thumbFilePath = GetThumbLocalPath(currentFileName);
                File.Delete(thumbFilePath);
            }
        }

        /// <summary>
        /// Get picture (thumb) local path
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <returns>Local picture thumb path</returns>
        protected virtual string GetThumbLocalPath(string thumbFileName)
        {
            var thumbsDirectoryPath = _webHelper.MapPath("~/content/images/thumbs");
            if (_mediaSettings.MultipleThumbDirectories)
            {
                //get the first two letters of the file name
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(thumbFileName);
                if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > MULTIPLE_THUMB_DIRECTORIES_LENGTH)
                {
                    var subDirectoryName = fileNameWithoutExtension.Substring(0, MULTIPLE_THUMB_DIRECTORIES_LENGTH);
                    thumbsDirectoryPath = Path.Combine(thumbsDirectoryPath, subDirectoryName);
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
        /// Get picture (thumb) URL 
        /// </summary>
        /// <param name="thumbFileName">Filename</param>
        /// <param name="franchiseLocation">Franchise location URL; null to use determine the current franchise location automatically</param>
        /// <returns>Local picture thumb path</returns>
        protected virtual string GetThumbUrl(string thumbFileName, string franchiseLocation = null)
        {
            franchiseLocation = !String.IsNullOrEmpty(franchiseLocation)
                                    ? franchiseLocation
                                    : _webHelper.GetFranchiseUrl();
            var url = franchiseLocation + "content/images/thumbs/";

            if (_mediaSettings.MultipleThumbDirectories)
            {
                //get the first two letters of the file name
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(thumbFileName);
                if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length > MULTIPLE_THUMB_DIRECTORIES_LENGTH)
                {
                    var subDirectoryName = fileNameWithoutExtension.Substring(0, MULTIPLE_THUMB_DIRECTORIES_LENGTH);
                    url = url + subDirectoryName + "/";
                }
            }

            url = url + thumbFileName;
            return url;
        }

        /// <summary>
        /// Get picture local path. Used when images franchised on file system (not in the database)
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="imagesDirectoryPath">Directory path with images; if null, then default one is used</param>
        /// <returns>Local picture path</returns>
        protected virtual string GetPictureLocalPath(string fileName, string imagesDirectoryPath = null)
        {
            if (String.IsNullOrEmpty(imagesDirectoryPath))
            {
                imagesDirectoryPath = _webHelper.MapPath("~/content/images/");
            }
            var filePath = Path.Combine(imagesDirectoryPath, fileName);
            return filePath;
        }

        /// <summary>
        /// Gets the loaded picture binary depending on picture storage settings
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <param name="fromDb">Load from database; otherwise, from file system</param>
        /// <returns>Picture binary</returns>
        protected virtual byte[] LoadPictureBinary(Picture picture, bool fromDb)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            var result = fromDb 
                ? picture.PictureBinary
                : LoadPictureFromFile(picture.Id, picture.MimeType);
            return result;
        }

        #endregion

        #region Getting picture local path/URL methods

        /// <summary>
        /// Gets the loaded picture binary depending on picture storage settings
        /// </summary>
        /// <param name="picture">Picture</param>
        /// <returns>Picture binary</returns>
        public virtual byte[] LoadPictureBinary(Picture picture)
        {
            return LoadPictureBinary(picture, this.StoreInDb);
        }

        /// <summary>
        /// Get picture SEO friendly name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public virtual string GetPictureSeName(string name)
        {
            return SeoExtensions.GetSeName(name, true, false);
        }

        /// <summary>
        /// Gets the default picture URL
        /// </summary>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <param name="franchiseLocation">Franchise location URL; null to use determine the current franchise location automatically</param>
        /// <returns>Picture URL</returns>
        public virtual string GetDefaultPictureUrl(int targetSize = 0, 
            PictureType defaultPictureType = PictureType.Entity,
            string franchiseLocation = null)
        {
            string defaultImageFileName;
            switch (defaultPictureType)
            {
                case PictureType.Entity:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.DefaultImageName", "default-image.gif");
                    break;
                case PictureType.Avatar:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.Candidate.DefaultAvatarImageName", "default-avatar.jpg");
                    break;
                default:
                    defaultImageFileName = _settingService.GetSettingByKey("Media.DefaultImageName", "default-image.gif");
                    break;
            }

            string filePath = GetPictureLocalPath(defaultImageFileName,
                imagesDirectoryPath: _settingService.GetSettingByKey<string>("Media.DefaultImageDirectoryPath"));

            if (!File.Exists(filePath))
            {
                return "";
            }
            if (targetSize == 0)
            {
                string url = (!String.IsNullOrEmpty(franchiseLocation)
                                 ? franchiseLocation
                                 : _webHelper.GetFranchiseUrl())
                                 + "content/images/" + defaultImageFileName;
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

        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="franchiseLocation">Franchise location URL; null to use determine the current franchise location automatically</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>Picture URL</returns>
        public virtual string GetPictureUrl(int pictureId,
            int targetSize = 0,
            bool showDefaultPicture = true, 
            string franchiseLocation = null, 
            PictureType defaultPictureType = PictureType.Entity)
        {
            var picture = GetPictureById(pictureId);
            return GetPictureUrl(picture, targetSize, showDefaultPicture, franchiseLocation, defaultPictureType);
        }
        
        /// <summary>
        /// Get a picture URL
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <param name="franchiseLocation">Franchise location URL; null to use determine the current franchise location automatically</param>
        /// <param name="defaultPictureType">Default picture type</param>
        /// <returns>Picture URL</returns>
        public virtual string GetPictureUrl(Picture picture, 
            int targetSize = 0,
            bool showDefaultPicture = true, 
            string franchiseLocation = null, 
            PictureType defaultPictureType = PictureType.Entity)
        {
            string url = string.Empty;
            byte[] pictureBinary = null;
            if (picture != null)
                pictureBinary = LoadPictureBinary(picture);
            if (picture == null || pictureBinary == null || pictureBinary.Length == 0)
            {
                if(showDefaultPicture)
                {
                    url = GetDefaultPictureUrl(targetSize, defaultPictureType, franchiseLocation);
                }
                return url;
            }

            MediaHelper _helper = new MediaHelper();
            string lastPart = _helper.GetFileExtensionFromMimeType(picture.MimeType);
            string thumbFileName;
            if (picture.IsNew)
            {
                DeletePictureThumbs(picture);

                //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown
                picture = UpdatePicture(picture.Id, 
                    pictureBinary, 
                    picture.MimeType, 
                    picture.SeoFilename, 
                    false, 
                    false);
            }
            lock (s_lock)
            {
                string seoFileName = picture.SeoFilename; // = GetPictureSeName(picture.SeoFilename); //just for sure
                if (targetSize == 0)
                {
                    thumbFileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{0}_{1}.{2}", picture.Id.ToString("0000000"), seoFileName, lastPart) :
                        string.Format("{0}.{1}", picture.Id.ToString("0000000"), lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!File.Exists(thumbFilePath))
                    {
                        File.WriteAllBytes(thumbFilePath, pictureBinary);
                    }
                }
                else
                {
                    thumbFileName = !String.IsNullOrEmpty(seoFileName) ?
                        string.Format("{0}_{1}_{2}.{3}", picture.Id.ToString("0000000"), seoFileName, targetSize, lastPart) :
                        string.Format("{0}_{1}.{2}", picture.Id.ToString("0000000"), targetSize, lastPart);
                    var thumbFilePath = GetThumbLocalPath(thumbFileName);
                    if (!File.Exists(thumbFilePath))
                    {
                        using (var stream = new MemoryStream(pictureBinary))
                        {
                            Bitmap b = null;
                            try
                            {
                                //try-catch to ensure that picture binary is really OK. Otherwise, we can get "Parameter is not valid" exception if binary is corrupted for some reasons
                                b = new Bitmap(stream);
                            }
                            catch (ArgumentException exc)
                            {
                                _logger.Error(string.Format("Error generating picture thumb. ID={0}", picture.Id), exc);
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
            url = GetThumbUrl(thumbFileName, franchiseLocation);
            return url;
        }

        /// <summary>
        /// Get a picture local path
        /// </summary>
        /// <param name="picture">Picture instance</param>
        /// <param name="targetSize">The target picture size (longest side)</param>
        /// <param name="showDefaultPicture">A value indicating whether the default picture is shown</param>
        /// <returns></returns>
        public virtual string GetThumbLocalPath(Picture picture, int targetSize = 0, bool showDefaultPicture = true)
        {
            string url = GetPictureUrl(picture, targetSize, showDefaultPicture);
            if(String.IsNullOrEmpty(url))
                return String.Empty;
            
            return GetThumbLocalPath(Path.GetFileName(url));
        }

        #endregion

        #region CRUD methods

        /// <summary>
        /// Gets a picture
        /// </summary>
        /// <param name="pictureId">Picture identifier</param>
        /// <returns>Picture</returns>
        public virtual Picture GetPictureById(int pictureId)
        {
            if (pictureId == 0)
                return null;

            return _pictureRepository.GetById(pictureId);
        }

        /// <summary>
        /// Deletes a picture
        /// </summary>
        /// <param name="picture">Picture</param>
        public virtual void DeletePicture(Picture picture)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            //delete thumbs
            DeletePictureThumbs(picture);
            
            //delete from file system
            if (!this.StoreInDb)
                DeletePictureOnFileSystem(picture);

            //delete from database
            _pictureRepository.Delete(picture);

            //event notification
            _eventPublisher.EntityDeleted(picture);
        }

        /// <summary>
        /// Gets a collection of pictures
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items on each page</param>
        /// <returns>Paged list of pictures</returns>
        public virtual IPagedList<Picture> GetPictures(int pageIndex, int pageSize)
        {
            var query = from p in _pictureRepository.Table
                       orderby p.Id descending
                       select p;
            var pics = new PagedList<Picture>(query, pageIndex, pageSize);
            return pics;
        }
        

        /// <summary>
        /// Gets pictures by candidate identifier
        /// </summary>
        /// <param name="candidateId">Candidate identifier</param>
        /// <param name="recordsToReturn">Number of records to return. 0 if you want to get all items</param>
        /// <returns>Pictures</returns>
        //public virtual IList<Picture> GetPicturesByCandidateId(int candidateId, int recordsToReturn = 0)
        //{
        //    if (candidateId == 0)
        //        return new List<Picture>();

            
        //    var query = from p in _pictureRepository.Table
        //                join pp in _candidatePictureRepository.Table on p.Id equals pp.PictureId
        //                orderby pp.DisplayOrder
        //                where pp.CandidateId == candidateId
        //                select p;

        //    if (recordsToReturn > 0)
        //        query = query.Take(recordsToReturn);

        //    var pics = query.ToList();
        //    return pics;
        //}

        /// <summary>
        /// Inserts a picture
        /// </summary>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
        /// <returns>Picture</returns>
        public virtual Picture InsertPicture(byte[] pictureBinary, string mimeType, string seoFilename,
            bool isNew, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            if (validateBinary)
                pictureBinary = ValidatePicture(pictureBinary, mimeType);

            var picture = new Picture
                              {
                                  PictureBinary = this.StoreInDb ? pictureBinary : new byte[0],
                                  MimeType = mimeType,
                                  SeoFilename = seoFilename,
                                  IsNew = isNew,
                                  CreatedOnUtc = DateTime.UtcNow,
                                  UpdatedOnUtc = DateTime.UtcNow,
                              };
            _pictureRepository.Insert(picture);

            if(!this.StoreInDb)
                SavePictureInFile(picture.Id, pictureBinary, mimeType);
            
            //event notification
            _eventPublisher.EntityInserted(picture);

            return picture;
        }

        /// <summary>
        /// Updates the picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="pictureBinary">The picture binary</param>
        /// <param name="mimeType">The picture MIME type</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <param name="isNew">A value indicating whether the picture is new</param>
        /// <param name="validateBinary">A value indicating whether to validated provided picture binary</param>
        /// <returns>Picture</returns>
        public virtual Picture UpdatePicture(int pictureId, byte[] pictureBinary, string mimeType,
            string seoFilename, bool isNew, bool validateBinary = true)
        {
            mimeType = CommonHelper.EnsureNotNull(mimeType);
            mimeType = CommonHelper.EnsureMaximumLength(mimeType, 20);

            seoFilename = CommonHelper.EnsureMaximumLength(seoFilename, 100);

            if (validateBinary)
                pictureBinary = ValidatePicture(pictureBinary, mimeType);

            var picture = GetPictureById(pictureId);
            if (picture == null)
                return null;

            //delete old thumbs if a picture has been changed
            if (seoFilename != picture.SeoFilename)
                DeletePictureThumbs(picture);

            picture.PictureBinary = (this.StoreInDb ? pictureBinary : new byte[0]);
            picture.MimeType = mimeType;
            picture.SeoFilename = seoFilename;
            picture.IsNew = isNew;

            _pictureRepository.Update(picture);

            if(!this.StoreInDb)
                SavePictureInFile(picture.Id, pictureBinary, mimeType);

            //event notification
            _eventPublisher.EntityUpdated(picture);

            return picture;
        }

        /// <summary>
        /// Updates a SEO filename of a picture
        /// </summary>
        /// <param name="pictureId">The picture identifier</param>
        /// <param name="seoFilename">The SEO filename</param>
        /// <returns>Picture</returns>
        public virtual Picture SetSeoFilename(int pictureId, string seoFilename)
        {
            var picture = GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            //update if it has been changed
            if (seoFilename != picture.SeoFilename)
            {
                //update picture
                picture = UpdatePicture(picture.Id, LoadPictureBinary(picture), picture.MimeType, seoFilename, true, false);
            }
            return picture;
        }

        /// <summary>
        /// Validates input picture dimensions
        /// </summary>
        /// <param name="pictureBinary">Picture binary</param>
        /// <param name="mimeType">MIME type</param>
        /// <returns>Picture binary or throws an exception</returns>
        public virtual byte[] ValidatePicture(byte[] pictureBinary, string mimeType)
        {
            using (var destStream = new MemoryStream())
            {
                ImageBuilder.Current.Build(pictureBinary, destStream, new ResizeSettings
                {
                    MaxWidth = _mediaSettings.MaximumImageSize,
                    MaxHeight = _mediaSettings.MaximumImageSize,
                    Quality = _mediaSettings.DefaultImageQuality
                });

                return destStream.ToArray();
            }
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
                return _settingService.GetSettingByKey("Media.Images.StoreInDB", false);
            }
            set
            {
                //check whether it's a new value
                if (this.StoreInDb != value)
                {
                    //save the new setting value
                    _settingService.SetSetting("Media.Images.StoreInDB", value);

                    //update all picture objects
                    var pictures = this.GetPictures(0, int.MaxValue);
                    foreach (var picture in pictures)
                    {
                        var pictureBinary = LoadPictureBinary(picture, !value);

                        //delete from file system
                        if (value)
                            DeletePictureOnFileSystem(picture);

                        //just update a picture (all required logic is in UpdatePicture method)
                        UpdatePicture(picture.Id,
                                      pictureBinary,
                                      picture.MimeType,
                                      picture.SeoFilename,
                                      true,
                                      false);
                        //we do not validate picture binary here to ensure that no exception ("Parameter is not valid") will be thrown when "moving" pictures
                    }
                }
            }
        }

        #endregion
    }
}
