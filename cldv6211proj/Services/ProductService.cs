using cldv6211proj.Data;
using cldv6211proj.Models.Database;

namespace cldv6211proj.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public bool AddProduct(Product product)
        {
            _context.Products.Add(product);
            return _context.SaveChanges() > 0;
        }

        public Product? GetProduct(int productID) => _context.Products.Find(productID);

        public List<Product> GetProducts() => _context.Products.ToList();

        public bool UpdateProductStock(int productID, int delta)
        {
            var product = _context.Products.Find(productID);
            if (product == null || product.Availability + delta < 0)
                return false;
            product.Availability += delta;
            return _context.SaveChanges() > 0;
        }
    }
}
