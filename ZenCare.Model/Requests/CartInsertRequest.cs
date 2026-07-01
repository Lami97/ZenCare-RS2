using System.ComponentModel.DataAnnotations;

namespace ZenCare.Model.Requests
{
    public class CartInsertRequest
    {
        [Required]
        public int UserId { get; set; }
    }
}
