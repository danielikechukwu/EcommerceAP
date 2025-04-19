using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.DTOs
{
    public class OrderDTO
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        public List<OrderItemDTO> Items { get; set; }  
    }
}
