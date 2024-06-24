using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class Product
    {
        public int ID { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int Availability { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? Category { get; set; }

        [Required]
        public string? ImageURL { get; set; }

        [Required]
        public int UserID { get; set; }
    }
}
