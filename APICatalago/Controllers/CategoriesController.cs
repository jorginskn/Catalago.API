using APICatalago.Data;
using APICatalago.DTOS;
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
                var categoriesDTO = new List<CategoryDTO>();
                foreach (var category in categories)
                {
                    var categoryDTO = new CategoryDTO()
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Name,
                        ImageUrl = category.ImageUrl,
                    };
                    categoriesDTO.Add(categoryDTO);
                }
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
                var CategoryDTO = new CategoryDTO()
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    ImageUrl = category.ImageUrl,
                };

                return Ok(CategoryDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação");
            }

        }


        [HttpPost]
        public ActionResult<CategoryDTO> InsertCategory(CategoryDTO categoryDTO)
        {
            if (categoryDTO is null)
            {
                _logger.LogWarning($"Dados invalidos...");
                return BadRequest();
            }
            var category = new Category()
            {
                CategoryId = categoryDTO.CategoryId,
                Name = categoryDTO.Name,
                ImageUrl = categoryDTO.ImageUrl,
            };

            var categoryCreated = _uof.CategoryRepository.Create(category);
            _uof.commit();

            var newCategoryDTO = new CategoryDTO()
            {
                CategoryId = categoryCreated.CategoryId,
                Name = categoryCreated.Name,
                ImageUrl = categoryCreated.ImageUrl,
            };

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
            var category = new Category()
            {
                CategoryId = categoryDTO.CategoryId,
                Name = categoryDTO.Name,
                ImageUrl = categoryDTO.ImageUrl,
            };

            var categoryUpdate = _uof.CategoryRepository.Update(category);
            _uof.commit();
            var CategoryUpdatedDTO = new CategoryDTO()
            {
                CategoryId = categoryUpdate.CategoryId,
                Name = categoryUpdate.Name,
                ImageUrl = categoryUpdate.ImageUrl,
            };

            return Ok(CategoryUpdatedDTO);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoryDTO> DeleteCategory(int id)
        {
            var category = _uof.CategoryRepository.Get(c => c.CategoryId == id);

            var excludedCategory = new CategoryDTO()
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
            };

            if (category is null)
            {
                return NotFound("Categoria não encontrada");
            }
            _uof.CategoryRepository.Delete(category);
            _uof.commit();
            return Ok(excludedCategory);
        }
    }
}
