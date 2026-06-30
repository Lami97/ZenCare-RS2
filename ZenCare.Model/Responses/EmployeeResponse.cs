namespace ZenCare.Model.Responses
{
    public class EmployeeResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string? Bio { get; set; }
        public DateTime? HireDate { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
