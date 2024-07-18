using APICatalago.Controllers;
using APICatalago.Data;
using APICatalago.Models;
using APICatalago.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace APICatalago.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {


        public CategoryRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParams)
        {
            var categories = await GetAllAsync();
            var orderedCategories = categories.OrderBy(c => c.CategoryId).AsQueryable();
            //var result = await PagedList<Category>.ToPagedList(orderedCategories, categoriesParams.PageNumber, categoriesParams.PageSize);
            var result = await orderedCategories.ToPagedListAsync(categoriesParams.PageNumber, categoriesParams.PageSize);
            return result;
        }

        public async Task<IPagedList<Category>> GetCategoryByNameAsync(CategoryFilterName categoryParams)
        {
            var categories = await GetAllAsync();
            if (!string.IsNullOrEmpty(categoryParams.Name))
            {
                categories = categories.Where(c => c.Name.Contains(categoryParams.Name));
            }

            //var filteredCategory = PagedList<Category>.ToPagedList(category.AsQueryable(), categoryParams.PageNumber, categoryParams.PageSize);
            var filteredCategory = await categories.ToPagedListAsync(categoryParams.PageNumber, categoryParams.PageSize);
            return filteredCategory;
        }
    }
}
