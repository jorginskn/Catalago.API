using APICatalago.Data;
using APICatalago.DTOS;
using APICatalago.Filters;
using APICatalago.Models;
using APICatalago.Pagination;
using APICatalago.Repositories;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using X.PagedList;

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
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            try
            {
                var products = await _uof.ProductRepository.GetAllAsync();
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
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            try
            {
                var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);
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
            var product = _uof.ProductRepository.GetProductsByCategoryAsync(id);
            if (product is null)
            {
                return NotFound();
            }
            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(product);
            return Ok(productsDTO);
        }
        [HttpGet("categories")]
        public ActionResult<IEnumerable<ProductDTO>> GetProductsWithCategories()
        {
            var product = _uof.ProductRepository.GetProductsWithCategories().ToList();
            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(product);
            return Ok(productsDTO);

        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts([FromQuery] ProductParameters productParameters)
        {
            var products = await _uof.ProductRepository.GetProductsAsync(productParameters);
            return GetProducts(products);

        }

        [HttpGet("filter/price/pagination")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductByPrice([FromQuery] ProductFilterPrice productsFilterParams)
        {
            var products = await _uof.ProductRepository.GetProductByPriceAsync(productsFilterParams);
            return GetProducts(products);
        }

        private ActionResult<IEnumerable<ProductDTO>> GetProducts(IPagedList<Product>? products)
        {
            var metadata = new
            {
                products.Count,
                products.PageSize,
                products.PageCount,
                products.TotalItemCount,
                products.HasNextPage,
                products.HasPreviousPage
            };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(productsDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> InsertProduct(ProductDTO productDTO)
        {
            if (productDTO is null)
            {
                _logger.LogWarning("Dados invalidos");
                return BadRequest();
            }
            var product = _mapper.Map<Product>(productDTO);
            var productCreated = _uof.ProductRepository.Create(product);
            _uof.commitAsync();
            var productCreatedDTO = _mapper.Map<ProductDTO>(productCreated);
            return new CreatedAtRouteResult("ObterProduto", new { id = productCreatedDTO.ProductId }, productCreatedDTO);

        }

        [HttpPatch("{id:int}/UpdatePartial")]
        public async Task<ActionResult<ProductDTOUpdateResponse>> Patch(int id, JsonPatchDocument<ProductDTOUpdateRequest> patchProductDto)
        {
            if (patchProductDto is null || id <= 0)
            {
                return BadRequest();
            }
            var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound();
            }
            var productDTO = _mapper.Map<ProductDTOUpdateRequest>(product);
            patchProductDto.ApplyTo(productDTO, ModelState);
            if (!ModelState.IsValid || TryValidateModel(productDTO))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(productDTO, product);
            _uof.ProductRepository.Update(product);
            _uof.commitAsync();
            return Ok(_mapper.Map<ProductDTOUpdateResponse>(product));
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
            _uof.commitAsync();
            return Ok(updatedProductDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProductDTO>> DeleteProduct(int id)
        {
            var product = await _uof.ProductRepository.GetAsync(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Produto não localizado...");
            }

            _uof.ProductRepository.Delete(product);
            _uof.commitAsync();
            var productDeleted = _mapper.Map<ProductDTO>(product);
            return Ok(productDeleted);
        }


    }
}
