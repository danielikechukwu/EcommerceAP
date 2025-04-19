using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        //Relation: One customer can have many order
        [Required]
        public int CustomerId { get; set; }

        [JsonIgnore]
        public Customer Customer { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OrderAmount { get; set; }

        // One Order can have multiple OrderItems
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
