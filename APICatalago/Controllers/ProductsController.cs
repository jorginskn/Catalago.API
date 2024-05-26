using APICatalago.Data;
using APICatalago.Filters;
using APICatalago.Models;
using APICatalago.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IRepository<Product> _repository;
        private readonly ILogger _logger;

        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger,IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository= productRepository;
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            try
            {
                var products = _repository.GetAll();
                if (products is null)
                {
                    return NotFound("Produtos não encontrados");
                }
                return Ok(products);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<Product> GetProductById(int id)
        {
            try
            {
                var product = _repository.Get(p => p.ProductId == id);
                if (product is null)
                {
                    return NotFound("Produto não encontrado");
                }
                return Ok(product);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Category/{id}")]
        public ActionResult<IEnumerable<Product>> GetProductsByCategory(int id)
        {
            var product = _productRepository.GetProductsByCategory(id);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public ActionResult InsertProduct(Product product)
        {
            if (product is null)
            {
                _logger.LogWarning("Dados invalidos");
                return BadRequest();
            }
            var productCreated = _repository.Create(product);
            return new CreatedAtRouteResult("ObterProduto", new { id = product.ProductId }, productCreated);

        }

        [HttpPut("{id:int}")]
        public ActionResult<Product> UpdateProduct(int id, Product product)
        {

            if (id != product.ProductId)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }
            var updatedProduct = _repository.Update(product);
            return Ok(updatedProduct);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Product> DeleteProduct(int id)
        {
            var product = _repository.Get(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Produto não localizado...");
            }
            _repository.Delete(product);
            return Ok(product);
        }


    }
}
