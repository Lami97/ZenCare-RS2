using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class EmployeeServiceInsertRequest
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int WellnessServiceId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
