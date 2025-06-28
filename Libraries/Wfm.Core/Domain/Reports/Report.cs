namespace Wfm.Core.Domain.Reports
{
    public partial class Report :BaseEntity
    {
        public string ReportName { get; set; }

        public string ReportTitle { get; set; }

        public string QueryString { get; set; }

        public int QueryId { get; set; }

        public string Note { get; set; }

        public string Comment { get; set; }

        public string Message { get; set; }

    }


}