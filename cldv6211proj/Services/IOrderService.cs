using cldv6211proj.Models.ViewModels;

namespace cldv6211proj.Services
{
    public interface IOrderService
    {
        int CreateOrder(OrderSubmitForm orderSubmission);
        bool ProcessOrder(int orderID);
        List<OrderInfo>? FindOrderInfos(int userID, bool isBuyer = true);
    }
}
