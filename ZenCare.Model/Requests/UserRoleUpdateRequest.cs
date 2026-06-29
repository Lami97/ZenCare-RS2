using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class UserRoleUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
