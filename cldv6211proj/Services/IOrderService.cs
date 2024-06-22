using cldv6211proj.Models;

namespace cldv6211proj.Services
{
    public interface IOrderService
    {
        int CreateOrder(int userID, int productID, string address, int quantity = 1);
        bool ProcessOrder(int OrderID);
        List<OrderInfo>? FindOrderInfos(int userID, bool isBuyer = true);
    }
}
