using Shared.Data;
using Shared.Enums;
using Shared.Models;

namespace Shared.Services
{
    public class OrderService : IOrderService
    {
        private readonly SharedDbContext _context;

        public OrderService(SharedDbContext context)
        {
            _context = context;
        }

        public int CreateOrder(int userID, int productID, int quantity, string address)
        {
            var order = new Order()
            {
                UserID = userID,
                ProductID = productID,
                Quantity = quantity,
                Address = address
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            return order.ID;
        }

        public bool ProcessPayment(int orderID, bool reverse = false)
        {
            var order = _context.Orders.Find(orderID);
            if (order == null)
                return false; // Invalid order?

            var productService = new ProductService(_context);
            var product = productService.GetProduct(order.ProductID);
            if (product == null)
                return false; // Invalid product?

            double orderTotal = product.Price * order.Quantity;

            return new UserService(_context).TransferBalance(
                order.UserID,
                product.UserID,
                orderTotal * (reverse ? -1 : 1)
            );
        }

        public bool ProcessInventory(int orderID, bool reverse = false)
        {
            var order = _context.Orders.Find(orderID);
            if (order == null)
                return false; // Invalid order?

            var productService = new ProductService(_context);
            var product = productService.GetProduct(order.ProductID);
            if (product == null)
                return false; // Invalid product?

            return new ProductService(_context).UpdateProductStock(
                order.ProductID,
                order.Quantity * (reverse ? 1 : -1)
            );
        }

        public bool ProcessOrder(int orderID)
        {
            var procInvResult = ProcessInventory(orderID);
            var procPayResult = ProcessPayment(orderID);
            var success = procInvResult && procPayResult;

            if (!success && procInvResult != procPayResult)
            { // One must have failed
                // Reverse successful process.
                if (procInvResult)
                    ProcessInventory(orderID, reverse: true);
                if (procPayResult)
                    ProcessPayment(orderID, reverse: true);

                // Set relevant order status and return false.
                UpdateOrderStatus(orderID, OrderStatus.Failed);
                return false;
            }

            // Set relevant order status.
            if (success)
                UpdateOrderStatus(orderID, OrderStatus.PendingShipping);
            else
                UpdateOrderStatus(orderID, OrderStatus.Failed);

            // return whether both successful
            return success;
        }

        public bool UpdateOrderStatus(int orderID, OrderStatus status)
        {
            var order = _context.Orders.Find(orderID);
            if (order == null)
                return false; // Invalid order?

            order.Status = status;
            _context.SaveChanges();
            return true;
        }

        List<Order> FindOrders(int userID, bool forBuyer = true)
        {
            if (forBuyer)
                return [.. _context.Orders.Where(o => o.UserID == userID)];
            var sellerProductIDs = _context
                .Products.Where(p => p.UserID == userID)
                .Select(p => p.ID)
                .ToHashSet();
            if (sellerProductIDs.Count == 0)
                return [];
            return [.. _context.Orders.Where(o => sellerProductIDs.Contains(o.ProductID))];
        }

        List<Order> FindActiveOrders(int userID, bool forBuyer = true) =>
            [.. FindOrders(userID, forBuyer).Where(o => o.Status < OrderStatus.Completed)];

        public List<OrderInfo>? FindOrderInfos(int userID, bool forBuyer = true)
        {
            // refs: https://stackoverflow.com/a/6253805
            //       https://learn.microsoft.com/en-us/dotnet/csharp/linq/standard-query-operators/join-operations
            if (forBuyer)
                return
                [
                    .. _context
                        .Orders.Where(o => o.UserID == userID)
                        .Join(
                            _context.Products,
                            ord => ord.ProductID,
                            prd => prd.ID,
                            (ord, prd) =>
                                new OrderInfo
                                {
                                    Order = ord,
                                    Product = prd,
                                    Buyer = _context.Users.First(u => u.ID == userID),
                                    Seller = _context.Users.First(u => u.ID == prd.UserID)
                                }
                        )
                ];

            return
            [
                .. _context.Orders.Join(
                    _context.Products.Where(prd => prd.UserID == userID),
                    ord => ord.ProductID,
                    prd => prd.ID,
                    (ord, prd) =>
                        new OrderInfo
                        {
                            Order = ord,
                            Product = prd,
                            Buyer = _context.Users.First(u => u.ID == ord.UserID),
                            Seller = _context.Users.First(u => u.ID == userID)
                        }
                )
            ];
        }
    }
}
