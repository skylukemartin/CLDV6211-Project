using cldv6211proj.Data;
using cldv6211proj.Models.Database;
using cldv6211proj.Models.ViewModels;

namespace cldv6211proj.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public int CreateOrder(OrderSubmitForm orderForm)
        {
            var productService = new ProductService(_context);
            var product = productService.GetProduct(orderForm.ProductID);
            if (product == null || product.Availability < orderForm.Quantity)
                return -1; // Can't create order without product

            // uncomment to check user has sufficient balance
            // var userService = new UserService(_context);
            // var buyer = userService.GetUser(orderForm.UserID);
            // var orderPrice = product.Price * orderSubmission.Quantity;
            // var buyerBalance = buyer.Balance;
            // var processingBalance = FindActiveOrders(buyer.ID)
            //     .Sum(o =>
            //     {
            //         var p = productService.GetProduct(o.ProductID);
            //         return p == null ? 0 : p.Price * o.Quantity;
            //     });
            // if (buyerBalance < orderPrice || orderPrice + processingBalance > buyerBalance)
            //     return -1; // Buyer has insufficient balance, can't create order

            var order = new Order()
            {
                UserID = orderForm.UserID,
                ProductID = orderForm.ProductID,
                Address = orderForm.Address,
                Quantity = orderForm.Quantity
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            return order.ID;
        }

        public bool ProcessOrder(int OrderID)
        {
            var order = _context.Orders.First(o => o.ID == OrderID);
            var userService = new UserService(_context);

            var productService = new ProductService(_context);
            var product = productService.GetProduct(order.ProductID);
            if (product == null || product.Availability < order.Quantity)
                return false; // Product unavailable

            double orderTotal = product.Price * order.Quantity;
            if (!userService.TransferBalance(order.UserID, product.UserID, orderTotal))
                return false; // Transaction failed
            if (!productService.UpdateProductStock(order.ProductID, -order.Quantity))
            { // Product unavailable, refund order total
                userService.TransferBalance(product.UserID, order.UserID, orderTotal);
                return false;
            }

            order.Processed = true;
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
            [.. FindOrders(userID, forBuyer).Where(o => !o.Processed)];

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
