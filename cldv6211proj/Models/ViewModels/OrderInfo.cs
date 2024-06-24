namespace cldv6211proj.Models.ViewModels
{
    using cldv6211proj.Models.Database;

    public class OrderInfo
    {
        public required Order Order { get; set; }
        public required Product Product { get; set; }
        public required User Buyer { get; set; }
        public required User Seller { get; set; }
    }
}
