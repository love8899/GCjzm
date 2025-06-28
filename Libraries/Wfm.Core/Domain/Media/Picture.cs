using System.Collections.Generic;
using Wfm.Core.Domain.Candidates;

namespace Wfm.Core.Domain.Media
{
    /// <summary>
    /// Represents a picture
    /// </summary>
    public partial class Picture : BaseEntity
    {
        //private ICollection<CandidatePicture> _candidatePictures;



        /// <summary>
        /// Gets or sets the picture binary
        /// </summary>
        public byte[] PictureBinary { get; set; }

        /// <summary>
        /// Gets or sets the picture mime type
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the SEO friednly filename of the picture
        /// </summary>
        public string SeoFilename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the picture is new
        /// </summary>
        public bool IsNew { get; set; }



        /// <summary>
        /// Gets or sets the candidate pictures
        /// </summary>
        //public virtual ICollection<CandidatePicture> CandidatePictures
        //{
        //    get { return _candidatePictures ?? (_candidatePictures = new List<CandidatePicture>()); }
        //    protected set { _candidatePictures = value; }
        //}
    }
}
