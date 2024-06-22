using System.ComponentModel.DataAnnotations;

namespace cldv6211proj.Models
{
    public class Order
    {
        public int ID { get; set; }
        public bool Processed { get; set; } = false;

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
