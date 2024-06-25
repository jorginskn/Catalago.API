using APICatalago.Models;
using APICatalago.Pagination;

namespace APICatalago.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetProducts(ProductParameters productParams);
        IEnumerable<Product> GetProductsByCategory(int id);
    }
}
