namespace cldv6211proj.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using cldv6211proj.Models.Database;

    public class OrderPlace
    {
        [Required]
        public required User User { get; set; }

        [Required]
        public required Product Product { get; set; }
    }
}
