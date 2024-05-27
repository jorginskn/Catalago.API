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
        
        private readonly ILogger _logger;
        private readonly IUnitOfWork _uof;
        public ProductsController(  ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
        {
            _uof= unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            try
            {
                var products = _uof.ProductRepository.GetAll();
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
                var product = _uof.ProductRepository.Get(p => p.ProductId == id);
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
            var product = _uof.ProductRepository.GetProductsByCategory(id);
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
            var productCreated = _uof.ProductRepository.Create(product);
            _uof.commit();
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
            var updatedProduct = _uof.ProductRepository.Update(product);
            _uof.commit();
            return Ok(updatedProduct);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Product> DeleteProduct(int id)
        {
            var product = _uof.ProductRepository.Get(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Produto não localizado...");
            }
            _uof.ProductRepository.Delete(product);
            _uof.commit();
            return Ok(product);
        }


    }
}
