using Shared.Models;

namespace cldv6211proj.Models.ViewModels
{
    public class UserOrderInfos
    {
        public required User User { get; set; }
        public required List<OrderInfo> OrderInfos { get; set; }
    }
}
