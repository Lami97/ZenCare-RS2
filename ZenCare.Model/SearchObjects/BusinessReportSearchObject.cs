namespace ZenCare.Model.SearchObjects
{
    public class BusinessReportSearchObject : BaseSearchObject
    {
        public string? ReportType { get; set; }

        public int? GeneratedByUserId { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}
