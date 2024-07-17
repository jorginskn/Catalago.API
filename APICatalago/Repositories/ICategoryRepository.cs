using APICatalago.Controllers;
using APICatalago.Models;
using APICatalago.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace APICatalago.Repositories
{
    public interface ICategoryRepository :IRepository<Category>
    {
        public Task<PagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParams);
        public Task<PagedList<Category>> GetCategoryByNameAsync(CategoryFilterName categoryParams);
    }
}
