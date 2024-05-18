using APICatalago.Data;
using APICatalago.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        IEnumerable<Product> IProductRepository.GetProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(product => product.ProductId == id);
        }

        public Product DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product is null)
            {
                throw new ArgumentNullException($"Produto não encontrado");
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return product;
        }

        public Product InsertProduct(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            _context.Add(product);
            _context.SaveChanges();
            return product;
        }

        public Product UpdateProduct(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            return product;
        }
    }
}
