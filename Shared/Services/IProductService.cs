using Shared.Models;

namespace Shared.Services
{
    public interface IProductService
    {
        bool AddProduct(Product product);
        Product? GetProduct(int productID);
        List<Product> GetProducts();
        bool UpdateProductStock(int productID, int delta);
    }
}
