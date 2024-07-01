using APICatalago.Data;
using APICatalago.DTOS;
using APICatalago.DTOS.Mappings;
using APICatalago.Filters;
using APICatalago.Models;
using APICatalago.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        public ActionResult<IEnumerable<CategoryDTO>> GetCategories()
        {
            try
            {
                var categories = _uof.CategoryRepository.GetAll();
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
        public ActionResult<CategoryDTO> GetCategoryById(int id)
        {
            try
            {
                var category = _uof.CategoryRepository.Get(c => c.CategoryId == id);
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
        public ActionResult<IEnumerable<CategoryDTO>> Get([FromQuery] CategoriesParameters categoriesParams)
        {
            var categories = _uof.CategoryRepository.GetCategories(categoriesParams);
            var metadata = new
            {
                categories.TotalCount,
                categories.PageSize,
                categories.CurrentPage,
                categories.TotalPages,
                categories.hasNext,
                categories.hasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            var categoriesDTO = categories.toCategorytDTOList();
            return Ok(categoriesDTO);
        }


        [HttpPost]
        public ActionResult<CategoryDTO> InsertCategory(CategoryDTO categoryDTO)
        {
            if (categoryDTO is null)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }

            var category = categoryDTO.toCategory();

            var categoryCreated = _uof.CategoryRepository.Create(category);
            _uof.commit();


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
            _uof.commit();
            var categoryUpdatedDTO = categoryUpdate.ToCategoryDTO();

            return Ok(categoryUpdatedDTO);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoryDTO> DeleteCategory(int id)
        {
            var category = _uof.CategoryRepository.Get(c => c.CategoryId == id);
            if (category is null)
            {
                return NotFound("Categoria não encontrada");
            }
            var excludedCategoryDTO = category.ToCategoryDTO();
            _uof.CategoryRepository.Delete(category);
            _uof.commit();
            return Ok(excludedCategoryDTO);
        }
    }
}
