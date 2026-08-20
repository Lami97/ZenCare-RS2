namespace ZenCare.Model.Responses;

public class BusinessAnalyticsResponse
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal TotalRevenue { get; set; }
    public int CompletedPurchases { get; set; }
    public int CompletedAppointments { get; set; }
    public int UniqueClients { get; set; }
    public int TotalUsers { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalAppointments { get; set; }
    public int TotalServices { get; set; }
    public int TotalProducts { get; set; }
    public int TotalPurchases { get; set; }
    public List<BestSellingProductAnalyticsResponse> BestSellingProducts { get; set; } = [];
    public List<ServiceUsageAnalyticsResponse> ServiceUsage { get; set; } = [];
    public List<WeeklyAttendanceAnalyticsResponse> WeeklyAttendance { get; set; } = [];
    public List<StatusCountAnalyticsResponse> AppointmentStatuses { get; set; } = [];
    public List<NamedCountAnalyticsResponse> EmployeeWorkload { get; set; } = [];
    public List<NamedCountAnalyticsResponse> ClientActivity { get; set; } = [];
}

public class BestSellingProductAnalyticsResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class ServiceUsageAnalyticsResponse
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
}

public class WeeklyAttendanceAnalyticsResponse
{
    public DateTime WeekStart { get; set; }
    public int AttendanceCount { get; set; }
}

public class StatusCountAnalyticsResponse
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class NamedCountAnalyticsResponse
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}
