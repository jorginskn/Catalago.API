using APICatalago.Controllers;
using APICatalago.Data;
using APICatalago.Models;
using APICatalago.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {


        public CategoryRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<PagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParams)
        {
            var categories = await GetAllAsync();
            var orderedCategories = categories.OrderBy(c => c.CategoryId).AsQueryable();
            var result = PagedList<Category>.ToPagedList(orderedCategories, categoriesParams.PageNumber, categoriesParams.PageSize);
            return result;
        }

        public async Task<PagedList<Category>> GetCategoryByNameAsync(CategoryFilterName categoryParams)
        {
            var category = await GetAllAsync();
            if (!string.IsNullOrEmpty(categoryParams.Name))
            {
                category = category.Where(c => c.Name.Contains(categoryParams.Name));
            }

            var filteredCategory = PagedList<Category>.ToPagedList(category.AsQueryable(), categoryParams.PageNumber, categoryParams.PageSize);
            return filteredCategory;
        }
    }
}
