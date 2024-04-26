using APICatalago.Data;
using APICatalago.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            try
            {
                var categories = _context.Categories.AsNoTracking().ToList();
                if (categories is null)
                {
                    return NotFound("Categorias não foram encontradas");
                }
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
                var category = _context.Categories.FirstOrDefault(category => category.CategoryId == id);
                if (category is null)
                {
                    return NotFound("Categoria não encontrada");
                }
                return category;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação");
            }

        }

        [HttpGet("product")]
        public ActionResult<IEnumerable<Category>> GetCategoriesAndProducts()
        {
            return _context.Categories.Include(p => p.Products).ToList();
        }

        [HttpPost]
        public ActionResult InsertCategory(Category category)
        {
            if (category is null)
            {
                return BadRequest();
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
            return new CreatedAtRouteResult("ObterCategoria", new { id = category.CategoryId }, category);
        }

        [HttpPut]
        public ActionResult UpdateCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }
            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(category);
        }
        [HttpDelete]
        public ActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(category => category.CategoryId == id);
            if (category is null)
            {
                return NotFound("Categoria não encontrada");
            }
            _context.Remove(category);
            _context.SaveChanges();
            return Ok(category);
        }
    }
}
