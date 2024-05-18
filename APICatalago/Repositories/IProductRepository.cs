using APICatalago.Models;

namespace APICatalago.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Product GetProductById(int id);
        Product InsertProduct(Product product);
        Product UpdateProduct(Product product);
        Product DeleteProduct(int id);
    }
}
