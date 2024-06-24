using Shared.Enums;
using Shared.Models;

namespace Shared.Services
{
    public interface IOrderService
    {
        int CreateOrder(int userID, int productID, int quantity, string address);
        bool ProcessPayment(int orderID, bool reverse = false);
        bool ProcessInventory(int orderID, bool reverse = false);
        bool ProcessOrder(int orderID);
        bool UpdateOrderStatus(int orderID, OrderStatus status);
        List<OrderInfo>? FindOrderInfos(int userID, bool isBuyer = true);
    }
}
