using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Shared.Models
{
    public class Order
    {
        public int ID { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Created;

        [Required]
        public required int UserID { get; set; }

        [Required]
        public required int ProductID { get; set; }

        [Required]
        public required int Quantity { get; set; }

        [Required]
        public required string Address { get; set; }
    }
}
