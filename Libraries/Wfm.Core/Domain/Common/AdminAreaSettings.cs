using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.Common
{
    public class AdminAreaSettings : ISettings
    {
       
        /// <summary>
        /// A value indicating whether to display candidate pictures in admin area
        /// </summary>
        public bool DisplayCandidatePictures { get; set; }
        /// <summary>
        /// Additional settings for rich editor
        /// </summary>
        public string RichEditorAdditionalSettings { get; set; }
        /// <summary>
        ///A value indicating whether to javascript is supported in rcih editor
        /// </summary>
        public bool RichEditorAllowJavaScript { get; set; }
    }
}