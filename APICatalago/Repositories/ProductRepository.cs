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

        public PagedList<Product> GetProductByPrice(ProductFilterPrice productFilterParams)
        {
            var products = GetAll().AsQueryable();
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
            var filteredProduct = PagedList<Product>.ToPagedList(products, productFilterParams.PageNumber, productFilterParams.PageSize);
            return filteredProduct;
        }


        public IQueryable<Product> GetProductsWithCategories()
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

        public PagedList<Product> GetProducts(ProductParameters productParams)
        {
            var products = GetAll().OrderBy(p => p.ProductId).AsQueryable();
            var productOrder = PagedList<Product>.ToPagedList(products, productParams.PageNumber, productParams.PageSize);
            return productOrder;

        }

        public IEnumerable<Product> GetProductsByCategory(int id)
        {
            return GetAll().Where(c => c.CategoryId == id);
        }
    }
}
