namespace cldv6211proj.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using cldv6211proj.Models.Database;

    public class OrderSubmitForm
    {
        [Required]
        public required int UserID { get; set; }

        [Required]
        public required int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Address { get; set; } = "";
    }
}
