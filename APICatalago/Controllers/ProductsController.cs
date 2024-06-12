using APICatalago.Data;
using APICatalago.DTOS;
using APICatalago.Filters;
using APICatalago.Models;
using APICatalago.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        public ProductsController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uof = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<ProductDTO>> GetProducts()
        {
            try
            {
                var products = _uof.ProductRepository.GetAll();
                if (products is null)
                {
                    return NotFound("Produtos não encontrados");
                }

                // Mapeia a coleção de Product para a coleção de ProductDTO
                var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products).ToList();
                return Ok(productsDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter produtos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }


        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<ProductDTO> GetProductById(int id)
        {
            try
            {
                var product = _uof.ProductRepository.Get(p => p.ProductId == id);
                if (product is null)
                {
                    return NotFound("Produto não encontrado");
                }
                var productDTO = _mapper.Map<ProductDTO>(product);
                return Ok(productDTO);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Category/{id}")]
        public ActionResult<IEnumerable<ProductDTO>> GetProductsByCategory(int id)
        {
            var product = _uof.ProductRepository.GetProductsByCategory(id);
            if (product is null)
            {
                return NotFound();
            }
            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(product);
            return Ok(productsDTO);
        }

        [HttpPost]
        public ActionResult<ProductDTO> InsertProduct(ProductDTO productDTO)
        {
            if (productDTO is null)
            {
                _logger.LogWarning("Dados invalidos");
                return BadRequest();
            }
            var product = _mapper.Map<Product>(productDTO);
            var productCreated = _uof.ProductRepository.Create(product);
            _uof.commit();
            var productCreatedDTO = _mapper.Map<ProductDTO>(productCreated);
            return new CreatedAtRouteResult("ObterProduto", new { id = productCreatedDTO.ProductId }, productCreatedDTO);

        }

        [HttpPut("{id:int}")]
        public ActionResult<ProductDTO> UpdateProduct(int id, ProductDTO productDTO)
        {

            if (id != productDTO.ProductId)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }
            var product = _mapper.Map<Product>(productDTO);
            var updatedProduct = _uof.ProductRepository.Update(product);
            var updatedProductDTO = _mapper.Map<ProductDTO>(updatedProduct);
            _uof.commit();
            return Ok(updatedProductDTO);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProductDTO> DeleteProduct(int id)
        {
            var product = _uof.ProductRepository.Get(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Produto não localizado...");
            }

            _uof.ProductRepository.Delete(product);
            _uof.commit();
            var productDeleted = _mapper.Map<ProductDTO>(product);
            return Ok(productDeleted);
        }


    }
}
