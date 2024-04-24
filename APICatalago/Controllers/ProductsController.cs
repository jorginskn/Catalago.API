using APICatalago.Data;
using APICatalago.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalago.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProduct()
        {
            var products = _context.Products.ToList();
            if (products is null)
            {
                return NotFound("Produtos não encontrados");
            }
            return products;
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<Product> GetProductById(int id )
        {
            var product = _context.Products.FirstOrDefault(product => product.ProductId == id);
            if (product is null)
            {
                return NotFound("Produto não encontrado");
            }
            return product;
        }

        [HttpPost]
        public ActionResult InsertProduct(Product product)
        {
            if (product is null)
            {
                return BadRequest();
            }

            _context.Add(product);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", new { id = product.ProductId }, product);
        }
    }
}
