using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class CartUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
