using APICatalago.Models;
using APICatalago.Pagination;

namespace APICatalago.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        //IEnumerable<Product> GetProducts(ProductParameters productParams);
        Task<PagedList<Product>> GetProductsAsync(ProductParameters productParams);
        IEnumerable<Product> GetProductsWithCategories();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id);
        Task<PagedList<Product>>GetProductByPriceAsync(ProductFilterPrice productFilterParams);

    }
}
