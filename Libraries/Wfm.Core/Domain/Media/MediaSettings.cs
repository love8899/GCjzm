using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.Media
{
    public class MediaSettings : ISettings
    {
        public bool SaveAttachmentAsFile { get; set; }
        public string AttachmentLocation { get; set; }

        public string CandidatePictureLocation { get; set; }
        public string CandidateTestResultFileLocation { get; set; }
        public string TestImageLocation { get; set; }
        public string VideoFileLocation { get; set; }
      
        public int CandidateDetailsPictureSize { get; set; }
        public int CandidateThumbPictureSizeOnCandidateDetailsPage { get; set; }
        public int AssociatedCandidatePictureSize { get; set; }
        public int AutoCompleteSearchThumbPictureSize { get; set; }

        public bool DefaultPictureZoomEnabled { get; set; }

        public int MaximumImageSize { get; set; }

        /// <summary>
        /// Geta or sets a default quality used for image generation
        /// </summary>
        public int DefaultImageQuality { get; set; }

        /// <summary>
        /// Geta or sets a vaue indicating whether single (/content/images/thumbs/) or multiple (/content/images/thumbs/001/ and /content/images/thumbs/002/) directories will used for picture thumbs
        /// </summary>
        public bool MultipleThumbDirectories { get; set; }

        // Email Resume
        public bool SaveResumeAsFile { get; set; }
        public string WebRoot { get; set; }
        public string EmailResumeLocation { get; set; }
        public int ResumeDownloadBatch { get; set; }
    }
}