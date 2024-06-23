namespace cldv6211proj.Models.ViewModels
{
    using cldv6211proj.Models.Database;

    public class UserOrderInfos
    {
        public required User User { get; set; }
        public required List<OrderInfo> OrderInfos { get; set; }
    }
}
