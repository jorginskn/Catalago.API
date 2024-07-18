using APICatalago.Models;
using APICatalago.Pagination;
using X.PagedList;

namespace APICatalago.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        //IEnumerable<Product> GetProducts(ProductParameters productParams);
        Task<IPagedList<Product>> GetProductsAsync(ProductParameters productParams);
        IEnumerable<Product> GetProductsWithCategories();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id);
        Task<IPagedList<Product>>GetProductByPriceAsync(ProductFilterPrice productFilterParams);

    }
}
