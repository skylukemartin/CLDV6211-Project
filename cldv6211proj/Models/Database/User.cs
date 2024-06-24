using System.ComponentModel.DataAnnotations;

namespace cldv6211proj.Models.Database
{
    public class User
    {
        public int ID { get; set; }
        public double Balance { get; set; } = 0;

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Surname { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
