﻿using Wfm.Core.Configuration;

namespace Wfm.Core.Domain.Common
{
    public class PdfSettings : ISettings
    {
        /// <summary>
        /// PDF logo picture identifier
        /// </summary>
        public int LogoPictureId { get; set; }

        /// <summary>
        /// Gets or sets whether letter page size is enabled
        /// </summary>
        public bool LetterPageSizeEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to render order notes in PDf reports
        /// </summary>
        public bool RenderOrderNotes { get; set; }

        /// <summary>
        /// Gets or sets the font file name that will be used
        /// </summary>
        public string FontFileName { get; set; }
    }
}