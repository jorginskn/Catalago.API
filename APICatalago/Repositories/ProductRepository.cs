using APICatalago.Data;
using APICatalago.Models;
using APICatalago.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        public ProductRepository(AppDbContext context) : base(context)
        {

        }

        public IEnumerable<Product> GetProducts(ProductParameters productParams)
        {
            return GetAll()
                   .OrderBy(p => p.Name)
                   .Skip((productParams.PageNumber - 1) * productParams.PageSize)
                   .Take(productParams.PageSize).ToList() ;
                     
        }

        public IEnumerable<Product> GetProductsByCategory(int id)
        {
            return GetAll().Where(c => c.CategoryId == id);
        }
    }
}
