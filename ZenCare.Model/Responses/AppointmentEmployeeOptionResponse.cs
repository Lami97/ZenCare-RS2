namespace ZenCare.Model.Responses
{
    public class AppointmentEmployeeOptionResponse
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public bool IsAvailable { get; set; }
    }
}
