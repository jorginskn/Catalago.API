using APICatalago.Models;
using Microsoft.AspNetCore.Mvc;

namespace APICatalago.Repositories
{
    public interface ICategoryRepository
    {
        public IEnumerable<Category> GetCategories();
        public Category GetCategoryById(int id);
        IEnumerable<Category>GetCategoriesAndProducts();
        Category InsertCategory(Category category);
        Category UpdateCategory(Category category);
        Category DeleteCategory(int id);

    }
}
