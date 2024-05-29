namespace cldv6211proj.Models.Db
{
    using Base;

    public class Order : RecordModel
    {
        public int UserID { get; set; } = -1;
        public int ProductID { get; set; } = -1;
        public int Quantity { get; set; } = 1;
        public string Address { get; set; } = "";
        public bool Processed { get; set; } = false;
    }

    public static class OrderManager
    {
        public static readonly Table<Order> table = new();

        private static int CreateOrder(Order order) => table.AddRecord(order);

        public static int CreateOrder(int userID, int productID, string address, int quantity = 1)
        {
            var product = ProductManager.FindProduct(productID);
            if (product == null)
                return -1;
            var buyer = UserManager.FindUser(userID);
            var seller = UserManager.FindUser(product.UserID);
            if (buyer == null || seller == null)
                return -1;

            if (!ProductManager.UpdateProductStock(product, -quantity))
                return -1;
            else if (!UserManager.TransferBalance(buyer, seller, product.Price * quantity))
            {
                ProductManager.UpdateProductStock(product, +quantity);
                return -1;
            }

            return CreateOrder(
                new Order()
                {
                    UserID = userID,
                    ProductID = productID,
                    Quantity = quantity,
                    Address = address,
                    Processed = false
                }
            );
        }

        public static bool ProcessOrder(int orderID)
        {
            var order = FindOrder(orderID);
            if (order == null)
                return false;
            order.Processed = true;
            return table.UpdateRecord(order);
        }

        public static List<Order> GetAllOrders() =>
            (table.Records.Count > 0 ? table.Records : table.FetchAllRecords())
                .Select(rec => rec.Model)
                .ToList();

        public static Order? FindOrder(int orderID) => table.LazyFindRecord(orderID)?.Model;

        public static List<Order>? FindOrders(Func<Order, bool> where) =>
            GetAllOrders().Where(where).ToList();

        public static List<OrderInfo>? ClientOrderInfos(int userID)
        { // refs: https://stackoverflow.com/a/6253805
            // https://learn.microsoft.com/en-us/dotnet/csharp/linq/standard-query-operators/join-operations
            var user = UserManager.FindUser(userID);
            if (user == null)
                return null;

            return FindOrders(ord => ord.UserID == userID)
                ?.Join(
                    ProductManager.GetAllProducts(),
                    ord => ord.ProductID,
                    prd => prd.ID,
                    (order, product) => new { order, product }
                )
                .Join(
                    UserManager.GetAllUsers(),
                    op => op.product.UserID,
                    usr => usr.ID,
                    (op, usr) =>
                        new OrderInfo
                        {
                            Order = op.order,
                            Product = op.product,
                            OtherUser = usr
                        }
                )
                .ToList();
        }

        public static List<OrderInfo>? SellerOrderInfos(int userID)
        {
            var user = UserManager.FindUser(userID);
            if (user == null)
                return null;

            return ProductManager
                .FindProducts(prd => prd.UserID == userID)
                ?.Join(
                    GetAllOrders(),
                    prd => prd.ID,
                    ord => ord.ProductID,
                    (product, order) => new { product, order }
                )
                .Join(
                    UserManager.GetAllUsers(),
                    po => po.order.UserID,
                    usr => usr.ID,
                    (prodOrder, user) =>
                        new OrderInfo()
                        {
                            Product = prodOrder.product,
                            Order = prodOrder.order,
                            OtherUser = user
                        }
                )
                .ToList();
        }

        // TODO: Seller order manager idk
        // .Join(
        // UserManager.GetAllUsers(),
        // op => op.product.UserID,
        // usr => usr.ID,
        // (ordProd, seller) =>
        // new OrderInfo
        // {
        // Order = ordProd.order,
        // Product = ordProd.product,
        // Buyer = user,
        // Seller = seller
        // }
        // )
    }
}
