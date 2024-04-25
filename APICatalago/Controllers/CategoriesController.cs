using APICatalago.Data;
using APICatalago.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var categories = _context.Categories.ToList();
            if (categories is null)
            {
                return NotFound("Categorias não foram encontradas");
            }
            return categories;
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Category> GetCategoryById(int id)
        {
            var category = _context.Categories.FirstOrDefault(category => category.CategoryId == id);
            if (category is null)
            {
                return NotFound("Categoria não encontrada");
            }
            return category;
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

    }
}
