using APICatalago.Data;
using APICatalago.DTOS;
using APICatalago.DTOS.Mappings;
using APICatalago.Filters;
using APICatalago.Models;
using APICatalago.Pagination;
using APICatalago.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalago.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;
        public CategoriesController(ILogger<CategoriesController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _uof = unitOfWork;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            try
            {
                var categories = await _uof.CategoryRepository.GetAllAsync();
                if (categories is null)
                {
                    return NotFound();
                }

                var categoriesDTO = categories.toCategorytDTOList();
                return Ok(categoriesDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação");
            }
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);
                if (category is null)
                {
                    return NotFound("Categoria não encontrada");
                }

                var categoryDTO = category.ToCategoryDTO();

                return Ok(categoryDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação");
            }

        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAsync([FromQuery] CategoriesParameters categoriesParams)
        {
            var categories = await _uof.CategoryRepository.GetCategoriesAsync(categoriesParams);
            return GetCategories(categories);
        }

        private  ActionResult<IEnumerable<CategoryDTO>> GetCategories(IPagedList<Category> categories)
        {
            var metadata = new
            {
                categories.Count,
                categories.PageSize,
                categories.PageCount,
                categories.TotalItemCount,
                categories.HasNextPage,
                categories.HasPreviousPage
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            var categoriesDTO = categories.toCategorytDTOList();
            return Ok(categoriesDTO);
        }

        [HttpGet("filter/name/pagination")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesByNameAsync([FromQuery]CategoryFilterName categoryFilter)
         {
            var categories = await _uof.CategoryRepository.GetCategoryByNameAsync(categoryFilter); 
            return GetCategories(categories);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> InsertCategory(CategoryDTO categoryDTO)
        {
            if (categoryDTO is null)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }

            var category = categoryDTO.toCategory();

            var categoryCreated = _uof.CategoryRepository.Create(category);
            await _uof.commitAsync();


            var newCategoryDTO = categoryCreated.ToCategoryDTO();


            return new CreatedAtRouteResult("ObterCategoria", new { id = newCategoryDTO.CategoryId }, newCategoryDTO);
        }

        [HttpPut("{id:int}")]
        public ActionResult<CategoryDTO> UpdateCategory(int id, CategoryDTO categoryDTO)
        {
            if (id != categoryDTO.CategoryId)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }

            var category = categoryDTO.toCategory();
            var categoryUpdate = _uof.CategoryRepository.Update(category);
            _uof.commitAsync();
            var categoryUpdatedDTO = categoryUpdate.ToCategoryDTO();

            return Ok(categoryUpdatedDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);
            if (category is null)
            {
                return NotFound("Categoria não encontrada");
            }
            var excludedCategoryDTO = category.ToCategoryDTO();
            _uof.CategoryRepository.Delete(category);
           await _uof.commitAsync();
            return Ok(excludedCategoryDTO);
        }
    }
}
