using Wfm.Web.Framework.Mvc;

namespace Wfm.Web.Models.Candidate
{
    public partial class MediaFileModel : BaseWfmModel
    {
        public int FileNo { get; set; }

        public string MediaFilePath { get; set; }

        public string ImageFilePath { get; set; }

        public string Title { get; set; }
    }
}