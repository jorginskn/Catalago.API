using APICatalago.Data;
using APICatalago.Models;
using APICatalago.Pagination;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace APICatalago.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        public ProductRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<IPagedList<Product>> GetProductByPriceAsync(ProductFilterPrice productFilterParams)
        {
            var products = await GetAllAsync();
            if (productFilterParams.Price.HasValue && !string.IsNullOrEmpty(productFilterParams.PriceCriterio))
            {
                if (productFilterParams.PriceCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price > productFilterParams.Price);
                }
                else if (productFilterParams.PriceCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price < productFilterParams.Price);
                }
                else if (productFilterParams.PriceCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price == productFilterParams.Price);
                }
            }
           // var filteredProduct = PagedList<Product>.ToPagedList(products.AsQueryable(), productFilterParams.PageNumber, productFilterParams.PageSize);
               var filteredProduct = await products.ToPagedListAsync(productFilterParams.PageNumber, productFilterParams.PageSize);
                return filteredProduct;
        }


        public IEnumerable<Product> GetProductsWithCategories()
        {
            var products = _context.Products.Include(p => p.Category).AsNoTracking().AsQueryable() ;          
            return products;
        }
        // IEnumerable<Product> GetProducts(ProductParameters productParams)
        // {
        //   return GetAll()
        //          .OrderBy(p => p.Name)
        //           .Skip((productParams.PageNumber - 1) * productParams.PageSize)
        //          .Take(productParams.PageSize).ToList() ;

        // }

        public async Task<IPagedList<Product>> GetProductsAsync(ProductParameters productParams)
        {
            var products = await GetAllAsync();
            var orderedCategories = products.OrderBy(p => p.ProductId).ToList();
          // var result = PagedList<Product>.ToPagedList(products.AsQueryable(), productParams.PageNumber, productParams.PageSize);
             var result = await orderedCategories.ToPagedListAsync(productParams.PageNumber, productParams.PageSize);
            return result;

        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id)
        {
            var products = await GetAllAsync();
            var productsCategory = products.Where(c => c.CategoryId == id);
            return productsCategory;
        }
    }
}
