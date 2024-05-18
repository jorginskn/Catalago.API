using APICatalago.Data;
using APICatalago.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories.FirstOrDefault(category => category.CategoryId == id);
        }
        public ActionResult<IEnumerable<Category>> GetCategoriesAndProducts(int id)
        {
            return _context.Categories.Include(c => c.Products).ToList();
        }

        public Category InsertCategory(Category category)
        {
            if (category is null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }
        public Category UpdateCategory(Category category)
        {
            if (category is null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();
            return category;
        }
        public Category DeleteCategory(int id)
        {
          var category = _context.Categories.Find(id);
            if (category is null)
            {
                throw new ArgumentNullException($"{nameof(category)} is null");
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return category;
        }

        public IEnumerable<Category> GetCategoriesAndProducts()
        {
            return _context.Categories.Include(c => c.Products).ToList();
        }
    }
}
