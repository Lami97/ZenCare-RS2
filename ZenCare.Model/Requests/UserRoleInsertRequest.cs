using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class UserRoleInsertRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
