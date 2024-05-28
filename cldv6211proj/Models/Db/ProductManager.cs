namespace cldv6211proj.Models.Db
{
    using Base;

    public class Product : RecordModel
    {
        public string? Name { get; set; }
        public double Price { get; set; }
        public int Availability { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? ImageURL { get; set; }
        public int UserID { get; set; }
    }

    public static class ProductManager
    {
        public static readonly Table<Product> table = new();

        public static bool AddProduct(Product product)
        {
            var index = table.AddRecord(product);
            return index != 0;
        }

        public static bool UpdateProductStock(Product product, int delta)
        {
            if (product.ID < 1)
                return false;
            var newVal = product.Availability + delta;
            if (newVal < 0)
                return false;
            product.Availability = newVal;
            return table.UpdateRecord(product);
        }

        public static List<Product> GetAllProducts() =>
            (table.Records.Count > 0 ? table.Records : table.FetchAllRecords())
                .Select(rec => rec.Model)
                .ToList();

        public static Product? FindProduct(int productID) => table.LazyFindRecord(productID)?.Model;

        public static List<Product>? FindProducts(Func<Product, bool> where) =>
            GetAllProducts().Where(where).ToList();
    }
}
