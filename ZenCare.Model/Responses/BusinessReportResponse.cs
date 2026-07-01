namespace ZenCare.Model.Responses
{
    public class BusinessReportResponse
    {
        public int Id { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int GeneratedByUserId { get; set; }
        public string GeneratedByUserName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalPurchases { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
