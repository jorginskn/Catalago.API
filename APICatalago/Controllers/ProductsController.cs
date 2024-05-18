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
        private readonly IProductRepository _repository;
        private readonly ILogger _logger;

        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            try
            {
                var products = _repository.GetProducts();
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
                var product = _repository.GetProductById(id);
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

        [HttpPost]
        public ActionResult InsertProduct(Product product)
        {
            if (product is null)
            {
                _logger.LogWarning("Dados invalidos");
                return BadRequest();
            }
            var productCreated = _repository.InsertProduct(product);
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
            _repository.UpdateProduct(product);
            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Product> DeleteProduct(int id)
        {
            var product = _repository.GetProductById(id);
            if (product is null)
            {
                return NotFound("Produto não localizado...");
            }
            _repository.DeleteProduct(id);
            return Ok(product);
        }
    }
}
