using APICatalago.Data;
using APICatalago.Filters;
using APICatalago.Models;
using APICatalago.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger _logger;
        public CategoriesController(ICategoryRepository repository, ILogger<CategoriesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            try
            {
                var categories = _repository.GetCategories().ToList();
                return categories;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação");
            }
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Category> GetCategoryById(int id)
        {
            try
            {
                var category = _repository.GetCategoryById(id);
                if (category is null)
                {
                    return NotFound("Categoria não encontrada");
                }
                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação");
            }

        }

        [HttpGet("products")]
        public ActionResult<IEnumerable<Category>> GetCategoriesAndProducts()
        {
            try
            {
                return _repository.GetCategoriesAndProducts().ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public ActionResult InsertCategory(Category category)
        {
            if (category is null)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }
            var categoryCreated = _repository.InsertCategory(category);
            return new CreatedAtRouteResult("ObterCategoria", new { id = category.CategoryId }, categoryCreated);
        }

        [HttpPut]
        public ActionResult UpdateCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }
            _repository.UpdateCategory(category);
            return Ok(category);
        }

        [HttpDelete]
        public ActionResult DeleteCategory(int id)
        {
            var category = _repository.GetCategoryById(id);
            if (category is null)
            {
                return NotFound("Categoria não encontrada");
            }
            _repository.DeleteCategory(id);
            return Ok(category);
        }
    }
}
