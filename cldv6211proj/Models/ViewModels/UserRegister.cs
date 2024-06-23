using System.ComponentModel.DataAnnotations;

namespace cldv6211proj.Models.ViewModels
{
    public class UserRegister
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Surname { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
