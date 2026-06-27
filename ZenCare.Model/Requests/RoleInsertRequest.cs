using System.ComponentModel.DataAnnotations;
using ZenCare.Model.Enums;

namespace ZenCare.Model.Requests
{
    public class RoleInsertRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public UserRoleType RoleType { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
