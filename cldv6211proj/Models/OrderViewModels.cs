namespace cldv6211proj.Models
{
    public class SubmitOrderModel
    {
        public required User User { get; set; }
        public required Product Product { get; set; }
        public required SubmitOrderForm OrderForm { get; set; }
    }

    public class SubmitOrderForm
    {
        public int Quantity { get; set; }
        public string Address { get; set; } = "";
    }

    public class UserOrdersModel
    {
        public required User User { get; set; }
        public required List<OrderInfo> OrderInfos { get; set; }
    }

    public class OrderInfo
    {
        public required Order Order { get; set; }
        public required Product Product { get; set; }
        public required User Buyer { get; set; }
        public required User Seller { get; set; }
    }
}
