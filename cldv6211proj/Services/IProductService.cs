using cldv6211proj.Models;

namespace cldv6211proj.Services
{
    public interface IProductService
    {
        bool AddProduct(Product product);
        Product? GetProduct(int productID);
        List<Product> GetProducts();
        bool UpdateProductStock(int productID, int delta);
    }
}
