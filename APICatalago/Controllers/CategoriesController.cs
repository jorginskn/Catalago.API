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
        private readonly IRepository<Category> _repository;
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
                var categories = _repository.GetAll().ToList();
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
                var category = _repository.Get(c => c.CategoryId== id);
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


        [HttpPost]
        public ActionResult InsertCategory(Category category)
        {
            if (category is null)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }
            var categoryCreated = _repository.Create(category);
            return new CreatedAtRouteResult("ObterCategoria", new { id = category.CategoryId }, categoryCreated);
        }

        [HttpPut("{id:int}")]
        public ActionResult UpdateCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }
            _repository.Update(category);
            return Ok(category);
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteCategory(int id)
        {
            var category = _repository.Get(c => c.CategoryId == id);
            if (category is null)
            {
                return NotFound("Categoria não encontrada");
            }
            _repository.Delete(category);
            return Ok(category);
        }
    }
}
