using System;


namespace Wfm.Core.Domain.Candidates
{
    /// <summary>
    /// Represents a candidate picture mapping
    /// </summary>
    public partial class CandidatePicture : BaseEntity
    {
        /// <summary>
        /// Gets or sets the candidate identifier
        /// </summary>
        public int CandidateId { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the picture mime type
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }


        public Guid CandidatePictureGuid { get; set; }

        public byte[] PictureFile { get; set; }


        /// <summary>
        /// Gets the candidate
        /// </summary>
        public virtual Candidate Candidate { get; set; }
    }

}
